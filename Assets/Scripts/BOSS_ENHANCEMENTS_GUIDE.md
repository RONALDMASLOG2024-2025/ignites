# Boss System - Optional Enhancements Setup Guide

This guide covers implementing all optional enhancements (A, B, C, D) from the boss system.

---

## 🎯 What You're Getting

✅ **A. Boss Entrance Sequence** - Camera focuses on boss spawn, plays boss music, then returns to player  
✅ **B. Boss Health Bar** - Dynamic UI slider showing boss health  
✅ **C. Boss Phases** - Boss gets faster, changes color, and becomes more aggressive at low health  
✅ **D. Minion Summoning** - Boss spawns regular enemies during the fight  

---

## 📦 New Scripts Created

1. **Boss_Behavior.cs** - Handles phases and minion summoning
2. **BossHealthBar.cs** - UI component for boss health display
3. **Updated EnemyManager.cs** - Boss entrance with Cinemachine camera control
4. **Updated MusicManager.cs** - Supports boss music switching
5. **Updated Enemy_Health.cs** - Notifies manager when boss dies

---

## 🎬 A. Boss Entrance with Cinemachine 3.x

### Step 1: Setup Cinemachine Cameras

**Player Camera (should already exist):**
1. In Hierarchy, find your existing Cinemachine camera that follows the player
2. Select it and note its name (e.g., `CM vcam1` or `PlayerFollowCamera`)
3. This camera should have:
   - **Follow**: Player GameObject
   - **Look At**: Player GameObject (optional)

**Boss Entrance Camera (new):**
1. In Hierarchy: `GameObject > Cinemachine > Cinemachine Camera`
2. Rename it to `BossEntranceCamera`
3. Position it to view the boss spawn point:
   - In Scene view, position the camera to frame the spawn point nicely
   - Or set Follow/Look At to the `BossSpawnPoint` GameObject
4. Set **Priority**: 10 (same as player camera, they'll switch via enable/disable)
5. **Disable the GameObject** (click the checkbox next to the name in Inspector) - it should only activate during boss entrance

### Step 2: Link Cameras to EnemyManager

1. Select `EnemyManager` in Hierarchy
2. In Inspector, find the **Camera - Cinemachine 3.x** section:
   - **Player Camera**: Drag your player-following camera here
   - **Boss Entrance Camera**: Drag the `BossEntranceCamera` here
   - **Boss Entrance Duration**: Set to 3 seconds (or adjust to taste)

### Step 3: Setup Boss Music (Optional)

1. Select `MusicManager` GameObject in Hierarchy
2. In Inspector:
   - **Normal Music**: Drag your regular gameplay music AudioClip
   - **Boss Music**: Drag your boss fight music AudioClip
3. The script will automatically add an AudioSource if missing

### Step 4: Test Camera Sequence

- Press Play
- Kill all regular enemies
- Watch the sequence:
  1. Camera switches to boss spawn point
  2. Boss spawns after 0.5s
  3. Boss music starts playing
  4. After 3 seconds, camera returns to player
  5. Fight begins!

**Note:** The camera automatically returns to the player after the entrance duration. No manual intervention needed.

---

## 💉 B. Boss Health Bar UI

### Step 1: Create Health Bar UI

1. **Create Canvas** (if you don't have one already):
   - `GameObject > UI > Canvas`
   - Set Canvas Scaler to "Scale With Screen Size" (optional, for responsive UI)

2. **Create Boss Health Panel:**
   - Right-click Canvas: `UI > Panel`
   - Rename to `BossHealthPanel`
   - Position at top-center of screen:
     - Anchor: Top-center
     - Pos Y: -50 to -100 (adjust to taste)
     - Width: 400-600, Height: 80-100

3. **Add Slider:**
   - Right-click `BossHealthPanel`: `UI > Slider`
   - Rename to `BossHealthSlider`
   - Stretch it to fill the panel
   - Style the slider:
     - Background: Dark red or black
     - Fill: Bright red or orange
     - Handle: Can disable/hide (set Handle Slide Area height to 0)

4. **Add Boss Name Text:**
   - Right-click `BossHealthPanel`: `UI > Text - TextMeshPro`
   - Rename to `BossNameText`
   - Position above the slider
   - Text: "Boss" (placeholder)
   - Font Size: 24-32, Color: White, Alignment: Center

5. **Add Health Numbers Text (Optional):**
   - Right-click `BossHealthPanel`: `UI > Text - TextMeshPro`
   - Rename to `HealthNumbersText`
   - Position on the slider or below it
   - Font Size: 18-24, Color: White
   - Text: "100 / 200" (placeholder)

### Step 2: Add BossHealthBar Script

1. Select `BossHealthPanel`
2. `Add Component > BossHealthBar`
3. In Inspector:
   - **Health Slider**: Drag the `BossHealthSlider` here
   - **Boss Name Text**: Drag `BossNameText` here
   - **Health Text**: Drag `HealthNumbersText` here (optional)
   - **Boss Name**: Type the boss's name (e.g., "Goblin King")
   - **Show Health Numbers**: Check if you want "100/200" format

### Step 3: Link to EnemyManager

1. Select `EnemyManager`
2. In Inspector, find **UI** section:
   - **Boss Health Bar**: Drag the `BossHealthPanel` GameObject here

The health bar will automatically show when the boss spawns and hide when defeated!

---

## ⚡ C. Boss Phases System

### Step 1: Add Boss_Behavior to Boss Prefab

1. Open `Assets/Prefabs/` and select your Boss prefab
2. `Add Component > Boss_Behavior`

### Step 2: Configure Phase Settings

In the Inspector (Boss_Behavior component):

**Phase System:**
- **Phase 2 Threshold**: 50 (triggers at 50% health)
- **Phase 3 Threshold**: 25 (triggers at 25% health)

**Phase 2 Bonuses:**
- **Phase 2 Speed Multiplier**: 1.5 (50% faster movement)
- **Phase 2 Color**: Yellow (or choose another color)

**Phase 3 Bonuses:**
- **Phase 3 Speed Multiplier**: 2.0 (2x original speed)
- **Phase 3 Color**: Red (enraged!)
- **Phase 3 Attack Cooldown Multiplier**: 0.7 (30% faster attacks)

### What Happens in Each Phase:

**Phase 1 (100% - 51% health):**
- Normal boss behavior
- Original speed and color

**Phase 2 (50% - 26% health):**
- Boss turns yellow
- Speed increases by 50%
- Summons first wave of minions immediately
- Continues summoning minions every 15 seconds

**Phase 3 (25% - 0% health):**
- Boss turns red (enraged)
- Speed doubles from original
- Attacks faster
- Summons minions more frequently (every 7.5 seconds)

### Step 3: Test Phases

- Spawn the boss
- Deal damage and watch for phase transitions:
  - At 50% health: "🔥 BOSS ENTERED PHASE 2!" in Console
  - Boss turns yellow, speeds up, summons minions
  - At 25% health: "⚡ BOSS ENTERED PHASE 3 - ENRAGED!" in Console
  - Boss turns red, speeds up more

---

## 👾 D. Minion Summoning

### Step 1: Create Minion Spawn Points

1. **Create Empty GameObjects** for spawn points:
   - In Hierarchy: `GameObject > Create Empty`
   - Rename to `MinionSpawnPoint1`
   - Position it near the boss arena (where you want minions to appear)
   - Duplicate it (Ctrl+D) to create `MinionSpawnPoint2`, `MinionSpawnPoint3`, etc.
   - Spread them around the arena (4-6 spawn points recommended)

2. **Optional: Parent them under the Boss:**
   - Create an empty GameObject named `BossArena`
   - Put all spawn points inside it for organization

### Step 2: Configure Boss_Behavior for Summoning

1. Select your Boss prefab
2. In Boss_Behavior component:
   - **Minion Prefab**: Drag your regular enemy prefab from `Assets/Prefabs/`
   - **Minion Spawn Points**: Set size to match number of spawn points (e.g., 4)
   - Drag each spawn point GameObject into the array slots
   - **Minions Per Summon**: 2 (boss spawns 2 minions at a time)
   - **Summon Cooldown**: 15 seconds (time between summon waves)

### How Summoning Works:

- **Phase 1**: No summoning
- **Phase 2 starts**: Summons minions immediately
- **Phase 2/3**: Summons minions every 15 seconds (7.5 in Phase 3)
- Minions spawn at random spawn points
- Summoned minions have **half health** of normal enemies (to avoid overwhelming player)

### Step 3: Test Minion Summoning

- Fight the boss to 50% health
- Watch for: "Boss summoning 2 minions!" in Console
- Minions should appear at spawn points
- They behave like normal enemies (register with EnemyManager, can be killed)

---

## 🎮 Complete Setup Checklist

### EnemyManager Setup:
- [ ] Player Camera linked
- [ ] Boss Entrance Camera created and linked
- [ ] Boss Entrance Duration set (3 seconds recommended)
- [ ] Boss Health Bar linked
- [ ] Enemy Count Text linked
- [ ] Boss Prefab linked
- [ ] Boss Spawn Point linked

### Boss Prefab Setup:
- [ ] Enemy_Health component: `isBoss = true`
- [ ] Boss_Behavior component added
- [ ] Phase thresholds configured
- [ ] Minion prefab linked
- [ ] Minion spawn points created and linked
- [ ] Increased stats (health: 100+, damage: 2-3, scale: 1.5-2x)

### UI Setup:
- [ ] Boss Health Panel created with Slider
- [ ] BossHealthBar script attached
- [ ] Boss Name Text created
- [ ] Health Numbers Text created (optional)
- [ ] Panel linked to EnemyManager

### Music Setup:
- [ ] MusicManager has AudioSource
- [ ] Normal music AudioClip assigned
- [ ] Boss music AudioClip assigned

### Cinemachine Setup:
- [ ] Player follow camera exists and follows player
- [ ] Boss entrance camera positioned at spawn point
- [ ] Boss entrance camera disabled by default
- [ ] Both cameras linked to EnemyManager

---

## 🐛 Troubleshooting

### Camera doesn't switch to boss
- Check both cameras are assigned in EnemyManager
- Ensure Boss Entrance Camera is disabled at start
- Verify camera priorities are equal (both 10)
- Check for "Camera focusing on boss spawn point..." in Console

### Camera stuck on boss
- Default entrance duration is 3 seconds - increase if needed
- Check "Camera returned to player." appears in Console
- Ensure Player Camera GameObject is active and enabled

### Boss health bar doesn't show
- Verify BossHealthBar is linked in EnemyManager
- Check Console for errors about missing Slider component
- Ensure the boss has Enemy_Health component with `isBoss = true`

### Phases don't trigger
- Check Boss_Behavior is attached to boss prefab
- Verify phase thresholds (50% and 25%)
- Watch Console for phase transition messages
- Ensure boss has Enemy_Health component

### Minions don't spawn
- Check Minion Prefab is assigned in Boss_Behavior
- Verify spawn points exist and are assigned
- Check "Boss summoning X minions!" in Console
- Ensure boss reaches Phase 2 (50% health)

### Music doesn't change
- Verify MusicManager exists in scene
- Check music AudioClips are assigned
- Ensure MusicManager has AudioSource
- Look for "Playing BOSS music!" in Console

---

## 🎨 Optional Customizations

### Custom Phase Behaviors

Edit `Boss_Behavior.cs` to add more phase effects:

```csharp
private void EnterPhase2()
{
    // ... existing code ...
    
    // Example: Increase attack range
    Enemy_Combat combat = GetComponent<Enemy_Combat>();
    if (combat != null)
    {
        combat.weaponRange *= 1.5f;
    }
    
    // Example: Change attack damage
    combat.damage += 1;
}
```

### Boss-Specific Attacks

Add special attack methods to `Boss_Behavior.cs`:

```csharp
public void SpecialAttack()
{
    Debug.Log("Boss uses special attack!");
    // AOE damage, projectiles, etc.
}
```

Call from animation events or on a timer.

### Victory Sequence

Add to `EnemyManager.OnBossDefeated()`:

```csharp
public void OnBossDefeated()
{
    Debug.Log("Boss has been defeated!");
    
    // Play victory music
    if (MusicManager.Instance != null)
    {
        MusicManager.Instance.PlayNormalMusic();
    }
    
    // Show victory UI
    // victoryPanel.SetActive(true);
    
    // Unlock next level
    // LevelManager.UnlockNextLevel();
}
```

---

## Summary

You now have a complete boss system with:
- 🎬 Cinematic entrance with Cinemachine camera
- 💉 Dynamic health bar UI
- ⚡ Three-phase boss fight with increasing difficulty
- 👾 Minion summoning mechanic
- 🎵 Music transitions

The camera automatically handles transitions: focuses on boss entrance, then returns to player after 3 seconds. No manual camera control needed!

Test the full sequence and adjust timings, colors, and stats to match your game's feel. Have fun!
