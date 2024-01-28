using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string START_SCENE = "Start";
    private const string GAME_SCENE = "GameScene";
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetState(GameState state)
    {
        switch (state)
        {
            case GameState.StartScreen:
                SceneManager.LoadScene(START_SCENE);
                //if(Sce)
                SceneManager.UnloadSceneAsync(GAME_SCENE);
                break;
            
        }
    }
}

public enum GameState
{
    StartScreen,
    TutorialScreen,
    Pause,
    Win,
    Lose,
    StartGame
}