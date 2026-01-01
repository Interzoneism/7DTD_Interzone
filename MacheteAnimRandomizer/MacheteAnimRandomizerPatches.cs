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
        public static float SpeedPlusMinus = 0.02f;

        /// <summary>
        /// Position soft force per-axis maximum (applied to spring system).
        /// These values add to the weapon's position through the spring system.
        /// Reasonable defaults for visible but not extreme variation.
        /// </summary>
        public static Vector3 PositionForcePlusMinus = new Vector3(0.09f, 0.13f, 0.08f);

        /// <summary>
        /// Rotation soft force per-axis maximum (degrees, applied to spring system).
        /// These values add angular force through the spring system.
        /// </summary>
        public static Vector3 RotationForcePlusMinus = new Vector3(34f, 80f, 52f);

        /// <summary>
        /// Number of frames over which to apply the soft force (smoothing).
        /// Higher = smoother but slower effect. Lower = more immediate.
        /// </summary>
        public static int SoftForceFrames = 3;

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
        /// We patch vp_FPWeapon.AddSoftForce directly and modify the forces being applied
        /// to add randomization before they reach the spring system.
        /// 
        /// NOTE: AddSoftForce has 4 overloads, we target the one with signature:
        /// AddSoftForce(Vector3 positional, Vector3 angular, int frames)
        /// </summary>
        [HarmonyPatch(typeof(vp_FPWeapon))]
        [HarmonyPatch("AddSoftForce", new Type[] { typeof(Vector3), typeof(Vector3), typeof(int) })]
        public class Patch_FPVMeleeRandomForce
        {
            private static float lastTriggerTime = 0f;
            private static readonly float cooldown = 0.1f; // Prevent multiple triggers per swing
            private static bool isApplyingRandomForce = false; // Prevent recursion

            /// <summary>
            /// Prefix modifies the forces before they are applied to add randomization.
            /// </summary>
            static void Prefix(vp_FPWeapon __instance, ref Vector3 PositionForce, ref Vector3 RotationForce, ref int Frames)
            {
                try
                {
                    // Prevent recursion if we're already applying random forces
                    if (isApplyingRandomForce)
                        return;

                    // Debug: Log all AddSoftForce calls to see what's happening
                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] AddSoftForce called: Pos={PositionForce}, Rot={RotationForce}, Frames={Frames}, RotMag={RotationForce.magnitude}");

                    // Only trigger once per swing (cooldown prevents double-application)
                    if (Time.time - lastTriggerTime < cooldown)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Skipped - cooldown active ({Time.time - lastTriggerTime:F3}s ago)");
                        return;
                    }

                    // Check if this is a melee weapon swing (swings have rotation force)
                    if (RotationForce.magnitude < 0.1f)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Skipped - rotation magnitude too low: {RotationForce.magnitude}");
                        return;
                    }

                    // Get local player and check weapon type
                    var localPlayer = GameManager.Instance?.World?.GetPrimaryPlayer();
                    if (localPlayer == null || localPlayer.inventory?.holdingItem == null)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Skipped - no player or holding item");
                        return;
                    }

                    ItemClass holdingItem = localPlayer.inventory.holdingItem;
                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Holding item: {holdingItem.GetItemName()}");

                    if (!IsMeleeWeaponToRandomize(holdingItem))
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Skipped - not a melee weapon to randomize");
                        return;
                    }

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

                    // ADD random forces to the existing forces (modify by reference)
                    Vector3 originalPos = PositionForce;
                    Vector3 originalRot = RotationForce;

                    PositionForce += randomPosForce;
                    RotationForce += randomRotForce;

                    lastTriggerTime = Time.time;

                    // Store the data for debugging/tracking
                    entityAttackData[entityId] = new AttackRandomData
                    {
                        lastAttackTime = Time.time,
                        appliedPositionForce = randomPosForce,
                        appliedRotationForce = randomRotForce,
                        speedMultiplier = 1f // Speed handled separately in animator patch
                    };

                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] ===== RANDOMIZATION APPLIED =====");
                    UnityEngine.Debug.Log($"  Random Position Force: {randomPosForce}");
                    UnityEngine.Debug.Log($"  Random Rotation Force: {randomRotForce}");
                    UnityEngine.Debug.Log($"  Original Swing: Pos={originalPos}, Rot={originalRot}");
                    UnityEngine.Debug.Log($"  Final Forces: Pos={PositionForce}, Rot={RotationForce}");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] AddSoftForce patch error: {ex}");
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
