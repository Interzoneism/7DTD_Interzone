# Testing Guide

## Quick Testing Steps

After loading the mod in-game, follow these steps to verify the fix works:

### 1. Test Speed Randomization (Already Working)
```
macheteanim speed 0.3
```
- Attack several times with a machete
- You should see varying attack speeds (0.7x to 1.3x)
- ? This was already working before the fix

### 2. Test Position Randomization (Now Fixed!)
```
macheteanim position 0.3 0.3 0.2
```
- Attack several times with a machete
- **Watch the weapon's position in your hand** - it should shift around significantly
- The weapon should be in noticeably different positions with each attack
- ? This should NOW be working

### 3. Test Rotation Randomization (Now Fixed!)
```
macheteanim rotation 30 30 45
```
- Attack several times with a machete
- **Watch the weapon's angle in your hand** - it should rotate differently each time
- The weapon should tilt, twist, and angle differently with each swing
- ? This should NOW be working

### 4. Extreme Values Test
```
macheteanim position 0.5 0.5 0.5
macheteanim rotation 60 60 90
```
- Attack several times
- The weapon should be **dramatically** different each time
- Some attacks might look weird/broken - that's expected with extreme values
- This confirms the system is working

### 5. Check Console Logs
Enable the console output window and look for:
```
[MacheteAnimRandomizer] Randomized machete attack for entity X:
  Speed: 1.05x (from 1.00 to 1.05)
  Position: (0, 0, 0) -> (0.15, -0.08, 0.05) (offset: (0.15, -0.08, 0.05))
  Rotation: (0, 0, 0) -> (12.5, -8.3, 15.2) (offset: (12.5, -8.3, 15.2))
```

And when the attack ends:
```
[MacheteAnimRandomizer] Restored original offsets for entity X
```

### 6. Visual Comparison

**Before the fix:**
- Speed changed: ?
- Position changed: ? (stayed the same)
- Rotation changed: ? (stayed the same)

**After the fix:**
- Speed changed: ?
- Position changed: ? (visibly shifts around)
- Rotation changed: ? (visibly rotates)

## Troubleshooting

If position/rotation still don't work:

1. **Check you're using a machete**: The mod only affects weapons with "machete" in the name
2. **Check action index**: Only normal attacks (action 0) are randomized, not power attacks
3. **Check the logs**: Make sure you see the "Randomized" and "Restored" messages
4. **Try extreme values**: Use very large values to make the effect obvious
5. **Check third-person view**: Sometimes the effect is more visible in third-person

## Expected Behavior

Each attack should:
1. Generate new random offsets (logged to console)
2. Apply offsets to `AnimationGunjointOffsetData`
3. Weapon appears in randomized position/rotation during attack
4. Restore original offsets when attack ends
5. Next attack gets completely new random values

The weapon should **visibly shift and rotate** between attacks!
