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

    GameObject object1 = null;
    GameObject object2 = null;

    void Update()
    {
        if (Input.GetButtonDown("LeftClick"))
        {
            GameObject go = CheckHit();

            if (!go || go.tag == "Ground")
            {
                if (object1) object1 = null;
            }
            else
            {
                object1 = go;
            }
        }

        if (Input.GetButtonDown("RightClick"))
        {
            GameObject go = CheckHit();

            if (go && object1)
            {
                object2 = go;
            }
        }

        if (object1 && object2)
        {
            
        }
    }

    GameObject CheckHit()
    {
        GameObject go = null;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            go = hit.transform.gameObject;
        }

        return go;
    }
}
