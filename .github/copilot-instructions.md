# 7DTD Interzone - Copilot Coding Agent Instructions

## Repository Overview

**Project Type:** 7 Days to Die modding framework and animation testing mod
**Primary Language:** C# (.NET Framework 4.8)
**Size:** ~43MB, with ~4,000+ decompiled C# source files
**Purpose:** Animation test mod for 7 Days to Die v2.4 focusing on melee weapon animation modifications

This repository contains:
1. **Decompiled game source code** (read-only reference) - DO NOT MODIFY
2. **ModBase** - Working mod project template using the game's API
3. **Examples** - XML and Harmony patch examples for animation modding
4. **Documentation** - Comprehensive melee animation modding guide

## Critical Build Requirements

### Prerequisites - READ FIRST

**IMPORTANT:** This project uses .NET Framework 4.8 which is NOT available in standard Linux environments. The project **CANNOT be built** on Linux without Windows-specific dependencies.

**Build Prerequisites (Windows only):**
- .NET Framework 4.8 Developer Pack
- Visual Studio 2017 or later (or MSBuild tools)
- 7 Days to Die game installation OR dedicated server files
- Assembly-CSharp.dll from the game (required for compilation)

**On Linux/CI environments:**
- Building will FAIL with error: `MSB3644: The reference assemblies for .NETFramework,Version=v4.8 were not found`
- This is EXPECTED and CANNOT be resolved without installing Windows-specific .NET Framework components
- Code changes should focus on logic/structure; build verification requires Windows environment

### Required Game DLLs

Before building, you MUST have the game's DLL files. Two options:

**Option 1 - Copy DLLs to 7dtd-binaries folder:**
```
7dtd-binaries/
  ├── Assembly-CSharp.dll
  └── [other DLLs from 7DaysToDieServer_Data/Managed as needed]
```

**Option 2 - Update project references:**
Edit `ModBase/ModBase.csproj` to point to your game installation:
```xml
<HintPath>C:\Path\To\7DaysToDie\7DaysToDie_Data\Managed\Assembly-CSharp.dll</HintPath>
```

## Build Instructions

### Building ModBase (Windows Only)

**Using Visual Studio:**
1. Ensure .NET Framework 4.8 Developer Pack is installed
2. Ensure Assembly-CSharp.dll is available (see above)
3. Open `ModBase.sln` in Visual Studio
4. Build solution (F6 or Build > Build Solution)
5. On success, DLL is copied to `ModBase/Mods/ModBase/` automatically via PostBuildEvent

**Using Command Line (Windows):**
```bash
# Ensure MSBuild is in PATH
msbuild ModBase.sln /p:Configuration=Release
```

**Expected Build Output:**
- `ModBase/bin/Release/ModBase.dll`
- `ModBase/Mods/ModBase/ModBase.dll` (auto-copied)
- `ModBase/Mods/ModBase/ModInfo.xml` (auto-copied)

**Build Time:** Typically < 10 seconds on modern hardware

### Post-Build Process

The ModBase.csproj has a PostBuildEvent that automatically:
1. Creates/cleans `ModBase/Mods/ModBase/` directory
2. Copies `ModBase.dll` to the Mods folder
3. Copies `ModInfo.xml` to the Mods folder

This prepares the mod for installation into the game's Mods directory.

## Project Structure

### Root Level Files
```
.
├── README.md                           # Project overview
├── MELEE_ANIMATION_MODDING_GUIDE.md   # 740-line technical guide
├── ModBase.sln                         # Visual Studio solution
├── .gitignore                          # Excludes bin/, obj/, *.dll
├── .gitattributes                      # Git LFS configuration
├── 7dtd-binaries/                      # Game DLL references (user-provided)
├── 7DTD_Decompiled_AssemblyCSharp/    # Decompiled game code (REFERENCE ONLY)
├── 7DTD_Decompiled_UnityEngine.AnimationModule/  # Unity animation code (REFERENCE ONLY)
├── Examples/                           # Modding examples
└── ModBase/                            # Working mod project
```

### ModBase Project
```
ModBase/
├── ModBase.csproj      # C# project file
├── API.cs              # Main mod API implementation (extends ModApiAbstract)
├── ModInfo.xml         # Mod metadata
├── Properties/
│   └── AssemblyInfo.cs
└── Mods/               # Build output directory (auto-generated)
    └── ModBase/        # Ready-to-install mod files
```

### Decompiled Source (REFERENCE ONLY - DO NOT MODIFY)

**7DTD_Decompiled_AssemblyCSharp/** - ~4,000 C# files, 735K+ lines
- Contains decompiled game source for v2.4
- Key files for animation modding:
  - `AnimatorMeleeAttackState.cs` - Melee attack state machine
  - `ItemActionDynamicMelee.cs` - Modern melee system with grazing
  - `ItemActionMelee.cs` - Legacy melee system
  - `AvatarController.cs` - Entity animation controller
  - `AnimationDelayData.cs` - Static animation timing data
  - `AnimationGunjointOffsetData.cs` - Weapon positioning

**CRITICAL:** These files are READ-ONLY references. Changes here have NO effect on the game or mods.

### Examples Directory
```
Examples/
├── README.md                       # Quick start guide for modding
├── XML/
│   └── MeleeAnimationExamples.xml # 7 XML-based mod examples
└── HarmonyPatches/
    └── MeleeAnimationPatches.cs   # 7 C# Harmony patch examples
```

## Modding Architecture

### Two Modding Approaches

**1. XML Modding (No code, beginner-friendly):**
- Modify weapon properties via XML patches
- Change attack speed, range, damage, grazing hits
- No compilation required
- Examples in `Examples/XML/MeleeAnimationExamples.xml`

**2. Harmony Patches (Advanced, requires C#):**
- Runtime code modifications using Harmony library
- Requires 0Harmony.dll (from TFP_Harmony mod)
- Full control over game behavior
- Examples in `Examples/HarmonyPatches/MeleeAnimationPatches.cs`

### Key Dependencies

**For XML Modding:**
- None (just edit XML files)

**For Harmony Modding:**
- 0Harmony.dll (from TFP_Harmony mod - NOT included in repo)
- Assembly-CSharp.dll (from game installation)
- Unity DLLs (from game installation)
- .NET Framework 4.8 SDK

## Development Workflow

### For XML Modifications
1. Copy examples from `Examples/XML/MeleeAnimationExamples.xml`
2. Create mod folder structure:
   ```
   Mods/YourModName/
   ├── ModInfo.xml
   └── Config/
       └── items.xml
   ```
3. Test in-game (no compilation needed)

### For C# Mod Development
1. Reference ModBase project structure
2. Add required DLL references (0Harmony.dll, Assembly-CSharp.dll)
3. Implement IModApi interface or extend ModApiAbstract
4. Build project (Windows only)
5. Copy DLL and ModInfo.xml to game's Mods folder
6. Test in-game

### Testing Workflow
Since building requires Windows/.NET Framework 4.8:
- **Development:** Make code changes and validate logic/syntax
- **Testing:** Build and test on Windows machine with game installed
- **Validation:** Use in-game console commands:
  - `giveself itemname` - Spawn items
  - `buff playername buffname` - Apply buffs
  - `showswings` - Visualize hitboxes
  - `debugshot` - Screenshot with debug info

## Common Issues and Workarounds

### Build Errors

**Error: MSB3644 - .NET Framework 4.8 not found**
- **Cause:** Building on Linux or without .NET Framework 4.8 Developer Pack
- **Solution:** Build on Windows with .NET Framework 4.8 installed
- **Workaround:** Focus on code logic; defer compilation to Windows environment

**Error: Assembly-CSharp.dll not found**
- **Cause:** Missing game DLLs
- **Solution:** Copy DLLs to `7dtd-binaries/` folder OR update HintPath in .csproj
- **Location:** Game installation at `7DaysToDie_Data/Managed/` or `7DaysToDieServer_Data/Managed/`

**Error: PostBuildEvent failed**
- **Cause:** Permission issues or path problems
- **Solution:** Ensure `ModBase/Mods/` directory is writable
- **Workaround:** Manually copy DLL and ModInfo.xml after successful build

### Runtime Issues

**Mod not loading in-game**
- Verify ModInfo.xml exists in mod folder
- Check game logs at `%AppData%\7DaysToDie\output_log.txt`
- Ensure DLL is not blocked (right-click > Properties > Unblock)

**Harmony patches not applying**
- Ensure TFP_Harmony mod loads before your mod
- Check Harmony ID doesn't conflict with other mods
- Use try-catch blocks to prevent crashes

## File Modification Guidelines

### DO Modify
- `ModBase/API.cs` - Implement mod functionality
- `ModBase/ModInfo.xml` - Update mod metadata
- `Examples/*` - Add new examples
- Documentation files (*.md)

### DO NOT Modify
- `7DTD_Decompiled_AssemblyCSharp/*` - Reference only, changes have no effect
- `7DTD_Decompiled_UnityEngine.AnimationModule/*` - Reference only
- `ModBase/Properties/AssemblyInfo.cs` - Unless changing assembly version

### Ignored by Git
- `bin/` and `obj/` directories (build artifacts)
- `*.dll` files (game binaries, user-specific)
- `.vs/` (Visual Studio user files)
- Build output in `ModBase/Mods/` (auto-generated)

## Testing and Validation

### No Automated Tests
This repository has NO test infrastructure. Validation is manual via in-game testing.

### Manual Validation Steps
1. Build succeeds without errors (Windows only)
2. DLL and ModInfo.xml appear in `ModBase/Mods/ModBase/`
3. Copy mod to game's Mods folder
4. Launch game and check mod loads
5. Test mod functionality in-game
6. Check game logs for errors

### In-Game Console Commands
```
giveself <itemname>              # Spawn items for testing
buff <playername> <buffname>     # Apply buffs/debuffs
showswings                       # Visualize attack hitboxes
showhits                         # Show raycast hit detection
debugshot                        # Screenshot with debug overlay
```

## Key Conventions

### Code Style
- Follow existing C# conventions in ModBase/API.cs
- Use PascalCase for public members
- Use camelCase for private fields
- Add XML documentation comments for public APIs

### Mod Structure
- ModInfo.xml is REQUIRED for all mods
- DLL name should match mod name
- Use semantic versioning (e.g., 1.0.0)

### Animation Modding Specific
- Attack speed: Use `AttacksPerMinute` passive effect (60 = 1 attack/sec)
- Hit timing: `RaycastTime` in seconds (default ~0.3)
- Range: `Range` for entities, `BlockRange` for blocks
- Grazing: Only works with `ItemActionDynamicMelee`, not `ItemActionMelee`

## Version Information

- **Game Version:** 7 Days to Die v2.4 (Alpha 21+)
- **Target Framework:** .NET Framework 4.8
- **Unity Version:** (Check game installation)
- **Last Updated:** December 2024

## Important Notes for Coding Agents

1. **Building is Windows-only** - Don't attempt to fix Linux build errors for .NET Framework
2. **Decompiled code is reference-only** - Never suggest modifications to 7DTD_Decompiled_* folders
3. **Game DLLs are user-provided** - Don't include DLLs in commits
4. **No CI/CD exists** - Manual testing required
5. **Trust these instructions** - If information here seems incomplete, ask for clarification before searching

## Additional Resources

- **Full Technical Guide:** `/MELEE_ANIMATION_MODDING_GUIDE.md` - 740 lines covering:
  - Animation pipeline architecture
  - Key classes and components
  - Animation speed and timing calculations
  - Passive effects system
  - 5 practical Harmony patch examples
  - Common issues and debugging tips

- **XML Examples:** `/Examples/XML/MeleeAnimationExamples.xml`
  - 7 complete examples from basic to advanced

- **C# Examples:** `/Examples/HarmonyPatches/MeleeAnimationPatches.cs`
  - 7 Harmony patch examples with detailed comments

- **Community Resources:**
  - 7DTD Official Forums
  - 7DTD Modding Discord
  - Harmony Documentation: https://harmony.pardeike.net/
