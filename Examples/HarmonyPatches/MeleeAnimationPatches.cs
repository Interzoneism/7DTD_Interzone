using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MeleeAnimationMod
{
    /// <summary>
    /// Example Harmony patches for modifying melee weapon animations in 7 Days to Die.
    /// These patches demonstrate various techniques for customizing attack behavior,
    /// animation speed, timing, and visual effects.
    /// 
    /// To use these patches:
    /// 1. Reference 0Harmony.dll (from TFP_Harmony mod)
    /// 2. Reference Assembly-CSharp.dll (from game files)
    /// 3. Create a ModApi class that initializes Harmony
    /// 4. Enable/disable specific patches as needed
    /// </summary>
    public class MeleeAnimationPatches
    {
        /// <summary>
        /// Initialize all Harmony patches. Call this from your ModApi.InitMod() method.
        /// </summary>
        public static void Initialize(Mod modInstance)
        {
            var harmony = new Harmony("com.example.meleeanimmod");
            
            // Apply all patches in this assembly
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Log.Out("[MeleeAnimMod] Harmony patches applied successfully");
        }
    }

    // ============================================================================
    // EXAMPLE 1: Modify Attack Speed
    // ============================================================================
    
    /// <summary>
    /// Doubles the attack speed for all melee weapons.
    /// This patch modifies the animation speed multiplier when entering the attack state.
    /// </summary>
    [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
    [HarmonyPatch("OnStateEnter")]
    public class Patch_DoubleAttackSpeed
    {
        static void Postfix(
            AnimatorMeleeAttackState __instance,
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            // Access private field using Harmony's AccessTools
            var originalSpeedField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), 
                "originalMeleeAttackSpeed"
            );
            
            float originalSpeed = (float)originalSpeedField.GetValue(__instance);
            float modifiedSpeed = originalSpeed * 2f; // Double the speed
            
            // Apply modified speed to animator
            animator.SetFloat("MeleeAttackSpeed", modifiedSpeed);
            
            // Optional: Log for debugging
            // Log.Out($"[MeleeAnimMod] Modified attack speed from {originalSpeed} to {modifiedSpeed}");
        }
    }

    // ============================================================================
    // EXAMPLE 2: Conditional Speed Boost Based on Weapon Type
    // ============================================================================
    
    /// <summary>
    /// Increases attack speed by 50% for weapons with the "fastattack" tag.
    /// Demonstrates how to apply conditional modifications based on item properties.
    /// </summary>
    [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
    [HarmonyPatch("OnStateEnter")]
    public class Patch_ConditionalSpeedBoost
    {
        static void Postfix(
            AnimatorMeleeAttackState __instance,
            Animator animator)
        {
            // Get the entity from the animator
            var entityField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), 
                "entity"
            );
            EntityAlive entity = (EntityAlive)entityField.GetValue(__instance);
            
            if (entity != null && entity.inventory?.holdingItem != null)
            {
                // Check if weapon has special tag
                if (entity.inventory.holdingItem.HasTag(FastTags.Parse("fastattack")))
                {
                    var originalSpeedField = AccessTools.Field(
                        typeof(AnimatorMeleeAttackState), 
                        "originalMeleeAttackSpeed"
                    );
                    
                    float originalSpeed = (float)originalSpeedField.GetValue(__instance);
                    float boostedSpeed = originalSpeed * 1.5f; // 50% faster
                    
                    animator.SetFloat("MeleeAttackSpeed", boostedSpeed);
                    
                    Log.Out($"[MeleeAnimMod] Applied fast attack boost to {entity.inventory.holdingItem.Name}");
                }
            }
        }
    }

    // ============================================================================
    // EXAMPLE 3: Modify Hit Detection Timing
    // ============================================================================
    
    /// <summary>
    /// Makes hits register earlier in the attack animation (at 20% instead of 30%).
    /// Useful for making weapons feel more responsive.
    /// </summary>
    [HarmonyPatch(typeof(ItemActionMelee))]
    [HarmonyPatch("ExecuteAction")]
    public class Patch_EarlierHitDetection
    {
        static void Prefix(ItemActionMelee __instance)
        {
            // Access the private rayCastDelay field
            var rayCastDelayField = AccessTools.Field(
                typeof(ItemActionMelee), 
                "rayCastDelay"
            );
            
            float currentDelay = (float)rayCastDelayField.GetValue(__instance);
            
            // Reduce delay to 66% of original (hits ~20% into animation instead of ~30%)
            rayCastDelayField.SetValue(__instance, currentDelay * 0.66f);
        }
    }

    // ============================================================================
    // EXAMPLE 4: Weapon Position and Rotation Offsets
    // ============================================================================
    
    /// <summary>
    /// Modifies weapon positioning and rotation for specific hold types.
    /// This affects how the weapon appears in the player's hands during animations.
    /// </summary>
    [HarmonyPatch(typeof(AnimationGunjointOffsetData))]
    [HarmonyPatch("InitStatic")]
    public class Patch_WeaponOffsets
    {
        static void Postfix()
        {
            // Example: Adjust position and rotation for hold type 5 (two-handed melee)
            int twoHandedMeleeHoldType = 5;
            
            // Apply custom rotation (15 degrees pitch, -5 degrees yaw)
            AnimationGunjointOffsetData.AnimationGunjointOffset[twoHandedMeleeHoldType].rotation = 
                new Vector3(15f, -5f, 0f);
            
            // Apply custom position offset (slightly lower and forward)
            AnimationGunjointOffsetData.AnimationGunjointOffset[twoHandedMeleeHoldType].position = 
                new Vector3(0.05f, -0.05f, 0.1f);
            
            Log.Out($"[MeleeAnimMod] Applied custom weapon offsets for hold type {twoHandedMeleeHoldType}");
        }
    }

    // ============================================================================
    // EXAMPLE 5: Custom Impact Slowdown Effect
    // ============================================================================
    
    /// <summary>
    /// Creates a more dramatic "impact pause" when hitting an enemy.
    /// Useful for making combat feel more visceral and impactful.
    /// Note: This is an advanced example showing IEnumerator patching
    /// </summary>
    /* Commented out as it requires special Harmony transpiler handling
    [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
    [HarmonyPatch("impactStop")]
    public class Patch_DramaticImpact
    {
        static IEnumerator Postfix(
            IEnumerator result,
            AnimatorMeleeAttackState __instance,
            Animator animator,
            AnimationClip clip)
        {
            // Get private fields
            var calculatedRaycastTimeField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), "calculatedRaycastTime");
            var originalSpeedField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), "originalMeleeAttackSpeed");
            
            float calculatedRaycastTime = (float)calculatedRaycastTimeField.GetValue(__instance);
            float originalSpeed = (float)originalSpeedField.GetValue(__instance);
            
            // Dramatic slow-down
            animator.SetFloat("MeleeAttackSpeed", originalSpeed * 0.1f);
            yield return new WaitForSeconds(0.15f);
            
            // Quick recovery
            animator.SetFloat("MeleeAttackSpeed", originalSpeed * 1.1f);
            
            yield break;
        }
    }
    */

    // ============================================================================
    // EXAMPLE 6: Multi-Hit Combo System
    // ============================================================================
    
    /// <summary>
    /// Implements a simple combo system that increases damage on consecutive hits.
    /// Each hit within 2 seconds increases the damage multiplier.
    /// </summary>
    [HarmonyPatch(typeof(ItemActionDynamicMelee))]
    [HarmonyPatch("ExecuteAction")]
    public class Patch_ComboSystem
    {
        private static Dictionary<int, ComboData> playerCombos = new Dictionary<int, ComboData>();
        
        private class ComboData
        {
            public int hitCount = 0;
            public float lastHitTime = 0f;
            public const float comboWindow = 2f; // 2 second window
            public const float maxMultiplier = 2f; // Up to 2x damage
        }
        
        static void Postfix(
            ItemActionDynamicMelee __instance,
            ItemActionData _actionData,
            bool _bReleased)
        {
            if (_bReleased || _actionData.invData?.holdingEntity == null)
                return;
            
            int entityId = _actionData.invData.holdingEntity.entityId;
            
            if (!playerCombos.ContainsKey(entityId))
            {
                playerCombos[entityId] = new ComboData();
            }
            
            ComboData combo = playerCombos[entityId];
            float currentTime = Time.time;
            
            // Check if within combo window
            if (currentTime - combo.lastHitTime <= ComboData.comboWindow)
            {
                combo.hitCount++;
            }
            else
            {
                combo.hitCount = 1; // Reset combo
            }
            
            combo.lastHitTime = currentTime;
            
            // Calculate damage multiplier (caps at 2x)
            float damageMultiplier = Mathf.Min(
                1f + (combo.hitCount - 1) * 0.2f, 
                ComboData.maxMultiplier
            );
            
            // Visual feedback for player
            if (_actionData.invData.holdingEntity is EntityPlayerLocal player)
            {
                GameManager.ShowTooltip(
                    player, 
                    $"Combo x{combo.hitCount} ({damageMultiplier:F1}x damage)", 
                    false
                );
            }
        }
    }

    // ============================================================================
    // EXAMPLE 7: Stamina-Based Speed Scaling
    // ============================================================================
    
    /// <summary>
    /// Reduces attack speed when stamina is low, making combat more tactical.
    /// Attack speed scales from 50% to 100% based on current stamina percentage.
    /// </summary>
    [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
    [HarmonyPatch("OnStateEnter")]
    public class Patch_StaminaBasedSpeed
    {
        static void Postfix(
            AnimatorMeleeAttackState __instance,
            Animator animator)
        {
            var entityField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), 
                "entity"
            );
            EntityAlive entity = (EntityAlive)entityField.GetValue(__instance);
            
            if (entity?.Stats?.Stamina == null)
                return;
            
            // Get current stamina percentage (0-1)
            float staminaPercent = entity.Stats.Stamina.ValuePercent;
            
            // Scale attack speed: 50% speed at 0 stamina, 100% at full stamina
            float speedMultiplier = Mathf.Lerp(0.5f, 1.0f, staminaPercent);
            
            // Apply to original speed
            var originalSpeedField = AccessTools.Field(
                typeof(AnimatorMeleeAttackState), 
                "originalMeleeAttackSpeed"
            );
            float originalSpeed = (float)originalSpeedField.GetValue(__instance);
            
            animator.SetFloat("MeleeAttackSpeed", originalSpeed * speedMultiplier);
            
            // Visual feedback when stamina is low
            if (staminaPercent < 0.3f && entity is EntityPlayerLocal player)
            {
                GameManager.ShowTooltip(player, "Low stamina - attacks slowed!", false);
            }
        }
    }
}
