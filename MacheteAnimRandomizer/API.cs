using HarmonyLib;
using System.Reflection;

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
            Log.Out("[MacheteAnimRandomizer] Loading Machete Animation Randomizer v1.0.0");
            
            try
            {
                // Initialize Harmony and apply all patches
                var harmony = new Harmony("com.interzoneism.macheteanimrandomizer");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                
                Log.Out("[MacheteAnimRandomizer] Harmony patches applied successfully");
                Log.Out("[MacheteAnimRandomizer] Mod initialized - machete attacks will now be randomized!");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[MacheteAnimRandomizer] Failed to initialize mod: {ex.Message}");
                Log.Error($"[MacheteAnimRandomizer] Stack trace: {ex.StackTrace}");
            }
        }
    }
}
