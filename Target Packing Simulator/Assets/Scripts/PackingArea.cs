using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackingArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag; // The dragged object
        if (droppedItem != null)
        {
            Debug.Log($"Item dropped into the Packing Area: {droppedItem.name}");
        }
        else
        {
            Debug.LogWarning("Dropped object is null.");
        }
    }

}
