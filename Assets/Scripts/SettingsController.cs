using System;
using System.Collections;
using System.Collections.Generic;
using Matchbox;
using UnityEngine;
using UnityEngine.Events;

public class SettingsController : MonoBehaviour
{
    public AudioSource backgroundMusic;

    private void Awake()
    {
        var musicVolume = PlayerPrefs.GetFloat("MusicVolume", DefaultSettings.MusicVolume);
        backgroundMusic.volume = musicVolume;
    }

    public void SoundToggle(float volume)   
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }
    
    public void MusicToggle(float volume)   
    {
        backgroundMusic.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
