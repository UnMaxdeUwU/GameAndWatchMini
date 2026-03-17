using UnityEngine;

[CreateAssetMenu(fileName = "SlowMotionConfig", menuName = "Scriptable Objects/SlowMotionConfig")]
public class SlowMotionConfig : ScriptableObject
{
    public float slowMotionScale = 0.2f;
    public float slowMotionDuration = 0.25f;
    public float slowMotionRecovery = 0.3f;   
}
