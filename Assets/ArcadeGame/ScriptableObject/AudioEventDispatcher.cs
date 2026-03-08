using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AudioType
{
    None,
    ObjectMovement,
    PlayerMovement,
    Destruction,
    Death,
    Win
}

[Serializable]
public struct AudioInfos
{
    public AudioType audioType;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioEventDispatcher", menuName = "Scriptable Objects/AudioEventDispatcher")]
public class AudioEventDispatcher : ScriptableObject
{
    [SerializeField] private AudioInfos[] audioInfos;
    
    public event Action<AudioClip> OnAudioEvent;
    
    public void Playaudio(AudioType audioType)
    {
        for (int i = 0; i < audioInfos.Length; i++)
        {
            if (audioInfos[i].audioType == audioType)
            {
                OnAudioEvent?.Invoke(audioInfos[i].audioClip);
                return;
            }
        }
        
        
    }
}
