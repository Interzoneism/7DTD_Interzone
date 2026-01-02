using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace MacheteAnimRandomizer
{
    /// <summary>
    /// Console command to configure machete animation randomization parameters.
    /// Usage: macheteanim <parameter> <value>
    /// Parameters: speed, position, rotation, applyfactor
    /// </summary>
    [Preserve]
    public class ConsoleCmdMacheteAnim : ConsoleCmdAbstract
    {
        public override string[] getCommands()
        {
            return new string[]
            {
                "macheteanim",
                "ma"
            };
        }

        public override string getDescription()
        {
            return "Configure melee animation randomization parameters";
        }

        public override string getHelp()
        {
            return "Configure melee animation randomization and combat feel enhancements.\n" +
                   "Uses the game's spring-based weapon system to add variation to melee attacks.\n\n" +
                   "Basic Parameters:\n" +
                   "  macheteanim speed <value>              - Set speed variation (+/- range, e.g., 0.12 = 0.88x to 1.12x speed)\n" +
                   "  macheteanim position <x> <y> <z>       - Set position force range (spring force per axis)\n" +
                   "  macheteanim rotation <pitch> <yaw> <roll> - Set rotation force range (degrees per axis)\n" +
                   "  macheteanim frames <value>             - Set soft force frames (smoothing, default 25)\n" +
                   "  macheteanim allmelee <true|false>      - Affect all melee weapons or just machete-type\n\n" +
                   "Combat Feel Enhancements:\n" +
                   "  macheteanim camerashake <true|false>   - Enable/disable camera shake on hits\n" +
                   "  macheteanim shakeintensity <value>     - Set camera shake intensity (default 0.08)\n" +
                   "  macheteanim momentum <true|false>      - Enable/disable weapon momentum/weight feel\n" +
                   "  macheteanim momentumforce <value>      - Set momentum force magnitude (default 0.15)\n" +
                   "  macheteanim combotiming <true|false>   - Enable/disable adaptive combo timing\n" +
                   "  macheteanim combospeed <value>         - Set combo speed boost per hit (default 0.06)\n" +
                   "  macheteanim maxcombo <value>           - Set maximum combo count (default 4)\n" +
                   "  macheteanim hitfeedback <true|false>   - Enable/disable enhanced hit feedback\n" +
                   "  macheteanim critmultiplier <value>     - Set critical hit shake multiplier (default 1.8)\n" +
                   "  macheteanim windup <true|false>        - Enable/disable windup variation\n" +
                   "  macheteanim windupvariation <value>    - Set windup variation range (default 0.08)\n\n" +
                   "Utility:\n" +
                   "  macheteanim debug <true|false>         - Enable/disable debug logging\n" +
                   "  macheteanim show                       - Display current settings\n" +
                   "  macheteanim reset                      - Reset all values to defaults\n\n" +
                   "Examples:\n" +
                   "  macheteanim speed 0.15                 - Speed will vary from 0.85x to 1.15x\n" +
                   "  macheteanim camerashake true           - Enable camera shake on hits\n" +
                   "  macheteanim combotiming true           - Enable faster follow-up attacks in combos";
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (_params.Count == 0)
            {
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim <parameter> <value(s)>");
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Type 'help macheteanim' for more information.");
                return;
            }

            string subCommand = _params[0].ToLower();

            try
            {
                switch (subCommand)
                {
                    case "speed":
                        if (_params.Count != 2)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim speed <value>");
                            return;
                        }
                        if (!float.TryParse(_params[1], out float speedValue) || speedValue < 0)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Speed value must be a positive number");
                            return;
                        }
                        MacheteAnimRandomizerPatches.SpeedPlusMinus = speedValue;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Speed variation set to +/- {speedValue:F3} (range: {1f - speedValue:F2}x to {1f + speedValue:F2}x)");
                        break;

                    case "position":
                    case "pos":
                        if (_params.Count != 4)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim position <x> <y> <z>");
                            return;
                        }
                        if (!float.TryParse(_params[1], out float posX) || posX < 0 ||
                            !float.TryParse(_params[2], out float posY) || posY < 0 ||
                            !float.TryParse(_params[3], out float posZ) || posZ < 0)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Position values must be positive numbers");
                            return;
                        }
                        MacheteAnimRandomizerPatches.PositionForcePlusMinus = new Vector3(posX, posY, posZ);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Position force set to X: +/- {posX:F3}, Y: +/- {posY:F3}, Z: +/- {posZ:F3}");
                        break;

                    case "rotation":
                    case "rot":
                        if (_params.Count != 4)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim rotation <pitch> <yaw> <roll>");
                            return;
                        }
                        if (!float.TryParse(_params[1], out float rotPitch) || rotPitch < 0 ||
                            !float.TryParse(_params[2], out float rotYaw) || rotYaw < 0 ||
                            !float.TryParse(_params[3], out float rotRoll) || rotRoll < 0)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Rotation values must be positive numbers");
                            return;
                        }
                        MacheteAnimRandomizerPatches.RotationForcePlusMinus = new Vector3(rotPitch, rotYaw, rotRoll);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Rotation force set to Pitch: +/- {rotPitch:F1}�, Yaw: +/- {rotYaw:F1}�, Roll: +/- {rotRoll:F1}�");
                        break;

                    case "frames":
                    case "softframes":
                        if (_params.Count != 2)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim frames <value>");
                            return;
                        }
                        if (!int.TryParse(_params[1], out int framesValue) || framesValue < 1)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Frames must be a positive integer");
                            return;
                        }
                                    MacheteAnimRandomizerPatches.SoftForceFrames = framesValue;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Soft force frames set to {framesValue}");
                                    break;

                                case "allmelee":
                                case "all":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim allmelee <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool allMelee))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.AffectAllMeleeWeapons = allMelee;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Affect all melee weapons: {allMelee}");
                                    break;

                                case "debug":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim debug <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool debugMode))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.DebugLogging = debugMode;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Debug logging: {debugMode}");
                                    break;

                                // Combat Feel Enhancement Commands
                                case "camerashake":
                                case "shake":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim camerashake <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool cameraShake))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.EnableCameraShake = cameraShake;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Camera shake on hits: {cameraShake}");
                                    break;

                                case "shakeintensity":
                                case "shakeint":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim shakeintensity <value>");
                                        return;
                                    }
                                    if (!float.TryParse(_params[1], out float shakeInt) || shakeInt < 0)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be a positive number");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.CameraShakeIntensity = shakeInt;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Camera shake intensity: {shakeInt:F3}");
                                    break;

                                case "momentum":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim momentum <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool momentum))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.EnableWeaponMomentum = momentum;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Weapon momentum/inertia: {momentum}");
                                    break;

                                case "momentumforce":
                                case "momentummag":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim momentumforce <value>");
                                        return;
                                    }
                                    if (!float.TryParse(_params[1], out float momentumForce) || momentumForce < 0)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be a positive number");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.MomentumForceMagnitude = momentumForce;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Momentum force magnitude: {momentumForce:F3}");
                                    break;

                                case "combotiming":
                                case "combo":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim combotiming <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool comboTiming))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.EnableComboTiming = comboTiming;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Adaptive combo timing: {comboTiming}");
                                    break;

                                case "combospeed":
                                case "comboboost":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim combospeed <value>");
                                        return;
                                    }
                                    if (!float.TryParse(_params[1], out float comboSpeed) || comboSpeed < 0)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be a positive number");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.ComboSpeedBoost = comboSpeed;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Combo speed boost per hit: {comboSpeed:F3} ({comboSpeed * 100:F1}%)");
                                    break;

                                case "maxcombo":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim maxcombo <value>");
                                        return;
                                    }
                                    if (!int.TryParse(_params[1], out int maxCombo) || maxCombo < 1)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be a positive integer");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.MaxComboCount = maxCombo;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Maximum combo count: {maxCombo}");
                                    break;

                                case "hitfeedback":
                                case "feedback":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim hitfeedback <true|false>");
                                        return;
                                    }
                                    if (!bool.TryParse(_params[1], out bool hitFeedback))
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be true or false");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.EnableHitFeedback = hitFeedback;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Enhanced hit feedback (crits/headshots): {hitFeedback}");
                                    break;

                                case "critmultiplier":
                                case "critmult":
                                    if (_params.Count != 2)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim critmultiplier <value>");
                                        return;
                                    }
                                    if (!float.TryParse(_params[1], out float critMult) || critMult < 1)
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Value must be >= 1.0");
                                        return;
                                    }
                                    MacheteAnimRandomizerPatches.CriticalHitShakeMultiplier = critMult;
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Critical hit shake multiplier: {critMult:F2}x");
                                    break;

                                case "windup":
                                case "windupvariation":
                                    if (_params.Count == 2 && bool.TryParse(_params[1], out bool windupEnable))
                                    {
                                        MacheteAnimRandomizerPatches.EnableWindupVariation = windupEnable;
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Windup variation: {windupEnable}");
                                    }
                                    else if (_params.Count == 2 && float.TryParse(_params[1], out float windupVar) && windupVar >= 0)
                                    {
                                        MacheteAnimRandomizerPatches.WindupVariation = windupVar;
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Windup variation range: +/- {windupVar:F3}");
                                    }
                                    else
                                    {
                                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim windup <true|false|value>");
                                        return;
                                    }
                                    break;

                                case "show":
                                case "display":
                                case "status":
                                    DisplayCurrentSettings();
                                    break;

                                case "reset":
                                    ResetToDefaults();
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("All machete animation parameters reset to default values");
                                    DisplayCurrentSettings();
                                    break;

                                default:
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Unknown parameter: {subCommand}");
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Valid parameters: speed, position, rotation, frames, allmelee, debug, show, reset");
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Type 'help macheteanim' for more information.");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Error executing command: {ex.Message}");
                UnityEngine.Debug.LogError($"[MacheteAnimRandomizer] Console command error: {ex}");
            }
        }

        private void DisplayCurrentSettings()
        {
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("=== Melee Animation Randomizer Settings ===");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Basic Settings:");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Speed Variation: +/- {MacheteAnimRandomizerPatches.SpeedPlusMinus:F3} (range: {1f - MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x to {1f + MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x)");
            Vector3 pos = MacheteAnimRandomizerPatches.PositionForcePlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Position Force: X: +/- {pos.x:F3}, Y: +/- {pos.y:F3}, Z: +/- {pos.z:F3}");
            Vector3 rot = MacheteAnimRandomizerPatches.RotationForcePlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Rotation Force: Pitch: +/- {rot.x:F1}°, Yaw: +/- {rot.y:F1}°, Roll: +/- {rot.z:F1}°");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Soft Force Frames: {MacheteAnimRandomizerPatches.SoftForceFrames}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Affect All Melee: {MacheteAnimRandomizerPatches.AffectAllMeleeWeapons}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Combat Feel Enhancements:");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Camera Shake: {MacheteAnimRandomizerPatches.EnableCameraShake} (Intensity: {MacheteAnimRandomizerPatches.CameraShakeIntensity:F3})");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Weapon Momentum: {MacheteAnimRandomizerPatches.EnableWeaponMomentum} (Force: {MacheteAnimRandomizerPatches.MomentumForceMagnitude:F3})");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Combo Timing: {MacheteAnimRandomizerPatches.EnableComboTiming} (Boost: {MacheteAnimRandomizerPatches.ComboSpeedBoost:F3}/hit, Max: {MacheteAnimRandomizerPatches.MaxComboCount})");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Hit Feedback: {MacheteAnimRandomizerPatches.EnableHitFeedback} (Crit Multiplier: {MacheteAnimRandomizerPatches.CriticalHitShakeMultiplier:F2}x)");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"  Windup Variation: {MacheteAnimRandomizerPatches.EnableWindupVariation} (Range: +/- {MacheteAnimRandomizerPatches.WindupVariation:F3})");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Debug Logging: {MacheteAnimRandomizerPatches.DebugLogging}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("==========================================");
        }

        private void ResetToDefaults()
        {
            // Basic settings
            MacheteAnimRandomizerPatches.SpeedPlusMinus = 0.12f;
            MacheteAnimRandomizerPatches.PositionForcePlusMinus = new Vector3(0.15f, 0.08f, 0.12f);
            MacheteAnimRandomizerPatches.RotationForcePlusMinus = new Vector3(15f, 12f, 8f);
            MacheteAnimRandomizerPatches.SoftForceFrames = 25;
            MacheteAnimRandomizerPatches.AffectAllMeleeWeapons = true;
            MacheteAnimRandomizerPatches.DebugLogging = false;
            
            // Combat feel enhancements
            MacheteAnimRandomizerPatches.EnableCameraShake = true;
            MacheteAnimRandomizerPatches.CameraShakeIntensity = 0.08f;
            MacheteAnimRandomizerPatches.EnableWeaponMomentum = true;
            MacheteAnimRandomizerPatches.MomentumForceMagnitude = 0.15f;
            MacheteAnimRandomizerPatches.EnableComboTiming = true;
            MacheteAnimRandomizerPatches.ComboSpeedBoost = 0.06f;
            MacheteAnimRandomizerPatches.MaxComboCount = 4;
            MacheteAnimRandomizerPatches.EnableHitFeedback = true;
            MacheteAnimRandomizerPatches.CriticalHitShakeMultiplier = 1.8f;
            MacheteAnimRandomizerPatches.EnableWindupVariation = true;
            MacheteAnimRandomizerPatches.WindupVariation = 0.08f;
        }

        public override int DefaultPermissionLevel
        {
            get
            {
                return 1000; // Available to all players (default is admin-only at 0)
            }
        }

        public override bool AllowedInMainMenu
        {
            get
            {
                return true; // Allow in main menu for testing
            }
        }
    }
}
