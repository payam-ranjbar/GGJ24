using System;
using System.Collections;
using System.Collections.Generic;
using Matchbox;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindowController : MonoBehaviour
{
    public Slider SoundSlider;
    public Slider MusicSlider;

    public void Awake()
    {
        SoundSlider.value = PlayerPrefs.GetFloat("SoundVolume", DefaultSettings.SFXVolume);
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", DefaultSettings.MusicVolume);
    }
}
