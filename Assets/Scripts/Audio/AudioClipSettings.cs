using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipSettings", menuName = "Audio/AudioClipSettings")]
public class AudioClipSettings : ScriptableObject
{
    public AudioCommand AudioCommand;
    [Range(0, 1)]
    public float Volume = 1;
    public AudioClip AudioClip;
}