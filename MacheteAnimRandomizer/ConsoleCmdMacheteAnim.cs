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
            return "Configure machete animation randomization parameters.\n" +
                   "Usage:\n" +
                   "  macheteanim speed <value>              - Set speed variation (+/- range, e.g., 0.1 = 0.9x to 1.1x speed)\n" +
                   "  macheteanim position <x> <y> <z>       - Set position offset range (+/- in meters for each axis)\n" +
                   "  macheteanim rotation <pitch> <yaw> <roll> - Set rotation offset range (+/- in degrees for each axis)\n" +
                   "  macheteanim applyfactor <value>        - Set weapon position apply factor (default 0.3)\n" +
                   "  macheteanim show                       - Display current settings\n" +
                   "  macheteanim reset                      - Reset all values to defaults\n" +
                   "Examples:\n" +
                   "  macheteanim speed 0.2                  - Speed will vary from 0.8x to 1.2x\n" +
                   "  macheteanim position 0.15 0.15 0.1     - Position varies +/- 0.15m on X/Y, +/- 0.1m on Z\n" +
                   "  macheteanim rotation 20 15 30          - Rotation varies +/- 20° pitch, +/- 15° yaw, +/- 30° roll";
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
                        MacheteAnimRandomizerPatches.PositionPlusMinus = new Vector3(posX, posY, posZ);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Position offset set to X: +/- {posX:F3}m, Y: +/- {posY:F3}m, Z: +/- {posZ:F3}m");
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
                        MacheteAnimRandomizerPatches.RotationPlusMinus = new Vector3(rotPitch, rotYaw, rotRoll);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Rotation offset set to Pitch: +/- {rotPitch:F1}°, Yaw: +/- {rotYaw:F1}°, Roll: +/- {rotRoll:F1}°");
                        break;

                    case "applyfactor":
                    case "apply":
                    case "factor":
                        if (_params.Count != 2)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: macheteanim applyfactor <value>");
                            return;
                        }
                        if (!float.TryParse(_params[1], out float applyValue) || applyValue < 0)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error: Apply factor must be a positive number");
                            return;
                        }
                        MacheteAnimRandomizerPatches.WeaponPositionApplyFactor = applyValue;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Weapon position apply factor set to {applyValue:F3}");
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
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Valid parameters: speed, position, rotation, applyfactor, show, reset");
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
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("=== Machete Animation Randomizer Settings ===");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Speed Variation: +/- {MacheteAnimRandomizerPatches.SpeedPlusMinus:F3} (range: {1f - MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x to {1f + MacheteAnimRandomizerPatches.SpeedPlusMinus:F2}x)");
            Vector3 pos = MacheteAnimRandomizerPatches.PositionPlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Position Offset: X: +/- {pos.x:F3}m, Y: +/- {pos.y:F3}m, Z: +/- {pos.z:F3}m");
            Vector3 rot = MacheteAnimRandomizerPatches.RotationPlusMinus;
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Rotation Offset: Pitch: +/- {rot.x:F1}°, Yaw: +/- {rot.y:F1}°, Roll: +/- {rot.z:F1}°");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Apply Factor: {MacheteAnimRandomizerPatches.WeaponPositionApplyFactor:F3}");
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("==========================================");
        }

        private void ResetToDefaults()
        {
            MacheteAnimRandomizerPatches.SpeedPlusMinus = 0.1f;
            MacheteAnimRandomizerPatches.PositionPlusMinus = new Vector3(0.1f, 0.1f, 0.075f);
            MacheteAnimRandomizerPatches.RotationPlusMinus = new Vector3(15f, 10f, 20f);
            MacheteAnimRandomizerPatches.WeaponPositionApplyFactor = 0.3f;
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
