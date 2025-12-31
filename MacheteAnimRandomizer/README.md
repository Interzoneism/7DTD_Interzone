# Machete Animation Randomizer Mod

A Harmony-based mod for 7 Days to Die that adds dynamic randomization to machete melee attack animations.

## Features

This mod randomizes the following aspects of machete normal attacks (non-power attacks):

### 1. **Animation Speed Randomization**
- Speed varies between **0.8x - 1.5x** of the base attack speed
- Makes each attack feel unique and unpredictable
- Applied in real-time for every attack

### 2. **Weapon Position Randomization**
- Random position offsets applied to weapon transform
- X, Y, Z offsets: Small variations to make attacks feel more organic
- Offsets reset after each attack completes

### 3. **Weapon Rotation Randomization**
- Random rotation angles applied to weapon during attack
- Pitch (up/down): -15° to +15°
- Yaw (left/right): -10° to +10°
- Roll (twist): -20° to +20°
- Creates varied swing trajectories

## How It Works

The mod uses Harmony patches to intercept the game's animation system:

1. **OnStateEnter**: When a machete attack begins, random values are generated for speed, position, and rotation
2. **OnStateUpdate**: During the attack, the weapon transform is continuously updated with the randomized offsets
3. **OnStateExit**: When the attack ends, the weapon is reset to its normal state

## Installation

1. Make sure you have the **TFP_Harmony** mod installed (required dependency)
2. Download the `MacheteAnimRandomizer` folder
3. Place it in your `7DaysToDie/Mods/` folder
4. Launch the game

The mod should load automatically and display confirmation messages in the console.

## Compatibility

- **7 Days to Die Version**: Alpha 21+ (tested on v2.4)
- **Required Mods**: TFP_Harmony (for Harmony patching support)
- **Weapon Types**: Designed for machete weapons, but may affect other blade-type weapons
- **Attack Types**: Only affects normal attacks, NOT power attacks

## Technical Details

### Affected Game Systems
- `AnimatorMeleeAttackState` - Core melee animation state machine
- `AnimationGunjointOffsetData` - Weapon positioning system
- `EntityAlive` - Entity animation controllers

### Randomization Ranges
```csharp
Speed Multiplier:     0.8 - 1.5x
Position X Offset:    -0.1 to +0.1 units
Position Y Offset:    -0.1 to +0.1 units
Position Z Offset:    -0.075 to +0.075 units
Rotation Pitch:       -15° to +15°
Rotation Yaw:         -10° to +10°
Rotation Roll:        -20° to +20°
```

## Configuration

Currently, the mod uses hardcoded randomization ranges. Future versions may include configuration files to adjust:
- Randomization intensity
- Specific weapon targeting
- Speed range limits
- Position/rotation offset limits

## Building from Source

### Prerequisites
1. .NET Framework 4.8 SDK
2. Visual Studio 2017 or later (or MSBuild)
3. Required DLLs (place in `7dtd-binaries/` folder):
   - `Assembly-CSharp.dll` (from game files)
   - `UnityEngine.CoreModule.dll` (from game files)
   - `0Harmony.dll` (from TFP_Harmony mod)

### Build Instructions
```bash
# Using MSBuild
msbuild MacheteAnimRandomizer.csproj /p:Configuration=Release

# Using Visual Studio
# Open MacheteAnimRandomizer.csproj and build in Release mode
```

The compiled mod will be output to `Mods/MacheteAnimRandomizer/`

## Console Output

When the mod loads successfully, you'll see:
```
[MacheteAnimRandomizer] Loading Machete Animation Randomizer v1.0.0
[MacheteAnimRandomizer] Harmony patches applied successfully
[MacheteAnimRandomizer] Mod initialized - machete attacks will now be randomized!
```

When you perform attacks, debug output shows:
```
[MacheteAnimRandomizer] Randomized melee[weapon_name] attack for entity [ID]:
  Speed: 1.23x (from 1.0 to 1.23)
  Position offset: (0.05, -0.03, 0.02)
  Rotation offset: (8.5, -4.2, 12.3)
```

## Troubleshooting

### Mod Not Loading
- Check console for error messages
- Verify TFP_Harmony is installed
- Ensure ModInfo.xml is present

### Attacks Not Randomizing
- Verify you're using a machete weapon
- Check that you're using normal attacks (not power attacks)
- Look for error messages in console logs

### Game Crashes
- Ensure all required DLLs are compatible with your game version
- Check for conflicts with other animation mods
- Report issues with full error logs

## Known Issues

- Transform changes may occasionally conflict with other animation mods
- Very rapid attacks might cause visual glitches with extreme randomization
- Third-person view may show weapon transform differently than first-person

## Future Enhancements

Potential features for future versions:
- [ ] Configuration file for customizable randomization ranges
- [ ] Per-weapon randomization profiles
- [ ] Visual effects synchronized with randomized attacks
- [ ] Combo system that increases randomization with consecutive hits
- [ ] Sound variation tied to attack speed changes
- [ ] Network synchronization for multiplayer consistency

## Credits

- **Author**: Interzoneism
- **Harmony Library**: Andreas Pardeike
- **7 Days to Die**: The Fun Pimps

## License

This mod is provided as-is for use with 7 Days to Die. Feel free to modify and redistribute with attribution.

## Version History

### v1.0.0 (Initial Release)
- Dynamic speed randomization (0.8x - 1.5x)
- Position offset randomization
- Rotation offset randomization
- Machete weapon targeting
- Normal attack filtering (excludes power attacks)
- Memory leak prevention with cleanup system
