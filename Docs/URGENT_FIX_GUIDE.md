# URGENT FIX - Level Select Not Showing & Next Level Error

## 🔥 Quick Fix Instructions

### Issue 1: Level Buttons Not Showing in MainMenu

**The `LevelSelectUI` script is added but the UI elements are not set up yet!**

#### Quick Setup (2 minutes):

1. **Open MainMenu scene**

2. **Create Button Prefab:**
   - Right-click in Hierarchy: `UI > Button - TextMeshPro`
   - Rename to: "LevelButtonPrefab"
   - Set size: Width 300, Height 80
   - Configure text (child Text TMP): Font Size 28, Center aligned
   - **Drag to Assets/Prefabs/** folder (create folder if needed)
   - **Delete from Hierarchy**

3. **Find or Create LevelSelectPanel:**
   - In Canvas Hierarchy, find `LevelSelectPanel` (or create: Right-click Canvas → UI > Panel)
   - Add child: Right-click LevelSelectPanel → `UI > Vertical Layout Group`
   - Rename child to: "ButtonContainer"

4. **Configure LevelSelectPanel:**
   - Select `LevelSelectPanel`
   - If it doesn't have `LevelSelectUI` script, add it: `Add Component > Level Select UI`
   - In Inspector:
     - **Button Container** → Drag "ButtonContainer" (the VerticalLayoutGroup child)
     - **Level Button Prefab** → Drag prefab from Assets/Prefabs/

5. **Configure ButtonContainer (VerticalLayoutGroup):**
   - Padding: Top/Bottom 20, Left/Right 50
   - Spacing: 20
   - Child Alignment: Middle Center
   - Control Child Size: Width ✓, Height ✓

6. **Press Play** → Should see 3 level buttons appear!

---

### Issue 2: Next Level Button Error/Not Working

**Fixed in code! But you need to set this in Inspector:**

#### For EACH Level Scene:

**Level1 (formerly MainGame):**
1. Open `Level1.unity` scene
2. Find `UIManager` GameObject in Hierarchy
3. Inspector → `UIManager` script
4. **Next Level Scene Name** field → Type: `Level2`
5. **Victory Panel** → Make sure it's assigned
6. Save scene (Ctrl+S)

**Level2 (if you have it):**
1. Open `Level2.unity` scene
2. Find `UIManager` GameObject
3. **Next Level Scene Name** → Type: `BossLevel`
4. **Victory Panel** → Make sure it's assigned
5. Save scene

**BossLevel (final level):**
1. Open `BossLevel.unity` scene
2. Find `UIManager` GameObject
3. **Next Level Scene Name** → **LEAVE EMPTY** (this triggers final victory)
4. **Final Victory Panel** → Make sure it's assigned
5. Save scene

---

## 🧪 Test After Setup

### Test Level Select:
1. Play MainMenu scene
2. **Check Console** for:
   ```
   LevelSelectUI: Starting initialization...
   LevelSelectUI: Generating 3 level buttons...
   LevelSelectUI: Successfully created 3 level buttons!
   ```
3. Should see:
   - ✅ Level 1 button (white, clickable)
   - 🔒 Level 2 button (gray, locked)
   - 🔒 Boss Level button (gray, locked)

### Test Next Level:
1. Play Level1
2. Beat all enemies
3. Victory panel appears
4. **Check Console** for:
   ```
   Next Level button clicked!
   ✅ Level 'Level1' marked as completed. Next level 'Level2' unlocked.
   Loading next level: Level2
   ```
5. Level2 should load

### Test Progress Saving:
1. Beat Level1
2. Click Next Level → Goes to Level2
3. Press Escape → Quit to Menu
4. Back in MainMenu, Level2 button should now be unlocked!

---

## 🐛 If Still Not Working

### Level Buttons Still Not Showing:

**Check Console for these error messages:**

```
LevelSelectUI: buttonContainer not assigned in inspector!
```
→ **Fix**: Assign ButtonContainer in LevelSelectPanel Inspector

```
LevelSelectUI: levelButtonPrefab not assigned in inspector!
```
→ **Fix**: Create and assign button prefab

```
LevelSelectUI: GameProgressManager.Instance is NULL!
```
→ **Fix**: Add GameProgressManager to MainMenu scene (or it will auto-create)

### Next Level Button Still Errors:

**Check Console for:**

```
UIManager: Next level scene name not set in inspector!
```
→ **Fix**: Set `nextLevelSceneName` in UIManager Inspector for that level

```
Scene 'Level2' couldn't be loaded because it has not been added to the build settings
```
→ **Fix**: File > Build Settings > Add Level2.unity scene

---

## 📋 Emergency Checklist

- [ ] Button prefab created and saved in Assets/Prefabs/
- [ ] LevelSelectPanel has LevelSelectUI script
- [ ] ButtonContainer (VerticalLayoutGroup) created and assigned
- [ ] Button prefab assigned to LevelSelectUI
- [ ] Level1 UIManager: nextLevelSceneName = "Level2"
- [ ] Level2 UIManager: nextLevelSceneName = "BossLevel" (or empty if no Level2)
- [ ] BossLevel UIManager: nextLevelSceneName = "" (empty)
- [ ] All scenes added to Build Settings (File > Build Settings)
- [ ] Victory panels assigned in each level's UIManager
- [ ] Pressed Play and checked Console for errors

---

## 🎯 What the Code Fixes Do

### UIManager.cs Changes:
✅ Added null checks for GameProgressManager  
✅ Added try-catch to prevent crashes  
✅ Added validation for nextLevelSceneName  
✅ Added cursor unlock when Victory panel shows  
✅ Added debug logs to track button clicks  

### LevelSelectUI.cs Changes:
✅ Added detailed debug logging  
✅ Added null check for GameProgressManager  
✅ Added error messages showing what's missing  
✅ Better error handling for missing prefab components  

**The code is now safe from crashes, but you still need to set up the UI in Unity Editor!**

---

## 🚀 After Setup Works

Once working, you can customize:
- Button colors/sizes
- Add images/icons
- Change layout (Grid instead of Vertical)
- Add progress text display
- Add level preview thumbnails

**But get the basic setup working first!**
