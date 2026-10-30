using UnityEngine;

public class AxeHitDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer ==
            LayerMask.NameToLayer("Trees"))
        {
            Debug.Log("Axe hit a tree: " + other.name);
        }
        else
        {
            Debug.Log("Axe hit something else: " + other.name);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
