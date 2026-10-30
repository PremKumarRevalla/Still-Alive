using UnityEngine;

public class LightStickController : MonoBehaviour
{
    [SerializeField] private GameObject lightStickPrefab;
    //[SerializeField] private DayNightCycle dayNightCycle;
    public bool isLightStickActive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeLightStick()
    {
        if (isLightStickActive)
        {
            return;
        }
        lightStickPrefab.SetActive(true);
        isLightStickActive = true;
    }
    public void StoreLightStick()
    {
        if (!isLightStickActive)
        {
            return;
        }
        lightStickPrefab.SetActive(false);
        isLightStickActive = false;
    }
}
