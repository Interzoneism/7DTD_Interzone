using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Platform;

namespace MacheteAnimRandomizer
{
    /// <summary>
    /// Harmony patches for randomizing melee attack animations in first-person view.
    /// 
    /// TECHNICAL EXPLANATION:
    /// In FPV (first-person view), melee animations are baked into Unity AnimationClips
    /// which directly control bone transforms through keyframe data. This data cannot be
    /// modified at runtime.
    /// 
    /// SOLUTION:
    /// The game uses a spring-based system (vp_FPWeapon) that ADDS position/rotation 
    /// forces on top of the base weapon position. The vp_FPWeaponMeleeAttack class already
    /// uses AddSoftForce() to create swing motion. We inject random additional forces
    /// into this spring system when an attack starts, creating procedural variation
    /// without fighting the animator.
    /// 
    /// For speed variation, we modify the "MeleeAttackSpeed" animator parameter which
    /// the game already uses to control attack speed.
    /// </summary>
    public class MacheteAnimRandomizerPatches
    {
        // Random number generator for animation variations
        private static System.Random random = new System.Random();

        // Track last randomization per entity to prevent double-applications
        private static Dictionary<int, AttackRandomData> entityAttackData = new Dictionary<int, AttackRandomData>();

        private class AttackRandomData
        {
            public float lastAttackTime;
            public Vector3 appliedPositionForce;
            public Vector3 appliedRotationForce;
            public float speedMultiplier;
        }

        // --------------------------
        // Configurable ranges (easy to change)
        // --------------------------

        /// <summary>
        /// Speed +/- range. Example 0.15 => speed will be sampled from [1 - 0.15, 1 + 0.15] (i.e. 0.85..1.15).
        /// </summary>
        public static float SpeedPlusMinus = 0.15f;

        /// <summary>
        /// Position soft force per-axis maximum (applied to spring system).
        /// These values add to the weapon's position through the spring system.
        /// Reasonable defaults for visible but not extreme variation.
        /// </summary>
        public static Vector3 PositionForcePlusMinus = new Vector3(0.03f, 0.03f, 0.02f);

        /// <summary>
        /// Rotation soft force per-axis maximum (degrees, applied to spring system).
        /// These values add angular force through the spring system.
        /// </summary>
        public static Vector3 RotationForcePlusMinus = new Vector3(8f, 8f, 5f);

        /// <summary>
        /// Number of frames over which to apply the soft force (smoothing).
        /// Higher = smoother but slower effect. Lower = more immediate.
        /// </summary>
        public static int SoftForceFrames = 15;

        /// <summary>
        /// Whether to affect all melee weapons (true) or only machete-type (false).
        /// </summary>
        public static bool AffectAllMeleeWeapons = true;

        // --------------------------
        // Helper: sample symmetric range
        // --------------------------
        private static float SampleSymmetric(float halfRange)
        {
            // returns value in [-halfRange, +halfRange]
            return (float)((random.NextDouble() * 2.0 - 1.0) * halfRange);
        }

        /// <summary>
        /// Check if the held item is a melee weapon we want to randomize.
        /// </summary>
        private static bool IsMeleeWeaponToRandomize(ItemClass holdingItem)
        {
            if (holdingItem == null) return false;

            string itemName = holdingItem.GetItemName().ToLower();

            // If affecting all melee, check for melee action
            if (AffectAllMeleeWeapons)
            {
                // Check if any action is a melee action
                if (holdingItem.Actions != null)
                {
                    foreach (var action in holdingItem.Actions)
                    {
                        if (action is ItemActionDynamicMelee || action is ItemActionMelee)
                            return true;
                    }
                }
                return false;
            }

            // Otherwise just check for machete/blade types
            bool isMachete = itemName.Contains("machete");
            if (!isMachete && holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("blade,knife,sword")))
            {
                isMachete = itemName.Contains("blade") || itemName.Contains("knife") || itemName.Contains("sword");
            }
            return isMachete;
        }

        /// <summary>
        /// PRIMARY PATCH: Hook into the FPV melee attack system to inject random forces.
        /// 
        /// The vp_FPWeaponMeleeAttack.UpdateAttack method is called each frame during an attack.
        /// When a new swing starts, the game calls m_Weapon.AddSoftForce() with swing forces.
        /// We patch this to add additional random forces at the same time.
        /// </summary>
        [HarmonyPatch(typeof(vp_FPWeaponMeleeAttack))]
        [HarmonyPatch("UpdateAttack")]
        public class Patch_FPVMeleeRandomForce
        {
            /// <summary>
            /// Prefix runs before the original method. We check if a new swing is about to start
            /// (same conditions the game uses) and if so, prepare to inject random forces.
            /// </summary>
            static void Prefix(vp_FPWeaponMeleeAttack __instance, ref bool __state)
            {
                __state = false; // Track whether we should apply forces in Postfix

                try
                {
                    if (__instance == null) return;

                    var player = __instance.Player;
                    if (player == null || !player.Attack.Active || player.SetWeapon.Active)
                        return;

                    var weapon = __instance.m_Weapon;
                    if (weapon == null || !weapon.Wielded)
                        return;

                    // Check if this is a NEW swing (not already swinging)
                    if (Time.time < __instance.m_NextAllowedSwingTime)
                        return;

                    // Get local player and check weapon type
                    var localPlayer = GameManager.Instance?.World?.GetPrimaryPlayer();
                    if (localPlayer == null || localPlayer.inventory?.holdingItem == null)
                        return;

                    if (!IsMeleeWeaponToRandomize(localPlayer.inventory.holdingItem))
                        return;

                    // A new swing is about to start - mark for postfix processing
                    __state = true;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Prefix error: {ex.Message}");
                }
            }

            /// <summary>
            /// Postfix runs after the original method. If a new swing started, inject random forces.
            /// </summary>
            static void Postfix(vp_FPWeaponMeleeAttack __instance, bool __state)
            {
                if (!__state) return; // Only proceed if Prefix detected a new swing

                try
                {
                    var weapon = __instance.m_Weapon;
                    if (weapon == null) return;

                    var localPlayer = GameManager.Instance?.World?.GetPrimaryPlayer();
                    if (localPlayer == null) return;

                    int entityId = localPlayer.entityId;

                    // Generate random force variations
                    Vector3 randomPosForce = new Vector3(
                        SampleSymmetric(PositionForcePlusMinus.x),
                        SampleSymmetric(PositionForcePlusMinus.y),
                        SampleSymmetric(PositionForcePlusMinus.z)
                    );

                    Vector3 randomRotForce = new Vector3(
                        SampleSymmetric(RotationForcePlusMinus.x),
                        SampleSymmetric(RotationForcePlusMinus.y),
                        SampleSymmetric(RotationForcePlusMinus.z)
                    );

                    // Apply the random forces via the spring system
                    // This adds ON TOP of the existing swing forces
                    weapon.AddSoftForce(randomPosForce, randomRotForce, SoftForceFrames);

                    // Store the data for debugging/tracking
                    entityAttackData[entityId] = new AttackRandomData
                    {
                        lastAttackTime = Time.time,
                        appliedPositionForce = randomPosForce,
                        appliedRotationForce = randomRotForce,
                        speedMultiplier = 1f // Speed handled separately in animator patch
                    };

                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Applied FPV random forces:");
                    UnityEngine.Debug.Log($"  Position Force: {randomPosForce}");
                    UnityEngine.Debug.Log($"  Rotation Force: {randomRotForce}");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Postfix error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// SECONDARY PATCH: Randomize animation speed through the animator system.
        /// This patch modifies the MeleeAttackSpeed animator parameter which controls
        /// how fast the attack animation plays.
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateEnter")]
        public class Patch_RandomizeMeleeSpeed
        {
            static void Postfix(
                Animator animator,
                ref float ___originalMeleeAttackSpeed,
                ref EntityAlive ___entity,
                ref int ___actionIndex)
            {
                try
                {
                    if (___entity == null || ___entity.inventory?.holdingItem == null)
                        return;

                    ItemClass holdingItem = ___entity.inventory.holdingItem;

                    if (!IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Only randomize primary attacks (index 0)
                    if (___actionIndex != 0)
                        return;

                    // Generate random speed multiplier
                    float speedMultiplier = 1f + SampleSymmetric(SpeedPlusMinus);

                    // Clamp to reasonable range
                    speedMultiplier = Mathf.Clamp(speedMultiplier, 0.7f, 1.3f);

                    // Apply to animator
                    float randomizedSpeed = ___originalMeleeAttackSpeed * speedMultiplier;
                    animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);

                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Speed: {speedMultiplier:F2}x " +
                        $"(from {___originalMeleeAttackSpeed:F2} to {randomizedSpeed:F2})");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Speed patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Periodic cleanup of old entity data to prevent memory leaks.
        /// Called periodically from game update.
        /// </summary>
        public static void CleanupOldData()
        {
            float currentTime = Time.time;
            List<int> toRemove = new List<int>();

            foreach (var kvp in entityAttackData)
            {
                // Remove data older than 10 seconds
                if (currentTime - kvp.Value.lastAttackTime > 10f)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (int id in toRemove)
            {
                entityAttackData.Remove(id);
            }
        }
    }
}
