# Quick Setup Checklist - Level Progression System

## ✅ Step-by-Step Setup (5 minutes)

### STEP 1: Rename Scene File
1. In Project window, navigate to `Assets/Scenes/`
2. Right-click `MainGame.unity` → Rename → `Level1.unity`
3. **Important**: Update Build Settings!
   - File > Build Settings
   - Remove old "MainGame" entry
   - Add "Level1" (drag scene file into list)
   - Ensure scene order: MainMenu (0), Level1 (1)

---

### STEP 2: Create Level Select UI in Main Menu

#### A. Setup Container
1. Open `MainMenu.unity` scene
2. Select Canvas (or create one if missing)
3. Right-click Canvas → `UI > Panel` (rename to **"LevelSelectPanel"**)
4. Position panel where you want (center of screen recommended)
5. Add component to panel: `Vertical Layout Group`
   - Padding: Top/Bottom 20, Left/Right 20
   - Spacing: 15
   - Child Alignment: Middle Center
   - Child Controls Size: Width ✓, Height ✓

#### B. Create Button Prefab
1. Right-click Hierarchy: `UI > Button - TextMeshPro` (rename to **"LevelButtonPrefab"**)
2. Configure button properties:
   - **RectTransform**: Width 250, Height 80
   - **Button colors**: 
     - Normal: White
     - Highlighted: Light Gray
     - Pressed: Dark Gray
     - Disabled: will be set by script
3. Select child `Text (TMP)`:
   - Font Size: **28**
   - Alignment: **Center (horizontal & vertical)**
   - Text: "Level 1" (placeholder, will auto-update)
   - Color: Black (or your preference)
4. **Create prefab**: Drag button from Hierarchy to `Assets/Prefabs/` folder
   - (Create Prefabs folder if it doesn't exist)
5. **Delete button from Hierarchy** (we only need the prefab)

#### C. Connect Components
1. Select **"LevelSelectPanel"** in Hierarchy
2. Add Component → `Level Select UI` script
3. In Inspector, assign:
   - **Button Container** → Drag "LevelSelectPanel" GameObject (the one with Vertical Layout Group)
   - **Level Button Prefab** → Drag prefab from Assets/Prefabs/
   - **Unlocked Color**: White (default)
   - **Locked Color**: Gray with 50% alpha
   - **Locked Text**: "🔒 Locked" (or just "Locked")

#### D. Optional: Add Progress Text
1. Right-click LevelSelectPanel → `UI > Text - TextMeshPro` (rename "ProgressText")
2. Position above buttons
3. Text: "Progress: 0/3 Levels Completed"
4. Font Size: 20, Alignment: Center
5. Select LevelSelectPanel → Assign "ProgressText" to `Progress Text` field in inspector

---

### STEP 3: Configure Level UIManagers

#### Level1 Scene:
1. Open `Level1.unity` (formerly MainGame)
2. Select `UIManager` GameObject
3. Inspector: Set **Next Level Scene Name** = `Level2`
4. Save scene

#### Level2 Scene (if exists):
1. Open `Level2.unity`
2. Select `UIManager` GameObject  
3. Inspector: Set **Next Level Scene Name** = `BossLevel`
4. Save scene

#### BossLevel Scene (if exists):
1. Open `BossLevel.unity`
2. Select `UIManager` GameObject
3. Inspector: **Leave Next Level Scene Name EMPTY** (this makes it the final level)
4. Ensure `Final Victory Panel` is assigned (separate from regular Victory Panel)
5. Save scene

---

### STEP 4: Update Build Settings
1. File > Build Settings
2. Ensure scenes are in correct order:
   ```
   [0] MainMenu
   [1] Level1
   [2] Level2      (if exists)
   [3] BossLevel   (if exists)
   ```
3. Close Build Settings

---

### STEP 5: Test!

#### Test Flow:
1. **Play MainMenu scene**
   - Should see 3 level buttons
   - Level1: White, clickable
   - Level2: Gray, locked 🔒
   - BossLevel: Gray, locked 🔒

2. **Click Level1** → Scene loads

3. **Beat Level1** (kill all enemies/boss)
   - Victory panel shows
   - Click "Next Level" → loads Level2
   - **Progress saved!**

4. **Stop Play Mode** → Play MainMenu again
   - Level1: White with ✅ (completed)
   - Level2: White, clickable (now unlocked!)
   - BossLevel: Still gray/locked

5. **Test death restart**:
   - Play Level2, let player die
   - Game Over panel shows
   - Click "Restart" → **stays on Level2** (doesn't go back to Level1)

6. **Test persistence**:
   - Stop Unity Editor completely
   - Restart and play MainMenu
   - Level2 should still be unlocked ✓

---

## 🎨 Adding Images Later

When you're ready to add button images:

### Option 1: Background Image
1. Open button prefab
2. Select Button GameObject → Add Component → `Image` (if not already there)
3. Assign your sprite
4. Move Text (TMP) forward in Hierarchy (renders on top)

### Option 2: Icon + Text
1. Open button prefab
2. Add child: `UI > Image` (rename "LevelIcon")
3. Assign sprite (e.g., level preview thumbnail)
4. Arrange layout: Icon on left, Text on right
5. Optional: Add `Horizontal Layout Group` to auto-arrange

### Option 3: Full Custom
1. Modify `LevelSelectUI.GenerateLevelButtons()` to reference your Image components
2. Set sprites dynamically per level

---

## 🐛 Troubleshooting

### "Level buttons not appearing"
- Check Console for errors
- Verify `LevelSelectUI` script is on LevelSelectPanel
- Verify `buttonContainer` is assigned (the VerticalLayoutGroup object)
- Verify `levelButtonPrefab` is assigned

### "All buttons are locked"
- First level should auto-unlock
- Check `GameProgressManager.allLevels[0]` = "Level1" (case-sensitive!)
- Clear PlayerPrefs: Add this to a test button: `PlayerPrefs.DeleteAll();`

### "Progress not saving"
- Check Console for "✅ Level completed" log when clicking Next Level
- Verify scene names **exactly match** in Build Settings and `allLevels` list
- Check UIManager calls `GameProgressManager.Instance.MarkLevelComplete()`

### "Scene not found error"
- File > Build Settings → Add all level scenes
- Verify scene names are **case-sensitive** ("Level1" ≠ "level1")

---

## 📋 Final Checklist

- [ ] MainGame.unity renamed to Level1.unity
- [ ] Build Settings updated with correct scene names
- [ ] LevelSelectPanel created with VerticalLayoutGroup
- [ ] Button prefab created and assigned
- [ ] LevelSelectUI script added to panel with references assigned
- [ ] UIManager in Level1: nextLevelSceneName = "Level2"
- [ ] UIManager in Level2: nextLevelSceneName = "BossLevel"
- [ ] UIManager in BossLevel: nextLevelSceneName = "" (empty)
- [ ] Tested in Play mode: Level unlock progression works
- [ ] Tested death restart: stays on current level
- [ ] Tested persistence: progress saves after closing Unity

---

**All done!** 🎉 You now have a working level progression system with basic UI. Add your custom images/sprites later without touching any code!
