# 7 Days to Die - Interzone Mods

Animation mods and modding resources for 7 Days to Die v2.4+

## Repository Contents

### Mods

#### 🔪 Machete Animation Randomizer
A Harmony-based mod that randomizes machete melee attack animations in real-time.

**Features:**
- Dynamic animation speed variation (0.8x - 1.5x)
- Randomized weapon position offsets
- Randomized weapon rotation angles
- Each attack feels unique and unpredictable
- Only affects normal attacks (not power attacks)

**Location:** `MacheteAnimRandomizer/`
**Status:** ✅ Complete and ready to use

[See detailed README](MacheteAnimRandomizer/README.md) | [Build & Usage Guide](MacheteAnimRandomizer/BUILD_AND_USAGE_GUIDE.md)

### Resources

#### 📚 Decompiled Source Code
Includes decompiled source code (for v2.4) for reference only. Decompiled source code cannot be changed, it is only for reading and understanding the base game.

**Locations:**
- `7DTD_Decompiled_AssemblyCSharp/` - Main game code
- `7DTD_Decompiled_UnityEngine.AnimationModule/` - Unity animation system

#### 📖 Modding Documentation
- `MELEE_ANIMATION_MODDING_GUIDE.md` - Comprehensive guide to melee animation modding
- `Examples/` - Example patches and XML configurations
  - `Examples/HarmonyPatches/MeleeAnimationPatches.cs` - Example Harmony patches
  - `Examples/XML/MeleeAnimationExamples.xml` - Example XML modifications

#### 🔧 Base Template
- `ModBase/` - Basic mod template for creating new mods

## Quick Start

### For Users (Installing Mods)

1. Install [TFP_Harmony](https://github.com/7DaysToDieModding/TFP_Harmony) mod (required)
2. Download the mod you want from the `Mods/` folders
3. Copy to your `7DaysToDie/Mods/` directory
4. Launch the game

### For Developers (Creating Mods)

1. Clone this repository
2. Copy required DLLs to `7dtd-binaries/` (see `7dtd-binaries/DOWNLOAD_INSTRUCTIONS.md`)
3. Use `ModBase/` as a template or explore `MacheteAnimRandomizer/` as an example
4. Read `MELEE_ANIMATION_MODDING_GUIDE.md` for technical details

## Project Structure

```
7DTD_Interzone/
├── MacheteAnimRandomizer/          # Machete animation randomizer mod
│   ├── API.cs                      # Mod initialization
│   ├── MacheteAnimRandomizerPatches.cs  # Harmony patches
│   ├── ModInfo.xml                 # Mod metadata
│   └── README.md                   # Mod documentation
├── ModBase/                        # Base mod template
├── Examples/                       # Example code and configurations
│   ├── HarmonyPatches/            # Example Harmony patches
│   └── XML/                       # Example XML modifications
├── 7DTD_Decompiled_AssemblyCSharp/ # Decompiled game code (reference)
├── 7DTD_Decompiled_UnityEngine.AnimationModule/ # Unity animation code (reference)
├── 7dtd-binaries/                 # DLL dependencies (user must provide)
└── MELEE_ANIMATION_MODDING_GUIDE.md  # Comprehensive modding guide
```

## Technical Details

### Animation System Overview
The mods in this repository primarily work with:
- `AnimatorMeleeAttackState` - Core melee attack state machine
- `ItemActionDynamicMelee` - Modern melee action system
- `AnimationGunjointOffsetData` - Weapon positioning system
- Harmony patches for runtime code modification

### Requirements
- **Game Version:** 7 Days to Die Alpha 21+
- **Framework:** .NET Framework 4.8
- **Modding Tool:** Harmony 2.2.2+

## Contributing

Contributions are welcome! Whether it's:
- New animation mods
- Improvements to existing mods
- Documentation updates
- Bug fixes

Please feel free to submit issues or pull requests.

## License

This project contains:
- **Original Mods:** Free to use and modify (attribution appreciated)
- **Decompiled Code:** Reference only, belongs to The Fun Pimps
- **Examples:** Free to use as templates

## Credits

- **Game:** 7 Days to Die by The Fun Pimps
- **Harmony:** Andreas Pardeike
- **Community:** 7DTD Modding Community

## Support

For questions, issues, or suggestions:
- Open an issue on GitHub
- Check the modding guide for technical details
- Review example code for implementation patterns

---

**Note:** This is a community project and is not affiliated with or endorsed by The Fun Pimps.
