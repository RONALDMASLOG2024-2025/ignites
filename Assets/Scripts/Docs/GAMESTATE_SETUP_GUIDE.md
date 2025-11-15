# GameState Setup Guide

## Overview
The `GameState` singleton centralizes pause, game-over, and victory state management across your Unity project. All systems (UI, AI, music, player) now query `GameState.Instance` instead of checking `Time.timeScale` directly.

---

## How GameState Works

### Core Properties
- `IsPaused` - true when the game is paused (Escape key pressed)
- `IsGameOver` - true when the player dies
- `IsVictory` - true when the boss is defeated
- `IsGameEnded` - shortcut for `IsGameOver || IsVictory`

### Core Methods
- `Pause()` - pauses the game (sets `Time.timeScale = 0f`)
- `Resume()` - resumes the game (sets `Time.timeScale = 1f`)
- `SetGameOver()` - marks game as over (broadcasts to all subscribers)
- `SetVictory()` - marks game as won (broadcasts to all subscribers)
- `ResetState()` - resets all flags to default (used on scene restart/load)

### Events (for graceful shutdown)
- `OnPauseChanged` - fired when pause state changes (parameter: isPaused)
- `OnGameOver` - fired when game over is triggered
- `OnVictory` - fired when victory is triggered
- `OnGameReset` - fired when state is reset (scene reload, quit, next level)

---

## Setup Instructions

### Option 1: Automatic Creation (Recommended)
The `UIManager` automatically creates a `GameState` GameObject if one doesn't exist at scene start. **No manual setup required** if you already have a `UIManager` in your scene.

**How it works:**
```csharp
// In UIManager.Start()
if (GameState.Instance == null)
{
    new GameObject("GameState").AddComponent<GameState>();
}
```

### Option 2: Manual Creation (for scenes without UIManager)
1. In the Unity Hierarchy, right-click and select **Create Empty**.
2. Rename it to `GameState`.
3. In the Inspector, click **Add Component** and search for `GameState`.
4. The GameState component will automatically persist across scenes (`DontDestroyOnLoad`).

### Option 3: Create a Prefab (for easy reuse across scenes)
1. Follow Option 2 to create a GameState GameObject.
2. Drag the GameObject from the Hierarchy into your `Assets/Resources` or `Assets/Prefabs` folder.
3. Now you can drag the prefab into any new scene you create.

---

## Systems That Use GameState

### 1. **UIManager** (`UIManager.cs`)
- Calls `GameState.Pause()` / `Resume()` when player presses Escape
- Calls `GameState.SetGameOver()` / `SetVictory()` when showing end panels
- Blocks Escape toggling when `GameState.IsGameEnded == true`
- Calls `GameState.ResetState()` on Restart / Quit / Next Level

### 2. **Player** (`PlayerMovement.cs`, `PlayerHealth.cs`)
- `PlayerMovement` checks `GameState.IsPaused` to block input/movement
- `PlayerHealth` calls `GameState.SetGameOver()` when player dies

### 3. **Enemies & Boss** (`Enemy_Movement.cs`, `Enemy_Combat.cs`, `Boss_Behavior.cs`)
- All check `GameState.IsPaused` or `GameState.IsGameEnded` to stop AI/attacks
- Boss behavior (phase changes, minion summoning) is paused when `IsPaused == true`

### 4. **Music** (`MusicManager.cs`)
- Subscribes to `GameState.OnGameOver` and `OnVictory` events
- Automatically fades out music when game ends (graceful shutdown)
- No longer requires manual `StopMusic()` calls in other scripts

---

## Event Subscription Example

If you want other systems (animations, VFX, save managers, etc.) to react to game-end events, subscribe like this:

```csharp
using UnityEngine;

public class MyCustomSystem : MonoBehaviour
{
    private void Start()
    {
        // Subscribe to GameState events
        if (GameState.Instance != null)
        {
            GameState.Instance.OnGameOver += HandleGameOver;
            GameState.Instance.OnVictory += HandleVictory;
            GameState.Instance.OnPauseChanged += HandlePauseChanged;
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (GameState.Instance != null)
        {
            GameState.Instance.OnGameOver -= HandleGameOver;
            GameState.Instance.OnVictory -= HandleVictory;
            GameState.Instance.OnPauseChanged -= HandlePauseChanged;
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over! Saving player stats...");
        // Stop animations, save progress, disable colliders, etc.
    }

    private void HandleVictory()
    {
        Debug.Log("Victory! Unlocking next level...");
        // Play victory VFX, award achievements, etc.
    }

    private void HandlePauseChanged(bool isPaused)
    {
        Debug.Log($"Pause state changed: {isPaused}");
        // Pause particle systems, show/hide debug UI, etc.
    }
}
```

---

## Testing Checklist

1. **Pause/Resume**
   - Press Escape during gameplay → Pause panel appears, enemies stop moving
   - Press Escape again → Resume, enemies continue moving

2. **Game Over**
   - Kill the player → Game Over panel appears, Escape does nothing, enemies stop attacking

3. **Victory**
   - Defeat boss → Victory panel appears, Escape does nothing, boss stops moving

4. **Scene Transitions**
   - Restart from Game Over → GameState resets, pause works again
   - Next Level from Victory → GameState resets, new scene plays normally
   - Quit to Menu → GameState resets, menu music plays

---

## Troubleshooting

### "GameState.Instance is null"
- Make sure `UIManager` exists in your scene and calls `Start()` before other scripts.
- Or manually create a GameState GameObject (see Option 2 above).

### "Enemies still moving after Game Over"
- Ensure all enemy scripts check `GameState.IsPaused` or `IsGameEnded` in their `Update()` loops.
- Check the Console for any errors that might be stopping GameState from initializing.

### "Music doesn't stop on Game Over"
- Ensure `MusicManager` subscribes to `GameState.OnGameOver` in its `Awake()` method.
- Check that `MusicManager.HandleGameOver()` calls `StopMusic()`.

### "Escape still works during Game Over"
- Verify `UIManager.Update()` checks `!GameState.Instance.IsGameEnded` before calling `TogglePause()`.

---

## Benefits of GameState

✅ **Single source of truth** - all systems query one place for pause/end state  
✅ **Cleaner code** - no manual `Time.timeScale = 0f` scattered everywhere  
✅ **Event-driven** - systems subscribe to events instead of polling or direct calls  
✅ **Easier debugging** - set breakpoints in GameState methods to see when/why state changes  
✅ **Graceful shutdown** - music, animations, and AI can clean up properly on game end  

---

## Additional Notes

- `GameState` uses `DontDestroyOnLoad`, so it persists across scene changes (like `MusicManager`).
- If you have multiple scenes, only one `GameState` will exist at a time (singleton pattern).
- Time.timeScale is **still managed by GameState** for physics/animations, so normal Unity behavior remains intact.

---

**That's it!** Your game now has centralized state management. 🎮
