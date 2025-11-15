# Music Not Playing - Troubleshooting Guide

## ❌ Problem: SFX works, but background music doesn't play

---

## ✅ Solution Checklist

### 1. Check MusicManager Has Audio Clips Assigned

**In MainMenu scene:**
1. Select `MusicManager` GameObject in Hierarchy
2. Look at Inspector → `MusicManager` script component
3. **Verify these fields are NOT "None (Audio Clip)":**
   - **Normal Music** ← Must have a clip assigned for gameplay levels!
   - **Boss Music** ← Must have a clip for boss fights
   - **Menu Music** ← Must have a clip for main menu
   - **Victory Music** ← Optional victory fanfare

**If all say "None (Audio Clip)"** → That's the problem! You need to assign music files.

---

### 2. Find or Import Music Files

#### Option A: Use Existing Audio Files
1. Project window → Search: `type:AudioClip`
2. Look in `Assets/Audio/` folder
3. Find music files (usually longer clips, like 30-120 seconds)
4. Drag to MusicManager fields in Inspector

#### Option B: Import New Music
1. Get royalty-free music from:
   - [OpenGameArt.org](https://opengameart.org/)
   - [Freesound.org](https://freesound.org/)
   - Your own compositions
2. Drag `.mp3` or `.wav` files into `Assets/Audio/Music/` folder
3. Select imported file → Inspector:
   - **Load Type**: Streaming (for long music)
   - **Preload Audio Data**: ✓ Checked
   - **Compression Format**: Vorbis (smaller file size)
4. Drag to MusicManager fields

---

### 3. Verify AudioSource Component

Select `MusicManager` GameObject:

**Should have an `AudioSource` component with:**
- **AudioClip**: (None) - This is OK, script sets it dynamically
- **Output**: Master (or your Audio Mixer)
- **Mute**: ✗ Unchecked
- **Bypass Effects**: ✗ Unchecked
- **Play On Awake**: ✗ Unchecked (script controls playback)
- **Loop**: ✓ Checked (script overrides per track)
- **Priority**: 128 (default)
- **Volume**: **1.0** ← Make sure this isn't 0!
- **Pitch**: 1.0
- **Spatial Blend**: 0 (2D sound)

**If AudioSource is missing:**
- The script auto-creates it in `Awake()`, but verify it exists after entering Play mode

---

### 4. Check Scene Names Match

Open `MusicManager` script in Inspector:

**Game Scenes array should show:**
```
Element 0: "Level1"
Element 1: "Level2"  
Element 2: "BossLevel"
```

**Menu Scenes array should show:**
```
Element 0: "MainMenu"
```

**If it shows old names like "MainGame":**
- The fix didn't apply yet
- Restart Unity Editor
- Or manually change in Inspector

---

### 5. Test in Play Mode

**Start from MainMenu scene:**

1. **Press Play** in Unity Editor
2. **Check Console** for these logs:
   ```
   MusicManager: Scene 'MainMenu' recognized. Playing menu music.
   ```
3. **Open Hierarchy** → Find "DontDestroyOnLoad" section
4. **Select MusicManager** → Inspector:
   - **AudioSource** → Playing: ✓ (checkmark should show)
   - **AudioSource.volume**: Should be animating (fading in)
   - **AudioSource.clip**: Should show your menu music name

5. **Click Play button** to enter Level1
6. **Check Console** for:
   ```
   MusicManager: Scene 'Level1' recognized. Playing normal music.
   ```
7. **AudioSource should switch to normalMusic clip**

---

### 6. Common Issues & Fixes

#### Issue: "Music clips are assigned but still no sound"

**Check Unity Editor Volume:**
- Top menu: `Edit > Preferences > Audio`
- System Master Volume: Should be > 0

**Check Windows Volume Mixer:**
- Right-click speaker icon → Open Volume Mixer
- Make sure Unity Editor volume isn't muted

**Check Audio Listener:**
- Your Main Camera should have `Audio Listener` component
- Only ONE Audio Listener allowed in scene
- If multiple exist → You'll hear nothing!

#### Issue: "Console shows 'Scene not configured'"

**Log message:**
```
MusicManager: Scene 'Level1' not configured. Stopping music.
```

**Fix:**
- Select MusicManager → Inspector
- Expand "Game Scenes" array
- Verify "Level1" is spelled exactly right (case-sensitive!)
- Click Apply

#### Issue: "Music plays but cuts out immediately"

**Possible causes:**
1. **Audio clip is too short** → Use longer loop or check loop settings
2. **Fade duration too long** → Reduce `fadeDuration` to 0.5 seconds
3. **Multiple MusicManagers** → Only one should exist (DontDestroyOnLoad)

#### Issue: "Music plays in MainMenu but not in Level1"

**Check:**
1. `normalMusic` clip is assigned (not just menuMusic)
2. Console shows "Playing normal music" when entering Level1
3. If console says "Stopping music" → scene name mismatch

---

### 7. Debug Test Code

**Add a test button to MainMenu:**

1. Create UI Button: "Test Music"
2. Add this script:

```csharp
using UnityEngine;

public class MusicDebugger : MonoBehaviour
{
    public void TestNormalMusic()
    {
        if (MusicManager.Instance != null)
        {
            Debug.Log("Testing normal music...");
            MusicManager.Instance.PlayNormalMusic();
        }
        else
        {
            Debug.LogError("MusicManager.Instance is NULL!");
        }
    }

    public void TestBossMusic()
    {
        if (MusicManager.Instance != null)
        {
            Debug.Log("Testing boss music...");
            MusicManager.Instance.PlayBossMusic();
        }
    }

    public void PrintMusicManagerState()
    {
        if (MusicManager.Instance != null)
        {
            var audioSource = MusicManager.Instance.GetComponent<AudioSource>();
            Debug.Log($"AudioSource exists: {audioSource != null}");
            Debug.Log($"Is Playing: {audioSource.isPlaying}");
            Debug.Log($"Volume: {audioSource.volume}");
            Debug.Log($"Current Clip: {audioSource.clip?.name ?? "None"}");
        }
    }
}
```

3. Assign button OnClick → `TestNormalMusic()`
4. Click in Play mode → Should force play music

---

## 🎵 Quick Fix Summary

**Most Common Cause:** Music clips not assigned in Inspector!

**Quick Fix:**
1. Select `MusicManager` in MainMenu scene
2. Inspector → Assign audio clips to all music fields
3. Press Play → Should hear music immediately

**If still no sound:**
- Check Audio Listener on Main Camera
- Check Console for error messages
- Verify clip file format is supported (.mp3, .wav, .ogg)
- Check AudioSource volume slider isn't at 0

---

## 📋 Final Verification

✅ MusicManager GameObject exists in MainMenu scene  
✅ MusicManager has AudioSource component  
✅ normalMusic clip assigned (for gameplay)  
✅ menuMusic clip assigned (for main menu)  
✅ Game Scenes array includes "Level1", "Level2", "BossLevel"  
✅ Menu Scenes array includes "MainMenu"  
✅ Audio Listener exists on Main Camera (only one)  
✅ Console shows "Playing X music" messages in Play mode  
✅ AudioSource.isPlaying = true when music should play  

---

**If all checked and still no music** → Share Console error messages for further debugging!
