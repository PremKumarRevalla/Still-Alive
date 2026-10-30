using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    Mushroom,
    Wood,
    SmallRock
}
public class InventoryManager : MonoBehaviour
{
    [Header("Resources")]
    public int mushrooms;
    public int wood;
    public int smallRocks = 2;
    public int tentCover = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Mushroom:
                mushrooms += amount;
                break;
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.SmallRock:
                smallRocks += amount;
                break;
            default:
                Debug.LogWarning("Unknown resource type: " + resourceType);
                break;
        }
        Debug.Log($"Inventory: Mushrooms: {mushrooms}, Wood: {wood}, Small Rocks: {smallRocks}");

    }
}
