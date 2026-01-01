using HarmonyLib;
using System.Reflection;
using Platform;
using UnityEngine;

namespace MacheteAnimRandomizer
{
    /// <summary>
    /// Main API class for the Machete Animation Randomizer mod.
    /// Initializes Harmony patches when the mod loads.
    /// </summary>
    public class API : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            UnityEngine.Debug.Log("[MacheteAnimRandomizer] Loading Machete Animation Randomizer v1.0.0");

            try
            {
                // Initialize Harmony and apply all patches
                var harmony = new Harmony("com.interzoneism.macheteanimrandomizer");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                UnityEngine.Debug.Log("[MacheteAnimRandomizer] Harmony patches applied successfully");
                UnityEngine.Debug.Log("[MacheteAnimRandomizer] Mod initialized - machete attacks will now be randomized!");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Failed to initialize mod: {ex.Message}");
                UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Stack trace: {ex.StackTrace}");
            }
        }
    }
}
