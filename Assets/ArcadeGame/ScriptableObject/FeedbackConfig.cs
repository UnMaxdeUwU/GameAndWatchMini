using UnityEngine;

[CreateAssetMenu(fileName = "FeedbackConfig", menuName = "Scriptable Objects/FeedbackConfig")]
public class FeedbackConfig : ScriptableObject
{
    [Header("--- Hit Normal ---")]
    public float hitStopDuration = 0.06f;
    public float hitShakeDuration = 0.15f;
    public float hitShakeMagnitude = 0.08f;

    [Header("--- Parry ---")]
    public float parryStopDuration = 0.12f;
    public float parryShakeDuration = 0.2f;
    public float parryShakeMagnitude = 0.15f;

    [Header("--- Counter Attack ---")]
    public float counterStopDuration = 0.18f;
    public float counterShakeDuration = 0.25f;
    public float counterShakeMagnitude = 0.22f;

    [Header("--- Stun Enemy ---")]
    public float stunDuration = 0.4f;
    public float parryStunDuration = 0.8f;
}
