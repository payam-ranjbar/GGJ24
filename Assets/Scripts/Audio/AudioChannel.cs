using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioChannel : MonoBehaviour
    {
        private AudioSource _audioSource;
        public bool IsPlaying => _audioSource.isPlaying;
        public AudioSource source => _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClipSettings audioClip)
        {
            _audioSource.outputAudioMixerGroup = audioClip.Group;
            _audioSource.PlayOneShot(audioClip.AudioClip, audioClip.Volume);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }
    }
}