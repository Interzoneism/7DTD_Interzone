# Melee Animation Randomizer - Technical Solution

## Deep Dive Analysis of Decompiled Source Code

After analyzing the decompiled 7DTD source code, here's how melee animations work:

### Key Classes Identified

1. **`vp_FPWeapon`** - First-person weapon controller with spring physics
   - `m_PositionSpring` / `m_RotationSpring` - Spring physics for smooth weapon movement
   - `AddSoftForce(Vector3 positional, Vector3 angular, int frames)` - Applies forces over time
   - Springs have `RestState`, `Stiffness`, `Damping` properties

2. **`vp_FPWeaponMeleeAttack`** - Melee attack handler
   - Calls `m_Weapon.AddSoftForce(SwingPositionSoftForce, SwingRotationSoftForce, SwingSoftForceFrames)`
   - Default values: `SwingPositionSoftForce=(-0.5,-0.1,0.3)`, `SwingRotationSoftForce=(50,-25,0)`
   - `SwingSoftForceFrames=50` for smooth application

3. **`vp_Spring`** - Physics spring implementation
   - `AddSoftForce(Vector3 force, float frames)` - Distributes force over multiple frames
   - Updates in `FixedUpdate()` calculating velocity and position

4. **`AnimatorMeleeAttackState`** - Animator state machine behavior
   - Controls `MeleeAttackSpeed` animator parameter
   - Calculates attack timing based on `AttacksPerMinute` passive effect

5. **`ItemActionDynamicMelee`** - Modern melee action system
   - `StartAttack()` initiates attacks, triggers animations
   - Works with animator state machine

## Why Previous Approaches Failed

1. **AnimationGunjointOffsetData** - TPV-only, FPV weapons use Vector3.zero
2. **Direct transform modification** - Animator overwrites every frame
3. **Animation clips** - Read-only baked keyframe data

## Solution: Spring System Force Injection

The spring system is **additive** - forces ADD to the base position rather than replacing it. This means we can inject random forces that work WITH the animation.

### Implementation Strategy

**Patch 1: `ItemActionDynamicMelee.StartAttack` (Postfix)**
- Most reliable detection point for melee attack initiation
- Apply additional random forces via `vp_FPWeapon.AddSoftForce()`

**Patch 2: `vp_FPWeapon.AddSoftForce(Vector3, Vector3, int)` (Prefix)**
- Intercepts existing swing forces from `vp_FPWeaponMeleeAttack`
- Modifies force parameters to add random variation
- Only affects melee weapons (checks for significant rotation force)

**Patch 3: `AnimatorMeleeAttackState.OnStateEnter` (Postfix)**
- Randomizes `MeleeAttackSpeed` animator parameter
- Creates speed variation in attack animations

## Configuration Defaults (Based on Game Values)

| Parameter | Default | Rationale |
|-----------|---------|-----------|
| `SpeedPlusMinus` | 0.12 | ~12% variation feels natural |
| `PositionForcePlusMinus` | (0.15, 0.08, 0.12) | Based on game's (-0.5,-0.1,0.3) |
| `RotationForcePlusMinus` | (15, 12, 8) | Based on game's (50,-25,0) |
| `SoftForceFrames` | 25 | Half game's 50 for faster response |

## Console Commands

```
macheteanim speed <value>           - Set speed variation (+/- range)
macheteanim position <x> <y> <z>    - Set position force per axis
macheteanim rotation <p> <y> <r>    - Set rotation force per axis (degrees)
macheteanim frames <value>          - Set soft force frames
macheteanim allmelee <true|false>   - Toggle all melee weapons
macheteanim debug <true|false>      - Toggle debug logging
macheteanim show                    - Display current settings
macheteanim reset                   - Reset to defaults
```

## Why This Works

1. **Spring system is additive** - Forces ADD to animation, don't overwrite
2. **Same technique as game** - `vp_FPWeaponMeleeAttack` uses identical approach
3. **Smooth motion** - `AddSoftForce` distributes force over multiple frames
4. **Works with animator** - Complements rather than fights animation system
