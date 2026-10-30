using UnityEngine;

public class MushroomSpawnerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Mushroom Settings")]
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private int mushroomsPerPoint = 20;

    [Header("Stick Settings")]
    [SerializeField] private GameObject stickPrefab;
    [SerializeField] private int sticksPerPoint = 10;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 15f;

    private void Start()
    {
        SpawnResources(mushroomPrefab, mushroomsPerPoint);
        SpawnResources(stickPrefab, sticksPerPoint);
    }

    private void SpawnResources(GameObject prefab, int count)
    {
        if (terrain == null || prefab == null)
        {
            Debug.LogWarning("Terrain or resource prefab is missing!");
            return;
        }

        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
                continue;

            for (int i = 0; i < count; i++)
            {
                Vector2 randomOffset =
                    Random.insideUnitCircle * spawnRadius;

                Vector3 position = spawnPoint.position +
                    new Vector3(randomOffset.x, 0f, randomOffset.y);

                if (position.x < terrainPosition.x ||
                    position.x > terrainPosition.x + terrainSize.x ||
                    position.z < terrainPosition.z ||
                    position.z > terrainPosition.z + terrainSize.z)
                    continue;

                position.y = terrain.SampleHeight(position)
                           + terrainPosition.y;

                Instantiate(
                    prefab,
                    position,
                    Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                );
            }
        }
    }
}