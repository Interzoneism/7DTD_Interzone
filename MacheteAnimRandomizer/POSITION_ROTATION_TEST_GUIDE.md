# Testing Guide: Verifying Position & Rotation Randomization

## Quick Test Procedure

### Step 1: Load into Game
1. Start 7 Days to Die
2. Load or create a world
3. Equip a machete (or any melee weapon with "machete", "blade", or "knife" in the name)

### Step 2: Configure Randomization
Open console (F1) and enter these commands:

```
macheteanim position 0.3 0.3 0.2
macheteanim rotation 45 30 45
macheteanim speed 0.2
```

These values are VERY exaggerated to make the effect obvious:
- Position will vary ±0.3m on X/Y axes, ±0.2m on Z
- Rotation will vary ±45° pitch, ±30° yaw, ±45° roll
- Speed will vary from 0.8x to 1.2x

### Step 3: Perform Attacks
1. Look at a wall or the ground
2. Perform several left-click attacks (not power attacks)
3. Watch the machete carefully

### Step 4: Check Debug Logs
1. Keep console (F1) open
2. Look for messages like:
   ```
   [MacheteAnimRandomizer] Randomized melee... attack for entity 171:
     Speed: 1.15x (from 1.00 to 1.15)
     Position Offset: (0.123, -0.087, 0.056)
     Rotation Offset: (23.4, -12.8, 31.2)
   [MacheteAnimRandomizer] Applied transform to weapon:
     New Position: (0.123, -0.087, 0.056)
     New Rotation: (23.4, -12.8, 31.2)
   ```

## What to Look For

### ? Position Randomization Working:
- **X-axis (left/right)**: Machete shifts horizontally with each attack
- **Y-axis (up/down)**: Machete shifts vertically with each attack
- **Z-axis (forward/back)**: Machete appears closer or farther from camera

**Visual cue**: The machete should appear in a slightly different location for each swing, as if you're attacking from different starting positions.

### ? Rotation Randomization Working:
- **Pitch (X rotation)**: Machete tilts forward/backward differently
- **Yaw (Y rotation)**: Machete points left/right at different angles
- **Roll (Z rotation)**: Machete rotates around its length axis

**Visual cue**: The blade should appear at different angles for each swing, as if you're using different slashing motions.

### ? Speed Randomization Working:
- Some attacks are noticeably faster
- Some attacks are noticeably slower
- The animation speed varies randomly

**Visual cue**: You'll "feel" some attacks are quicker or more sluggish.

## Common Issues and What They Mean

### Issue: Speed works, but position/rotation don't change
**Meaning**: The old code is still being used (before the fix)
**Solution**: 
- Verify the mod was recompiled
- Check mod is loaded (F1 console should show mod loaded message)
- Restart the game

### Issue: Position/rotation change once, then stay the same
**Meaning**: Transform is being set but not regenerated per attack
**Solution**: 
- Check that `OnStateEnter` is generating new random values each time
- Verify `entityRandomData[entityId]` is being updated

### Issue: Position/rotation change but "snap back" mid-swing
**Meaning**: The game is resetting the transform during the animation
**Solution**: 
- The `OnStateUpdate` patch should fix this
- Check console for continuous reapplication messages

### Issue: Weapon stays offset after attack finishes
**Meaning**: The `OnStateExit` cleanup isn't working
**Solution**: 
- Verify `OnStateExit` patch is applied
- Check that weapon transform is being restored

### Issue: No debug messages in console
**Meaning**: Patches aren't being triggered
**Solution**:
- Check Harmony is working: look for Harmony initialization messages
- Verify the weapon name contains "machete", "blade", or "knife"
- Try with different melee weapons

## Advanced Testing

### Test Different Values
Try various configuration combinations:

**Subtle (realistic)**:
```
macheteanim position 0.05 0.05 0.03
macheteanim rotation 5 3 8
macheteanim speed 0.05
```

**Moderate**:
```
macheteanim position 0.15 0.15 0.1
macheteanim rotation 20 15 25
macheteanim speed 0.15
```

**Extreme (for debugging)**:
```
macheteanim position 0.5 0.5 0.4
macheteanim rotation 90 60 90
macheteanim speed 0.5
```

### Test Edge Cases

1. **Rapid attacks**: Click as fast as possible - each should still randomize
2. **Power attack**: Hold mouse button - should NOT randomize (by design)
3. **Switch weapons**: Equip knife, then machete - should reset between weapons
4. **Die and respawn**: Randomization should continue working
5. **FPV vs TPV**: Change view mode (F5) - position/rotation randomization is disabled in FPV by design to prevent camera issues

## Performance Check

While attacking, open F3 (debug info) and watch:
- **FPS**: Should not drop significantly
- **Memory**: Should not continuously increase

The mod should have minimal performance impact (<1% FPS change).

## Expected Console Output Example

```
[Harmony] Patching method: AnimatorMeleeAttackState.OnStateEnter
[Harmony] Patching method: AnimatorMeleeAttackState.OnStateUpdate
[Harmony] Patching method: AnimatorMeleeAttackState.OnStateExit
[Harmony] Patching method: AvatarMultiBodyController.SetInRightHand
[MacheteAnimRandomizer] Weapon offset system initialized

--- After attacking ---

[MacheteAnimRandomizer] Randomized melee... attack for entity 171:
  Speed: 1.08x (from 1.00 to 1.08)
  Position Offset: (0.234, -0.156, 0.089)
  Rotation Offset: (12.4, -8.3, 19.7)
[MacheteAnimRandomizer] Applied transform to weapon:
  New Position: (0.234, -0.156, 0.089)
  New Rotation: (12.4, -8.3, 19.7)

--- After second attack ---

[MacheteAnimRandomizer] Randomized melee... attack for entity 171:
  Speed: 0.94x (from 1.00 to 0.94)
  Position Offset: (-0.178, 0.223, -0.045)
  Rotation Offset: (-15.2, 11.8, -23.4)
[MacheteAnimRandomizer] Applied transform to weapon:
  New Position: (-0.178, 0.223, -0.045)
  New Rotation: (-15.2, 11.8, -23.4)

--- After attack animation ends ---

[MacheteAnimRandomizer] Restored weapon transform for entity 171
```

## Troubleshooting Checklist

- [ ] Mod DLL is in the Mods folder
- [ ] Game was restarted after installing mod
- [ ] Using a weapon with "machete", "blade", or "knife" in the name
- [ ] Performing normal attacks (not power attacks/hold mouse)
- [ ] In third-person view (FPV is disabled by design)
- [ ] Console shows mod initialization messages
- [ ] Console shows randomization messages when attacking
- [ ] Position offset values are not zero
- [ ] Rotation offset values are not zero

## Success Criteria

The fix is working correctly if:
1. ? Each attack shows different position offset in console
2. ? Each attack shows different rotation offset in console
3. ? Weapon visually appears at different positions/angles
4. ? Speed varies (as before)
5. ? Weapon returns to normal position after attack
6. ? No errors in console
7. ? No performance degradation
