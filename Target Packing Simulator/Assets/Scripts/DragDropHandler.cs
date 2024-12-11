using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform packingArea;
    private GameManager gameManager;
    private bool isInPackingArea = false; // Tracks whether the item is already in the PackingArea


    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        // Add CanvasGroup if it doesn't already exist
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
            Debug.Log($"PackingArea found: {packingArea.name}");
        }
        else
        {
            Debug.LogError("PackingArea not found. Check the name in the Hierarchy.");
        }

        // Find the GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found. Ensure there is a GameManager in the scene.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the original parent
        originalParent = transform.parent;
        Debug.Log($"OnBeginDrag: Original parent is {originalParent.name}");

        // Temporarily reparent to the root Canvas
        transform.SetParent(canvas.transform);
        Debug.Log($"OnBeginDrag: Item temporarily reparented to Canvas");

        canvasGroup.blocksRaycasts = false; // Disable raycasts for smooth dragging
        transform.SetAsLastSibling(); // Bring the dragged item to the front
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow the mouse position
        transform.position = Input.mousePosition;
        Debug.Log($"OnDrag: Item position updated to {transform.position}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts

        Debug.Log($"OnEndDrag: Checking PackingArea detection...");

        // Check if the mouse is over the Packing Area
        if (RectTransformUtility.RectangleContainsScreenPoint(packingArea, Input.mousePosition, null))
        {
            Debug.Log($"OnEndDrag: Item dropped in PackingArea: {packingArea.name}");

            // Convert mouse position to local position within the PackingArea
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                packingArea,
                Input.mousePosition,
                null,
                out localPointerPosition
            );

            // Temporarily set the item's local position to the desired drop point
            transform.SetParent(packingArea, false);
            transform.localPosition = localPointerPosition;

            // Check for overlaps with other items in the PackingArea
            bool isOverlapping = false;
            foreach (Transform child in packingArea)
            {
                if (child == transform) continue; // Skip the current item

                RectTransform otherRect = child.GetComponent<RectTransform>();
                RectTransform thisRect = GetComponent<RectTransform>();

                if (thisRect != null && otherRect != null && RectOverlaps(thisRect, otherRect))
                {
                    isOverlapping = true;
                    Debug.Log($"OnEndDrag: Overlap detected with {child.name}");
                    break;
                }
            }

            if (isOverlapping)
            {
                // If overlapping, return to original parent
                Debug.Log("OnEndDrag: Overlap detected. Returning to original parent.");
                transform.SetParent(originalParent);
                transform.localPosition = Vector3.zero; // Reset position in original parent
                isInPackingArea = false; // Mark the item as not in the PackingArea
            }
            else
            {
                Debug.Log("OnEndDrag: No overlap detected. Item successfully placed.");

                // Only add points if the item is being placed in the PackingArea for the first time
                if (!isInPackingArea)
                {
                    isInPackingArea = true; // Mark the item as being in the PackingArea
                    if (gameManager != null)
                    {
                        gameManager.AddScore(10); // Add 10 points
                        Debug.Log("OnEndDrag: 10 points added to score.");
                    }
                    else
                    {
                        Debug.LogError("OnEndDrag: GameManager reference is null!");
                    }
                }
                else
                {
                    Debug.Log("OnEndDrag: Item already in PackingArea. No points added.");
                }
            }
        }
        else
        {
            Debug.Log("OnEndDrag: Item dropped outside PackingArea. Returning to original parent.");
            transform.SetParent(originalParent); // Return to original parent if not dropped in PackingArea
            transform.localPosition = Vector3.zero; // Reset position to original parent’s local space
            Debug.Log($"OnEndDrag: Item reparented to original parent: {originalParent.name}");
            isInPackingArea = false; // Mark the item as not in the PackingArea
        }
    }



    private bool RectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = GetWorldRect(rect1);
        Rect r2 = GetWorldRect(rect2);

        return r1.Overlaps(r2);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return new Rect(corners[0].x, corners[0].y, 
                        corners[2].x - corners[0].x, 
                        corners[2].y - corners[0].y);
    }

}

