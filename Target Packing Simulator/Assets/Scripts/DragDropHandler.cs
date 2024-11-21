using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform packingArea;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // Add if missing

        // Reference to the Packing Area (set this in the Inspector or find it dynamically)
        packingArea = GameObject.Find("PackingArea").GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the original parent so we can reparent later if needed
        originalParent = transform.parent;

        // Temporarily reparent to the root Canvas to make it visible
        transform.SetParent(canvas.transform);

        canvasGroup.blocksRaycasts = false; // Disable raycasts for smooth dragging
        transform.SetAsLastSibling(); // Bring the dragged item to the front
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if the item is within the Packing Area
        if (RectTransformUtility.RectangleContainsScreenPoint(packingArea, Input.mousePosition, canvas.worldCamera))
        {
            // If inside the Packing Area, reparent to it
            transform.SetParent(packingArea);
        }
        else
        {
            // Otherwise, return to the original parent (Scroll View Content)
            transform.SetParent(originalParent);
        }

        canvasGroup.blocksRaycasts = true; // Re-enable raycasts
    }
}
