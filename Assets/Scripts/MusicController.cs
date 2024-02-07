using System;
using Matchbox;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource MusicSource;

    private void Awake()
    {
        MusicSource.volume = PlayerPrefs.GetFloat("MusicVolume", DefaultSettings.MusicVolume);
    }
}