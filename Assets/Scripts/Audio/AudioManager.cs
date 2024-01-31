using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace DefaultNamespace
{
    public class AudioManager : MonoBehaviour
    {
        public int AudioChannelCount = 5;
        public static AudioManager Instance { get; private set; }
        public AudioMixerGroup sfxGroup;
        public AudioSource BackGroundMusic;
        public List<AudioClipSettings> AudioClips = new List<AudioClipSettings>();
        private List<AudioChannel> _channels = new List<AudioChannel>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                for (int i = 0; i < AudioChannelCount; i++)
                {
                    var gameObject = new GameObject($"AudioChannel {i}");
                    var audioObject = gameObject.AddComponent<AudioChannel>();
                    gameObject.transform.SetParent(transform);
                    _channels.Add(audioObject);
                }
            }

            DontDestroyOnLoad(this);
        }

        public void Play(AudioCommand audioCommand)
        {
            var channel = GetAvailableChannel();
            if (channel == null)
            {
                Debug.LogWarning("No available audio channel");
                return;
            }
            
            var audioSettings = GetAudioClip(audioCommand);
            if (audioSettings != null)
            {
                
                channel.Play(audioSettings);
            }
        }

        private AudioClipSettings GetAudioClip(AudioCommand audioCommand)
        {
            return AudioClips.FirstOrDefault(x => x.AudioCommand == audioCommand);
        }

        public void PlayBackGroundMusic()
        {
            BackGroundMusic.Play();
        }

        public void StopBackGroundMusic()
        {
            BackGroundMusic.Stop();
        }

        private AudioChannel GetAvailableChannel()
        {
            return _channels.FirstOrDefault(x => !x.IsPlaying);
        }
    }
}