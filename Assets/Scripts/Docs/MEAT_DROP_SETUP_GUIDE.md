# Meat Drop System - Setup Guide

This guide shows you how to create meat drops that heal the player when enemies die.

---

## 📦 What Was Added

1. **MeatPickup.cs** - Script that heals the player when they collect meat
2. **Updated Enemy_Health.cs** - Enemies now drop loot (meat) when they die
3. **Configurable drop chance** - Set probability per enemy (0-100%)

---

## 🥩 Step 1: Create the Meat Prefab

### A. Create Meat GameObject

1. In Hierarchy: `GameObject > 2D Object > Sprite`
2. Rename it to `Meat`
3. Select it and in Inspector:

**Sprite Renderer:**
- **Sprite**: Assign your meat sprite (chicken leg, steak, etc.)
- **Sorting Layer**: Make sure it's visible (e.g., "Default" or same layer as player)
- **Order in Layer**: Set to 1 or higher so it appears above ground

**Transform:**
- **Scale**: Adjust size (e.g., 0.5 to 1.0 for smaller pickup)

### B. Add Collider (Trigger)

1. With `Meat` selected: `Add Component > Circle Collider 2D` (or Box Collider 2D)
2. In the collider component:
   - **Is Trigger**: Check this box ✓
   - **Radius**: Adjust to match your sprite size (e.g., 0.3-0.5)

### C. Add MeatPickup Script

1. With `Meat` selected: `Add Component > Meat Pickup`
2. In the Inspector:
   - **Heal Amount**: Set to 1 (or however much you want it to heal)
   - **Destroy On Pickup**: Checked ✓
   - **Pickup Sound**: (Optional) Drag an AudioClip for pickup sound effect

### D. Save as Prefab

1. Drag the `Meat` GameObject from Hierarchy into `Assets/Prefabs/` folder
2. You should now have `Meat.prefab`
3. Delete the `Meat` GameObject from the Hierarchy (we only need the prefab)

---

## 🎮 Step 2: Configure Enemies to Drop Meat

### A. Setup Regular Enemies

1. Open your enemy prefab (in `Assets/Prefabs/`)
2. Select it and find the `Enemy_Health` component
3. In Inspector, you'll see new fields under **Loot Drops**:
   - **Drop Prefab**: Drag the `Meat` prefab here
   - **Drop Chance**: Set to desired % (e.g., 50 = 50% chance to drop)

**Recommended drop chances:**
- Regular enemies: 30-50%
- Stronger enemies: 60-80%
- Boss: 100% (guaranteed drop)

### B. Setup Boss Drops (Optional)

1. Open your boss prefab
2. In `Enemy_Health` component:
   - **Drop Prefab**: Assign meat (or a special "big meat" that heals more)
   - **Drop Chance**: 100% (boss always drops)

You can also create a separate "Boss Meat" prefab with higher heal amount (e.g., 5 HP).

---

## ⚙️ How It Works

1. **Enemy dies** → `Enemy_Health.Die()` is called
2. **Random roll** checks if drop chance succeeds (0-100%)
3. **If success** → Meat spawns at enemy's position
4. **Player walks over meat** → `MeatPickup.OnTriggerEnter2D()` detects player
5. **Player heals** → `PlayerHealth.UpdateHealth(+healAmount)` is called
6. **Meat is destroyed** → Pickup disappears

---

## 🎨 Customization Options

### Different Heal Amounts

Create multiple meat types:

**Small Meat:**
- Heal Amount: 1
- Sprite: Small chicken leg

**Medium Meat:**
- Heal Amount: 3
- Sprite: Steak

**Large Meat (Boss Drop):**
- Heal Amount: 5-10
- Sprite: Whole turkey

Assign different prefabs to different enemies based on their difficulty.

### Adjust Drop Chance Per Enemy

In each enemy's `Enemy_Health` component:
- Weak goblins: 30% drop chance
- Strong orcs: 60% drop chance
- Elite enemies: 80% drop chance
- Boss: 100% drop chance

### Add Pickup Animation

To make meat more noticeable:

1. Select `Meat` prefab
2. Add `Animator` component
3. Create an animation that:
   - Bounces up and down
   - Rotates slowly
   - Pulses scale

### Add Pickup Sound

1. Find a pickup sound effect (e.g., "nom" or "eat" sound)
2. Import it to `Assets/Audio/`
3. Select `Meat` prefab
4. In `MeatPickup` component, drag the AudioClip into **Pickup Sound** field

---

## 🐛 Troubleshooting

### Meat doesn't spawn when enemy dies
- Check that `Drop Prefab` is assigned in `Enemy_Health`
- Check drop chance is > 0%
- Look for "[Enemy] dropped loot!" in Console
- Verify the meat prefab exists in Assets/Prefabs

### Player doesn't pick up meat
- Ensure meat has `Collider2D` with **Is Trigger** checked
- Ensure player has a `Collider2D` (non-trigger)
- Verify player GameObject has tag "Player"
- Check that `MeatPickup` script is attached to meat prefab
- Look for "Player picked up meat!" message in Console

### Meat doesn't heal player
- Check `PlayerHealth` component exists on player
- Verify `healAmount` is set (not 0)
- Check player's health is not already at max
- Check Console for any errors

### Meat spawns inside walls/obstacles
- Adjust enemy death position slightly
- Add a small upward offset when spawning:
  ```csharp
  Vector3 dropPos = transform.position + Vector3.up * 0.5f;
  Instantiate(dropPrefab, dropPos, Quaternion.identity);
  ```

### Too many/too few drops
- Adjust `dropChance` percentage per enemy
- Lower for common enemies (20-40%)
- Higher for rare/strong enemies (60-80%)

---

## ✅ Setup Checklist

- [ ] `MeatPickup.cs` script created
- [ ] Meat GameObject created with sprite
- [ ] Collider2D added to meat (Is Trigger = true)
- [ ] MeatPickup script added to meat
- [ ] Heal amount configured (default: 1)
- [ ] Meat saved as prefab in Assets/Prefabs/
- [ ] Enemy prefabs have Drop Prefab assigned
- [ ] Drop chance set per enemy (30-50% recommended)
- [ ] Player has tag "Player"
- [ ] Tested: kill enemy → meat spawns → walk over it → heals

---

## 🎮 Testing

1. Press Play
2. Attack and kill an enemy
3. Watch for meat to spawn (50% chance by default)
4. Walk your player over the meat
5. Check Console: "Player picked up meat! Healed for X HP."
6. Check player health UI - should increase

---

## 🚀 Advanced Features

### Meat Despawn Timer

Add to `MeatPickup.cs`:

```csharp
[Header("Despawn")]
public float despawnTime = 10f; // seconds before meat disappears

private void Start()
{
    // ...existing code...
    
    // Auto-destroy after time
    Destroy(gameObject, despawnTime);
}
```

### Magnetic Pickup (Auto-collect)

Make meat move toward player when nearby:

```csharp
[Header("Magnet")]
public float magnetRange = 3f;
public float magnetSpeed = 5f;

private Transform player;

private void Update()
{
    if (player == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }
    
    if (player != null)
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);
        }
    }
}
```

### Different Loot Types

Create more pickup types:
- **Coin** - gives gold/points
- **Potion** - heals more (5+ HP)
- **Buff Item** - temporary speed/damage boost

Use the same pattern as `MeatPickup.cs` but adjust the effect in `OnTriggerEnter2D`.

---

## Summary

You now have a complete loot drop system:
- 🥩 Enemies drop meat when killed (configurable chance)
- ❤️ Meat heals the player when picked up
- ⚙️ Drop chance and heal amount are adjustable per enemy
- 🎵 Optional pickup sound effect

Customize drop chances, heal amounts, and create different meat types to balance your game!
