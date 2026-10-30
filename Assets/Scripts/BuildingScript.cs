using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BuildingScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private Transform player;
    [SerializeField] private Terrain terrain;

    [Header("Building Prefabs")]
    [SerializeField] private GameObject tentPrefab;
    [SerializeField] private GameObject campfirePrefab;

    [Header("Building Settings")]
    [SerializeField] private float tentBuildDistance = 2.5f;
    [SerializeField] private float campfireBuildDistance = 1f; [SerializeField] private float tentBuildTime = 4f;
    [SerializeField] private float campfireBuildTime = 3f;
    [SerializeField] private float tentHeightOffset = 0.05f;
    private Animator anim;

    private bool isBuilding;

    private void Start()
    {
        anim = player.GetComponent<Animator>();
    }
    private void Update()
    {
        if (isBuilding || Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
            StartCoroutine(BuildTent());

        if (Keyboard.current.cKey.wasPressedThisFrame)
            StartCoroutine(BuildCampfire());
    }

    private IEnumerator BuildTent()
    {
        if (tentPrefab == null || inventory == null)
        {
            Debug.LogWarning("Tent prefab or inventory is missing!");
            yield break;
        }

        if (inventory.wood < 10 || inventory.tentCover < 1)
        {
            Debug.Log("Need 10 wood and 1 tent cover to build a tent.");
            yield break;
        }

        inventory.wood -= 10;
        inventory.tentCover -= 1;

        isBuilding = true;
        Debug.Log("Building tent...");
        anim.SetBool("BuildingMode", isBuilding);

        yield return new WaitForSeconds(tentBuildTime);

        SpawnStructure(tentPrefab);

        Debug.Log("Tent built!");
        isBuilding = false;
        anim.SetBool("BuildingMode", isBuilding);
    }

    private IEnumerator BuildCampfire()
    {
        if (campfirePrefab == null || inventory == null)
        {
            Debug.LogWarning("Campfire prefab or inventory is missing!");
            yield break;
        }

        if (inventory.wood < 3 || inventory.smallRocks < 2)
        {
            Debug.Log("Need 3 wood and 2 rocks to build a campfire.");
            yield break;
        }

        inventory.wood -= 3;

        // Rocks are required but are NOT consumed.

        isBuilding = true;
        Debug.Log("Building campfire...");
        anim.SetBool("BuildingMode", isBuilding);

        yield return new WaitForSeconds(campfireBuildTime);

        SpawnStructure(campfirePrefab);

        Debug.Log("Campfire built!");
        isBuilding = false;
        anim.SetBool("BuildingMode", isBuilding);
    }

    private void SpawnStructure(GameObject prefab)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }
        float distance = prefab == tentPrefab ? tentBuildDistance : campfireBuildDistance;

        Vector3 position =
            player.position + player.forward * distance;

        if (terrain != null)
        {
            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            bool insideTerrain =
                position.x >= terrainPosition.x &&
                position.x <= terrainPosition.x + terrainSize.x &&
                position.z >= terrainPosition.z &&
                position.z <= terrainPosition.z + terrainSize.z;

            if (!insideTerrain)
            {
                Debug.LogWarning("Cannot build outside the terrain!");
                return;
            }

            position.y = terrain.SampleHeight(position)
           + terrainPosition.y;

            if (prefab == tentPrefab)
            {
                position.y += tentHeightOffset;
            }
            else if (prefab == campfirePrefab)
            {
                position.y += 0.05f; // Slightly above the ground for campfire
            }
        }
        else
        {
            Debug.LogWarning("Terrain is missing. Using player height.");
        }

        Quaternion rotation = Quaternion.Euler(0f,player.eulerAngles.y,0f);

        if (prefab == tentPrefab)
        {
            rotation *= Quaternion.Euler(-90f, 0f, 180f);
        }

        Instantiate(prefab, position, rotation);
    }
}
