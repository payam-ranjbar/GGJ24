using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject StartWindow;
    public GameObject TutorialWindow;
    
    private const string START_SCENE = "Start";
    private const string GAME_SCENE = "GameScene";
    
    private void Awake()
    {
        TutorialWindow.SetActive(false);
        StartWindow.SetActive(true);
    }

    public void GoToTutorialScreen()
    {
        TutorialWindow.SetActive(true);
        StartWindow.SetActive(false);
    }

    public void GoToStartScreen()
    {
        TutorialWindow.SetActive(false);
        StartWindow.SetActive(true);
        SceneManager.LoadScene(START_SCENE);
        var scene = SceneManager.GetSceneByName(GAME_SCENE);
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(GAME_SCENE);
        }
    }

    public void GotoMainGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
        SceneManager.UnloadSceneAsync(START_SCENE);
    }
}