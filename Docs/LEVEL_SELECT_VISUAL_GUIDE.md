# Level Select Panel - Visual Setup Guide (Super Mario Style)

## 🎮 Current Hierarchy (Good Structure!)

```
MainMenu
├── Main Camera
├── Canvas
│   ├── LevelSelectPanel ← Add LevelSelectUI script here
│   │   ├── VerticalLayoutGroup
│   │   ├── Panel (Header)
│   │   │   ├── Image (Background)
│   │   │   └── Title (Text)
│   │   └── Panel (Footer - Optional)
│   ├── EventSystem
│   └── MusicManager
```

---

## ✅ Step-by-Step Visual Setup

### STEP 1: Configure LevelSelectPanel

1. **Select `LevelSelectPanel`** in Hierarchy
2. **Add Component** → `Level Select UI` script
3. **Inspector Settings:**
   - **Button Container** → Drag `VerticalLayoutGroup` (child object)
   - **Level Button Prefab** → (We'll create this next)
   - **Unlocked Color**: RGB(255, 255, 255, 255) - White
   - **Locked Color**: RGB(100, 100, 100, 128) - Gray, semi-transparent
   - **Locked Text**: "🔒 Locked"

### STEP 2: Create Button Prefab (Super Mario Style)

#### Basic Version (Text Only - For Now):
1. Right-click in Hierarchy: `UI > Button - TextMeshPro`
2. Rename to: `LevelButtonPrefab`
3. **Configure Button:**
   - **RectTransform**: Width 300, Height 100
   - **Button Component**:
     - **Normal Color**: White
     - **Highlighted Color**: Light Yellow (255, 255, 200)
     - **Pressed Color**: Yellow (255, 230, 0)
     - **Disabled Color**: Dark Gray (100, 100, 100)
   - **Image Component** (background):
     - Color: Light Brown (200, 170, 130) - parchment style
     - Or use your custom button sprite

4. **Configure Text (TMP):**
   - Select child `Text (TMP)`
   - Font Size: **36**
   - Alignment: **Center (H + V)**
   - Color: Dark Brown (60, 40, 20)
   - Text: "Level 1" (placeholder)
   - Font: Bold recommended

5. **Add Locked Icon (Optional but Recommended):**
   - Right-click Button → `UI > Image`
   - Rename to: "LockedIcon"
   - Position: Top-right corner
   - Size: 40x40
   - Image: Use a padlock sprite (or emoji 🔒)
   - **This will be hidden for unlocked levels**

6. **Add Star Icon (For Completed Levels):**
   - Right-click Button → `UI > Image`
   - Rename to: "CompletedStar"
   - Position: Top-left corner
   - Size: 40x40
   - Image: Use a star sprite (or emoji ⭐)
   - **This shows when level is beaten**

7. **Save as Prefab:**
   - Drag button from Hierarchy to `Assets/Prefabs/` folder
   - Delete from Hierarchy

#### Enhanced Version (With Preview Image - Add Later):
```
LevelButtonPrefab
├── Image (Background - button sprite)
├── Image (Preview - level thumbnail)
├── Text (TMP) (Level name)
├── Image (LockedIcon - padlock, hidden if unlocked)
└── Image (CompletedStar - star, hidden if not completed)
```

### STEP 3: Configure VerticalLayoutGroup

Select `VerticalLayoutGroup` (child of LevelSelectPanel):

**Layout Settings:**
- Padding: Top 20, Bottom 20, Left 50, Right 50
- Spacing: **20** (gap between buttons)
- Child Alignment: **Upper Center** (or Middle Center)
- Control Child Size:
  - Width: ✓ Checked
  - Height: ✓ Checked
- Child Force Expand:
  - Width: ✗ Unchecked
  - Height: ✗ Unchecked

**Content Size Fitter (Optional):**
- Add Component → `Content Size Fitter`
- Vertical Fit: Preferred Size (auto-adjusts height)

### STEP 4: Add Visual Polish

#### Title Text (Above Buttons):
1. Right-click LevelSelectPanel → `UI > Text - TextMeshPro`
2. Rename to: "TitleText"
3. Position: Top of panel
4. Text: "Select Level"
5. Font Size: 48
6. Alignment: Center
7. Color: White (with black outline for readability)

#### Progress Text (Optional):
1. Right-click LevelSelectPanel → `UI > Text - TextMeshPro`
2. Rename to: "ProgressText"
3. Position: Below title, above buttons
4. Text: "Progress: 0/3 Levels Completed"
5. Font Size: 24
6. Alignment: Center
7. **Assign to `LevelSelectUI.progressText` in Inspector**

#### Background Panel:
1. Select `Panel` (if you have one)
2. Add `Image` component
3. Color: Semi-transparent black (0, 0, 0, 180)
4. Or use a fancy border sprite

---

## 🎨 Super Mario Style Design Recommendations

### Color Scheme:
- **Unlocked Levels**: 
  - Button: Bright, colorful (yellow, orange, green)
  - Text: Dark, bold
  - Icon: Visible level preview or number

- **Locked Levels**:
  - Button: Grayed out, desaturated
  - Text: "🔒 Locked" or "???"
  - Icon: Padlock overlay
  - **Non-clickable** (button.interactable = false)

- **Completed Levels**:
  - Button: Same as unlocked
  - **Star badge** in corner (⭐ or ✅)
  - Text: "Level 1 ✅"

### Layout Options:

#### Option A: Vertical List (Current - Good for 3-5 levels)
```
┌─────────────────────┐
│   Select Level      │
├─────────────────────┤
│  ⭐ [Level 1] ✓     │
├─────────────────────┤
│     [Level 2]       │
├─────────────────────┤
│  🔒 [Level 3] 🔒    │
└─────────────────────┘
```

#### Option B: Grid Layout (Better for many levels)
Change `VerticalLayoutGroup` to `Grid Layout Group`:
```
┌─────────────────────────────┐
│      Select Level           │
├─────────┬─────────┬─────────┤
│ ⭐ Lvl1 │  Lvl 2  │ 🔒 Lvl3│
├─────────┼─────────┼─────────┤
│ 🔒 Lvl4 │ 🔒 Lvl5 │ 🔒 Lvl6│
└─────────┴─────────┴─────────┘
```

Settings for Grid:
- Cell Size: 200 x 200 (square buttons)
- Spacing: 20 x 20
- Constraint: Fixed Column Count = 3

---

## 🔓 Unlock System (Already Implemented!)

The `LevelSelectUI` script automatically handles:
- ✅ **Level1 always unlocked** (first level in list)
- ✅ **Level2 locked until Level1 completed**
- ✅ **BossLevel locked until Level2 completed**
- ✅ **Visual feedback**: Unlocked = clickable, Locked = grayed out
- ✅ **Progress saves** via PlayerPrefs (persists across sessions)

### How It Works:
```
NEW PLAYER:
[Level 1]     ← White, clickable ✓
[🔒 Level 2]  ← Gray, locked
[🔒 BossLevel]← Gray, locked

AFTER BEATING LEVEL 1:
[Level 1 ⭐]  ← White with star
[Level 2]     ← White, clickable ✓ (NOW UNLOCKED!)
[🔒 BossLevel]← Gray, locked

AFTER BEATING LEVEL 2:
[Level 1 ⭐]  ← Completed
[Level 2 ⭐]  ← Completed
[BossLevel]   ← White, clickable ✓ (NOW UNLOCKED!)
```

---

## 🎯 Final Checklist

- [ ] LevelSelectPanel has `LevelSelectUI` script
- [ ] VerticalLayoutGroup configured (padding, spacing)
- [ ] Button prefab created with proper size/colors
- [ ] Button prefab assigned to `levelButtonPrefab` field
- [ ] VerticalLayoutGroup assigned to `buttonContainer` field
- [ ] Optional: ProgressText assigned
- [ ] Optional: Title text added
- [ ] Test in Play mode: Level1 unlocked, others locked
- [ ] Test unlock: Beat Level1 → return to menu → Level2 now unlocked

---

## 🖼️ Adding Custom Graphics Later

When you have sprites ready:

1. **Button Background**: Replace `Image` component sprite
2. **Level Preview**: Add child `Image` with level screenshot/thumbnail
3. **Lock Icon**: Replace 🔒 text with padlock sprite
4. **Star Icon**: Replace ⭐ text with star sprite
5. **Hover Effects**: Add `Button` transition animations

**No code changes needed!** The `LevelSelectUI` script works with any button design.

---

## 🚀 Quick Tips

- **Test unlock flow**: Use Console command to unlock all:
  ```csharp
  // In a test button or Debug menu:
  GameProgressManager.Instance.MarkLevelComplete("Level1");
  GameProgressManager.Instance.MarkLevelComplete("Level2");
  ```

- **Reset progress**: Use Console command:
  ```csharp
  GameProgressManager.Instance.ResetAllProgress();
  ```

- **Custom level names**: Edit `LevelSelectUI.GetLevelDisplayName()` method

---

Your hierarchy is already set up well! Just need to connect the prefab and configure the layout. 🎮✨
