using UnityEngine;

public class SpriteYOrderSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    // Set the base sorting order to 5000 to match your initial setup.
    private const int SORTING_ORDER_BASE = 5000; 

    // We use a multiplier to give enough space between Y-positions.
    private const int Y_MULTIPLIER = 100; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Higher Y-position (further down the screen) needs a higher 
        // Order in Layer (to be drawn on top).
        
        // This calculates the correct order:
        // Example: If Player Y is -2, Order is 5000 - (-2 * 100) = 5200 (Draws on Top)
        // Example: If Player Y is 2, Order is 5000 - (2 * 100) = 4800 (Draws Behind)
        
        spriteRenderer.sortingOrder = SORTING_ORDER_BASE - (int)(transform.position.y * Y_MULTIPLIER);
    }
}