# 7 Days to Die - Melee Animation Modding Guide

## Overview
This guide documents the animation system for melee weapons in 7 Days to Die (7DTD), focusing on modding opportunities using Harmony patches and code modifications. The game uses Unity's animation system with custom state machines and action handlers.

## Table of Contents
1. [Core Animation Architecture](#core-animation-architecture)
2. [Key Classes and Components](#key-classes-and-components)
3. [Animation Speed and Timing](#animation-speed-and-timing)
4. [Modding Approaches](#modding-approaches)
5. [Practical Examples](#practical-examples)
6. [Advanced Topics](#advanced-topics)

---

## Core Animation Architecture

### Animation Pipeline
The melee attack animation system follows this flow:
```
Player Input → ItemActionDynamicMelee/ItemActionMelee → AvatarController → AnimatorMeleeAttackState → Animation Playback
```

### Key Components
1. **ItemActionDynamicMelee** - Modern melee action system with grazing hits
2. **ItemActionMelee** - Legacy melee action system
3. **AnimatorMeleeAttackState** - Unity StateMachineBehaviour that controls attack timing
4. **AvatarController** - Base controller for entity animations
5. **AnimationDelayData** - Static timing configuration for different weapon types

---

## Key Classes and Components

### 1. AnimatorMeleeAttackState (AnimatorMeleeAttackState.cs)
Controls the melee attack animation state machine.

**Key Properties:**
- `RaycastTime` (default: 0.3f) - When the hit detection occurs during animation
- `ImpactDuration` (default: 0.01f) - Duration of impact slowdown
- `ImpactPlaybackSpeed` (default: 1f) - Animation speed during impact

**Key Methods:**
- `OnStateEnter()` - Called when entering attack animation state
- `impactStart()` - Coroutine that triggers hit detection
- `impactStop()` - Coroutine that manages impact animation slowdown
- `OnStateExit()` - Cleanup when exiting attack state

**Modifiable via Item Properties:**
```xml
<property name="Action0.RaycastTime" value="0.3"/>
<property name="Action0.ImpactDuration" value="0.01"/>
<property name="Action0.ImpactPlaybackSpeed" value="1.0"/>
```

### 2. ItemActionDynamicMelee (ItemActionDynamicMelee.cs)
Modern melee system with advanced features.

**Key Features:**
- **Grazing Hits**: Hit detection during swing animation (not just at impact point)
- **Entity Penetration**: Hit multiple entities with one swing
- **Swing Customization**: Horizontal/vertical swing patterns
- **Harvesting Animation**: Separate animation for resource gathering

**Configuration Properties:**
```xml
<!-- Range and Damage -->
<property name="Range" value="2.0"/>
<property name="BlockRange" value="3.0"/>
<property name="Sphere" value="0.4"/>  <!-- Sphere cast radius -->
<property name="EntityPenetrationCount" value="0"/>

<!-- Grazing System -->
<property name="UseGrazingHits" value="true"/>
<property name="GrazeStart" value="-0.15"/>  <!-- Animation percentage -->
<property name="GrazeEnd" value="0.15"/>
<property name="GrazeDamagePercentage" value="0.1"/>
<property name="GrazeStaminaPercentage" value="0.01"/>

<!-- Swing Configuration -->
<property name="IsVerticalSwing" value="false"/>
<property name="IsHorizontalSwing" value="false"/>
<property name="InvertSwing" value="false"/>
<property name="SwingDegrees" value="65"/>
<property name="SwingAngle" value="0"/>

<!-- Animation Control -->
<property name="UsePowerAttackAnimation" value="false"/>
<property name="UsePowerAttackTriggers" value="false"/>
<property name="HarvestLength" value="1.0"/>
```

### 3. AvatarController (AvatarController.cs)
Manages all entity animations.

**Key Animation Hashes:**
- `meleeAttackSpeedHash` - Controls melee attack speed
- `attackHash` - Attack animation trigger
- `harvestingHash` - Harvesting state boolean
- `weaponHoldTypeHash` - Weapon hold type (affects animation set)

**Important Methods:**
- `UpdateFloat(hash, value, sync)` - Update float animator parameters
- `UpdateBool(hash, value, sync)` - Update boolean animator parameters
- `TriggerEvent(eventName)` - Trigger animation events

### 4. AnimationDelayData (AnimationDelayData.cs)
Static timing data indexed by hold type.

**Structure:**
```csharp
public struct AnimationDelays
{
    public float RayCast;        // Hit detection timing when standing
    public float RayCastMoving;  // Hit detection timing when moving
    public float Holster;        // Time to holster weapon
    public float Unholster;      // Time to unholster weapon
    public bool TwoHanded;       // Whether weapon is two-handed
}
```

**Accessed via:**
```csharp
AnimationDelayData.AnimationDelay[holdType]
```

### 5. AnimationGunjointOffsetData (AnimationGunjointOffsetData.cs)
Weapon positioning and rotation offsets during animations.

**Structure:**
```csharp
public struct AnimationGunjointOffsets
{
    public Vector3 position;  // Position offset
    public Vector3 rotation;  // Rotation offset (Euler angles)
}
```

---

## Animation Speed and Timing

### Speed Calculation
Attack speed is calculated using the `AttacksPerMinute` passive effect:

```csharp
// Base attacks per minute (default: 60)
float attacksPerMinute = EffectManager.GetValue(
    PassiveEffects.AttacksPerMinute, 
    itemValue, 
    60f, 
    entity, 
    null, 
    itemTags, 
    true, true, true, true, true, 1, true, false
);

// Convert to animation speed multiplier
float animationLength = clip.length;
float attackTime = 60f / attacksPerMinute;
float animationSpeed = attackTime / animationLength;

// Apply to animator
animator.SetFloat("MeleeAttackSpeed", animationSpeed);
```

### Timing Points in Animation
1. **Attack Start** (0%) - Button pressed, animation begins
2. **Raycast Time** (~30-40%) - Hit detection occurs
3. **Impact** (if hit) - Animation slows/pauses briefly
4. **Attack End** (100%) - Animation completes

### Passive Effects for Speed
```xml
<passive_effect name="AttacksPerMinute" operation="base_set" value="60"/>
<passive_effect name="AttacksPerMinute" operation="perc_add" value="0.15"/>  <!-- 15% faster -->
```

---

## Modding Approaches

### Approach 1: XML Modding (No Code)
Modify weapon properties in XML to change animation behavior.

**Example: Faster Attack Speed**
```xml
<append xpath="/items/item[@name='meleeToolPickaxeIron']">
  <effect_group>
    <passive_effect name="AttacksPerMinute" operation="base_set" value="120"/>
  </effect_group>
</append>
```

**Example: Longer Reach with Grazing**
```xml
<append xpath="/items/item[@name='meleeToolSpear']">
  <property name="Range" value="4.0"/>
  <property name="UseGrazingHits" value="true"/>
  <property name="GrazeStart" value="-0.2"/>
  <property name="GrazeEnd" value="0.2"/>
</append>
```

### Approach 2: Harmony Patches (C# Code Mod)
Use Harmony to patch game methods at runtime.

**Setup:**
1. Add `0Harmony.dll` reference (usually via TFP_Harmony mod)
2. Create a mod class that initializes Harmony patches
3. Use `[HarmonyPatch]` attributes to target methods

**Example Mod Structure:**
```csharp
using HarmonyLib;
using System.Reflection;

namespace MyMeleeAnimMod
{
    public class ModAPI : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            Log.Out("Loading MyMeleeAnimMod");
            
            var harmony = new Harmony("com.yourname.meleeanimmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
```

### Approach 3: Custom Item Actions
Create new ItemAction classes that extend the base classes.

```csharp
public class ItemActionCustomMelee : ItemActionDynamicMelee
{
    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        // Custom logic before attack
        
        base.ExecuteAction(_actionData, _bReleased);
        
        // Custom logic after attack
    }
}
```

---

## Practical Examples

### Example 1: Double Attack Speed (Harmony Patch)

```csharp
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(AnimatorMeleeAttackState))]
[HarmonyPatch("OnStateEnter")]
public class Patch_DoubleAttackSpeed
{
    static void Postfix(
        AnimatorMeleeAttackState __instance,
        Animator animator,
        ref float ___originalMeleeAttackSpeed
    )
    {
        // Double the animation speed
        float doubledSpeed = ___originalMeleeAttackSpeed * 2f;
        animator.SetFloat("MeleeAttackSpeed", doubledSpeed);
        
        Log.Out($"Modified melee attack speed from {___originalMeleeAttackSpeed} to {doubledSpeed}");
    }
}
```

### Example 2: Custom Weapon Angle Offset (Harmony Patch)

```csharp
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(FirstPersonAnimator))]
[HarmonyPatch(MethodType.Constructor)]
[HarmonyPatch(new Type[] { 
    typeof(EntityAlive), 
    typeof(AvatarCharacterController.AnimationStates), 
    typeof(Transform), 
    typeof(BodyAnimator.EnumState) 
})]
public class Patch_WeaponOffset
{
    static void Postfix()
    {
        // Modify weapon rotation offsets for specific hold types
        // HoldType values are indexed (e.g., 0 = handgun, 1 = rifle, etc.)
        int meleeHoldType = 10; // Example hold type for melee
        
        AnimationGunjointOffsetData.AnimationGunjointOffset[meleeHoldType].rotation = 
            new Vector3(15f, 0f, 0f); // 15 degree pitch offset
            
        AnimationGunjointOffsetData.AnimationGunjointOffset[meleeHoldType].position = 
            new Vector3(0f, -0.1f, 0f); // Lower weapon slightly
    }
}
```

### Example 3: Custom Raycast Timing (Harmony Patch)

```csharp
using HarmonyLib;

[HarmonyPatch(typeof(ItemActionMelee))]
[HarmonyPatch("ExecuteAction")]
public class Patch_CustomRaycastTiming
{
    static void Prefix(
        ItemActionMelee __instance,
        ItemActionData _actionData
    )
    {
        // Make attacks hit earlier in the animation
        var inventoryData = _actionData as ItemActionMelee.InventoryDataMelee;
        if (inventoryData != null)
        {
            // Access the private rayCastDelay field using reflection
            var rayCastDelayField = typeof(ItemActionMelee)
                .GetField("rayCastDelay", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
            
            if (rayCastDelayField != null)
            {
                float currentDelay = (float)rayCastDelayField.GetValue(__instance);
                rayCastDelayField.SetValue(__instance, currentDelay * 0.5f); // Hit 50% earlier
            }
        }
    }
}
```

### Example 4: Replace Animation with Custom One (Advanced)

```csharp
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(AvatarController))]
[HarmonyPatch("TriggerEvent")]
public class Patch_CustomAnimation
{
    static bool Prefix(
        AvatarController __instance,
        string _event
    )
    {
        if (_event == "AttackTrigger")
        {
            // Get the animator
            var animator = __instance.GetComponent<Animator>();
            
            if (animator != null && __instance.entity.inventory.holdingItem.Id == 123) // Specific item
            {
                // Trigger custom animation instead
                animator.SetTrigger("CustomMeleeAttack");
                return false; // Skip original method
            }
        }
        
        return true; // Continue with original method
    }
}
```

### Example 5: Impact Slowdown Modifier

```csharp
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(AnimatorMeleeAttackState))]
[HarmonyPatch("impactStop")]
public class Patch_ImpactSlowdown
{
    static IEnumerator Postfix(
        IEnumerator result,
        AnimatorMeleeAttackState __instance,
        Animator animator,
        AnimatorStateInfo stateInfo,
        AnimationClip clip,
        int layerIndex
    )
    {
        // Get the calculated values
        float calculatedRaycastTime = (float)AccessTools.Field(
            typeof(AnimatorMeleeAttackState), 
            "calculatedRaycastTime"
        ).GetValue(__instance);
        
        float originalSpeed = (float)AccessTools.Field(
            typeof(AnimatorMeleeAttackState), 
            "originalMeleeAttackSpeed"
        ).GetValue(__instance);
        
        // Apply custom slowdown - make it more dramatic
        animator.Play(0, layerIndex, 
            Mathf.Min(1f, calculatedRaycastTime * originalSpeed / clip.length));
        
        yield return new WaitForSeconds(0.1f); // Longer slowdown
        
        // Speed back up with acceleration effect
        float targetSpeed = originalSpeed * 1.2f; // Slightly faster after impact
        animator.SetFloat("MeleeAttackSpeed", targetSpeed);
        
        yield break;
    }
}
```

---

## Advanced Topics

### Working with PassiveEffects

PassiveEffects control most gameplay values, including animation-related parameters:

**Relevant PassiveEffects for Animations:**
- `AttacksPerMinute` - Attack speed
- `EntityDamage` - Melee damage to entities
- `BlockDamage` - Melee damage to blocks
- `MaxRange` / `BlockRange` - Weapon reach
- `SphereCastRadius` - Hit detection sphere size
- `GrazeDamageMultiplier` - Grazing hit damage multiplier
- `GrazeStaminaMultiplier` - Stamina cost for grazing hits
- `StaminaLoss` - Stamina cost per attack

**Modifying PassiveEffects with Harmony:**
```csharp
[HarmonyPatch(typeof(EffectManager))]
[HarmonyPatch("GetValue")]
public class Patch_PassiveEffects
{
    static void Postfix(
        ref float __result,
        PassiveEffects _effect,
        ItemValue _itemValue
    )
    {
        if (_effect == PassiveEffects.AttacksPerMinute && 
            _itemValue.ItemClass.HasTag(FastTags.Parse("customWeapon")))
        {
            __result *= 2f; // Double attack speed for custom weapons
        }
    }
}
```

### Custom Animation States

You can create custom animation state behaviors:

```csharp
using UnityEngine;

public class CustomMeleeState : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator, 
        AnimatorStateInfo stateInfo, 
        int layerIndex
    )
    {
        // Custom logic when entering custom melee state
        var entity = animator.GetComponent<EntityAlive>();
        if (entity != null)
        {
            // Apply custom effects
            entity.PlayOneShot("custom_attack_sound");
            
            // Spawn particles
            ParticleEffect.CreateParticleEffect(
                "electric_spark", 
                entity.GetPosition(), 
                Vector3.zero
            );
        }
    }
}
```

### Animation Events

Unity animations can have events that trigger at specific frames:

```csharp
[HarmonyPatch(typeof(AnimationEventBridge))]
[HarmonyPatch("OnAnimationEventStart")]
public class Patch_AnimationEvents
{
    static void Postfix(
        AnimationEventBridge __instance,
        string _eventName
    )
    {
        if (_eventName == "MeleeImpact")
        {
            // Custom logic when animation event fires
            EntityAlive entity = __instance.entity;
            
            // Screen shake
            GameManager.Instance.StartCoroutine(
                CameraShake(0.2f, 0.1f)
            );
        }
    }
    
    static IEnumerator CameraShake(float duration, float magnitude)
    {
        // Camera shake implementation
        yield break;
    }
}
```

### Hold Type System

Each weapon has a hold type that determines which animation set is used:

**Common Hold Types:**
- 0 = Handgun
- 1 = Rifle  
- 2 = Rocket Launcher
- 3 = Bow
- 4 = Spear (one-handed melee)
- 5 = Club (two-handed melee)
- 6 = Knife (knife melee)
- And more...

**Changing Hold Type:**
```xml
<property name="HoldType" value="4"/>
```

Or with Harmony:
```csharp
[HarmonyPatch(typeof(ItemClass))]
[HarmonyPatch("get_HoldType")]
public class Patch_HoldType
{
    static void Postfix(ItemClass __instance, ref ItemClass.EnumHoldType __result)
    {
        if (__instance.HasTag(FastTags.Parse("myCustomWeapon")))
        {
            __result = new ItemClass.EnumHoldType(10); // Custom hold type
        }
    }
}
```

### Weapon Trail Effects

For custom weapon trails during swing:

```csharp
[HarmonyPatch(typeof(ItemActionDynamicMelee))]
[HarmonyPatch("ExecuteAction")]
public class Patch_WeaponTrail
{
    static void Postfix(
        ItemActionDynamicMelee __instance,
        ItemActionData _actionData,
        bool _bReleased
    )
    {
        if (!_bReleased)
        {
            var entity = _actionData.invData.holdingEntity;
            var weaponTransform = entity.emodel?.GetWeaponTransform();
            
            if (weaponTransform != null)
            {
                // Attach trail effect
                var trail = weaponTransform.gameObject.AddComponent<TrailRenderer>();
                trail.time = 0.3f;
                trail.startWidth = 0.1f;
                trail.endWidth = 0.01f;
                trail.material = new Material(Shader.Find("Particles/Additive"));
                
                // Clean up after animation
                UnityEngine.Object.Destroy(trail, 0.5f);
            }
        }
    }
}
```

### Root Motion Integration

For weapons that should move the player during attacks:

```csharp
[HarmonyPatch(typeof(AvatarRootMotion))]
[HarmonyPatch("OnAnimatorMove")]
public class Patch_RootMotion
{
    static void Postfix(AvatarRootMotion __instance)
    {
        if (__instance.entity.inventory.holdingItem?.HasTag(
            FastTags.Parse("dashAttack")) == true)
        {
            // Apply extra forward movement during attack
            var deltaPosition = __instance.transform.forward * 0.5f * Time.deltaTime;
            __instance.entity.SetPosition(
                __instance.entity.position + deltaPosition
            );
        }
    }
}
```

---

## Best Practices

### 1. Testing Your Mods
- Test with different weapon types (knife, club, spear, etc.)
- Test at different attack speeds (buffs, debuffs)
- Test in both first-person and third-person views
- Test with moving and stationary targets

### 2. Performance Considerations
- Avoid expensive calculations in `OnHoldingUpdate()` (called every frame)
- Cache references instead of using `GetComponent()` repeatedly
- Use object pooling for particle effects
- Profile your code with Unity Profiler if available

### 3. Compatibility
- Check for null references (entity might die mid-animation)
- Handle edge cases (item broken, animation interrupted)
- Use try-catch in Harmony patches to prevent crashes
- Test with other popular mods

### 4. Documentation
- Document your changes clearly
- Provide configuration options when possible
- Include examples and defaults
- Note any incompatibilities

### 5. Debugging Tips
- Use `Log.Out()` for debugging messages
- Enable console command `ShowSwings` to visualize hit detection
- Use `ShowHits` console command to see raycast results
- Monitor `AnimatorMeleeAttackState` variables in watch window

---

## Common Issues and Solutions

### Issue 1: Animation Speed Not Changing
**Problem:** Setting attack speed has no effect.
**Solution:** Make sure you're modifying the right value:
- Check if using `ItemActionDynamicMelee` or `ItemActionMelee`
- Ensure `AttacksPerMinute` passive effect is being applied
- Verify animator has the parameter `MeleeAttackSpeed`

### Issue 2: Hits Not Registering
**Problem:** Attacks play animation but don't damage.
**Solution:** 
- Check raycast timing (`RaycastTime` property)
- Verify weapon range is sufficient
- Check if attack is being canceled early
- Ensure `IsAttackValid()` returns true

### Issue 3: Grazing Hits Not Working
**Problem:** `UseGrazingHits` enabled but no grazing.
**Solution:**
- Only works with `ItemActionDynamicMelee`
- Check `GrazeStart` and `GrazeEnd` values (normalized time)
- Verify entity has sufficient stamina
- Ensure `GrazeStaminaPercentage` is configured

### Issue 4: Weapon Position Offset
**Problem:** Weapon appears in wrong position.
**Solution:**
- Check `AnimationGunjointOffsetData` for hold type
- Verify `HoldType` value is correct
- Check both position and rotation offsets
- Test in both FPV and TPV

---

## Resources and References

### Decompiled Code Files
- `/7DTD_Decompiled_AssemblyCSharp/AnimatorMeleeAttackState.cs`
- `/7DTD_Decompiled_AssemblyCSharp/ItemActionDynamicMelee.cs`
- `/7DTD_Decompiled_AssemblyCSharp/ItemActionMelee.cs`
- `/7DTD_Decompiled_AssemblyCSharp/AvatarController.cs`
- `/7DTD_Decompiled_AssemblyCSharp/AnimationDelayData.cs`
- `/7DTD_Decompiled_AssemblyCSharp/AnimationGunjointOffsetData.cs`
- `/7DTD_Decompiled_AssemblyCSharp/PassiveEffects.cs`
- `/7DTD_Decompiled_AssemblyCSharp/EffectManager.cs`

### Unity Animation References
- Unity Animator Controller
- Unity StateMachineBehaviour
- Unity Animation Events
- Unity Root Motion

### Harmony Documentation
- Harmony Wiki: https://harmony.pardeike.net/
- Harmony Patching Guide
- Accessing Private Fields/Methods

---

## Conclusion

The 7DTD melee animation system is highly customizable through both XML configuration and C# code mods using Harmony. Key areas for modification include:

1. **Attack Speed** - Via `AttacksPerMinute` passive effect
2. **Hit Timing** - Via `RaycastTime` property and raycast delays
3. **Animation Behavior** - Via state machine patches and custom states
4. **Weapon Positioning** - Via `AnimationGunjointOffsetData`
5. **Special Effects** - Via animation events and custom triggers
6. **Grazing System** - Via `ItemActionDynamicMelee` configuration

Whether you're creating faster weapons, adding special attack patterns, or implementing entirely new combat mechanics, the animation system provides extensive hooks for modification.

For additional help:
- Examine the decompiled code in this repository
- Join the 7DTD modding community
- Test thoroughly with debug visualization enabled
- Share your creations with the community!

---

**Last Updated:** December 2024  
**Game Version:** 7 Days to Die (Alpha 21+)  
**Author:** Generated from decompiled code analysis
