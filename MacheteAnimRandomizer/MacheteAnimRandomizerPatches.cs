using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MacheteAnimRandomizer
{
    /// <summary>
    /// Harmony patches for randomizing melee attack animations in first-person view.
    /// 
    /// TECHNICAL ANALYSIS (from decompiled 7DTD source code):
    /// 
    /// 1. vp_FPWeapon uses a SPRING-BASED physics system with:
    ///    - m_PositionSpring: Controls weapon position via spring physics
    ///    - m_RotationSpring: Controls weapon rotation via spring physics
    ///    - AddSoftForce(Vector3 positional, Vector3 angular, int frames): Applies forces over time
    /// 
    /// 2. vp_FPWeaponMeleeAttack.UpdateAttack() calls:
    ///    m_Weapon.AddSoftForce(SwingPositionSoftForce, SwingRotationSoftForce, SwingSoftForceFrames)
    ///    Default values: SwingPositionSoftForce=(-0.5,-0.1,0.3), SwingRotationSoftForce=(50,-25,0)
    /// 
    /// 3. AnimatorMeleeAttackState controls animation speed via "MeleeAttackSpeed" animator parameter.
    /// 
    /// 4. ItemActionDynamicMelee.StartAttack() initiates attacks and can be used for detection.
    /// 
    /// SOLUTION:
    /// We hook into the spring force application to add random variation to melee swings.
    /// This works WITH the animation system rather than fighting it.
    /// </summary>
    public class MacheteAnimRandomizerPatches
    {
        // Random number generator for animation variations
        private static System.Random random = new System.Random();

        // Track when attacks start to prevent multiple randomizations per attack
        private static float lastAttackTime = 0f;
        private static float attackCooldown = 0.15f; // Minimum time between randomizations
        
        // Track combo state for adaptive timing
        private static int comboCounter = 0;
        private static float lastComboResetTime = 0f;
        private static float comboResetWindow = 1.5f; // Time window to maintain combo

        // --------------------------
        // Configurable ranges
        // --------------------------

        /// <summary>
        /// Speed +/- range. Example 0.15 => speed will be 0.85x to 1.15x.
        /// </summary>
        public static float SpeedPlusMinus = 0.12f;

        /// <summary>
        /// Position soft force per-axis maximum (applied to spring system).
        /// These add to the weapon's position through the spring physics.
        /// Values based on game's default SwingPositionSoftForce of (-0.5,-0.1,0.3).
        /// </summary>
        public static Vector3 PositionForcePlusMinus = new Vector3(0.15f, 0.08f, 0.12f);

        /// <summary>
        /// Rotation soft force per-axis maximum (degrees, applied to spring system).
        /// Values based on game's default SwingRotationSoftForce of (50,-25,0).
        /// </summary>
        public static Vector3 RotationForcePlusMinus = new Vector3(15f, 12f, 8f);

        /// <summary>
        /// Number of frames over which to apply the soft force.
        /// Game default is 50 frames. Lower = snappier, Higher = smoother.
        /// </summary>
        public static int SoftForceFrames = 25;

        /// <summary>
        /// Whether to affect all melee weapons (true) or only machete-type (false).
        /// </summary>
        public static bool AffectAllMeleeWeapons = true;

        /// <summary>
        /// Enable debug logging.
        /// </summary>
        public static bool DebugLogging = false;

        // --------------------------
        // Enhanced Combat Feel Parameters
        // --------------------------

        /// <summary>
        /// Enable camera shake on successful hits for visceral feedback.
        /// </summary>
        public static bool EnableCameraShake = true;

        /// <summary>
        /// Intensity of camera shake on hit (0 = disabled).
        /// </summary>
        public static float CameraShakeIntensity = 0.08f;

        /// <summary>
        /// Enable weapon trail/momentum system for weight feel.
        /// Weapons continue moving slightly after swing, then settle.
        /// </summary>
        public static bool EnableWeaponMomentum = true;

        /// <summary>
        /// Momentum force magnitude (how much the weapon continues moving).
        /// </summary>
        public static float MomentumForceMagnitude = 0.15f;

        /// <summary>
        /// Enable adaptive combo timing - faster follow-up attacks.
        /// </summary>
        public static bool EnableComboTiming = true;

        /// <summary>
        /// Speed boost per combo hit (stacks up to max combo).
        /// Example: 0.05 = 5% faster per hit in combo.
        /// </summary>
        public static float ComboSpeedBoost = 0.06f;

        /// <summary>
        /// Maximum combo count for speed boost.
        /// </summary>
        public static int MaxComboCount = 4;

        /// <summary>
        /// Enable enhanced hit feedback (different feel for crits/headshots/kills).
        /// </summary>
        public static bool EnableHitFeedback = true;

        /// <summary>
        /// Additional camera shake multiplier for critical hits.
        /// </summary>
        public static float CriticalHitShakeMultiplier = 1.8f;

        /// <summary>
        /// Enable anticipation variation - slight windup delays/speedups.
        /// </summary>
        public static bool EnableWindupVariation = true;

        /// <summary>
        /// Windup speed variation range (+/- percentage).
        /// </summary>
        public static float WindupVariation = 0.08f;

        // --------------------------
        // Helper methods
        // --------------------------

        private static float SampleSymmetric(float halfRange)
        {
            return (float)((random.NextDouble() * 2.0 - 1.0) * halfRange);
        }

        /// <summary>
        /// Apply camera shake to player camera for impact feedback.
        /// </summary>
        private static void ApplyCameraShake(EntityPlayerLocal player, float intensity)
        {
            if (!EnableCameraShake || player == null || intensity <= 0f)
                return;

            try
            {
                // Access player camera and apply rotational shake
                var camera = player.playerCamera;
                if (camera != null)
                {
                    var fpWeapon = FindFPWeapon(player);
                    if (fpWeapon != null)
                    {
                        // Apply shake as a quick rotational impulse
                        Vector3 shakeRotation = new Vector3(
                            SampleSymmetric(intensity * 50f), // Pitch
                            SampleSymmetric(intensity * 30f), // Yaw
                            SampleSymmetric(intensity * 20f)  // Roll
                        );
                        
                        // Apply shake with very short duration for snappy feedback
                        fpWeapon.AddSoftForce(Vector3.zero, shakeRotation, 3);
                    }
                }
            }
            catch (Exception ex)
            {
                if (DebugLogging)
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Camera shake error: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply weapon momentum - weapon continues moving slightly after swing.
        /// </summary>
        private static void ApplyWeaponMomentum(vp_FPWeapon weapon, Vector3 swingDirection)
        {
            if (!EnableWeaponMomentum || weapon == null || MomentumForceMagnitude <= 0f)
                return;

            try
            {
                // Apply a trailing force in the swing direction that settles over time
                Vector3 momentumForce = swingDirection.normalized * MomentumForceMagnitude;
                Vector3 momentumRotation = new Vector3(
                    swingDirection.x * 8f,
                    swingDirection.y * 5f,
                    swingDirection.z * 3f
                );
                
                // Apply over more frames for smooth settling
                weapon.AddSoftForce(momentumForce, momentumRotation, 35);

                if (DebugLogging)
                {
                    UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Momentum applied: Force={momentumForce}, Rot={momentumRotation}");
                }
            }
            catch (Exception ex)
            {
                if (DebugLogging)
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Momentum error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update combo counter and return current combo multiplier for speed.
        /// </summary>
        private static float GetComboSpeedMultiplier()
        {
            if (!EnableComboTiming)
                return 1f;

            // Check if combo window expired
            if (Time.time - lastComboResetTime > comboResetWindow)
            {
                comboCounter = 0;
            }

            // Increment combo
            comboCounter = Mathf.Min(comboCounter + 1, MaxComboCount);
            lastComboResetTime = Time.time;

            // Calculate speed boost (1.0 + boost per hit)
            float speedMultiplier = 1f + (comboCounter - 1) * ComboSpeedBoost;

            if (DebugLogging)
            {
                UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Combo: {comboCounter} hits, Speed: {speedMultiplier:F2}x");
            }

            return speedMultiplier;
        }

        /// <summary>
        /// Reset combo counter (called when combo window expires or player stops attacking).
        /// </summary>
        private static void ResetCombo()
        {
            comboCounter = 0;
            lastComboResetTime = 0f;
        }

        /// <summary>
        /// Check if the held item is a melee weapon we want to randomize.
        /// </summary>
        public static bool IsMeleeWeaponToRandomize(ItemClass holdingItem)
        {
            if (holdingItem == null) return false;

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

            // Otherwise check for machete/blade types by name/tags
            string itemName = holdingItem.GetItemName().ToLower();
            if (itemName.Contains("machete") || itemName.Contains("blade") || 
                itemName.Contains("knife") || itemName.Contains("sword"))
            {
                return true;
            }

            if (holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("blade,knife,sword,machete")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generate and apply random forces to a vp_FPWeapon's spring system.
        /// Enhanced with momentum for better weapon feel.
        /// </summary>
        private static void ApplyRandomForces(vp_FPWeapon weapon, string context)
        {
            if (weapon == null) return;

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

            // Apply via the spring system
            weapon.AddSoftForce(randomPosForce, randomRotForce, SoftForceFrames);

            // Apply momentum effect for weight/inertia feel
            if (EnableWeaponMomentum)
            {
                ApplyWeaponMomentum(weapon, randomPosForce);
            }

            if (DebugLogging)
            {
                UnityEngine.Debug.Log($"[MacheteAnimRandomizer] {context} - Applied random forces: " +
                    $"Pos={randomPosForce}, Rot={randomRotForce}");
            }
        }

        /// <summary>
        /// PATCH 1: Hook ItemActionDynamicMelee.ExecuteAction when attack starts (_bReleased = false).
        /// This is called when melee attacks begin.
        /// </summary>
        [HarmonyPatch(typeof(ItemActionDynamicMelee))]
        [HarmonyPatch("ExecuteAction")]
        public class Patch_ExecuteAction
        {
            static void Prefix(ItemActionData _actionData, bool _bReleased)
            {
                try
                {
                    // Only trigger on attack start, not release
                    if (_bReleased)
                        return;

                    // Prevent multiple triggers per attack
                    if (Time.time - lastAttackTime < attackCooldown)
                        return;

                    if (_actionData?.invData?.holdingEntity == null)
                        return;

                    EntityAlive holdingEntity = _actionData.invData.holdingEntity;

                    // Only affect local player
                    if (!(holdingEntity is EntityPlayerLocal localPlayer))
                        return;

                    ItemClass holdingItem = holdingEntity.inventory?.holdingItem;
                    if (holdingItem == null || !IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Find the vp_FPWeapon component
                    vp_FPWeapon fpWeapon = FindFPWeapon(localPlayer);
                    if (fpWeapon != null)
                    {
                        ApplyRandomForces(fpWeapon, "ExecuteAction");
                        lastAttackTime = Time.time;
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] ExecuteAction patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// PATCH 2: Hook the 3-parameter AddSoftForce overload (Vector3, Vector3, int).
        /// This is called by vp_FPWeaponMeleeAttack during swing animations.
        /// We modify the parameters to add random variation.
        /// </summary>
        [HarmonyPatch(typeof(vp_FPWeapon))]
        [HarmonyPatch("AddSoftForce", new Type[] { typeof(Vector3), typeof(Vector3), typeof(int) })]
        public class Patch_AddSoftForce
        {
            static void Prefix(ref Vector3 positional, ref Vector3 angular, ref int frames)
            {
                try
                {
                    // Only modify if this looks like a melee swing (has significant rotation force)
                    if (angular.magnitude < 5f)
                        return;

                    var localPlayer = GameManager.Instance?.World?.GetPrimaryPlayer();
                    if (localPlayer?.inventory?.holdingItem == null)
                        return;

                    ItemClass holdingItem = localPlayer.inventory.holdingItem;
                    if (!IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Add random variation to the forces
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

                    positional += randomPosForce;
                    angular += randomRotForce;

                    if (DebugLogging)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] AddSoftForce modified: " +
                            $"+Pos={randomPosForce}, +Rot={randomRotForce}");
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] AddSoftForce patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// PATCH 3: Randomize animation speed via AnimatorMeleeAttackState.
        /// Enhanced with combo timing and windup variation for better feel.
        /// </summary>
        [HarmonyPatch(typeof(AnimatorMeleeAttackState))]
        [HarmonyPatch("OnStateEnter")]
        public class Patch_RandomizeMeleeSpeed
        {
            static void Postfix(
                Animator animator,
                AnimatorStateInfo stateInfo,
                int layerIndex,
                float ___originalMeleeAttackSpeed,
                EntityAlive ___entity,
                int ___actionIndex)
            {
                try
                {
                    if (___entity == null || ___entity.inventory?.holdingItem == null)
                        return;

                    // Only affect local player
                    if (!(___entity is EntityPlayerLocal))
                        return;

                    ItemClass holdingItem = ___entity.inventory.holdingItem;
                    if (!IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Base random speed multiplier
                    float speedMultiplier = 1f + SampleSymmetric(SpeedPlusMinus);
                    
                    // Add windup variation for anticipation feel
                    if (EnableWindupVariation)
                    {
                        float windupMod = 1f + SampleSymmetric(WindupVariation);
                        speedMultiplier *= windupMod;
                        
                        if (DebugLogging)
                        {
                            UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Windup variation: {windupMod:F2}x");
                        }
                    }

                    // Apply combo timing boost for flow
                    if (EnableComboTiming)
                    {
                        float comboMultiplier = GetComboSpeedMultiplier();
                        speedMultiplier *= comboMultiplier;
                    }
                    
                    // Clamp to reasonable range
                    speedMultiplier = Mathf.Clamp(speedMultiplier, 0.7f, 1.5f);

                    float randomizedSpeed = ___originalMeleeAttackSpeed * speedMultiplier;
                    animator.SetFloat("MeleeAttackSpeed", randomizedSpeed);

                    if (DebugLogging)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Speed: {speedMultiplier:F2}x " +
                            $"(base={___originalMeleeAttackSpeed:F2}, new={randomizedSpeed:F2})");
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Speed patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// PATCH 4: Hook hit detection for camera shake feedback.
        /// Triggers on successful melee hits to provide visceral impact feedback.
        /// </summary>
        [HarmonyPatch(typeof(ItemActionAttack))]
        [HarmonyPatch("Hit")]
        public class Patch_MeleeHit
        {
            static void Postfix(
                ItemActionData _actionData,
                WorldRayHitInfo hitInfo,
                int _attackerEntityId,
                bool __result)
            {
                try
                {
                    // Only process if hit was successful
                    if (!__result || !EnableCameraShake)
                        return;

                    if (_actionData?.invData?.holdingEntity == null)
                        return;

                    // Only affect local player
                    if (!(_actionData.invData.holdingEntity is EntityPlayerLocal localPlayer))
                        return;

                    ItemClass holdingItem = localPlayer.inventory?.holdingItem;
                    if (holdingItem == null || !IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Calculate shake intensity based on hit type
                    float shakeIntensity = CameraShakeIntensity;

                    // Check if this was a critical hit or headshot for enhanced feedback
                    if (EnableHitFeedback && hitInfo.tag != null)
                    {
                        string hitTag = hitInfo.tag.ToLower();
                        
                        // Enhanced shake for headshots
                        if (hitTag.Contains("head"))
                        {
                            shakeIntensity *= CriticalHitShakeMultiplier;
                            
                            if (DebugLogging)
                                UnityEngine.Debug.Log("[MacheteAnimRandomizer] Headshot detected - enhanced shake");
                        }
                    }

                    // Apply camera shake for hit feedback
                    ApplyCameraShake(localPlayer, shakeIntensity);

                    if (DebugLogging)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Hit registered - shake intensity: {shakeIntensity:F3}");
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Hit patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// PATCH 5: Detect damage events for critical hit feedback.
        /// Provides enhanced feedback for critical/killing hits.
        /// </summary>
        [HarmonyPatch(typeof(EntityAlive))]
        [HarmonyPatch("DamageEntity")]
        public class Patch_DamageEntity
        {
            static void Postfix(
                EntityAlive __instance,
                DamageSource _damageSource,
                int _strength,
                bool _criticalHit,
                float _criticalMultiplier)
            {
                try
                {
                    if (!EnableHitFeedback || !_criticalHit)
                        return;

                    // Find the attacker
                    EntityAlive attacker = _damageSource.getAttackerEntity();
                    if (attacker == null || !(attacker is EntityPlayerLocal localPlayer))
                        return;

                    ItemClass holdingItem = localPlayer.inventory?.holdingItem;
                    if (holdingItem == null || !IsMeleeWeaponToRandomize(holdingItem))
                        return;

                    // Enhanced shake for critical hits
                    float critShake = CameraShakeIntensity * CriticalHitShakeMultiplier;
                    
                    // Extra shake for killing blows
                    if (__instance.IsDead())
                    {
                        critShake *= 1.2f;
                        
                        if (DebugLogging)
                            UnityEngine.Debug.Log("[MacheteAnimRandomizer] Killing blow - maximum feedback");
                    }

                    ApplyCameraShake(localPlayer, critShake);

                    if (DebugLogging)
                    {
                        UnityEngine.Debug.Log($"[MacheteAnimRandomizer] Critical hit - enhanced shake: {critShake:F3}");
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] Damage patch error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Find the vp_FPWeapon component for a player entity.
        /// </summary>
        private static vp_FPWeapon FindFPWeapon(EntityPlayerLocal player)
        {
            if (player == null) return null;

            try
            {
                // Try to get from player's camera/weapon hierarchy
                var playerCamera = player.playerCamera;
                if (playerCamera != null)
                {
                    var fpWeapon = playerCamera.GetComponentInChildren<vp_FPWeapon>();
                    if (fpWeapon != null)
                        return fpWeapon;
                }

                // Fallback: search in active weapon transforms
                var vp = player.vp_FPController;
                if (vp != null)
                {
                    var fpWeapon = vp.GetComponentInChildren<vp_FPWeapon>();
                    if (fpWeapon != null)
                        return fpWeapon;
                }
            }
            catch (Exception ex)
            {
                if (DebugLogging)
                    UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] FindFPWeapon error: {ex.Message}");
            }

            return null;
        }
    }
}
