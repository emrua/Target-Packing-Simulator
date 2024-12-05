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
    int totalItemsToSpawn = 7; // Total items to spawn
    List<GameObject> allItems = new List<GameObject>();

    // Populate the allItems list with random items from itemPrefabs
    for (int i = 0; i < totalItemsToSpawn; i++)
    {
        // Pick a random item from the prefabs
        GameObject randomItem = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        allItems.Add(randomItem);
    }

    // Instantiate the selected items in the order queue
    foreach (GameObject itemPrefab in allItems)
    {
        GameObject newItem = Instantiate(itemPrefab);
        newItem.transform.SetParent(orderQueueParent, false); // Maintain RectTransform properties
        newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset position
    }
}


}
