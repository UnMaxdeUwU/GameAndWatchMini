using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioLowPassFilter lowPass;

    private float normalCutoff = 22000f;
    private float underwaterCutoff = 600f;

    private float originalVolume;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        originalVolume = musicSource.volume;
    }
    
    public void UnderwaterEffect(float duration)
    {
        StartCoroutine(LowPassTransition(normalCutoff, underwaterCutoff, duration));
    }

    public void BackToNormal(float duration)
    {
        StartCoroutine(LowPassTransition(underwaterCutoff, normalCutoff, duration));
    }

    private IEnumerator LowPassTransition(float start, float target, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            lowPass.cutoffFrequency = Mathf.Lerp(start, target, t);

            yield return null;
        }

        lowPass.cutoffFrequency = target;
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeMusic(musicSource.volume, 0.3f, duration));
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeMusic(musicSource.volume, originalVolume, duration));
    }

    private IEnumerator FadeMusic(float startVolume, float targetVolume, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, t);

            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}