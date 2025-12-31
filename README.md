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


### Resources

#### 📚 Decompiled Source Code
Includes decompiled source code (for v2.4) for reference only. Decompiled source code cannot be changed, it is only for reading and understanding the base game.

**Locations:**
- `7DTD_Decompiled_AssemblyCSharp/` - Main game code
- `7DTD_Decompiled_UnityEngine.AnimationModule/` - Unity animation system


#### 🔧 Base Template
- `ModBase/` - Basic mod template for creating new mods


## Technical Details

### Animation System Overview
The mods in this repository primarily work with:
- `AnimatorMeleeAttackState` - Core melee attack state machine
- `ItemActionDynamicMelee` - Modern melee action system
- `AnimationGunjointOffsetData` - Weapon positioning system
- Harmony patches for runtime code modification


**Note:** This is a community project and is not affiliated with or endorsed by The Fun Pimps.
