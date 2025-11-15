# Level Progression System - Setup Guide

## Overview
This system provides:
- **Persistent level unlock tracking** (saved via PlayerPrefs)
- **Level restart on death** (stays on current level)
- **Level selection UI** in Main Menu (only unlocked levels clickable)
- **Sequential unlock** (beat Level 1 → unlock Level 2, etc.)

---

## 📋 Quick Start

### 1. Configure Level List
Open `GameProgressManager.cs` and verify the `allLevels` list:

```csharp
public List<string> allLevels = new List<string>
{
    "Level1",      // Always unlocked
    "Level2",      // Unlocks after beating Level1
    "BossLevel",   // Final level - unlocks after beating Level2
};
```

**Important:** First level is always unlocked. Sequential unlock pattern (beat Level1 → unlock Level2 → unlock BossLevel).

---

### 2. Setup Main Menu Scene

#### A. Create Level Select Panel
1. Open `MainMenu.unity`
2. Create: `Canvas > Panel` (rename to "LevelSelectPanel")
3. Add child: `Vertical Layout Group` (or Grid Layout Group)
   - Configure spacing, padding as desired
4. Add `LevelSelectUI` script to "LevelSelectPanel"

#### B. Create Button Prefab (Basic - Images Added Later)
1. Right-click in Hierarchy: `UI > Button - TextMeshPro` (rename to "LevelButtonPrefab")
2. Configure button:
   - **Size**: Width: 200, Height: 80 (or your preference)
   - **Colors**: Normal: White, Locked: Gray (configured in LevelSelectUI script)
3. Select the child `Text (TMP)` component:
   - **Font Size**: 24
   - **Alignment**: Center
   - **Text**: "Level X" (will be auto-generated)
4. **Drag button to Assets/Prefabs/** to create prefab (create Prefabs folder if needed)
5. Delete from Hierarchy
6. **Assign prefab** to `LevelSelectUI.levelButtonPrefab` in inspector
7. **Assign container** (Vertical Layout Group) to `LevelSelectUI.buttonContainer`

**Note**: Later you can add Image components for icons, backgrounds, preview thumbnails, etc.

#### C. Optional: Progress Text
- Add `TMP_Text` above button container
- Assign to `LevelSelectUI.progressText` (shows "Progress: 2/5 Levels Completed")

---

### 3. Configure Each Level Scene

For **each gameplay level** (Level1, Level2, BossLevel):

#### A. UIManager Setup
1. Select `UIManager` GameObject
2. Set `nextLevelSceneName` to the **next level** scene name:
   - **Level1** → Set to "Level2"
   - **Level2** → Set to "BossLevel"
   - **BossLevel** → **Leave empty** (this triggers final victory panel)

#### B. Verify Panels
Ensure these panels are assigned in UIManager:
- `hudPanel`
- `pausePanel`
- `gameOverPanel`
- `victoryPanel`
- `finalVictoryPanel` (for final level only)

---

## 🎮 How It Works

### Player Death → Restart Current Level
- Player dies → Game Over panel shows
- Click "Restart" → **reloads current scene** (not Level 1 or Main Menu)
- Progress is **not saved** (must beat level to unlock next)

### Player Victory → Unlock Next Level
- Defeat all enemies/boss → Victory panel shows
- Click "Next Level" → **saves progress** (unlocks next level in sequence)
- Next time in Main Menu, that level button is clickable

### Level Select UI
- **Unlocked levels**: Normal color, clickable, shows "✅" if completed
- **Locked levels**: Grayed out, shows "🔒 Locked", not clickable
- **Sequential unlock**: Must beat Level 1 to unlock Level 2, etc.

---

## 🛠️ Testing

### Test Flow
1. **Start new game**: Only Level 1 unlocked
2. **Beat Level 1**: Next level unlocks, progress saved
3. **Return to Main Menu**: Level 2 now clickable
4. **Die on Level 2**: Restart → stays on Level 2 (not back to Level 1)
5. **Close game and reopen**: Progress persists (Level 2 still unlocked)

### Debug Commands (optional)
Add these buttons in Main Menu for testing:

```csharp
// Reset all progress (unlock only Level 1)
GameProgressManager.Instance.ResetAllProgress();

// Unlock all levels (for testing)
foreach (string level in GameProgressManager.Instance.allLevels)
{
    GameProgressManager.Instance.MarkLevelComplete(level);
}
```

---

## 📁 Files Added/Modified

### New Files
- `Assets/Scripts/GameProgressManager.cs` - Persistent progress tracking
- `Assets/Scripts/LevelSelectUI.cs` - Main Menu level selection UI
- `Docs/LEVEL_PROGRESSION_SETUP.md` - This guide

### Modified Files
- `Assets/Scripts/UIManager.cs`
  - `OnRestartButton()`: Now reloads current scene (not hardcoded)
  - `OnNextLevelButton()`: Saves progress before advancing

---

## 🎨 Customization

### Change Level Display Names
Edit `LevelSelectUI.GetLevelDisplayName()`:

```csharp
private string GetLevelDisplayName(string sceneName, int levelNumber, bool isCompleted)
{
    // Custom names per level
    string[] levelNames = { "Forest", "Castle", "Dragon's Lair" };
    string name = levelNumber <= levelNames.Length ? levelNames[levelNumber - 1] : $"Level {levelNumber}";
    return isCompleted ? $"{name} ✅" : name;
}
```

### Non-Sequential Unlock
Want to unlock multiple levels at once? Modify `GameProgressManager.IsLevelUnlocked()`:

```csharp
// Example: Unlock all levels after beating Level 1
if (completedLevels.Contains(allLevels[0])) return true;
```

### Add Level Preview Images
Add `Image` component to button prefab, then in `LevelSelectUI.GenerateLevelButtons()`:

```csharp
Image previewImage = buttonObj.transform.Find("PreviewImage").GetComponent<Image>();
previewImage.sprite = Resources.Load<Sprite>($"LevelPreviews/{sceneName}");
```

---

## ⚠️ Common Issues

### "Level not unlocking"
- Check `GameProgressManager.allLevels` list has correct scene names
- Verify `UIManager.nextLevelSceneName` matches the actual scene name (case-sensitive!)
- Ensure victory triggers `UIManager.ShowVictory()` (not ShowGameOver)

### "Progress not saving"
- `MarkLevelComplete()` must be called **before** loading next scene
- Check Unity logs for "✅ Level completed" message
- PlayerPrefs data location: Windows Registry (`HKEY_CURRENT_USER\Software\[company]\[product]`)

### "All buttons locked"
- First level is auto-unlocked. If locked, check:
  - `GameProgressManager.allLevels[0]` exists and is valid
  - No errors in Unity console preventing script execution

---

## 🚀 Next Steps

1. **Add more levels**: Create new scenes, add names to `allLevels` list
2. **Style buttons**: Customize prefab colors, fonts, layout
3. **Add achievements**: Track stats in GameProgressManager (enemies killed, time, deaths)
4. **Boss rush mode**: Unlock special mode after beating all levels
5. **Difficulty settings**: Separate progress tracks per difficulty

---

## 📞 Support

If you encounter issues:
1. Check Unity Console for error messages
2. Verify all scripts are assigned in inspector (no missing references)
3. Test in a fresh build (not just editor Play mode)
4. Clear PlayerPrefs: `PlayerPrefs.DeleteAll()` in Unity > Tools > Clear All Player Prefs

Happy coding! 🎮✨
