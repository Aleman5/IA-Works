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
    public TextMeshProUGUI obstacleMessage;

    private float time = 0.0f;

    private void Update()
    {
        if (obstacleMessage)
        {
            obstacleMessage.alpha = Mathf.Lerp(1.0f, 0.0f, time);

            time += Time.deltaTime;

            if (obstacleMessage.alpha <= 0.0f)
            {
                obstacleMessage.alpha = 1.0f;
                obstacleMessage.enabled = false;
            }
        }
    }

    public void OnGoalNotOAttainable()
    {
        obstacleMessage.enabled = true;
        time = 0.0f;
    }
}
