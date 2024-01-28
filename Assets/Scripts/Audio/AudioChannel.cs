using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioChannel : MonoBehaviour
    {
        private AudioSource _audioSource;
        public bool IsPlaying => _audioSource.isPlaying;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClipSettings audioClip)
        {
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