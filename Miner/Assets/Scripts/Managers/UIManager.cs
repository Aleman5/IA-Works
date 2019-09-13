using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance {
        get {
            instance = FindObjectOfType<UIManager>();
            if(instance == null) {
                GameObject go = new GameObject("Managers");
                instance = go.AddComponent<UIManager>();
            }
            return instance;
        }
    }

    [Header("Error Messages")]
    public TextMeshProUGUI text;

    private float time = 0.0f;

    private void Update()
    {
        if (text)
        {
            text.alpha = Mathf.Lerp(1.0f, 0.0f, time);

            time += Time.deltaTime;

            if (text.alpha <= 0.0f)
            {
                text.alpha = 1.0f;
                text.enabled = false;
            }
        }
    }

    public void OnGoalNotOAttainable()
    {
        text.enabled = true;
        text.text = "You can't reach this location";
        text.color = Color.red;
        time = 0.0f;
    }

    public void OnExcessedWorkersCapacity(EElement elementType)
    {
        text.enabled = true;
        text.text = elementType + " is full of workers";
        text.color = new Color(255.0f, 0.0f, 50.0f);
        time = 0.0f;
    }

    public void OnObjectiveNotFound(EElement entity, EElement objective)
    {
        text.enabled = true;
        text.text = entity + " couldn´t found " + objective;
        text.color = new Color(255.0f, 50.0f, 50.0f);
        time = 0.0f;
    }
}
