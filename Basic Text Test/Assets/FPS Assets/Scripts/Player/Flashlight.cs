using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    public Light flashLight;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip flashlightSwitch;
    
    private bool isActive;

    private void Start()
    {
        isActive = false;
        
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isActive)
            {
                flashLight.enabled = true;
                isActive = true;
                audioSource.PlayOneShot(flashlightSwitch);
            }
            else if (isActive)
            {
                flashLight.enabled = false;
                isActive = false;
                audioSource.PlayOneShot(flashlightSwitch);
            }
        }

        
	}
}
