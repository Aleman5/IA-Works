using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private float switchDelay = 1f;

    private int index;
    private bool isSwitching;

    private void Start()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);          
        }
        weapons[0].SetActive(true);                
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
        {
            index++;

            if (index >= weapons.Length)
            {
                index = 0;
            }
            StartCoroutine(SwitchAfterDelay(index));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
        {
            index--;

            if (index < 0)
            {
                index = weapons.Length - 1;
            }
            StartCoroutine(SwitchAfterDelay(index));
        }
    }

    private IEnumerator SwitchAfterDelay(int newIndex)
    {
        isSwitching = true;
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;
        SwitchWeapons(newIndex);
    }

    private void SwitchWeapons(int newIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);           
        }
        weapons[newIndex].SetActive(true);         
    }
}
