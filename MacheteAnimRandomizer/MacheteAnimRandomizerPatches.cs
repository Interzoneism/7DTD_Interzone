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

                    // Apply randomized speed
                    float randomizedSpeed = ___originalMeleeAttackSpeed * randData.speedMultiplier;
                    animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);

                    // IMPORTANT: Directly apply to heldItemTransform for immediate visual effect
                    // This is critical because SetInRightHand is not called during attacks
                    AvatarController avatarController = ___entity.emodel?.avatarController;
                    if (avatarController != null)
                    {
                        Transform heldTransform = avatarController.GetRightHandTransform();
                        if (heldTransform != null && heldTransform.childCount > 0)
                        {
                            Transform weaponTransform = heldTransform.GetChild(0);
                            if (weaponTransform != null)
                            {
                                // Apply position offset (additive to current position)
                                weaponTransform.localPosition += randData.positionOffset;

                                // Apply rotation offset (additive to current rotation)
                                weaponTransform.localRotation *= Quaternion.Euler(randData.rotationOffset);

                                UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Applied transform to weapon:");
                                UnityEngine.Debug.Log($"  New Position: {weaponTransform.localPosition}");
                                UnityEngine.Debug.Log($"  New Rotation: {weaponTransform.localEulerAngles}");
                            }
                        }
                    }

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
                        // Apply position offset (additive)
                        _transform.localPosition += randData.positionOffset;

                        // Apply rotation offset (multiplicative)
                        _transform.localRotation *= Quaternion.Euler(randData.rotationOffset);

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
        /// Patch OnStateUpdate to continuously apply position and rotation offsets during the attack animation.
        /// This ensures the randomization is maintained throughout the animation.
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateUpdate")]
        public class Patch_MaintainOffsetsDuringAnimation
        {
            static void Postfix(
                AnimatorMeleeAttackState __instance,
                Animator animator,
                AnimatorStateInfo stateInfo,
                int layerIndex,
                ref EntityAlive ___entity)
            {
                try
                {
                    if (___entity == null)
                        return;

                    int entityId = ___entity.entityId;

                    // Check if we have active randomization data for this entity
                    if (!entityRandomData.ContainsKey(entityId))
                        return;

                    RandomizationData randData = entityRandomData[entityId];

                    // Check if this is during an attack (recent randomization)
                    float timeSinceAttack = Time.time - randData.lastAttackTime;
                    if (timeSinceAttack > 2f) // Only apply if attack was within last 2 seconds
                        return;

                    // Continuously apply offsets to maintain them during animation
                    AvatarController avatarController = ___entity.emodel?.avatarController;
                    if (avatarController != null)
                    {
                        Transform heldTransform = avatarController.GetRightHandTransform();
                        if (heldTransform != null && heldTransform.childCount > 0)
                        {
                            Transform weaponTransform = heldTransform.GetChild(0);
                            if (weaponTransform != null && !___entity.emodel.IsFPV)
                            {
                                // Check if the weapon position has been reset (distance from expected)
                                Vector3 expectedPos = randData.originalPosition + randData.positionOffset;
                                Vector3 expectedRot = randData.originalRotation + randData.rotationOffset;

                                // If position or rotation deviated significantly, reapply
                                float positionDiff = Vector3.Distance(weaponTransform.localPosition, expectedPos);
                                float rotationDiff = Quaternion.Angle(weaponTransform.localRotation, Quaternion.Euler(expectedRot));

                                if (positionDiff > 0.01f || rotationDiff > 1f)
                                {
                                    weaponTransform.localPosition = expectedPos;
                                    weaponTransform.localRotation = Quaternion.Euler(expectedRot);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Silently fail to avoid spam during Update
                }
            }
        }

        /// <summary>
        /// Clean up randomization data when animation state exits
        /// to prevent memory leaks and restore weapon to default position.
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateExit")]
        public class Patch_CleanupOnExit
        {
            static void Postfix(
                AnimatorMeleeAttackState __instance,
                Animator animator,
                AnimatorStateInfo stateInfo,
                int layerIndex,
                ref EntityAlive ___entity)
            {
                try
                {
                    if (___entity == null)
                        return;

                    int entityId = ___entity.entityId;

                    // Restore original weapon transform
                    if (entityRandomData.ContainsKey(entityId))
                    {
                        RandomizationData randData = entityRandomData[entityId];

                        // Restore the weapon transform to original position/rotation
                        AvatarController avatarController = ___entity.emodel?.avatarController;
                        if (avatarController != null)
                        {
                            Transform heldTransform = avatarController.GetRightHandTransform();
                            if (heldTransform != null && heldTransform.childCount > 0)
                            {
                                Transform weaponTransform = heldTransform.GetChild(0);
                                if (weaponTransform != null)
                                {
                                    // Restore to the original AnimationGunjointOffsetData position/rotation
                                    if (!___entity.emodel.IsFPV || ___entity.isEntityRemote)
                                    {
                                        weaponTransform.localPosition = randData.originalPosition;
                                        weaponTransform.localRotation = Quaternion.Euler(randData.originalRotation);

                                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Restored weapon transform for entity {entityId}");
                                    }
                                }
                            }
                        }

                        // Mark as old by setting a very old time
                        entityRandomData[entityId].lastAttackTime = Time.time - 1000f;
                    }
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Error in cleanup: {ex.Message}");
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
