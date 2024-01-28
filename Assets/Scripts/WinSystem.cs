using UnityEngine;
using UnityEngine.Events;

public class WinSystem : MonoBehaviour
{
    public UnityEvent PlayerWin;
    public UnityEvent PlayerLose;

    private int _currentAmountOfPlayers = 0;

    private void Awake()
    {
        _currentAmountOfPlayers = FindObjectsOfType<CharacterController>().Length;
    }

    public void CharacterDied()
    {
        Debug.Log("Character Died");
        _currentAmountOfPlayers--;
        if (_currentAmountOfPlayers == 1)
        {
            PlayerWin.Invoke();
        }
    }

    public void MainPlayerDied()
    {
        PlayerLose.Invoke();
    }
}