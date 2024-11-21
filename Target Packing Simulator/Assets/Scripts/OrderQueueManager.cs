using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderQueueManager : MonoBehaviour
{
    public List<GameObject> itemPrefabs; // Prefabs for each packing item (assign in Inspector)
    public Transform orderQueueParent;  // Parent object (OrderQueue) to hold items
    public int minItems = 1;            // Minimum items per type
    public int maxItems = 5;            // Maximum items per type


    void Start()
    {
        PopulateOrderQueue();
    }

    void PopulateOrderQueue()
    {
        foreach (GameObject itemPrefab in itemPrefabs)
        {
            // Generate a random number of this item
            int itemCount = Random.Range(minItems, maxItems + 1);

            for (int i = 0; i < itemCount; i++)
            {
                GameObject newItem = Instantiate(itemPrefab);
                newItem.transform.SetParent(orderQueueParent, false); // False ensures RectTransform properties are preserved
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset to start at the top of the Scroll View

            }
        }
    }
}
