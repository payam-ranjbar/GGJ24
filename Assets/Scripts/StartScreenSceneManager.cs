using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenSceneManager : MonoBehaviour
{
    public GameObject StartWindow;
    public GameObject TutorialWindow;
    
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
        SceneManager.LoadScene(SceneNames.START_SCENE);
        var scene = SceneManager.GetSceneByName(SceneNames.GAME_SCENE);
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(SceneNames.GAME_SCENE);
        }
    }

    public void GotoMainGame()
    {
        SceneManager.LoadScene(SceneNames.GAME_SCENE);
        SceneManager.UnloadSceneAsync(SceneNames.START_SCENE);
    }
}