using System.Collections.Generic;
using UnityEngine;

public class OrderQueueManager : MonoBehaviour
{
    public List<GameObject> itemPrefabs; // Prefabs for each packing item
    public Transform Content;  // Parent object to hold items (must be assigned in the Inspector)

    private void Start()
    {
        // If Content is not assigned in the Inspector, attempt to find it dynamically
        if (Content == null)
        {
            Content = GameObject.Find("Content")?.transform; // Replace "Content" with the name of your Content GameObject
            if (Content == null)
            {
                Debug.LogError("Content could not be found dynamically! Ensure it's properly named and in the hierarchy.");
                return;
            }
            else
            {
                Debug.Log($"Content assigned dynamically: {Content.name}");
            }
        }
        else
        {
            Debug.Log($"Content is assigned: {Content.name}");
        }
    }


    public void ClearOrderQueue()
    {
        Debug.Log("Clearing order queue...");
        foreach (Transform child in Content) // Use Content directly to clear children
        {
            Destroy(child.gameObject);
        }
    }

    public void PopulateOrderQueue(int totalItems)
    {
        Debug.Log($"Populating order queue with {totalItems} items...");

        for (int i = 0; i < totalItems; i++)
        {
            // Randomly select an item prefab
            GameObject randomItemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            // Instantiate and add it to the Content GameObject
            GameObject newItem = Instantiate(randomItemPrefab, Content, false);
            newItem.name = $"Item {i + 1}";
        }
    }
}