# Deep Dive Analysis: Machete Animation Randomization Fix

## Problem Summary
The rotation and position randomization was not working for machete animations in 7 Days to Die, while speed randomization worked correctly.

## Root Cause Analysis

### How 7DTD Handles Weapon Animations

After deep investigation of the decompiled source code, here's the critical flow:

#### 1. **Animation Trigger Flow**
```
Player attacks with melee weapon
  ?
AnimatorMeleeAttackState.OnStateEnter() called
  ?
Animation speed is set via animator.SetFloat("MeleeAttackSpeed", speed)
  ?
Animation plays with the set speed
```

#### 2. **Weapon Position/Rotation Flow**
```
Weapon is equipped or view changes (FPV/TPV)
  ?
AvatarMultiBodyController.SetInRightHand(Transform) called
  ?
Reads AnimationGunjointOffsetData.AnimationGunjointOffset[holdType]
  ?
Sets transform.localPosition and transform.localRotation
  ?
Weapon visual is positioned (ONE TIME EVENT)
```

### The Critical Discovery

**KEY INSIGHT**: `SetInRightHand` is **NOT** called during each attack animation. It's only called when:
- The weapon is first equipped
- The player switches between FPV (First Person View) and TPV (Third Person View)
- The weapon model is changed

This means:
1. ? **Speed works**: Because `animator.SetFloat()` is called in `OnStateEnter`, which runs for each attack
2. ? **Position/Rotation don't work**: Because modifying `AnimationGunjointOffsetData` in `OnStateEnter` has no effect - the weapon transform was already set earlier by `SetInRightHand`

## The Solution

### Approach 1: Direct Transform Manipulation (Implemented)

Instead of modifying the static `AnimationGunjointOffsetData` array, we directly manipulate the weapon's `Transform` component:

```csharp
// In OnStateEnter:
Transform weaponTransform = GetWeaponTransform(entity);
weaponTransform.localPosition += randomPositionOffset;
weaponTransform.localRotation *= Quaternion.Euler(randomRotationOffset);
```

### Approach 2: Continuous Update (Also Implemented)

We also patch `OnStateUpdate` to maintain the offsets throughout the animation, in case the game engine or other systems try to reset the transform:

```csharp
// In OnStateUpdate:
// Check if transform has been reset
if (positionDeviated || rotationDeviated)
{
    weaponTransform.localPosition = expectedPosition;
    weaponTransform.localRotation = expectedRotation;
}
```

### Approach 3: Intercept SetInRightHand (Also Implemented)

We patch `SetInRightHand` to reapply our offsets if it's called during an active attack:

```csharp
// In SetInRightHand Postfix:
if (attackIsActive)
{
    transform.localPosition += storedOffset;
    transform.localRotation *= storedRotationOffset;
}
```

### Approach 4: Restore on Exit (Implemented)

We patch `OnStateExit` to restore the weapon to its original position/rotation:

```csharp
// In OnStateExit:
weaponTransform.localPosition = originalPosition;
weaponTransform.localRotation = Quaternion.Euler(originalRotation);
```

## Code Architecture

### Key Classes in 7DTD

1. **`AnimatorMeleeAttackState`**: StateMachineBehaviour that controls melee attack animations
   - `OnStateEnter()`: Called when attack animation starts
   - `OnStateUpdate()`: Called every frame during animation
   - `OnStateExit()`: Called when animation ends

2. **`AvatarMultiBodyController`**: Manages the avatar's body parts and held items
   - `SetInRightHand()`: Positions the weapon transform in the right hand
   - `Update()`: Updates animation parameters every frame

3. **`AnimationGunjointOffsetData`**: Static data structure holding default weapon offsets
   - `AnimationGunjointOffset[]`: Array indexed by hold type
   - `AnimationGunjointOffsets`: Struct with position and rotation Vector3s

4. **`ItemActionMelee`**: The action handler for melee attacks
   - `ExecuteAction()`: Triggers the attack
   - `OnHoldingUpdate()`: Updates during attack

### Why AnimationGunjointOffsetData Modification Failed

The original code tried to modify `AnimationGunjointOffsetData.AnimationGunjointOffset[holdType]` in `OnStateEnter`. However:

1. **Static Data Pattern**: This is a static array that holds *default* values
2. **One-Time Application**: These values are only read during `SetInRightHand`, which is not called during attacks
3. **No Continuous Polling**: The animation system doesn't continuously read these values during playback

### Why Direct Transform Manipulation Works

By directly modifying `weaponTransform.localPosition` and `weaponTransform.localRotation`:

1. **Immediate Visual Effect**: Unity immediately updates the visual representation
2. **Frame-by-Frame Persistence**: The transform retains these values across frames
3. **No Dependency on Game Systems**: We're not waiting for the game to read our data

## Testing Recommendations

### In-Game Testing

1. **Position Variation**:
   ```
   macheteanim position 0.2 0.2 0.15
   ```
   - Attack multiple times
   - Weapon should visibly shift position each attack
   - Watch for: X (left/right), Y (up/down), Z (forward/back)

2. **Rotation Variation**:
   ```
   macheteanim rotation 30 20 40
   ```
   - Attack multiple times
   - Weapon should rotate differently each attack
   - Watch for: Pitch (nose up/down), Yaw (turn left/right), Roll (barrel roll)

3. **Combined Test**:
   ```
   macheteanim speed 0.3
   macheteanim position 0.15 0.15 0.1
   macheteanim rotation 20 15 25
   ```
   - Each attack should have: different speed, position, AND rotation

### Debug Logging

The mod includes comprehensive debug logging:
- Check F1 console for `[MacheteAnimRandomizer]` messages
- Logs show: speed multiplier, position offset, rotation offset
- Logs confirm when transforms are applied/restored

## Potential Issues and Solutions

### Issue 1: Offsets Reset Between Attacks
**Symptom**: First attack works, subsequent attacks don't vary
**Cause**: `OnStateExit` is restoring too aggressively
**Solution**: Verify timing in cleanup code, ensure new random values are generated each attack

### Issue 2: Offsets Not Applied in FPV
**Symptom**: Works in third-person, not first-person
**Cause**: FPV uses different weapon positioning system
**Solution**: Check the condition `!___entity.emodel.IsFPV` - we currently skip FPV to avoid camera issues

### Issue 3: Weapon "Snaps Back" Mid-Animation
**Symptom**: Weapon starts offset then returns to normal during swing
**Cause**: Game engine or other mod resetting transform
**Solution**: The `OnStateUpdate` patch should catch and fix this

## Performance Considerations

### Overhead Analysis

1. **OnStateEnter**: Runs once per attack
   - Cost: Negligible (few vector calculations)
   - Memory: Small RandomizationData struct per active entity

2. **OnStateUpdate**: Runs every frame during attack
   - Cost: Transform comparison (distance + angle calculation)
   - Mitigation: Only runs during active attacks (< 2 seconds)

3. **Memory**: Dictionary of RandomizationData
   - Cleanup: Periodic removal of old entries (10+ seconds old)
   - Expected size: < 10 entities typically

### Optimization Notes

- No heap allocations in hot path
- Vector operations are Unity built-ins (highly optimized)
- Dictionary lookups are O(1)
- Cleanup runs periodically, not every frame

## Future Enhancements

### Possible Improvements

1. **Per-Weapon Configuration**:
   - Allow different randomization ranges for different weapons
   - XML config file per weapon type

2. **Animation-Aware Randomization**:
   - Different offsets for different attack animations
   - Overhead vs. side swing variations

3. **Smoothing**:
   - Interpolate between attacks for less jarring transitions
   - Requires tracking previous attack values

4. **FPV Support**:
   - Carefully apply smaller offsets for first-person view
   - Avoid camera clipping or motion sickness

5. **Network Sync**:
   - Ensure randomization is same for all clients
   - Seed random generator with network-synced value

## Conclusion

The fix works by directly manipulating Unity Transform components instead of relying on the game's static data structures. This ensures immediate visual effect and persistence throughout the animation. The multi-layered approach (OnStateEnter + OnStateUpdate + SetInRightHand + OnStateExit) provides redundancy against various game engine behaviors.

The key insight was understanding **when** the game applies weapon positioning (equipment time, not attack time) and working around that by directly controlling the visual representation.
