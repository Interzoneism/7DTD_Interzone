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
            return "Configure machete animation randomization parameters";
        }

        public override string getHelp()
        {
            return "Configure melee animation randomization parameters.\n" +
                   "Usage:\n" +
                   "  macheteanim speed <value>              - Set speed variation (+/- range, e.g., 0.15 = 0.85x to 1.15x speed)\n" +
                   "  macheteanim position <x> <y> <z>       - Set position force range (spring force per axis)\n" +
                   "  macheteanim rotation <pitch> <yaw> <roll> - Set rotation force range (degrees per axis)\n" +
                   "  macheteanim frames <value>             - Set soft force frames (smoothing, default 15)\n" +
                   "  macheteanim allmelee <true|false>      - Affect all melee weapons or just machete-type\n" +
                   "  macheteanim show                       - Display current settings\n" +
                   "  macheteanim reset                      - Reset all values to defaults\n" +
                   "Examples:\n" +
                   "  macheteanim speed 0.2                  - Speed will vary from 0.8x to 1.2x\n" +
                   "  macheteanim position 0.05 0.05 0.03    - Position force varies per axis\n" +
                   "  macheteanim rotation 10 10 8           - Rotation force varies per axis";
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
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Rotation force set to Pitch: +/- {rotPitch:F1}°, Yaw: +/- {rotYaw:F1}°, Roll: +/- {rotRoll:F1}°");
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
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Valid parameters: speed, position, rotation, frames, allmelee, show, reset");
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
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Speed Variation: +/- {MacheteAnimRandomizerPatches.SpeedPlusMinus:F3} (range: {1f - MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x to {1f + MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x)");
            Vector3 pos = MacheteAnimRandomizerPatches.PositionForcePlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Position Force: X: +/- {pos.x:F3}, Y: +/- {pos.y:F3}, Z: +/- {pos.z:F3}");
            Vector3 rot = MacheteAnimRandomizerPatches.RotationForcePlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Rotation Force: Pitch: +/- {rot.x:F1}°, Yaw: +/- {rot.y:F1}°, Roll: +/- {rot.z:F1}°");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Soft Force Frames: {MacheteAnimRandomizerPatches.SoftForceFrames}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Affect All Melee: {MacheteAnimRandomizerPatches.AffectAllMeleeWeapons}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("==========================================");
        }

        private void ResetToDefaults()
        {
            MacheteAnimRandomizerPatches.SpeedPlusMinus = 0.15f;
            MacheteAnimRandomizerPatches.PositionForcePlusMinus = new Vector3(0.03f, 0.03f, 0.02f);
            MacheteAnimRandomizerPatches.RotationForcePlusMinus = new Vector3(8f, 8f, 5f);
            MacheteAnimRandomizerPatches.SoftForceFrames = 15;
            MacheteAnimRandomizerPatches.AffectAllMeleeWeapons = true;
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
