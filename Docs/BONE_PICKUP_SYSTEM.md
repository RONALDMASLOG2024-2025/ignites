# 🦴 Bone Pickup System - Stat Enhancement Drops

## Overview

Enemies now drop **two types of pickups**:
- **🥩 Meat** - Heals player
- **🦴 Bone** - Enhances player stats (damage and/or movement speed)

Both drops are independent and can drop from the same enemy!

---

## 🎮 How It Works

### Meat Pickup (Existing):
- Heals player health
- Drop chance: Configurable per enemy
- Already implemented

### Bone Pickup (NEW!):
- **Damage Boost** - Increases player attack damage permanently
- **Speed Boost** - Increases player movement speed permanently
- Drop chance: Configurable per enemy
- Bonuses last for the current level (reset on level restart)

---

## 🔧 Setup Instructions

### Step 1: Create Bone Prefab

1. **Create sprite/GameObject:**
   - Right-click in Hierarchy → `2D Object > Sprite`
   - Rename to: "BonePrefab"
   - Assign a bone sprite to SpriteRenderer

2. **Add components:**
   - Add Component → `Circle Collider 2D`
     - Set **Is Trigger** ✓
     - Adjust radius to fit sprite
   - Add Component → `Bone Pickup` script

3. **Configure BonePickup:**
   - **Damage Boost**: `1` (adds 1 damage per pickup)
   - **Speed Boost**: `0.5` (adds 0.5 speed per pickup)
   - **Destroy On Pickup**: ✓ Checked
   - **Pickup Sound**: Drag audio clip (optional)
   - **Pickup Volume**: 0.7
   - **Max Pickup Distance**: 20

4. **Save as prefab:**
   - Drag "BonePrefab" to `Assets/Prefabs/` folder
   - Delete from Hierarchy

---

### Step 2: Configure Enemy Drops

**For each enemy/boss GameObject:**

1. Select enemy in Hierarchy or prefab
2. Find `Enemy_Health` component in Inspector
3. Configure drops:

#### Option A: Use New System (Recommended)
```
Loot Drops:
├── Meat Drop Prefab: [Drag MeatPrefab here]
│   Meat Drop Chance: 50 (50% chance)
├── Bone Drop Prefab: [Drag BonePrefab here]
│   Bone Drop Chance: 30 (30% chance)
```

#### Option B: Use Legacy System
```
Legacy Drop:
├── Drop Prefab: [Your old meat prefab]
│   Drop Chance: 50
```

**Note:** You can mix both systems! Enemy can drop from both new and legacy systems.

---

### Step 3: Adjust Drop Rates

**Recommended drop chances:**

| Enemy Type | Meat Drop | Bone Drop | Notes |
|------------|-----------|-----------|-------|
| Weak enemies | 40% | 20% | Common, low rewards |
| Normal enemies | 50% | 30% | Standard drop rates |
| Strong enemies | 60% | 40% | Higher chances |
| Mini-boss | 80% | 60% | Almost guaranteed drops |
| Boss | 100% | 80% | Always drops meat, high bone chance |

**Balancing tips:**
- Lower bone drop rates than meat (bones are more powerful)
- Increase rates for harder enemies as reward
- Boss should drop both to feel rewarding

---

## ⚙️ Customization

### Adjust Stat Boosts:

**For each BonePrefab (can have multiple variants):**

#### Weak Bone (Common):
- Damage Boost: `1`
- Speed Boost: `0.3`

#### Normal Bone (Standard):
- Damage Boost: `1`
- Speed Boost: `0.5`

#### Strong Bone (Rare):
- Damage Boost: `2`
- Speed Boost: `1.0`

**Create multiple bone prefabs with different boost values!**

---

### Different Drop Positions:

Bones spawn slightly offset from meat to avoid overlap:
```csharp
// In Enemy_Health.cs Die() method
Vector3 bonePosition = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
```

Adjust the offset range for more spread or tighter grouping.

---

## 🧪 Testing

### Test Stat Boosts:

1. **Play Level1**
2. **Before pickup:**
   - Note player damage (check `Player_Combat.Damage` in Inspector during Play mode)
   - Note player speed (check `PlayerMovement.speed` in Inspector)
3. **Kill enemy** → Pick up bone
4. **Check Console:**
   ```
   Player picked up bone! Damage +1 (Now: 2) Speed +0.5 (Now: 5.5)
   ```
5. **Verify in Inspector:**
   - Player's Damage should increase
   - Player's Speed should increase
6. **Test gameplay:**
   - Movement should feel slightly faster
   - Attacks should deal more damage to enemies

---

## 📊 Drop Probability Examples

### Single Enemy (50% meat, 30% bone):
- **65%** chance: Drop nothing
- **20%** chance: Drop only meat
- **9%** chance: Drop only bone  
- **15%** chance: Drop BOTH meat and bone

### Boss (100% meat, 80% bone):
- **0%** chance: Drop nothing
- **20%** chance: Drop only meat
- **0%** chance: Drop only bone
- **80%** chance: Drop BOTH meat and bone

**Both drops are independent rolls!**

---

## 🎨 Visual Customization

### Add Pickup Effect:

1. Create particle system (Right-click → `Effects > Particle System`)
2. Configure (sparkles, glow, etc.)
3. Save as prefab: "BonePickupEffect"
4. Assign to `BonePickup.pickupEffect` field
5. Automatically spawns when bone is collected

### Add Audio:

1. Find/import bone pickup sound effect
2. Drag to `BonePickup.pickupSound` field
3. Adjust `Pickup Volume` slider
4. Uses 3D spatial audio (louder when closer)

---

## 🔄 Stat Reset Behavior

**Current implementation:**
- Stat boosts are **permanent for the current level**
- Reset when:
  - Player dies and restarts level
  - Player returns to Main Menu
  - Next level loads

**To make stats persistent across levels:**
Add this to a new `PlayerStats.cs` singleton (DontDestroyOnLoad).

---

## 💡 Advanced Usage

### Create Different Bone Types:

**Speed Bone (fast but weak):**
- Damage Boost: `0`
- Speed Boost: `1.0`
- Blue sprite

**Damage Bone (strong but slow):**
- Damage Boost: `3`
- Speed Boost: `0`
- Red sprite

**Balanced Bone:**
- Damage Boost: `1`
- Speed Boost: `0.5`
- White sprite

**Configure enemies to drop different bone types!**

---

## 🐛 Troubleshooting

### Bone doesn't drop:
- ✅ Check `boneDropChance` is > 0
- ✅ Check `boneDropPrefab` is assigned
- ✅ RNG - try 100% drop chance to test

### Stats don't increase:
- ✅ Check Console for "Player picked up bone!" message
- ✅ Verify player has `Player_Combat` and `PlayerMovement` components
- ✅ Check `damageBoost` and `speedBoost` are > 0

### Bone doesn't get picked up:
- ✅ Bone prefab has Collider2D with **Is Trigger** checked
- ✅ Player has Collider2D
- ✅ Player GameObject has tag: "Player"

### Both meat and bone spawn in same spot:
- ✅ This is normal! They overlap slightly
- ✅ Increase offset in `Enemy_Health.cs` Die() method
- ✅ Or pick up both at once (works fine)

---

## 📋 Quick Setup Checklist

```
□ Created BonePrefab with sprite
□ Added Circle Collider 2D (Is Trigger ✓)
□ Added BonePickup script
□ Configured damageBoost and speedBoost
□ Saved as prefab in Assets/Prefabs/
□ Assigned to enemies' boneDropPrefab field
□ Set boneDropChance (30-50% recommended)
□ Optional: Added pickup sound
□ Optional: Added pickup effect
□ Tested in Play mode
```

---

## 🎯 Summary

**Files Added:**
- `BonePickup.cs` - Handles stat enhancement on pickup

**Files Modified:**
- `Enemy_Health.cs` - Added separate meat/bone drop system

**Features:**
- ✅ Dual drop system (meat + bone)
- ✅ Independent drop chances
- ✅ Configurable stat boosts
- ✅ Permanent boosts per level
- ✅ Spatial audio support
- ✅ Visual effects support
- ✅ Backwards compatible with old drop system

**Balance the drop rates and boost values to match your game's difficulty curve!** 🎮✨
