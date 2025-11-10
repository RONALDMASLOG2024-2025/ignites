// ...existing code...
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteYOrderSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SortingGroup sortingGroup;
    private SpriteRenderer[] childRenderers;

    [Header("Y-Sort Settings")]
    public int sortingOrderBase = 50;
    public int yMultiplier = 30;
    public int sortingOrderOffset = 0;        // per-object offset for big props
    public Transform feet;                    // optional: assign a child at the object's feet

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingGroup = GetComponent<SortingGroup>();
        childRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void LateUpdate()
    {
        float bottomY = GetBottomY();
        int order = sortingOrderBase - Mathf.RoundToInt(bottomY * yMultiplier) + sortingOrderOffset;

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = order;
        }
        else if (spriteRenderer != null && spriteRenderer.gameObject == gameObject)
        {
            spriteRenderer.sortingOrder = order;
        }
        else if (childRenderers != null && childRenderers.Length > 0)
        {
            foreach (var r in childRenderers)
                r.sortingOrder = order;
        }
    }

    private float GetBottomY()
    {
        if (feet != null)
            return feet.position.y;

        if (spriteRenderer != null)
            return spriteRenderer.bounds.min.y;

        float minY = transform.position.y;
        if (childRenderers != null && childRenderers.Length > 0)
        {
            minY = float.MaxValue;
            foreach (var r in childRenderers)
                minY = Mathf.Min(minY, r.bounds.min.y);
        }
        return minY;
    }
}
// ...existing code...