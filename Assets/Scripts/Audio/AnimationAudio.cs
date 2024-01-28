using UnityEngine;

namespace DefaultNamespace
{
    public class AnimationAudio : MonoBehaviour
    {
        public AudioSource source;

        public void Play()
        {
            source.Play();
        }
    }
}