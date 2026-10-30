using NUnit.Framework.Internal;
using UnityEngine;
using System.Collections;
public class WorldItemPickUp : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int quantity = 1;
    private InventoryManager inventoryManager;
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if (inventoryManager != null)
        {
            StartCoroutine(PickDelay());
        }
        else
        {
            Debug.LogWarning("InventoryManager not found in the scene.");
        }
    }
    private IEnumerator PickDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay for 1 second
        inventoryManager.AddResource(resourceType, quantity);
        Destroy(gameObject); // Remove the item from the world after picking it up
    }
}
