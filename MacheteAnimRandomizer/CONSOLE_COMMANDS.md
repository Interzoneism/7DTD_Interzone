# Machete Animation Randomizer - Console Commands

## Overview
This mod adds console commands to dynamically adjust machete animation randomization parameters in-game.

## Console Command: `macheteanim` (alias: `ma`)

### Usage

#### View Current Settings
```
macheteanim show
```
Displays all current randomization parameters and their values.

#### Set Speed Variation
```
macheteanim speed <value>
```
- Sets the speed variation range (+/- around 1.0x)
- Example: `macheteanim speed 0.2` ? animations will play between 0.8x and 1.2x speed
- Default: `0.1` (0.9x to 1.1x)

#### Set Position Offset
```
macheteanim position <x> <y> <z>
```
- Sets the position offset range (+/- in meters for each axis)
- Example: `macheteanim position 0.15 0.15 0.1` ? weapon position varies ±0.15m on X/Y, ±0.1m on Z
- Default: `0.1 0.1 0.075`

#### Set Rotation Offset
```
macheteanim rotation <pitch> <yaw> <roll>
```
- Sets the rotation offset range (+/- in degrees for each axis)
- Example: `macheteanim rotation 20 15 30` ? rotation varies ±20° pitch, ±15° yaw, ±30° roll
- Default: `15 10 20`

#### Set Apply Factor *(Legacy - No Longer Used)*
```
macheteanim applyfactor <value>
```
- **Note:** This parameter is no longer used in the current implementation
- The position and rotation offsets are now applied directly to the game's weapon offset system
- This command remains for backwards compatibility but has no effect

#### Reset to Defaults
```
macheteanim reset
```
Resets all parameters to their default values.

## Examples

### Subtle Randomization
```
macheteanim speed 0.05
macheteanim position 0.05 0.05 0.03
macheteanim rotation 5 5 10
```

### Extreme Randomization
```
macheteanim speed 0.3
macheteanim position 0.3 0.3 0.2
macheteanim rotation 30 20 40
```

### Disable Randomization
```
macheteanim speed 0
macheteanim position 0 0 0
macheteanim rotation 0 0 0
```

## Permissions
- Default Permission Level: **1000** (available to all players)
- Can be used in main menu: **Yes**

## Notes
- Changes take effect immediately for the next attack
- Settings are per-session (reset when game restarts)
- All values must be positive numbers
- Position values are in Unity units (meters)
- Rotation values are in degrees
