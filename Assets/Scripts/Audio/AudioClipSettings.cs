using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipSettings", menuName = "Audio/AudioClipSettings")]
public class AudioClipSettings : ScriptableObject
{
    public AudioCommand AudioCommand;
    public AudioMixerGroup Group;
    [Range(0, 1)]
    public float Volume = 1;
    public AudioClip AudioClip;
}