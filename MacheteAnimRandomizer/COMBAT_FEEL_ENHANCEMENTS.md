# Combat Feel Enhancements for Melee Animations

## Overview

This document describes the enhanced melee combat feel features that go beyond basic randomization to create a more visceral, satisfying, and dynamic first-person melee experience.

## Enhancement Categories

### 1. **Camera Shake on Impact** 🎯
**Purpose:** Provides visceral, physical feedback when hits connect.

**How it works:**
- Triggers when melee attacks successfully hit enemies
- Applies a brief rotational impulse to the camera via the weapon spring system
- Intensity varies based on hit type (normal, headshot, critical, killing blow)
- Creates a "punch" feeling that reinforces the impact

**Configuration:**
- `EnableCameraShake` - Enable/disable the feature (default: `true`)
- `CameraShakeIntensity` - Base shake intensity (default: `0.08`)
- `CriticalHitShakeMultiplier` - Multiplier for critical hits (default: `1.8x`)

**Why it enhances feel:**
From a first-person perspective, camera shake is one of the most immediate forms of feedback. It makes hits feel weighty and impactful, turning each successful strike into a satisfying moment.

---

### 2. **Weapon Momentum/Inertia System** ⚖️
**Purpose:** Makes weapons feel like they have physical weight and mass.

**How it works:**
- After each swing, weapon continues moving slightly in the swing direction
- Gradually settles back to rest position via spring physics
- Creates a sense of follow-through and weight
- Heavier swings have more momentum

**Configuration:**
- `EnableWeaponMomentum` - Enable/disable the feature (default: `true`)
- `MomentumForceMagnitude` - How much weapons continue moving (default: `0.15`)

**Why it enhances feel:**
Real weapons have mass and inertia. This system makes virtual weapons feel less "floaty" and more grounded. The settling motion after a swing reinforces the sensation of swinging a physical object.

---

### 3. **Adaptive Combo Timing** 🔥
**Purpose:** Rewards consecutive attacks with improved flow and responsiveness.

**How it works:**
- Tracks consecutive attacks within a time window (1.5 seconds)
- Each hit in a combo increases attack speed slightly
- Stacks up to a maximum combo count
- Resets when the combo window expires
- Creates rhythm and flow in sustained combat

**Configuration:**
- `EnableComboTiming` - Enable/disable the feature (default: `true`)
- `ComboSpeedBoost` - Speed increase per combo hit (default: `0.06` = 6%)
- `MaxComboCount` - Maximum combo stacking (default: `4` hits)

**Why it enhances feel:**
This creates a "flow state" in melee combat. As you successfully land hits, your attacks become faster and more fluid, rewarding aggressive play and creating a satisfying crescendo effect. It feels like your character is "warming up" or gaining momentum.

**Example:** 
- 1st hit: Normal speed (1.0x)
- 2nd hit: 1.06x speed (6% faster)
- 3rd hit: 1.12x speed (12% faster)
- 4th hit: 1.18x speed (18% faster)

---

### 4. **Enhanced Hit Feedback** 💥
**Purpose:** Differentiates between normal hits, headshots, critical hits, and killing blows.

**How it works:**
- Detects hit type and location (headshot detection via hit tags)
- Detects critical hits via damage system
- Detects killing blows when target dies
- Applies stronger camera shake for more significant hits
- Creates a hierarchy of satisfaction

**Configuration:**
- `EnableHitFeedback` - Enable/disable the feature (default: `true`)
- `CriticalHitShakeMultiplier` - Extra shake for crits/headshots (default: `1.8x`)
- Killing blows get additional 1.2x multiplier on top of critical

**Why it enhances feel:**
Not all hits should feel the same. This creates clear, immediate feedback about hit quality. Landing a headshot or killing blow should feel noticeably more satisfying than a body hit. This reinforces player skill and creates memorable moments.

---

### 5. **Anticipation/Windup Variation** ⏱️
**Purpose:** Adds unpredictability and prevents combat from feeling robotic.

**How it works:**
- Varies the attack windup/startup speed slightly
- Some attacks start a bit faster, some a bit slower
- Combined with base speed randomization for maximum variety
- Makes each swing feel unique

**Configuration:**
- `EnableWindupVariation` - Enable/disable the feature (default: `true`)
- `WindupVariation` - Variation range (default: `0.08` = ±8%)

**Why it enhances feel:**
Perfectly consistent timing feels mechanical and predictable. Slight variations in windup make combat feel more organic and dynamic, like you're actually swinging a weapon rather than triggering a canned animation.

---

## Design Philosophy

These enhancements follow key principles of "game feel" from a first-person perspective:

### 1. **Immediate Feedback**
Every action should have immediate, clear consequences. Camera shake provides instant visual/kinesthetic feedback that a hit connected.

### 2. **Weight and Physicality**
Weapons should feel like physical objects with mass. Momentum and spring-based movement create this sensation.

### 3. **Reward Skill**
Better play (headshots, combos, critical hits) should feel noticeably better. This creates a reward loop that encourages mastery.

### 4. **Variety and Unpredictability**
No two swings should feel exactly the same. Variation prevents repetitive actions from feeling stale.

### 5. **Flow and Rhythm**
Combat should have a natural rhythm that skilled players can tap into. Combo timing creates this musical quality.

---

## Comparison: Small vs. Big Changes

### Small Random Changes (Existing)
✓ Speed variation (±12%)
✓ Position randomization
✓ Rotation randomization

**Effect:** Adds variety, prevents repetition
**Limitation:** Doesn't fundamentally change how combat *feels*

### Big Changes (New Enhancements)

#### **Camera Shake**
- **Impact:** HIGH - Most immediate feedback
- **Feel Change:** Hits feel weighty and impactful
- **Distinctiveness:** Makes your mod feel completely different from vanilla

#### **Combo System**  
- **Impact:** HIGH - Changes combat dynamics
- **Feel Change:** Rewards aggression, creates flow state
- **Distinctiveness:** Fundamentally alters combat pacing

#### **Hit Feedback**
- **Impact:** MEDIUM-HIGH - Skill reinforcement
- **Feel Change:** Makes good hits feel extra satisfying
- **Distinctiveness:** Creates clear quality differentiation

#### **Weapon Momentum**
- **Impact:** MEDIUM - Subtle but persistent
- **Feel Change:** Weapons feel less floaty, more grounded
- **Distinctiveness:** Adds physicality to every swing

#### **Windup Variation**
- **Impact:** LOW-MEDIUM - Enhances variety
- **Feel Change:** Prevents mechanical feeling
- **Distinctiveness:** Works with randomization for maximum unpredictability

---

## Technical Implementation Notes

All enhancements use the existing spring-based physics system, ensuring compatibility with the game's animation framework:

- **Camera shake:** Brief rotational impulse via `AddSoftForce` with very short frame count (3 frames)
- **Momentum:** Trailing force applied over longer duration (35 frames) for smooth settling
- **Combo timing:** Multiplies animation speed parameter based on tracked combo state
- **Hit feedback:** Hooks damage and hit detection to apply scaled camera shake
- **Windup variation:** Additional multiplier in the speed calculation chain

---

## Console Commands

All features can be toggled and tuned in real-time:

```
macheteanim camerashake <true|false>     - Toggle camera shake
macheteanim shakeintensity <value>       - Adjust shake strength
macheteanim momentum <true|false>        - Toggle weapon momentum
macheteanim combotiming <true|false>     - Toggle combo system
macheteanim combospeed <value>           - Adjust combo speed boost
macheteanim hitfeedback <true|false>     - Toggle enhanced hit feedback
macheteanim windup <true|false>          - Toggle windup variation
macheteanim show                         - Display all settings
```

---

## Recommended Configurations

### Subtle Enhancement (Conservative)
```
macheteanim camerashake true
macheteanim shakeintensity 0.05
macheteanim momentum true
macheteanim combotiming false
macheteanim hitfeedback true
```

### Balanced (Default)
All features enabled with default values.

### Maximum Impact (Aggressive)
```
macheteanim shakeintensity 0.12
macheteanim momentumforce 0.2
macheteanim combospeed 0.08
macheteanim critmultiplier 2.2
macheteanim windupvariation 0.12
```

### Arcade Style (Fast & Responsive)
```
macheteanim combotiming true
macheteanim combospeed 0.1
macheteanim maxcombo 6
macheteanim shakeintensity 0.15
```

---

## Future Enhancement Ideas

Potential additions that could further enhance melee combat feel:

1. **Recovery Variation** - Different return-to-idle speeds/paths after attacks
2. **Miss Feedback** - Extra "whoosh" movement when swinging at air vs hitting
3. **Material-Based Feedback** - Different shake intensity for flesh vs. armor vs. wood
4. **Fatigue System** - Slower swings after many consecutive attacks (stamina simulation)
5. **Directional Variation** - Different feels for horizontal vs. vertical vs. overhead swings
6. **Charged Attacks** - Hold to "wind up" for more powerful strikes with exaggerated momentum

---

## Conclusion

These enhancements transform melee combat from "randomized animations" to a cohesive, visceral first-person experience. Each feature contributes to making hits feel impactful, weapons feel weighty, and combat feel dynamic and rewarding.

The combination of **immediate feedback** (camera shake), **physical sensation** (momentum), **skill rewards** (hit feedback), and **flow state** (combo timing) creates a melee system that feels fundamentally different and more satisfying than vanilla, while still working within the game's existing animation framework.
