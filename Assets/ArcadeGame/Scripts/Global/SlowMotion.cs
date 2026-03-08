using System.Collections;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionTimeScale = 0.1f;

    float startTimeScale;
    float startFixedDeltaTime;

    Coroutine currentRoutine;

    void Awake()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void StartSlowMotion(float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SlowRoutine(duration));
    }

    IEnumerator SlowRoutine(float duration)
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;

        currentRoutine = null;
    }
}