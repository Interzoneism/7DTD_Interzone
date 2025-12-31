# Machete Animation Randomizer - Build & Usage Guide

## Quick Start

This guide will help you build and install the Machete Animation Randomizer mod for 7 Days to Die.

## Prerequisites

### For Building the Mod

1. **Development Environment**
   - Windows, Linux, or macOS
   - .NET Framework 4.8 SDK (or Mono on Linux/macOS)
   - MSBuild or Visual Studio 2017+

2. **Required Game Files**
   
   You need to obtain these DLLs from your 7 Days to Die installation and place them in the `7dtd-binaries/` folder:
   
   ```
   7DaysToDie/
   └── 7DaysToDieServer_Data/
       └── Managed/
           ├── Assembly-CSharp.dll          ← Copy this
           ├── UnityEngine.CoreModule.dll   ← Copy this
           └── ... (other DLLs)
   ```

3. **Harmony Library**
   
   Option A: Extract from TFP_Harmony mod
   - Install the TFP_Harmony mod in your game
   - Copy `0Harmony.dll` from the mod folder
   
   Option B: Download from GitHub
   - Visit: https://github.com/pardeike/Harmony/releases
   - Download version 2.2.2 or later
   - Extract `0Harmony.dll` from the archive

### For Using the Mod

1. **7 Days to Die** - Alpha 21 or later
2. **TFP_Harmony Mod** - Required for Harmony patching support

## Building the Mod

### Option 1: Using MSBuild (Command Line)

1. **Setup DLLs**
   ```bash
   # Navigate to the project root
   cd 7DTD_Interzone
   
   # Copy required DLLs to 7dtd-binaries/
   cp /path/to/7dtd/7DaysToDieServer_Data/Managed/Assembly-CSharp.dll 7dtd-binaries/
   cp /path/to/7dtd/7DaysToDieServer_Data/Managed/UnityEngine.CoreModule.dll 7dtd-binaries/
   cp /path/to/0Harmony.dll 7dtd-binaries/
   ```

2. **Build the Project**
   ```bash
   cd MacheteAnimRandomizer
   msbuild MacheteAnimRandomizer.csproj /p:Configuration=Release
   ```

3. **Output Location**
   ```
   The compiled mod will be in:
   MacheteAnimRandomizer/Mods/MacheteAnimRandomizer/
   ├── MacheteAnimRandomizer.dll
   └── ModInfo.xml
   ```

### Option 2: Using Visual Studio

1. **Open the Solution**
   - Open `ModBase.sln` in Visual Studio
   - Add the `MacheteAnimRandomizer.csproj` to the solution (optional)
   - Or open `MacheteAnimRandomizer.csproj` directly

2. **Configure References**
   - Ensure all DLL references are in `7dtd-binaries/`
   - Or update references to point directly to your game's Managed folder

3. **Build**
   - Select "Release" configuration
   - Build → Build Solution (or Ctrl+Shift+B)

4. **Output**
   - Check `MacheteAnimRandomizer/Mods/MacheteAnimRandomizer/`

### Option 3: Using Alternative Paths

If you don't want to copy DLLs, edit `MacheteAnimRandomizer.csproj`:

```xml
<Reference Include="Assembly-CSharp">
  <HintPath>C:\Program Files\Steam\steamapps\common\7 Days To Die\7DaysToDieServer_Data\Managed\Assembly-CSharp.dll</HintPath>
  <Private>False</Private>
</Reference>
```

Change the `HintPath` to point directly to your game installation.

## Installing the Mod

### Manual Installation

1. **Locate your 7 Days to Die Mods folder**
   - Windows: `C:\Program Files\Steam\steamapps\common\7 Days To Die\Mods\`
   - Linux: `~/.local/share/7DaysToDie/Mods/`
   - Or your custom installation path

2. **Copy the mod folder**
   ```bash
   # From the build output
   cp -r MacheteAnimRandomizer/Mods/MacheteAnimRandomizer /path/to/7dtd/Mods/
   ```

3. **Verify installation**
   ```
   7 Days To Die/
   └── Mods/
       ├── TFP_Harmony/          ← Required dependency
       └── MacheteAnimRandomizer/
           ├── MacheteAnimRandomizer.dll
           └── ModInfo.xml
   ```

### Verify TFP_Harmony is Installed

The mod requires Harmony to function. Check that TFP_Harmony is installed:

```
7 Days To Die/
└── Mods/
    └── TFP_Harmony/
        ├── 0Harmony.dll
        └── ModInfo.xml
```

If not installed, download it from the 7DTD mod forums or GitHub.

## Using the Mod

### Starting the Game

1. Launch 7 Days to Die
2. Check the console (F1) for mod loading messages:
   ```
   [MacheteAnimRandomizer] Loading Machete Animation Randomizer v1.0.0
   [MacheteAnimRandomizer] Harmony patches applied successfully
   [MacheteAnimRandomizer] Mod initialized - machete attacks will now be randomized!
   ```

### Testing the Mod

1. **Get a machete weapon**
   ```
   # Open console (F1) and type:
   giveself meleeToolMachete
   ```

2. **Perform attacks**
   - Equip the machete
   - Perform normal attacks (left click, not power attacks)
   - Each attack should feel different:
     - Speed varies
     - Weapon angle changes
     - Swing trajectory varies

3. **Check console output** (optional)
   ```
   [MacheteAnimRandomizer] Randomized meleeToolMachete attack for entity 171:
     Speed: 1.23x (from 1.0 to 1.23)
     Position offset: (0.05, -0.03, 0.02)
     Rotation offset: (8.5, -4.2, 12.3)
   ```

### Troubleshooting

#### Mod Not Loading

**Symptom**: No console messages from MacheteAnimRandomizer

**Solutions**:
1. Check TFP_Harmony is installed
2. Verify `ModInfo.xml` exists in the mod folder
3. Check game logs for errors:
   - Windows: `%APPDATA%/7DaysToDie/Logs/`
   - Linux: `~/.local/share/7DaysToDie/Logs/`

#### Attacks Not Randomized

**Symptom**: Machete attacks look the same every time

**Solutions**:
1. Verify you're using a machete weapon (check item name)
2. Ensure you're doing normal attacks, not power attacks
3. Check console for error messages
4. Try with different machete variants

#### Build Errors

**Symptom**: Compilation fails with missing references

**Solutions**:
1. Verify all DLLs are in `7dtd-binaries/`
2. Check DLL versions match your game version
3. Update .csproj HintPath references if needed

#### Game Crashes

**Symptom**: Game crashes when attacking

**Solutions**:
1. Check game version compatibility (Alpha 21+)
2. Look for mod conflicts (disable other animation mods)
3. Update to latest version of TFP_Harmony
4. Report the issue with full error logs

## Advanced Configuration

### Adjusting Randomization Ranges

To modify the randomization ranges, edit `MacheteAnimRandomizerPatches.cs`:

```csharp
// Around line 80-100 in Patch_RandomizeMacheteAnimation
RandomizationData randData = new RandomizationData
{
    // Change these values:
    speedMultiplier = (float)(0.8 + random.NextDouble() * 0.7),  // 0.8-1.5x
    
    positionOffset = new Vector3(
        (float)(random.NextDouble() * 0.2 - 0.1),  // Increase for more position variation
        (float)(random.NextDouble() * 0.2 - 0.1),
        (float)(random.NextDouble() * 0.15 - 0.075)
    ),
    
    rotationOffset = new Vector3(
        (float)(random.NextDouble() * 30 - 15),    // Increase for more rotation
        (float)(random.NextDouble() * 20 - 10),
        (float)(random.NextDouble() * 40 - 20)
    ),
};
```

After making changes, rebuild the mod.

### Targeting Different Weapons

To affect weapons other than machetes, edit the weapon detection logic:

```csharp
// Around line 60 in Patch_RandomizeMacheteAnimation
bool isMachete = itemName.Contains("machete");

// Change to target all melee weapons:
bool isMachete = true; // Affects all weapons

// Or target specific weapons:
bool isMachete = itemName.Contains("machete") || 
                 itemName.Contains("sword") || 
                 itemName.Contains("knife");
```

### Disabling Debug Logging

To reduce console spam, comment out the Log.Out lines:

```csharp
// Log.Out($"[MacheteAnimRandomizer] Randomized {itemName} attack for entity {entityId}:");
// Log.Out($"  Speed: {randData.speedMultiplier:F2}x (from {___originalMeleeAttackSpeed:F2} to {randomizedSpeed:F2})");
// Log.Out($"  Position offset: {randData.positionOffset}");
// Log.Out($"  Rotation offset: {randData.rotationOffset}");
```

## Mod Compatibility

### Compatible With
- Most gameplay mods
- UI mods
- World generation mods
- Non-animation weapon mods

### May Conflict With
- Other animation modification mods
- Mods that patch AnimatorMeleeAttackState
- Custom melee weapon animation mods

If you experience conflicts, try loading this mod last or adjusting load order.

## Performance Notes

The mod is designed to be lightweight:
- Randomization occurs only on attack start
- Minimal per-frame overhead during attacks
- Automatic cleanup prevents memory leaks
- No impact when not attacking

Typical performance impact: < 0.1% CPU usage during attacks

## Uninstalling

1. Stop the game
2. Delete the mod folder:
   ```bash
   rm -rf /path/to/7dtd/Mods/MacheteAnimRandomizer
   ```
3. Restart the game

No save file modifications are made, so uninstalling is safe at any time.

## Support and Feedback

### Reporting Issues

When reporting issues, please include:
1. Game version (e.g., Alpha 21.2)
2. Mod version
3. Full error log from console
4. Steps to reproduce
5. List of other installed mods

### Feature Requests

Have ideas for improvements? Suggestions welcome:
- Configuration file support
- Additional weapon types
- Visual effects
- Sound variations
- Multiplayer sync

## Credits

- **Mod Author**: Interzoneism
- **Harmony Library**: Andreas Pardeike
- **Game**: 7 Days to Die by The Fun Pimps
- **Special Thanks**: 7DTD modding community

## License

Free to use and modify. Attribution appreciated but not required.

---

**Happy Randomized Attacking!** 🔪
