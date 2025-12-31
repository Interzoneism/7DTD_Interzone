using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                    if (!isMachete && holdingItem.HasAnyTags(FastTags.Parse("blade,knife,sword")))
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
                    
                    // Generate new random values for this attack
                    RandomizationData randData = new RandomizationData
                    {
                        // Speed: Random between 0.8x and 1.5x
                        speedMultiplier = (float)(0.8 + random.NextDouble() * 0.7),
                        
                        // Position offset: Small random offsets in all axes
                        positionOffset = new Vector3(
                            (float)(random.NextDouble() * 0.2 - 0.1),  // -0.1 to +0.1
                            (float)(random.NextDouble() * 0.2 - 0.1),  // -0.1 to +0.1
                            (float)(random.NextDouble() * 0.15 - 0.075) // -0.075 to +0.075
                        ),
                        
                        // Rotation offset: Random rotation angles (in degrees)
                        rotationOffset = new Vector3(
                            (float)(random.NextDouble() * 30 - 15),    // Pitch: -15 to +15 degrees
                            (float)(random.NextDouble() * 20 - 10),    // Yaw: -10 to +10 degrees
                            (float)(random.NextDouble() * 40 - 20)     // Roll: -20 to +20 degrees
                        ),
                        
                        lastAttackTime = currentTime
                    };
                    
                    entityRandomData[entityId] = randData;
                    
                    // Apply randomized speed
                    float randomizedSpeed = ___originalMeleeAttackSpeed * randData.speedMultiplier;
                    animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);
                    
                    // Log the randomization for debugging (can be disabled in production)
                    Log.Out($"[MacheteAnimRandomizer] Randomized {itemName} attack for entity {entityId}:");
                    Log.Out($"  Speed: {randData.speedMultiplier:F2}x (from {___originalMeleeAttackSpeed:F2} to {randomizedSpeed:F2})");
                    Log.Out($"  Position offset: {randData.positionOffset}");
                    Log.Out($"  Rotation offset: {randData.rotationOffset}");
                }
                catch (Exception ex)
                {
                    Log.Error($"[MacheteAnimRandomizer] Error in Patch_RandomizeMacheteAnimation: {ex.Message}");
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
                    Log.Out("[MacheteAnimRandomizer] Weapon offset system initialized");
                }
                catch (Exception ex)
                {
                    Log.Error($"[MacheteAnimRandomizer] Error in Patch_WeaponTransformOffset: {ex.Message}");
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
                    if (___entity.emodel?.GetWeaponTransform() != null)
                    {
                        Transform weaponTransform = ___entity.emodel.GetWeaponTransform();
                        
                        // Apply position offset (additive to existing position)
                        Vector3 baseLocalPos = weaponTransform.localPosition;
                        weaponTransform.localPosition = baseLocalPos + randData.positionOffset * 0.3f;
                        
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
                        Log.Warning($"[MacheteAnimRandomizer] Error applying transform: {ex.Message}");
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
                        if (___entity.emodel?.GetWeaponTransform() != null)
                        {
                            Transform weaponTransform = ___entity.emodel.GetWeaponTransform();
                            
                            // Reset position by removing our offset
                            Vector3 currentPos = weaponTransform.localPosition;
                            weaponTransform.localPosition = currentPos - entityRandomData[entityId].positionOffset * 0.3f;
                            
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
                    Log.Warning($"[MacheteAnimRandomizer] Error in cleanup: {ex.Message}");
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
