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
        // Helper methods
        // --------------------------

        private static float SampleSymmetric(float halfRange)
        {
            return (float)((random.NextDouble() * 2.0 - 1.0) * halfRange);
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

            if (DebugLogging)
            {
                UnityEngine.Debug.Log($"[MacheteAnimRandomizer] {context} - Applied random forces: " +
                    $"Pos={randomPosForce}, Rot={randomRotForce}");
            }
        }

        /// <summary>
        /// PATCH 1: Hook the melee action start point. Some game builds renamed the entry
        /// (StartAttack/StartAction/etc), so we resolve it dynamically and skip the patch
        /// cleanly if nothing matches to avoid Harmony errors.
        /// </summary>
        [HarmonyPatch]
        public class Patch_StartAttack
        {
            private static MethodBase _targetMethod;

            static bool Prepare()
            {
                _targetMethod = FindTargetMethod();
                if (_targetMethod == null)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning("[MacheteAnimRandomizer] No StartAttack/StartAction method found; skipping Patch_StartAttack.");
                    return false;
                }

                return true;
            }

            static MethodBase TargetMethod()
            {
                return _targetMethod;
            }

            private static MethodBase FindTargetMethod()
            {
                var type = typeof(ItemActionDynamicMelee);

                // Common signatures observed across builds
                var signatures = new List<Type[]>
                {
                    new[] { typeof(ItemActionData) },
                    new[] { typeof(ItemActionData), typeof(bool) },
                    new[] { typeof(ItemActionData), typeof(bool), typeof(bool) }
                };

                var names = new[]
                {
                    "StartAttack",
                    "StartAction",
                    "StartActionInternal"
                };

                foreach (var name in names)
                {
                    foreach (var sig in signatures)
                    {
                        var method = AccessTools.DeclaredMethod(type, name, sig);
                        if (method != null)
                            return method;
                    }
                }

                // Fallback: first void method whose first parameter is ItemActionData and name contains Start/Attack
                return AccessTools.FirstMethod(type, m =>
                {
                    var parameters = m.GetParameters();
                    return m.ReturnType == typeof(void)
                        && parameters.Length > 0
                        && typeof(ItemActionData).IsAssignableFrom(parameters[0].ParameterType)
                        && (m.Name.IndexOf("Start", StringComparison.OrdinalIgnoreCase) >= 0
                            || m.Name.IndexOf("Attack", StringComparison.OrdinalIgnoreCase) >= 0);
                });
            }

            static void Postfix(object[] __args)
            {
                try
                {
                    var actionData = __args != null && __args.Length > 0
                        ? __args[0] as ItemActionData
                        : null;

                    // Prevent multiple triggers per attack
                    if (Time.time - lastAttackTime < attackCooldown)
                        return;

                    if (actionData?.invData?.holdingEntity == null)
                        return;

                    EntityAlive holdingEntity = actionData.invData.holdingEntity;

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
                        ApplyRandomForces(fpWeapon, "StartAttack");
                        lastAttackTime = Time.time;
                    }
                }
                catch (Exception ex)
                {
                    if (DebugLogging)
                        UnityEngine.Debug.LogWarning($"[MacheteAnimRandomizer] StartAttack patch error: {ex.Message}");
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

                    // Generate random speed multiplier
                    float speedMultiplier = 1f + SampleSymmetric(SpeedPlusMinus);
                    speedMultiplier = Mathf.Clamp(speedMultiplier, 0.7f, 1.3f);

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
