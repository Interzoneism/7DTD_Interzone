# Fix Explanation: Why Rotation and Position Weren't Working

## The Problem

Your original approach tried to modify the weapon's `Transform.localPosition` and `Transform.localRotation` directly during the animation's `OnStateUpdate`. However, this didn't work because:

1. **7 Days to Die sets weapon position/rotation from static data**: The game uses `AnimationGunjointOffsetData.AnimationGunjointOffset[holdType]` to position weapons
2. **The transform is reset every frame**: In `AvatarMultiBodyController.SetInRightHand()`, the game constantly resets the weapon transform to match the static offset data
3. **Your changes were being overwritten**: Any direct transform modifications were immediately overwritten by the game's own positioning system

## The Solution

Instead of trying to modify the transform directly, we now **modify the source data** that the game uses:

### Key Changes:

1. **Patch `OnStateEnter` to modify `AnimationGunjointOffsetData`**:
   - Store the original position and rotation values
   - Apply your randomized offsets to the `AnimationGunjointOffsetData.AnimationGunjointOffset` array
   - This is what the game actually reads to position the weapon

2. **Patch `SetInRightHand` to maintain offsets**:
   - Re-applies randomized offsets if `SetInRightHand` is called during an active attack
   - Ensures offsets persist even if the game tries to reset the weapon

3. **Patch `OnStateExit` to restore original values**:
   - Restores the original `AnimationGunjointOffsetData` values when the attack ends
   - Prevents permanent modification of weapon positioning

## How It Works in the Game

```
Player attacks with machete
    ?
OnStateEnter fires
    ?
We modify AnimationGunjointOffsetData[holdType]
    ?
Game calls SetInRightHand (during animation setup/updates)
    ?
Game reads from AnimationGunjointOffsetData[holdType] (our modified values!)
    ?
Weapon is positioned with our randomized offsets
    ?
OnStateExit fires
    ?
We restore original AnimationGunjointOffsetData values
```

## What Was Removed

- **`WeaponPositionApplyFactor`**: No longer needed since we modify the data source directly
- **`OnStateUpdate` patch**: Replaced with `SetInRightHand` patch
- **Direct transform modifications**: No longer attempting to modify `localPosition`/`localRotation`

## Testing

The speed randomization was already working because you were correctly using `animator.SetFloat("MeleeAttackSpeed", ...)`. The position and rotation randomization now works the same way - by modifying the game's source data rather than fighting against it.

Try these console commands to test:
```
macheteanim position 0.5 0.5 0.5    # Large position variation
macheteanim rotation 45 45 45       # Large rotation variation
macheteanim show                     # Display current settings
```

You should now see visible position and rotation changes with each attack!
