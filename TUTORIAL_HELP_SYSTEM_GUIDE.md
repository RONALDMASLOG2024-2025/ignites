# 🎮 Tutorial & Help System - Implementation Guide

## 📚 Overview

This guide will help you add:
1. **Auto-showing Tutorial Panel** - Appears at level start, fades after 5 seconds
2. **Help Button in Pause Menu** - Shows detailed instructions anytime
3. **Skip functionality** - Players can press any key to skip tutorial

---

## 🛠️ PHASE 1: Create the Tutorial Panel UI

### Step 1: Create Tutorial Panel GameObject

1. **Open Level1 scene** in Unity
2. **In Hierarchy**, right-click → **UI** → **Panel**
3. **Rename** it to `TutorialPanel`
4. **Set position**: 
   - Anchor: Center-Center
   - Position: (0, 0, 0)
   - Width: 800, Height: 600

### Step 2: Style the Tutorial Panel

1. **Select `TutorialPanel`**
2. **In Inspector**, find **Image component**:
   - Color: Black with alpha ~180 (semi-transparent)
   - Or use a custom background sprite

3. **Add CanvasGroup component**:
   - Click **Add Component** → **Canvas Group**
   - This enables smooth fading

### Step 3: Add Tutorial Content (Text)

1. **Right-click `TutorialPanel`** → **UI** → **Text - TextMeshPro** (or regular Text)
2. **Rename** to `TitleText`
3. **Set properties**:
   - Text: "HOW TO PLAY"
   - Font Size: 48
   - Alignment: Center-Top
   - Position: Top of panel
   - Color: White or Yellow

4. **Create another Text** (right-click `TutorialPanel` → UI → Text - TextMeshPro)
5. **Rename** to `ControlsText`
6. **Set text content**:

```
CONTROLS
━━━━━━━━━━━━━━━━━━━━━━━━━━
🖱️ Left Click - Primary Attack
🖱️ Right Click - Secondary Attack
⌨️ WASD / Arrow Keys - Movement
⌨️ ESC - Pause Menu

COMBAT
━━━━━━━━━━━━━━━━━━━━━━━━━━
⚔️ Defeat all enemies to save the village
👑 Face the boss to complete the level
🍖 Collect meat to restore health
🦴 Collect bones to increase damage & speed

Press any key to continue...
```

7. **Set properties**:
   - Font Size: 24-28
   - Alignment: Left
   - Position: Center of panel
   - Color: White

### Step 4: Add the TutorialPanel Script

1. **Select `TutorialPanel`** in Hierarchy
2. **In Inspector**, click **Add Component**
3. **Search** for `TutorialPanel` script (we created this)
4. **Configure settings**:
   - Display Duration: `5` (shows for 5 seconds)
   - Fade Duration: `1` (fades over 1 second)
   - Allow Skip: ✅ Checked
   - Show Only Once: ✅ Checked
   - Tutorial Key: `Level1_Tutorial_Shown`

5. **Canvas Group** field should auto-populate. If not, drag the CanvasGroup component into it.

---

## 🛠️ PHASE 2: Create the Help Panel UI

### Step 1: Create Help Panel GameObject

1. **In Hierarchy**, right-click **Canvas** → **UI** → **Panel**
2. **Rename** it to `HelpPanel`
3. **Set position**:
   - Anchor: Stretch-Stretch (full screen)
   - Offsets: All zeros

### Step 2: Style the Help Panel

1. **Select `HelpPanel`**
2. **Image component**:
   - Color: Dark gray/black with alpha ~200

### Step 3: Add Help Content with Scroll View

1. **Right-click `HelpPanel`** → **UI** → **Scroll View**
2. **Position it** in the center with margins
3. **Inside Scroll View → Viewport → Content**, add multiple Text elements:

**Title Text:**
```
GAME GUIDE
```

**Controls Section:**
```
━━━━━━━━ CONTROLS ━━━━━━━━
🖱️ LEFT MOUSE - Primary Attack (Fast)
🖱️ RIGHT MOUSE - Secondary Attack (Strong)
⌨️ W/↑ - Move Up
⌨️ S/↓ - Move Down
⌨️ A/← - Move Left
⌨️ D/→ - Move Right
⌨️ ESC - Pause Menu
```

**Combat Section:**
```
━━━━━━━━ COMBAT ━━━━━━━━
⚔️ Use combos of attacks to defeat enemies
🛡️ Watch enemy patterns and dodge attacks
⚡ Attacks have cooldowns - time them well
💥 Knockback enemies to create space
```

**Power-Ups Section:**
```
━━━━━━━ POWER-UPS ━━━━━━━
🍖 MEAT - Restores health when picked up
🦴 BONE - Permanently increases:
   • Damage (up to max 20)
   • Movement Speed (up to max 15)
```

**Objectives Section:**
```
━━━━━━━ OBJECTIVES ━━━━━━━
🏘️ Save villages from monster hordes
⚔️ Defeat all enemies in each level
👑 Battle powerful bosses
🌟 Collect power-ups to grow stronger
🏆 Complete all levels to save Acclia
```

**Tips Section:**
```
━━━━━━━━━ TIPS ━━━━━━━━━
💡 Keep moving to avoid enemy attacks
💡 Use knockback to separate enemy groups
💡 Collect bones early for easier fights
💡 Save strong attacks for bosses
💡 Don't let enemies surround you
```

### Step 4: Add Close/Back Button

1. **Right-click `HelpPanel`** → **UI** → **Button**
2. **Rename** to `BackButton`
3. **Position** at top-right or bottom-center
4. **Button Text**: "BACK" or "CLOSE" or "×"
5. **Style**: 
   - Color: Red or theme color
   - Font Size: 32

### Step 5: Add the HelpPanel Script

1. **Select `HelpPanel`** in Hierarchy
2. **Add Component** → `HelpPanel` script
3. **Configure**:
   - Pause Menu Panel: Drag your `PausePanel` here (we'll connect this next)

4. **Select `BackButton`**
5. **In Button component** → **On Click ()**:
   - Click **+** to add event
   - Drag `HelpPanel` into object field
   - Select `HelpPanel.OnBackButton`

---

## 🛠️ PHASE 3: Add Help Button to Pause Menu

### Step 1: Add Help Button to Pause Panel

1. **Open your `PausePanel`** (or pause menu UI)
2. **Right-click PausePanel** → **UI** → **Button**
3. **Rename** to `HelpButton`
4. **Position** it below Resume/Restart buttons
5. **Button Text**: "HELP" or "HOW TO PLAY"
6. **Style** to match other buttons

### Step 2: Connect Help Button to HelpPanel

1. **Select `HelpButton`**
2. **In Button component** → **On Click ()**:
   - Click **+**
   - Drag `HelpPanel` into object field
   - Select `HelpPanel.ShowHelp`

### Step 3: Connect HelpPanel back to PausePanel

1. **Select `HelpPanel`**
2. **In HelpPanel script component**:
   - **Pause Menu Panel**: Drag `PausePanel` here

---

## 🛠️ PHASE 4: Testing & Polish

### Testing Checklist

**Tutorial Panel:**
- ✅ Appears automatically when level starts
- ✅ Fades out after 5 seconds
- ✅ Can be skipped by pressing any key
- ✅ Only shows once (on first playthrough)

**Help Panel:**
- ✅ Opens from pause menu Help button
- ✅ Hides pause menu when showing
- ✅ Back button returns to pause menu
- ✅ Content is readable and scrollable

### Step-by-Step Test:

1. **Enter Play Mode**
2. **Wait and watch** - Tutorial should appear and fade
3. **Press ESC** - Pause menu appears
4. **Click HELP** - Help panel shows, pause menu hides
5. **Click BACK** - Returns to pause menu
6. **Resume game**
7. **Reload scene** - Tutorial should NOT appear again (showOnlyOnce)

---

## 🎨 OPTIONAL: Advanced Customization

### Make Tutorial Different Per Level

1. **Duplicate `TutorialPanel`** for Level2, Level3, etc.
2. **Change text content** for each level:
   - Level1: Basic controls
   - Level2: Boss mechanics
   - Level3: Advanced tactics

3. **Change Tutorial Key** for each:
   - Level1: `Level1_Tutorial_Shown`
   - Level2: `Level2_Tutorial_Shown`
   - etc.

### Add Tutorial Toggle in Settings

1. **In Main Menu**, add a settings panel
2. **Add toggle**: "Show Tutorials"
3. **Save to PlayerPrefs**: `ShowTutorials`
4. **Check in TutorialPanel.Start()**:

```csharp
bool showTutorials = PlayerPrefs.GetInt("ShowTutorials", 1) == 1;
if (!showTutorials)
{
    gameObject.SetActive(false);
    return;
}
```

### Add Animated Tutorial

1. **Add Animator component** to `TutorialPanel`
2. **Create animation**:
   - Fade in from bottom
   - Scale from 0.8 to 1.0
   - Fade out with scale

---

## 🐛 Troubleshooting

### Tutorial doesn't appear:
- Check if `TutorialPanel` is active in Hierarchy
- Check if `showOnlyOnce` is enabled and key was saved
- Reset PlayerPrefs: `PlayerPrefs.DeleteAll()` in Unity console

### Tutorial doesn't fade:
- Ensure `CanvasGroup` component is attached
- Check if `canvasGroup` reference is set in script

### Help button does nothing:
- Check if `HelpPanel` reference is set in button's OnClick()
- Check if `HelpPanel` script is attached to panel
- Check console for errors

### Text is cut off:
- Increase panel size
- Use Scroll View for help panel
- Adjust text size/margins

---

## 📝 Summary

**Created Files:**
- ✅ `TutorialPanel.cs` - Auto-fading tutorial at level start
- ✅ `HelpPanel.cs` - Pause menu help system

**UI Hierarchy:**
```
Canvas
├── TutorialPanel (appears at start, fades out)
│   ├── TitleText
│   └── ControlsText
│
├── PausePanel
│   ├── ResumeButton
│   ├── RestartButton
│   ├── HelpButton (NEW - opens HelpPanel)
│   └── MainMenuButton
│
└── HelpPanel (hidden by default)
    ├── ScrollView
    │   └── Content (all help text)
    └── BackButton
```

**Player Experience:**
1. 🎮 Level starts → Tutorial appears → Auto-fades after 5s
2. ⏸️ Press ESC → Pause menu → Click Help → Detailed instructions
3. 📖 Read help → Click Back → Resume playing

---

## 🎯 Next Steps

1. **Copy this setup** to Level2, Level3, etc.
2. **Customize tutorial text** for each level's unique mechanics
3. **Add tutorial images/icons** for visual appeal
4. **Test with new players** to ensure clarity
5. **Consider adding tooltips** for first-time actions

**Your game now has a complete tutorial system!** 🎉
