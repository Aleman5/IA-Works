using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionerManager : MonoBehaviour
{
    private static SelectionerManager instance;

    public static SelectionerManager Instance {
        get {
            instance = FindObjectOfType<SelectionerManager>();
            if(instance == null) {
                GameObject go = new GameObject("Managers");
                instance = go.AddComponent<SelectionerManager>();
            }
            return instance;
        }
    }

    public Camera mainCamera;

    


    void Update()
    {

    }
}
