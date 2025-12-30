# 7 Days to Die - Animation Modding Examples

This directory contains practical examples for modding melee weapon animations in 7 Days to Die.

## Directory Structure

```
Examples/
├── XML/
│   └── MeleeAnimationExamples.xml    # XML-based animation modifications (no coding required)
└── HarmonyPatches/
    └── MeleeAnimationPatches.cs      # C# Harmony patches for advanced modifications
```

## Quick Start

### For XML Modding (Easy - No Coding Required)

1. Review `XML/MeleeAnimationExamples.xml` for examples
2. Create your mod structure:
   ```
   Mods/
   └── YourModName/
       ├── ModInfo.xml
       └── Config/
           └── items.xml
   ```
3. Copy relevant examples to your `items.xml`
4. Launch the game and test!

**Example ModInfo.xml:**
```xml
<?xml version="1.0" encoding="UTF-8"?>
<xml>
  <ModInfo>
    <Name value="YourModName"/>
    <Description value="Custom melee animations"/>
    <Author value="YourName"/>
    <Version value="1.0"/>
  </ModInfo>
</xml>
```

### For Harmony Modding (Advanced - C# Required)

1. Install prerequisites:
   - TFP_Harmony mod (provides 0Harmony.dll)
   - Visual Studio or similar C# IDE
   - .NET Framework 4.8

2. Create a mod project:
   - Reference `Assembly-CSharp.dll` from game files
   - Reference `0Harmony.dll` from TFP_Harmony mod
   - Reference Unity DLLs as needed

3. Use examples from `HarmonyPatches/MeleeAnimationPatches.cs`

4. Build your DLL and place in:
   ```
   Mods/
   └── YourModName/
       ├── ModInfo.xml
       └── YourModName.dll
   ```

## XML Examples Overview

The XML examples demonstrate:
- ✅ Attack speed modifications
- ✅ Weapon reach adjustments
- ✅ Grazing hit system configuration
- ✅ Hit detection timing
- ✅ Multi-target penetration
- ✅ Custom swing patterns
- ✅ Complete custom weapon creation

## Harmony Patch Examples Overview

The C# examples demonstrate:
- ✅ Dynamic attack speed modifications
- ✅ Conditional speed boosts based on weapon tags
- ✅ Hit detection timing modifications
- ✅ Weapon position and rotation offsets
- ✅ Custom impact effects
- ✅ Combo system implementation
- ✅ Stamina-based speed scaling
- ✅ Multiple concurrent patches

## Common Modifications

### Speed Up Attacks (XML)
```xml
<passive_effect name="AttacksPerMinute" operation="base_set" value="90"/>
```

### Speed Up Attacks (Harmony)
```csharp
[HarmonyPatch(typeof(AnimatorMeleeAttackState), "OnStateEnter")]
static void Postfix(Animator animator) {
    animator.SetFloat("MeleeAttackSpeed", 2f);
}
```

### Increase Range (XML)
```xml
<property name="Range" value="4.0"/>
<property name="BlockRange" value="5.0"/>
```

### Enable Grazing Hits (XML)
```xml
<property name="UseGrazingHits" value="true"/>
<property name="GrazeStart" value="-0.2"/>
<property name="GrazeEnd" value="0.2"/>
```

## Testing Your Mods

### In-Game Console Commands
Press F1 to open console:

- `giveself itemname` - Spawn items for testing
- `buff playername buffname` - Apply buffs
- `showswings` - Visualize attack hitboxes
- `debugshot` - Screenshot with debug info

### Debug Logging
Add to your C# code:
```csharp
Log.Out($"[YourMod] Debug message here");
```

Check logs at: `%AppData%\7DaysToDie\output_log.txt`

## Key Animation Properties

### ItemAction Properties
| Property | Description | Default |
|----------|-------------|---------|
| Range | Entity hit range | 2.0 |
| BlockRange | Block hit range | 3.0 |
| Sphere | Hit detection sphere radius | 0.4 |
| UseGrazingHits | Enable grazing system | false |
| GrazeStart | Graze window start (normalized) | -0.15 |
| GrazeEnd | Graze window end (normalized) | 0.15 |
| RaycastTime | Hit timing (seconds) | 0.3 |
| ImpactDuration | Impact pause (seconds) | 0.01 |
| EntityPenetrationCount | Entities hit per swing | 0 |
| SwingDegrees | Swing arc (degrees) | 65 |

### Passive Effects
| Effect | Description |
|--------|-------------|
| AttacksPerMinute | Attack speed (60 = 1/sec) |
| EntityDamage | Damage to entities |
| BlockDamage | Damage to blocks |
| StaminaLoss | Stamina per attack |
| MaxRange | Maximum attack range |
| SphereCastRadius | Hit detection sphere size |

## Troubleshooting

### Problem: Changes Not Applied
**Solution:**
- Check XML syntax (must be valid XML)
- Verify XPath is correct
- Check game logs for parsing errors
- Ensure mod is enabled in-game

### Problem: Game Crashes
**Solution:**
- Check for null references in C# code
- Use try-catch blocks in Harmony patches
- Test with minimal changes first
- Check mod load order (TFP_Harmony must load first)

### Problem: Animation Feels Wrong
**Solution:**
- Adjust `RaycastTime` to match visual swing
- Check `ImpactDuration` isn't too long
- Verify `AttacksPerMinute` is reasonable (30-150)
- Test in both FPV and TPV

## Best Practices

### XML Modding
- ✅ Use `append` for adding new properties
- ✅ Use `set` for changing existing values
- ✅ Test changes one at a time
- ✅ Comment your XML for clarity
- ❌ Don't remove vanilla content unless necessary

### Harmony Modding
- ✅ Use `Postfix` patches when possible (safer)
- ✅ Check for null references
- ✅ Use try-catch to prevent crashes
- ✅ Log important events for debugging
- ❌ Don't modify fields unnecessarily
- ❌ Avoid expensive operations in frequently-called methods

## Performance Considerations

- Avoid expensive calculations in `OnHoldingUpdate` (called every frame)
- Cache reflection results instead of looking up fields repeatedly
- Use static fields for frequently accessed data
- Profile your code if you suspect performance issues

## Compatibility

### With Other Mods
- XML mods: Usually compatible unless modifying same items
- Harmony mods: Can conflict if patching same methods
- Load order matters: TFP_Harmony must load first

### Game Versions
- Examples are for Alpha 21+
- May work on earlier versions with modifications
- Check game version when reporting issues

## Additional Resources

See the main documentation:
- `/MELEE_ANIMATION_MODDING_GUIDE.md` - Complete technical guide
- Decompiled code in `/7DTD_Decompiled_AssemblyCSharp/`
- Unity Animation documentation
- Harmony documentation: https://harmony.pardeike.net/

## Contributing

Found a useful pattern? Create an example and submit a PR!

## License

These examples are provided as-is for educational purposes.
7 Days to Die is property of The Fun Pimps.

## Support

For questions:
- 7DTD Official Forums
- 7DTD Modding Discord
- GitHub Issues (for this repository)

---

**Last Updated:** December 2024  
**Game Version:** 7 Days to Die Alpha 21+
