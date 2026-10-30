using UnityEngine;

public class TerrainWoodCollector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform axeHead;

    [Header("Settings")]
    [SerializeField] private float hitRadius = 1.5f;
    [SerializeField] private int woodPerHit = 1;

    public void HitTree()
    {
        if (terrain == null || axeHead == null)
            return;

        TerrainData data = terrain.terrainData;
        Vector3 axePosition = axeHead.position;

        foreach (TreeInstance tree in data.treeInstances)
        {
            Vector3 worldPosition =
                Vector3.Scale(tree.position, data.size)
                + terrain.transform.position;

            float distance = Vector2.Distance(
                new Vector2(axePosition.x, axePosition.z),
                new Vector2(worldPosition.x, worldPosition.z)
            );

            if (distance <= hitRadius &&
                axePosition.y >= worldPosition.y &&
                axePosition.y <= worldPosition.y
                    + data.size.y * tree.heightScale)
            {
                Debug.Log("Collected " + woodPerHit + " wood!");
                return;
            }
        }

        Debug.Log("No tree close enough to the axe.");
    }
}
