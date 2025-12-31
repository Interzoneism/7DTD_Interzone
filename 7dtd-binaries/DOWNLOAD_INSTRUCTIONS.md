# Required DLLs for Building Mods

To build the MacheteAnimRandomizer mod, you need to place the following DLLs in this directory:

## From 7 Days to Die Game Files
Copy these from your game installation folder `7DaysToDieServer_Data/Managed/`:

1. **Assembly-CSharp.dll** - Main game assembly
2. **UnityEngine.CoreModule.dll** - Unity engine core

## From Harmony Mod
Copy this from the TFP_Harmony mod or download from GitHub:

3. **0Harmony.dll** - Harmony patching library
   - Download from: https://github.com/pardeike/Harmony/releases/
   - Version: 2.2.2 or later

## Current Status
- [ ] Assembly-CSharp.dll - NOT FOUND
- [ ] UnityEngine.CoreModule.dll - NOT FOUND  
- [ ] 0Harmony.dll - NOT FOUND

## Alternative: Update Project References
Instead of copying DLLs here, you can edit the .csproj file to point directly to your game's Managed folder.
