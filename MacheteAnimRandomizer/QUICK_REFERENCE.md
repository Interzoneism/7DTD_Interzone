# Quick Reference: Combat Feel Enhancements

## What's New?

This mod now includes 5 major combat feel enhancements that make melee combat more visceral and satisfying!

## Features at a Glance

| Feature | What It Does | Default |
|---------|-------------|---------|
| 🎯 **Camera Shake** | Screen shakes when you hit enemies | ON |
| ⚖️ **Weapon Momentum** | Weapons feel heavy with realistic inertia | ON |
| 🔥 **Combo System** | Get faster as you land consecutive hits | ON |
| 💥 **Hit Feedback** | Headshots and crits feel extra satisfying | ON |
| ⏱️ **Windup Variation** | Unpredictable attack timing | ON |

## Quick Start

All features are enabled by default with balanced settings. Just install and play!

## Console Commands Cheat Sheet

### View Current Settings
```
macheteanim show
```

### Toggle Features On/Off
```
macheteanim camerashake true/false
macheteanim momentum true/false
macheteanim combotiming true/false
macheteanim hitfeedback true/false
macheteanim windup true/false
```

### Adjust Intensity
```
macheteanim shakeintensity 0.08    # Camera shake strength (0-1)
macheteanim momentumforce 0.15     # Weapon weight feel (0-0.5)
macheteanim combospeed 0.06        # Speed boost per combo hit (0-0.2)
macheteanim critmultiplier 1.8     # Extra shake for crits (1-3)
```

### Advanced Tuning
```
macheteanim maxcombo 4             # Max combo count (2-10)
macheteanim windupvariation 0.08   # Attack timing variety (0-0.2)
macheteanim frames 25              # Animation smoothness (10-50)
```

### Reset Everything
```
macheteanim reset
```

## Presets

### Want Subtle Effects?
```
macheteanim shakeintensity 0.05
macheteanim combotiming false
```

### Want Maximum Impact?
```
macheteanim shakeintensity 0.12
macheteanim combospeed 0.08
macheteanim maxcombo 6
```

### Want Arcade Style?
```
macheteanim combotiming true
macheteanim combospeed 0.1
macheteanim maxcombo 6
macheteanim shakeintensity 0.15
```

### Disable All Enhancements (Classic Mode)
```
macheteanim camerashake false
macheteanim momentum false
macheteanim combotiming false
macheteanim hitfeedback false
macheteanim windup false
```

## What You'll Notice In-Game

### Camera Shake
- Normal hits → Small screen wobble
- Headshots → Bigger wobble
- Critical hits → Even bigger wobble
- Killing blows → Maximum wobble

### Combo System
Try attacking rapidly:
- 1st hit → Normal speed
- 2nd hit → 6% faster
- 3rd hit → 12% faster
- 4th hit → 18% faster

Stop attacking for 1.5 seconds and combo resets.

### Weapon Momentum
After each swing, your weapon will:
1. Continue moving in the swing direction
2. Gradually slow down
3. Settle back to center

Makes weapons feel like they have actual weight!

### Hit Feedback
Different hit types feel different:
- Body hit → Base feedback
- Headshot → Extra shake (1.8x)
- Critical → Extra shake (1.8x)
- Killing blow → Maximum shake (1.8x × 1.2x)

### Windup Variation
Each attack starts slightly differently:
- Some attacks wind up a bit faster
- Some wind up a bit slower
- Prevents robotic feeling

## Troubleshooting

**Q: Too much screen shake?**
```
macheteanim shakeintensity 0.04
```

**Q: Combo makes me dizzy?**
```
macheteanim combotiming false
```

**Q: Weapons feel too floaty?**
```
macheteanim momentumforce 0.2
```

**Q: Weapons feel too heavy?**
```
macheteanim momentumforce 0.1
```

**Q: Want vanilla experience?**
```
macheteanim reset
macheteanim camerashake false
macheteanim momentum false
macheteanim combotiming false
macheteanim hitfeedback false
macheteanim windup false
```

## Debug Mode

Enable detailed logging:
```
macheteanim debug true
```

Check game console (F1) to see:
- When combos trigger
- Shake intensities applied
- Momentum forces
- Speed multipliers

Disable when done:
```
macheteanim debug false
```

## Technical Notes

- Settings are per-session (reset when you restart the game)
- Changes take effect immediately
- No game restart required
- Works with all melee weapons by default
- Can be limited to machete-type weapons: `macheteanim allmelee false`

## More Information

- **Design Details:** See `COMBAT_FEEL_ENHANCEMENTS.md`
- **Technical Implementation:** See `IMPLEMENTATION_SUMMARY.md`
- **Basic Usage:** See `CONSOLE_COMMANDS.md`
- **Architecture:** See `SPRING_BASED_SOLUTION.md`

## Feedback & Tuning

Start with defaults and adjust to taste. Everyone has different preferences!

Common adjustments:
- **Too intense?** → Lower shake and combo values
- **Too subtle?** → Increase shake and combo values
- **Want pure randomization?** → Disable new features, keep base randomization
- **Want maximum feel?** → Crank everything up!

Experiment and find your sweet spot! 🎮
