using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainGameScreenSceneManager : MonoBehaviour
{
    public GameObject WinWindow;
    public GameObject LoseWindow;
    public GameObject PauseWindow;
    public GameObject Hud;

    [SerializeField] private AudioSource musicIntro;
    [SerializeField] private AudioSource music;
    [SerializeField] private float fadeDuration = 1f;
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public AiBehaviour[] npcs;
    public UnityEvent onStart;

    private bool lost;
    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AwakeAI(true);
            StartMusic();
            CameraManager.Instance.ActivateMainCamera();
            onStart?.Invoke();

        }
    }

    public void AwakeAI(bool awake)
    {
        foreach (var aiBehaviour in npcs)
        {
            aiBehaviour.active = awake;
        }

        BombSpawnSystem.instance.Active = awake;
    }

    public void StartMusic()
    {
        // StartCoroutine(FadeAudioSources());
        musicIntro.DOFade(0f, fadeDuration).SetEase(fadeCurve);
        music.DOFade(1f, fadeDuration).SetEase(fadeCurve);
    }

    public void StopMusic()
    {
        var seq = DOTween.Sequence();
        seq.Append(music.DOFade(0f, fadeDuration).SetEase(fadeCurve))
            .Join(music.DOPitch(0.5f, 0.6f))
            .Join(musicIntro.DOFade(1f, fadeDuration).SetEase(fadeCurve));
    }

    private IEnumerator FadeAudioSources(bool toMusic = true)
    {
        float timer = 0f;
        var m2 = toMusic ? music : musicIntro;
        var m1 = toMusic ? musicIntro : music;
        float startVolume1 = m1.volume;
        float startVolume2 = m2.volume;

        m2.volume = 0f;
        m2.Play();
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            // Calculate the interpolation factor between 0 and 1 based on the timer and duration
            float t = fadeCurve.Evaluate(Mathf.Clamp01(timer / fadeDuration));
            
            // Fade out the first audio source
            m1.volume = Mathf.Lerp(startVolume1, 0f, t);

            // Fade in the second audio source
            m2.volume = Mathf.Lerp(0f, startVolume2, t);

            yield return null;
        }

        // Ensure volumes are set to the final values to avoid floating-point errors
        m1.volume = 0f;
        m2.volume = startVolume2;
    }


    public void GoToWinScreen()
    {
        if(lost) return;
        WinWindow.SetActive(true);
        LoseWindow.SetActive(false);
        PauseWindow.SetActive(false);
        Hud.SetActive(false);
    }

    public void GoToLoseScreen()
    {
        lost = true;
        StopMusic();
        CameraManager.Instance.DeathEffect();
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