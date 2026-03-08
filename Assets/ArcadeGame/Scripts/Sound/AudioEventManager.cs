using UnityEngine;

public class AudioEventManager : MonoBehaviour
{
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        _audioEventDispatcher.OnAudioEvent += PlayAudioFX;
    }

    private void OnDisable()
    {
        _audioEventDispatcher.OnAudioEvent -= PlayAudioFX;
    }
    
    private void PlayAudioFX(AudioClip clip)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
