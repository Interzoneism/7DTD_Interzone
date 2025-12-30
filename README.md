# 7 Days to Die - Interzone Modding Repository

This repository contains decompiled code from 7 Days to Die and comprehensive documentation for modding, with a focus on animation systems.

## Contents

### 📚 Documentation
- **[MELEE_ANIMATION_MODDING_GUIDE.md](MELEE_ANIMATION_MODDING_GUIDE.md)** - Complete guide to modifying melee weapon animations
- **[Examples/](Examples/)** - Practical code and XML examples

### 🔍 Decompiled Code
- **7DTD_Decompiled_AssemblyCSharp/** - Decompiled game code (Assembly-CSharp.dll)
- **7DTD_Decompiled_UnityEngine.AnimationModule/** - Unity animation module

### 🛠️ Mod Template
- **ModBase/** - Template for creating API mods (dedicated servers)

## Quick Start

### For Animation Modding

#### XML-Based (No Coding Required)
1. Read the [Examples/README.md](Examples/README.md)
2. Copy examples from `Examples/XML/MeleeAnimationExamples.xml`
3. Create your mod and test!

#### Code-Based (Advanced)
1. Read the [MELEE_ANIMATION_MODDING_GUIDE.md](MELEE_ANIMATION_MODDING_GUIDE.md)
2. Review examples in `Examples/HarmonyPatches/`
3. Set up Harmony and create your patches

### For General Modding

Simply add the required game dll to the 7dtd-binaries folder (Assembly-CSharp.dll by default), then load the solution into Visual Studio

Alternatively add your references directly from your dedicated server '7DaysToDieServer_Data/Managed' folder

## Animation Modding Highlights

The animation modding guide covers:

- ✅ Attack speed modification
- ✅ Hit detection timing
- ✅ Weapon positioning and rotation
- ✅ Grazing hit system
- ✅ Custom animation states
- ✅ Combo systems
- ✅ Visual effects and trails
- ✅ And much more!

### Key Features You Can Modify

**Via XML:**
- Attack speed (AttacksPerMinute)
- Weapon range and hit detection
- Grazing hit configuration
- Swing patterns (horizontal/vertical)
- Multi-target penetration

**Via Harmony (C#):**
- Dynamic speed modifications
- Custom impact effects
- Animation event handlers
- Weapon trail effects
- Complex combo systems
- Conditional behavior based on game state

## Examples

### Increase Attack Speed (XML)
```xml
<passive_effect name="AttacksPerMinute" operation="base_set" value="90"/>
```

### Custom Melee Weapon (XML)
```xml
<item name="meleeCustomKatana">
    <property name="HoldType" value="6"/>
    <property class="Action0">
        <property name="UseGrazingHits" value="true"/>
        <property name="EntityPenetrationCount" value="3"/>
    </property>
    <effect_group>
        <passive_effect name="AttacksPerMinute" operation="base_set" value="120"/>
    </effect_group>
</item>
```

### Speed Boost Patch (Harmony)
```csharp
[HarmonyPatch(typeof(AnimatorMeleeAttackState), "OnStateEnter")]
public static void Postfix(Animator animator) {
    animator.SetFloat("MeleeAttackSpeed", 2f);
}
```

## Repository Structure

```
7DTD_Interzone/
├── MELEE_ANIMATION_MODDING_GUIDE.md     # Complete technical guide
├── Examples/
│   ├── README.md                         # Examples documentation
│   ├── XML/
│   │   └── MeleeAnimationExamples.xml   # XML modding examples
│   └── HarmonyPatches/
│       └── MeleeAnimationPatches.cs     # C# Harmony patches
├── 7DTD_Decompiled_AssemblyCSharp/      # Decompiled game code
├── 7DTD_Decompiled_UnityEngine.AnimationModule/
├── ModBase/                              # Mod template
└── 7dtd-binaries/                        # Game DLL references
```

## Key Animation Files

Reference these decompiled files for understanding the system:

- `AnimatorMeleeAttackState.cs` - Melee attack state machine
- `ItemActionDynamicMelee.cs` - Modern melee action system
- `ItemActionMelee.cs` - Legacy melee system
- `AvatarController.cs` - Entity animation controller
- `AnimationDelayData.cs` - Timing configurations
- `PassiveEffects.cs` - Game effect definitions

## Contributing

This is a research and documentation repository. Contributions welcome:
- Additional examples
- Documentation improvements
- Bug reports in examples
- New modding techniques

## Resources

- [7 Days to Die Official Forums](https://community.7daystodie.com/)
- [Harmony Patching Documentation](https://harmony.pardeike.net/)
- [Unity Animation Documentation](https://docs.unity3d.com/Manual/AnimationSection.html)

## License

7 Days to Die is property of The Fun Pimps.

This repository contains educational decompiled code for modding purposes. Examples and documentation are provided as-is for the modding community.

## Disclaimer

This repository is for educational and modding purposes only. Always respect the game's EULA and terms of service. Use mods at your own risk and always backup your saves.

---

**Last Updated:** December 2024  
**Game Version:** 7 Days to Die Alpha 21+
