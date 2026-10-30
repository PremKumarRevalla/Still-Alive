using System;
using System.Collections;
using UnityEngine;

public class PlayerEquipmentScript : MonoBehaviour
{
    [SerializeField] private GameObject storedAxe;
    [SerializeField] private GameObject equippedAxe;

    private bool hasAxeEquipped = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeWeapon()
    {
        if (hasAxeEquipped)
        {
            return;
        }
        StartCoroutine(WeaponPickDelay());
    }
    public void StoreWeapon()
    {
        if(!hasAxeEquipped)
        {
            return;
        }
        StartCoroutine(StoreWeaponDelay());
    }

    private IEnumerator StoreWeaponDelay()
    {
        yield return new WaitForSeconds(0.7f); // Delay for 1 second

        equippedAxe.SetActive(false); 
        storedAxe.SetActive(true);

        hasAxeEquipped = false;
    }
    private IEnumerator WeaponPickDelay()
    {
        yield return new WaitForSeconds(0.25f); // Delay for 1 second
        storedAxe.SetActive(false);
        equippedAxe.SetActive(true);
        hasAxeEquipped = true;
    }
}
