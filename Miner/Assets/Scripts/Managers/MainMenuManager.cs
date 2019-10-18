using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    const int GAME_INDEX = 1;

    public void StartPVsPGame()
    {
        SceneManager.LoadScene(GAME_INDEX);
    }

    public void StartVsIAGame()
    {
        SceneManager.LoadScene(GAME_INDEX);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
