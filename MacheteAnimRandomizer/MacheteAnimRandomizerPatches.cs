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
                        
                        lastAttackTime = currentTime
                    };
                    
                    entityRandomData[entityId] = randData;
                    
                        // Apply randomized speed
                        float randomizedSpeed = ___originalMeleeAttackSpeed * randData.speedMultiplier;
                        animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);

                        // Log the randomization for debugging (can be disabled in production)
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Randomized {itemName} attack for entity {entityId}:");
                        UnityEngine.Debug.Log($"  Speed: {randData.speedMultiplier:F2}x (from {___originalMeleeAttackSpeed:F2} to {randomizedSpeed:F2})");
                        UnityEngine.Debug.Log($"  Position offset: {randData.positionOffset}");
                        UnityEngine.Debug.Log($"  Rotation offset: {randData.rotationOffset}");
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Error in Patch_RandomizeMacheteAnimation: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Patch to apply weapon position and rotation offsets during the animation.
        /// This modifies the weapon's transform in the player's hands.
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
        /// Alternative approach: Patch the update loop to continuously apply transform changes
        /// during the attack animation. This provides more dynamic visual variation.
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateUpdate")]
        public class Patch_ApplyTransformDuringAnimation
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
                    
                    // Check if we have randomization data for this entity
                    if (!entityRandomData.ContainsKey(entityId))
                        return;
                    
                    RandomizationData randData = entityRandomData[entityId];
                    
                    // Get the weapon transform if available
                    Transform weaponTransform = ___entity.emodel?.GetRightHandTransform();

                    if (weaponTransform != null)
                    {
                        // Apply position offset (additive to existing position) using configured apply factor
                        Vector3 baseLocalPos = weaponTransform.localPosition;
                        weaponTransform.localPosition = baseLocalPos + randData.positionOffset * WeaponPositionApplyFactor;
                        
                        // Apply rotation offset (additive to existing rotation)
                        Quaternion baseLocalRot = weaponTransform.localRotation;
                        Quaternion offsetRot = Quaternion.Euler(randData.rotationOffset);
                        weaponTransform.localRotation = baseLocalRot * offsetRot;
                    }
                }
                catch (Exception ex)
                {
                    // Silently fail in update loop to avoid spam
                    // Only log once per entity
                    if (!loggedErrors.Contains(___entity?.entityId ?? -1))
                    {
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Error applying transform: {ex.Message}");
                        if (___entity != null)
                            loggedErrors.Add(___entity.entityId);
                    }
                }
            }
            
            private static HashSet<int> loggedErrors = new HashSet<int>();
        }
        
        /// <summary>
        /// Clean up randomization data when animation state exits
        /// to prevent memory leaks.
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
                    
                    // Reset weapon transform to original state
                    if (entityRandomData.ContainsKey(entityId))
                    {
                        Transform weaponTransform = ___entity.emodel?.GetRightHandTransform();

                        if (weaponTransform != null)
                        {
                            // Reset position by removing our offset (use same factor as when applied)
                            Vector3 currentPos = weaponTransform.localPosition;
                            weaponTransform.localPosition = currentPos - entityRandomData[entityId].positionOffset * WeaponPositionApplyFactor;
                            
                            // Reset rotation by removing our offset
                            Quaternion offsetRot = Quaternion.Euler(entityRandomData[entityId].rotationOffset);
                            weaponTransform.localRotation = weaponTransform.localRotation * Quaternion.Inverse(offsetRot);
                        }
                        
                        // Don't remove the data immediately - keep it for potential debugging
                        // Just mark it as old by setting a very old time
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
