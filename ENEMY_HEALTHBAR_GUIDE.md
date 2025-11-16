# 🩸 Enemy Health Bar Setup Guide

## 📋 Overview
Add simple health bars above enemy heads that update in real-time as they take damage.

---

## 🎨 STEP 1: Create the Health Bar Prefab

### Create Canvas (World Space)

1. **In Hierarchy**, right-click your **Enemy prefab** (or any enemy) → **UI** → **Canvas**
2. **Rename** it to `HealthBarCanvas`
3. **Select `HealthBarCanvas`** in Inspector:

**Canvas Component Settings:**
   - **Render Mode**: `World Space`
   - **Width**: `100`
   - **Height**: `20`
   - **Rect Transform**:
     - Position X: `0`
     - Position Y: `1.5` (adjust to position above enemy head)
     - Position Z: `0`
     - Scale X: `0.01`
     - Scale Y: `0.01`
     - Scale Z: `0.01`

**Canvas Scaler** (if present):
   - Dynamic Pixels Per Unit: `10`

---

## 🎨 STEP 2: Create Background Bar

1. **Right-click `HealthBarCanvas`** → **UI** → **Image**
2. **Rename** to `Background`
3. **Configure**:

**Rect Transform:**
   - Anchor: Stretch-Stretch
   - Left: `0`, Right: `0`, Top: `0`, Bottom: `0`

**Image Component:**
   - Color: **Dark Red** or **Black** (RGB: 50, 0, 0, 255)
   - Or use a sprite for custom background

---

## 🎨 STEP 3: Create Fill Bar (The Health Indicator)

1. **Right-click `Background`** → **UI** → **Image**
2. **Rename** to `FillBar`
3. **Configure**:

**Rect Transform:**
   - Anchor: Left-Stretch
   - Pivot: X=`0`, Y=`0.5`
   - Left: `0`, Right: `0`, Top: `0`, Bottom: `0`

**Image Component:**
   - **Image Type**: `Filled`
   - **Fill Method**: `Horizontal`
   - **Fill Origin**: `Left`
   - **Fill Amount**: `1` (start at full)
   - **Color**: **Green** (RGB: 0, 255, 0, 255)

---

## 🎨 STEP 4: Add the Script

1. **Select `HealthBarCanvas`**
2. **Add Component** → Search for `EnemyHealthBar`
3. **Configure the script**:

**References:**
   - **Fill Bar**: Drag the `FillBar` Image into this field
   - **Enemy Health**: Leave empty (auto-finds from parent)

**Visual Settings:**
   - **Full Health Color**: Green (0, 255, 0)
   - **Low Health Color**: Red (255, 0, 0)
   - **Low Health Threshold**: `0.3` (turns red at 30% health)

**Behavior:**
   - **Hide When Full**: ✅ Checked (bar appears only when damaged)
   - **Hide On Death**: ✅ Checked (bar disappears when enemy dies)

---

## 🎨 STEP 5: Optional - Add Border/Outline

For better visibility:

1. **Select `Background`**
2. **Add Component** → **Outline**
3. **Configure**:
   - Effect Color: White or Light Gray
   - Effect Distance: X=`1`, Y=`1`

---

## 📦 STEP 6: Make it Part of Enemy Prefab

### Option A: If Editing Prefab Directly
1. The health bar is already part of the prefab
2. **Save the prefab**
3. Done! All instances will have health bars

### Option B: If Testing on Scene Enemy
1. **Select the enemy GameObject** in Hierarchy
2. **Right-click the enemy** → **Prefab** → **Apply All** (if it's a prefab instance)
3. Or manually add to the prefab in Project view

---

## 🎯 STEP 7: Adjust Positioning

The health bar should float above the enemy's head:

1. **Select `HealthBarCanvas`**
2. **Adjust Position Y** in Rect Transform:
   - Small enemies: `1` to `1.5`
   - Medium enemies: `1.5` to `2`
   - Large enemies/bosses: `2` to `3`

3. **Test in Play Mode** to check visibility

---

## 🎨 STEP 8: Customize Appearance (Optional)

### Make it Smaller/Larger
**Select `HealthBarCanvas`**:
- Scale X/Y/Z: Increase to `0.015` or decrease to `0.008`

### Change Bar Colors
**Select `FillBar`**:
- Full Health: Bright Green (0, 255, 100)
- Low Health: Orange/Red (255, 150, 0)

### Add Smooth Color Transition
The script automatically transitions from green → yellow → red based on health percentage.

### Add Background Shadow
**Select `Background`**:
- **Add Component** → **Shadow**
- Effect Distance: (2, -2)
- Color: Black with alpha ~150

---

## 🧪 Testing

1. **Enter Play Mode**
2. **Attack an enemy**
3. **Watch the health bar**:
   - ✅ Should appear when enemy takes damage
   - ✅ Should decrease smoothly
   - ✅ Should change color (green → red)
   - ✅ Should disappear when enemy dies

---

## 🔧 Troubleshooting

### Health bar doesn't appear
- Check if `HealthBarCanvas` is active
- Check if `Enemy_Health` component is found (check console)
- Ensure Canvas Render Mode is **World Space**

### Health bar is too small/large
- Adjust `HealthBarCanvas` Scale (X, Y, Z)
- Typical range: `0.005` to `0.02`

### Health bar is in wrong position
- Adjust Position Y on `HealthBarCanvas`
- Make sure it's a child of the enemy GameObject

### Health bar doesn't update
- Check if `fillBar` reference is assigned in script
- Check console for errors
- Ensure `Enemy_Health` component exists on enemy

### Health bar faces wrong direction
- Ensure Canvas Render Mode is **World Space**
- You can add a script to make it always face camera (optional)

### Colors don't change
- Check `fullHealthColor` and `lowHealthColor` in script
- Adjust `lowHealthThreshold` (0-1 range)

---

## 🎨 Advanced: Camera-Facing Health Bar (Optional)

If you want health bars to always face the camera:

1. **Create new script** `FaceCamera.cs`:

```csharp
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
```

2. **Attach to `HealthBarCanvas`**
3. Health bar will always face player camera

---

## 📊 Final Hierarchy Structure

```
Enemy (Prefab)
├── EnemySprite
├── Collider2D
├── Enemy_Health (script)
├── Enemy_Movement (script)
└── HealthBarCanvas (World Space Canvas)
    ├── EnemyHealthBar (script)
    └── Background (Image - dark red/black)
        └── FillBar (Image - green, Filled type)
```

---

## 🎯 Apply to All Enemies

### Method 1: Prefab Variant
1. Open each enemy prefab
2. Follow steps above
3. Save prefab

### Method 2: Copy Component
1. Set up health bar on one enemy
2. **Right-click `HealthBarCanvas`** → **Copy**
3. **Select other enemy prefabs** → **Right-click** → **Paste**

### Method 3: Duplicate in Prefab
1. Open enemy prefab in Prefab mode
2. Copy `HealthBarCanvas` from one enemy
3. Paste into other enemy prefabs
4. Adjust Position Y for each enemy size

---

## ✅ Summary

**You now have:**
- ✅ Health bars above enemy heads
- ✅ Color changes (green → red) based on health
- ✅ Auto-hides when full health (optional)
- ✅ Auto-hides on death
- ✅ Real-time updates when damaged
- ✅ Simple, clean design

**Health bar features:**
- 🟢 Green when healthy
- 🟡 Yellow/orange when medium health
- 🔴 Red when low health (below 30%)
- 👻 Hidden until damaged
- 💀 Disappears on death

Your enemies now have visual health feedback! 🎮
