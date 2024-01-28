using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameScreenSceneManager : MonoBehaviour
{
    public GameObject WinWindow;
    public GameObject LoseWindow;
    public GameObject PauseWindow;
    public GameObject Hud;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    public void GoToWinScreen()
    {
        Time.timeScale = 0f;
        WinWindow.SetActive(true);
        LoseWindow.SetActive(false);
        PauseWindow.SetActive(false);
        Hud.SetActive(false);
    }

    public void GoToLoseScreen()
    {
        Time.timeScale = 0f;
        LoseWindow.SetActive(true);
        WinWindow.SetActive(false);
        PauseWindow.SetActive(false);
        Hud.SetActive(false);
    }

    public void GoToPauseScreen()
    {
        Time.timeScale = 0f;
        PauseWindow.SetActive(true);
        WinWindow.SetActive(false);
        LoseWindow.SetActive(false);
        Hud.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseWindow.SetActive(false);
        WinWindow.SetActive(false);
        LoseWindow.SetActive(false);
        Hud.SetActive(true);
    }

    public void GoToMainScreen()
    {
        SceneManager.LoadScene(SceneNames.START_SCENE);
        SceneManager.UnloadSceneAsync(SceneNames.GAME_SCENE);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneNames.GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}