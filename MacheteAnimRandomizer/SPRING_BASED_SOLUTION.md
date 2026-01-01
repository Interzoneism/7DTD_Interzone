# Melee Animation Randomizer - Technical Solution

## Problem
Previous attempts to randomize melee attack animations in first-person view (FPV) did NOT work because:

1. **AnimationGunjointOffsetData is TPV-only**: In FPV, weapons are positioned at `Vector3.zero` and animated bones control movement
2. **Direct transform modification fails**: The animator overwrites transform changes every frame
3. **Animation clips are read-only**: Unity AnimationClips contain baked keyframe data that cannot be modified at runtime

## Solution: Spring System Force Injection

The game uses `vp_FPWeapon` with a **spring-based physics system** that ADDS forces on top of the base position. The `vp_FPWeaponMeleeAttack.UpdateAttack()` method already calls `AddSoftForce()` for swing motion:

```csharp
m_Weapon.AddSoftForce(this.SwingPositionSoftForce, this.SwingRotationSoftForce, this.SwingSoftForceFrames);
```

We inject **additional random forces** into this spring system when an attack starts, creating procedural variation that works WITH the animator instead of fighting it.

## Implementation

### Patch 1: `Patch_FPVMeleeRandomForce`
- **Target**: `vp_FPWeapon.AddSoftForce` (Prefix)
- **Detects melee swings**: Checks if rotation force magnitude > 0.1 (melee swings have significant rotation)
- **Modifies forces**: Adds random position/rotation forces to the parameters before they reach the spring system
- **Cooldown**: 0.1s between triggers to prevent double-randomization per swing

### Patch 2: `Patch_RandomizeMeleeSpeed`
- **Target**: `AnimatorMeleeAttackState.OnStateEnter`
- **Modifies**: `MeleeAttackSpeed` animator parameter for speed variation

## Configuration

| Parameter | Default | Description |
|-----------|---------|-------------|
| `SpeedPlusMinus` | 0.15 | Speed variation range (0.85x to 1.15x) |
| `PositionForcePlusMinus` | (0.03, 0.03, 0.02) | Position force per axis |
| `RotationForcePlusMinus` | (8, 8, 5) | Rotation force per axis (degrees) |
| `SoftForceFrames` | 15 | Smoothing frames |
| `AffectAllMeleeWeapons` | true | Affect all melee or just machete-type |

## Console Commands

```
macheteanim speed <value>           - Set speed variation
macheteanim position <x> <y> <z>    - Set position force
macheteanim rotation <p> <y> <r>    - Set rotation force  
macheteanim frames <value>          - Set soft force frames
macheteanim allmelee <true|false>   - Toggle all melee weapons
macheteanim show                    - Display current settings
macheteanim reset                   - Reset to defaults
```

## Why This Works

1. **Spring system is additive**: Forces ADD to the base position, not overwrite it
2. **Works with animator**: The spring applies forces that complement the animation
3. **Smooth motion**: `AddSoftForce` applies force over multiple frames for natural movement
4. **Same technique as game**: This is how the game creates swing motion; we just add randomness
