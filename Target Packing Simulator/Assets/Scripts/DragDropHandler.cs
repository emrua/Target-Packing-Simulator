using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform packingArea;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

    // Only add CanvasGroup if it doesn't already exist
    canvasGroup = GetComponent<CanvasGroup>();
    if (canvasGroup == null)
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Find the Packing Area
    GameObject packingAreaObject = GameObject.Find("PackingArea");
    if (packingAreaObject != null)
    {
        packingArea = packingAreaObject.GetComponent<RectTransform>();
        Debug.Log("PackingArea found and assigned.");
    }
    else
    {
        Debug.LogError("PackingArea not found. Check the name in the Hierarchy.");
    }
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
    canvasGroup.blocksRaycasts = true; // Re-enable raycasts

    // Log mouse position and packing area bounds
    Debug.Log($"Mouse Position: {Input.mousePosition}");
    Debug.Log($"Packing Area Screen Rect: {packingArea.position}, Size: {packingArea.rect.size}");

    // Check if the mouse is over the Packing Area
    if (RectTransformUtility.RectangleContainsScreenPoint(packingArea, Input.mousePosition, null))
    {
        transform.SetParent(packingArea);
        Debug.Log("Item placed in Packing Area.");
    }
    else
    {
        transform.SetParent(originalParent);
        Debug.Log("Dropped outside Packing Area. Returning to original position.");
    }
}



}


