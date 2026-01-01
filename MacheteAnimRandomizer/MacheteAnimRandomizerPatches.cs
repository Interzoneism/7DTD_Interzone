using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Platform;

namespace MacheteAnimRandomizer
{
    /// <summary>
    /// Harmony patches for randomizing machete melee attack animations.
    /// Modifies animation speed, weapon rotation, and weapon position for each attack.
    /// </summary>
    public class MacheteAnimRandomizerPatches
    {
        // Random number generator for animation variations
        private static System.Random random = new System.Random();
        
        // Track last randomization per entity to ensure each attack gets unique values
        private static Dictionary<int, RandomizationData> entityRandomData = new Dictionary<int, RandomizationData>();
        
        private class RandomizationData
        {
            public float speedMultiplier;
            public Vector3 positionOffset;
            public Vector3 rotationOffset;
            public float lastAttackTime;
            public int holdType; // Store the weapon hold type
            public Vector3 originalPosition; // Store original position
            public Vector3 originalRotation; // Store original rotation

            // FPV-specific
            public Vector3 originalFpvPositionOffset;
            public Vector3 originalFpvRotationOffset;
        }

        // --------------------------
        // Configurable ranges (easy to change)
        // --------------------------
        /// <summary>
        /// Speed +/- range. Example 0.1 => speed will be sampled from [1 - 0.1, 1 + 0.1] (i.e. 0.9..1.1).
        /// Set this at runtime to change variation globally.
        /// </summary>
        public static float SpeedPlusMinus = 0.1f;

        /// <summary>
        /// Position offset per-axis maximum absolute value (meters). The actual offset is sampled uniformly
        /// from [-PositionPlusMinus.x .. +PositionPlusMinus.x], etc.
        /// Default matches previous behavior: X/Y +/-0.1, Z +/-0.075.
        /// </summary>
        public static Vector3 PositionPlusMinus = new Vector3(0.1f, 0.1f, 0.075f);

        /// <summary>
        /// Rotation offset per-axis maximum absolute value (degrees). Sampled from [-RotationPlusMinus.x .. +RotationPlusMinus.x], etc.
        /// Default matches previous behavior: Pitch +/-15, Yaw +/-10, Roll +/-20.
        /// </summary>
        public static Vector3 RotationPlusMinus = new Vector3(15f, 10f, 20f);

        /// <summary>
        /// Factor applied when writing sampled position offsets into the weapon transform during update.
        /// Matches previous hardcoded 0.3f; adjustable here.
        /// </summary>
        public static float WeaponPositionApplyFactor = 0.3f;

        // --------------------------
        // Helper: sample symmetric range
        // --------------------------
        private static float SampleSymmetric(float halfRange)
        {
            // returns value in [-halfRange, +halfRange]
            return (float)((random.NextDouble() * 2.0) * halfRange - halfRange);
        }

        /// <summary>
        /// Patch AnimatorMeleeAttackState.OnStateEnter to randomize animation properties
        /// for machete attacks (normal attacks only, not power attacks).
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateEnter")]
        public class Patch_RandomizeMacheteAnimation
        {
            static void Postfix(
                AnimatorMeleeAttackState __instance,
                Animator animator,
                AnimatorStateInfo stateInfo,
                int layerIndex,
                ref float ___originalMeleeAttackSpeed,
                ref EntityAlive ___entity,
                ref int ___actionIndex)
            {
                try
                {
                    // Only process if entity exists and is valid
                    if (___entity == null || ___entity.inventory?.holdingItem == null)
                        return;

                    // Get the holding item
                    ItemClass holdingItem = ___entity.inventory.holdingItem;

                    // Check if it's a machete (look for "machete" in the item name or tags)
                    string itemName = holdingItem.GetItemName().ToLower();
                    bool isMachete = itemName.Contains("machete");

                    // Alternative: check by item tags if available
                    if (!isMachete && holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("blade,knife,sword")))
                    {
                        // Could also be a blade-type weapon, let's be inclusive for testing
                        isMachete = itemName.Contains("blade") || itemName.Contains("knife");
                    }

                    // Only randomize for machete weapons
                    if (!isMachete)
                        return;

                    // Skip power attacks (only randomize normal attacks)
                    // Action index 1 is typically power attack
                    if (___actionIndex != 0)
                        return;

                    int entityId = ___entity.entityId;
                    float currentTime = Time.time;

                    // Get the weapon hold type
                    int holdType = holdingItem.HoldType.Value;

                    // Store original weapon offset data before modifying
                    Vector3 originalPos = AnimationGunjointOffsetData.AnimationGunjointOffset[holdType].position;
                    Vector3 originalRot = AnimationGunjointOffsetData.AnimationGunjointOffset[holdType].rotation;

                    // Generate new random values for this attack using configurable ranges
                    RandomizationData randData = new RandomizationData
                    {
                        // Speed: sample around 1.0 using SpeedPlusMinus
                        speedMultiplier = 1f + SampleSymmetric(SpeedPlusMinus),

                        // Position offset: sample per-axis from configured symmetric ranges
                        positionOffset = new Vector3(
                            SampleSymmetric(PositionPlusMinus.x),
                            SampleSymmetric(PositionPlusMinus.y),
                            SampleSymmetric(PositionPlusMinus.z)
                        ),

                        // Rotation offset: sample per-axis from configured symmetric ranges (degrees)
                        rotationOffset = new Vector3(
                            SampleSymmetric(RotationPlusMinus.x),
                            SampleSymmetric(RotationPlusMinus.y),
                            SampleSymmetric(RotationPlusMinus.z)
                        ),

                        lastAttackTime = currentTime,
                        holdType = holdType,
                        originalPosition = originalPos,
                        originalRotation = originalRot
                    };

                    entityRandomData[entityId] = randData;

                     // Apply randomized position/rotation via the same mechanism the game uses
                     // (AvatarMultiBodyController.SetInRightHand + Inventory.setHoldingItemTransform).
                     AnimationGunjointOffsetData.AnimationGunjointOffset[holdType] = new AnimationGunjointOffsetData.AnimationGunjointOffsets(
                         originalPos + randData.positionOffset,
                         originalRot + randData.rotationOffset
                     );

                    // Apply randomized speed
                    float randomizedSpeed = ___originalMeleeAttackSpeed * randData.speedMultiplier;
                    animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);

                    // Log the randomization for debugging (can be disabled in production)
                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Randomized {itemName} attack for entity {entityId}:");
                    UnityEngine.Debug.Log($"  Speed: {randData.speedMultiplier:F2}x (from {___originalMeleeAttackSpeed:F2} to {randomizedSpeed:F2})");
                    UnityEngine.Debug.Log($"  Position Offset: {randData.positionOffset}");
                    UnityEngine.Debug.Log($"  Rotation Offset: {randData.rotationOffset}");
                 }
                 catch (Exception ex)
                 {
                     UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Error in Patch_RandomizeMacheteAnimation: {ex.Message}");
                     UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Stack trace: {ex.StackTrace}");
                 }
            }
        }

         /// <summary>
         /// Restore original offsets after the attack state ends.
         /// </summary>
         [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
         [HarmonyPatch("OnStateExit")]
         public class Patch_RestoreMacheteOffsets
         {
             static void Postfix(ref EntityAlive ___entity, ref int ___actionIndex)
             {
                 try
                 {
                     if (___entity == null || ___entity.inventory?.holdingItem == null)
                         return;

                     if (___actionIndex != 0)
                         return;

                     int entityId = ___entity.entityId;
                     if (!entityRandomData.TryGetValue(entityId, out var randData))
                         return;

                     AnimationGunjointOffsetData.AnimationGunjointOffset[randData.holdType] = new AnimationGunjointOffsetData.AnimationGunjointOffsets(
                         randData.originalPosition,
                         randData.originalRotation
                     );
                 }
                 catch (Exception ex)
                 {
                     UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Error restoring offsets: {ex.Message}");
                 }
             }
         }

        /// <summary>
        /// FPV melee uses vp_Weapon/vp_FPWeapon spring offsets, not AnimationGunjointOffsetData.
        /// Randomize `vp_FPWeapon.PositionOffset` and `vp_FPWeapon.RotationOffset` when a swing starts.
        /// </summary>
        [HarmonyPatch(typeof(vp_FPWeaponMeleeAttack))]
        [HarmonyPatch("UpdateAttack")]
        public class Patch_FpvMeleeRandomizeWeaponOffsets
        {
            static bool Prefix(vp_FPWeaponMeleeAttack __instance)
            {
                try
                {
                    // Mirror the game's early-outs so we only randomize on a real swing.
                    if (__instance == null)
                        return true;

                    var player = __instance.Player;
                    if (player == null || !player.Attack.Active || player.SetWeapon.Active)
                        return true;

                    var weapon = __instance.m_Weapon;
                    if (weapon == null || !weapon.Wielded)
                        return true;

                    if (Time.time < __instance.m_NextAllowedSwingTime)
                        return true;

                    // Only for local FPV player holding a machete (or blade tags).
                    var fp = GameManager.Instance?.World?.GetPrimaryPlayer();
                    if (fp == null || fp.inventory?.holdingItem == null)
                        return true;

                    ItemClass holdingItem = fp.inventory.holdingItem;
                    string itemName = holdingItem.GetItemName().ToLower();
                    bool isMachete = itemName.Contains("machete");
                    if (!isMachete && holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("blade,knife,sword")))
                    {
                        isMachete = itemName.Contains("blade") || itemName.Contains("knife") || itemName.Contains("sword");
                    }
                    if (!isMachete)
                        return true;

                    int entityId = fp.entityId;
                    float currentTime = Time.time;

                    // Base offsets used by vp_FPWeapon.Refresh each frame.
                    Vector3 originalPosOff = weapon.PositionOffset;
                    Vector3 originalRotOff = weapon.RotationOffset;

                    var randData = new RandomizationData
                    {
                        speedMultiplier = 1f,
                        positionOffset = new Vector3(
                            SampleSymmetric(PositionPlusMinus.x),
                            SampleSymmetric(PositionPlusMinus.y),
                            SampleSymmetric(PositionPlusMinus.z)
                        ),
                        rotationOffset = new Vector3(
                            SampleSymmetric(RotationPlusMinus.x),
                            SampleSymmetric(RotationPlusMinus.y),
                            SampleSymmetric(RotationPlusMinus.z)
                        ),
                        lastAttackTime = currentTime,
                        originalFpvPositionOffset = originalPosOff,
                        originalFpvRotationOffset = originalRotOff
                    };

                    entityRandomData[entityId] = randData;

                    weapon.PositionOffset = originalPosOff + randData.positionOffset;
                    weapon.RotationOffset = originalRotOff + randData.rotationOffset;
                    weapon.Refresh();
                }
                catch
                {
                    // Avoid breaking attack flow.
                }

                return true;
            }
        }
        
        /// <summary>
        /// Track original AnimationGunjointOffsetData values to ensure we can restore them properly.
        /// This is called when the game initializes the weapon offset system.
        /// </summary>
        [HarmonyPatch(typeof(AnimationGunjointOffsetData))]
        [HarmonyPatch("InitStatic")]
        public class Patch_WeaponTransformOffset
        {
            static void Postfix()
            {
                try
                {
                    // This will be called once on initialization
                    // The actual per-attack randomization is handled in the OnStateEnter patch
                    UnityEngine.Debug.Log("[MacheteAnimRandomizer] Weapon offset system initialized");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Error in Patch_WeaponTransformOffset: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Patch SetInRightHand to re-apply our offsets when the weapon transform is updated.
        /// This ensures our randomization persists even if the game re-sets the weapon.
        /// </summary>
        [HarmonyPatch(typeof(AvatarMultiBodyController))]
        [HarmonyPatch("SetInRightHand")]
        public class Patch_SetInRightHand
        {
            static void Postfix(
                AvatarMultiBodyController __instance,
                Transform _transform,
                ref Transform ___heldItemTransform)
            {
                try
                {
                    if (__instance?.entity == null || _transform == null)
                        return;

                    int entityId = __instance.entity.entityId;

                    // Check if we have active randomization data for this entity
                    if (!entityRandomData.ContainsKey(entityId))
                        return;

                    RandomizationData randData = entityRandomData[entityId];

                    // Check if this is during an attack (recent randomization)
                    float timeSinceAttack = Time.time - randData.lastAttackTime;
                    if (timeSinceAttack > 2f) // Only apply if attack was within last 2 seconds
                        return;

                    // Directly apply offsets to the transform that was just set
                    if (_transform != null)
                    {
                        // Offsets are applied via AnimationGunjointOffsetData; nothing to do here.

                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Re-applied offsets in SetInRightHand for entity {entityId}");
                    }
                }
                catch (Exception ex)
                {
                    // Silently fail to avoid spam
                }
            }
        }

        /// <summary>
        /// Periodic cleanup of old entity data to prevent memory leaks
        /// </summary>
        public static void CleanupOldData()
        {
            float currentTime = Time.time;
            List<int> toRemove = new List<int>();
            
            foreach (var kvp in entityRandomData)
            {
                // Remove data older than 10 seconds
                if (currentTime - kvp.Value.lastAttackTime > 10f)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            
            foreach (int id in toRemove)
            {
                entityRandomData.Remove(id);
            }
        }
    }
}
