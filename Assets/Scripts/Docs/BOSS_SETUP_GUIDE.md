# Boss System Setup Guide

This guide shows you how to implement the "kill all enemies, then boss spawns" system in IGNITES.

---

## Overview

The system consists of:
1. **EnemyManager.cs** - Singleton that tracks enemy count and spawns the boss
2. **Enemy_Health.cs** - Modified to register/unregister with the manager
3. **UI Counter** - Shows remaining enemies using TextMeshPro
4. **Boss Enemy** - Can reuse existing enemy components or inherit from them

---

## Step 1: Unity Editor Setup

### A. Create the EnemyManager GameObject

1. In your scene Hierarchy, create an empty GameObject: `GameObject > Create Empty`
2. Rename it to `EnemyManager`
3. Add the `EnemyManager` script component to it
4. Leave it in the scene (don't make it a prefab)

### B. Set Up the UI Counter

1. Create a Canvas if you don't have one: `GameObject > UI > Canvas`
2. Inside the Canvas, create a TextMeshPro text: `Right-click Canvas > UI > Text - TextMeshPro`
   - If prompted to import TMP Essentials, click "Import TMP Essentials"
3. Rename the text object to `EnemyCountText`
4. Position it where you want (e.g., top-right corner):
   - Set Anchor to top-right
   - Adjust Pos X, Pos Y to taste (e.g., X: -100, Y: -50)
5. Style the text:
   - Font Size: 24-36
   - Color: White or red
   - Alignment: Center or Right
   - Text: "Enemies: 0" (placeholder - script will update it)

6. **Link UI to EnemyManager:**
   - Select the `EnemyManager` GameObject in Hierarchy
   - In the Inspector, find the `Enemy Count Text` field
   - Drag the `EnemyCountText` object into that field

### C. Create a Boss Spawn Point

1. In Hierarchy: `GameObject > Create Empty`
2. Rename it to `BossSpawnPoint`
3. Position it where you want the boss to appear (e.g., center of arena, or off-screen)
4. **Link to EnemyManager:**
   - Select `EnemyManager`
   - Drag `BossSpawnPoint` into the `Boss Spawn Point` field

---

## Step 2: Prepare Your Regular Enemies

### Mark Enemies as Regular (Not Bosses)

1. Select each enemy GameObject or prefab in the scene/Project
2. In the Inspector, find the `Enemy_Health` component
3. Ensure the `Is Boss` checkbox is **unchecked** (default)
4. The enemy will now automatically register with the EnemyManager on Start()

### Test Enemy Registration

- Press Play
- Check the Console: you should see "Enemy registered. Total enemies: X" for each enemy
- Kill an enemy (using player attacks)
- Console should show "Enemy killed. Remaining enemies: Y"

---

## Step 3: Create the Boss

You have **two main options** for creating a boss:

### Option A: Reuse Existing Enemy Components (Recommended for Quick Setup)

The boss can use the same scripts as regular enemies, just with different stats.

**Steps:**
1. Duplicate an existing enemy prefab or GameObject
2. Rename it to `Boss` or `Boss_Goblin`, etc.
3. Modify components:
   - **Enemy_Health:**
     - Check the `Is Boss` checkbox (so it doesn't count toward regular enemy total)
     - Increase `Max Health` (e.g., 100 instead of 20)
   - **Enemy_Movement:**
     - Increase `Speed` if you want a faster boss
     - Increase `Attack Range`
   - **Enemy_Combat:**
     - Increase `Damage` (e.g., 2-3 instead of 1)
     - Increase `Weapon Range`
     - Increase `Knockback Force`
   - **Transform:**
     - Increase `Scale` (e.g., scale to 1.5x or 2x) to make the boss larger
4. Adjust visuals:
   - Change sprite color (Sprite Renderer > Color) to red or another distinctive color
   - Or swap the sprite to a different enemy type
5. Save as a prefab: drag it into `Assets/` to create `Boss.prefab`
6. **Link to EnemyManager:**
   - Select `EnemyManager` in Hierarchy
   - Drag the `Boss` prefab into the `Boss Prefab` field

**Test:**
- Kill all regular enemies
- Console should log "All enemies defeated! Boss is spawning..."
- Boss should appear at the spawn point

---

### Option B: Create a Boss Class (Inherits from Enemy Components)

If you want more custom boss behavior (phases, special attacks, etc.), create a custom script.

**Example: Boss_Health.cs** (inherits behavior, adds phases)

```csharp
using UnityEngine;

public class Boss_Health : Enemy_Health
{
    [Header("Boss-Specific")]
    public int phase = 1;
    public int phase2Threshold = 50; // Health % to trigger phase 2

    private bool hasEnteredPhase2 = false;

    // Override the ChangeHealth method to add phase logic
    public override void ChangeHealth(int amount)
    {
        base.ChangeHealth(amount); // Call parent logic

        // Check for phase transitions
        float healthPercent = (float)currentHealth / maxHealth * 100f;

        if (!hasEnteredPhase2 && healthPercent <= phase2Threshold)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        hasEnteredPhase2 = true;
        phase = 2;
        Debug.Log("Boss entered Phase 2!");

        // Example: speed up movement
        Enemy_Movement movement = GetComponent<Enemy_Movement>();
        if (movement != null)
        {
            movement.speed *= 1.5f;
        }

        // Example: change color to red
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.red;
        }
    }
}
```

**Note:** For inheritance to work, you'll need to change `ChangeHealth()` in `Enemy_Health.cs` to `public virtual void ChangeHealth(...)` and make `currentHealth` and `maxHealth` protected instead of private. Or, duplicate the logic and customize directly.

**Alternative (simpler):** Just add a new script `Boss_Behavior.cs` to the boss GameObject that handles special logic, keeping `Enemy_Health` as-is with `isBoss = true`.

---

## Step 4: Test the Full Flow

1. Press Play in Unity Editor
2. Check the UI: should show "Enemies: X" (number of regular enemies)
3. Kill enemies one by one:
   - UI counter should decrease
   - Console logs each kill
4. When the last enemy dies:
   - Console: "All enemies defeated! Boss is spawning..."
   - Boss appears at the spawn point
   - UI shows "Enemies: 0"
5. Kill the boss:
   - Console: "Boss defeated!"
   - Boss is destroyed

---

## Step 5: Optional Enhancements

### A. Boss Entrance Animation or Cutscene

In `EnemyManager.SpawnBoss()`, you can:
- Play a camera shake
- Trigger a cutscene
- Play boss entrance music
- Show a boss health bar UI

Example:
```csharp
private void SpawnBoss()
{
    bossSpawned = true;
    Debug.Log("All enemies defeated! Boss is spawning...");

    // Play boss music
    if (MusicManager.Instance != null)
    {
        MusicManager.Instance.PlayBossMusic();
    }

    // Camera shake
    // CameraShake.Instance.Shake(0.5f, 0.3f);

    // Spawn boss after delay
    Invoke(nameof(SpawnBossDelayed), 2f);
}

private void SpawnBossDelayed()
{
    if (bossPrefab != null && bossSpawnPoint != null)
    {
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        Debug.Log($"Boss spawned at {bossSpawnPoint.position}");
    }
}
```

### B. Boss Health Bar

Create a separate UI for the boss with a health slider:

1. Create a new Canvas or panel: `GameObject > UI > Panel`
2. Add a Slider: `Right-click Panel > UI > Slider`
3. Create a script `BossHealthBar.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Enemy_Health bossHealth;

    private void Update()
    {
        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }
    }
}
```

### C. Multiple Boss Phases

Add this to `Boss_Behavior.cs`:
- Phase 1: Normal attacks
- Phase 2 (50% health): Increased speed, new attack pattern
- Phase 3 (25% health): Summon minions, enrage mode

### D. Boss Summons Minions

In `Boss_Behavior.cs`:
```csharp
public GameObject minionPrefab;
public Transform[] minionSpawnPoints;

public void SummonMinions()
{
    foreach (Transform spawnPoint in minionSpawnPoints)
    {
        Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
    }
}
```

---

## Troubleshooting

### "EnemyManager.Instance is null"
- Make sure `EnemyManager` GameObject exists in the scene
- Check Console for "Multiple EnemyManagers detected" warning
- Ensure enemies spawn **after** EnemyManager's `Awake()` runs (they should if placed in scene)

### Boss doesn't spawn
- Check EnemyManager Inspector: is `Boss Prefab` assigned?
- Is `Boss Spawn Point` assigned?
- Check Console for "Boss spawn skipped" warning
- Ensure `remainingEnemies` actually reaches 0 (add Debug.Log in `UnregisterEnemy`)

### UI shows wrong count
- Make sure `Enemy Count Text` field is assigned in EnemyManager
- Check that all regular enemies have `Is Boss` unchecked
- Verify TextMeshPro is imported (TMP Essentials)

### Boss counts as regular enemy
- Select boss prefab/GameObject
- In `Enemy_Health` component, check the `Is Boss` checkbox

---

## Summary Checklist

- [ ] `EnemyManager` GameObject exists in scene
- [ ] `EnemyManager.cs` script attached
- [ ] UI TextMeshPro created and linked to EnemyManager
- [ ] `BossSpawnPoint` GameObject created and linked
- [ ] Regular enemies have `Is Boss = false` in Enemy_Health
- [ ] Boss prefab created with `Is Boss = true`
- [ ] Boss prefab assigned to EnemyManager
- [ ] Tested: kill all enemies → boss spawns
- [ ] Tested: kill boss → Console logs "Boss defeated!"

---

## Questions Answered

### "How to implement the kill-all-enemies-first flow?"
1. Use the singleton `EnemyManager` to track enemy count
2. Enemies register on `Start()`, unregister on death
3. When count hits 0, `SpawnBoss()` is called
4. Use `isBoss` flag to exclude boss from the regular enemy count

### "Can I reuse existing enemy code for the boss?"
**Yes!** The boss can use:
- Same `Enemy_Health`, `Enemy_Movement`, `Enemy_Combat` scripts
- Just adjust stats (health, damage, speed) and set `isBoss = true`
- Optionally increase scale for visual impact

### "Can the boss inherit from the enemy class?"
**Yes!** You can create `Boss_Health : Enemy_Health` and override methods like `ChangeHealth()` to add custom behavior (phases, etc.). But for simplicity, using the same components with different values works great.

---

If you have more questions or need help with custom boss attacks, let me know!
