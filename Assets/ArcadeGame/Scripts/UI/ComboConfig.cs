using UnityEngine;

[CreateAssetMenu(fileName = "ComboConfig", menuName = "Scriptable Objects/ComboConfig")]
public class ComboConfig : ScriptableObject
{
    [Header("--- Paliers de rank ---")]
    public int[] rankThresholds = { 5, 15, 30, 50, 80 };
    public string[] rankLabels   = { "BRUTAL", "SAVAGE", "MAYHEM", "INSANE", "GODLIKE" };
    public Color[] rankColors    =
    {
        new Color(1f, 0.6f, 0.1f),   // orange
        new Color(1f, 0.2f, 0.2f),   // rouge
        new Color(0.8f, 0f,  1f),    // violet
        new Color(0.1f, 0.8f, 1f),   // cyan
        new Color(1f, 1f,  0.2f),    // or
    };

    [Header("--- Compteur UI ---")]
    public float basePunchScale    = 1.35f;
    public float punchOutDuration  = 0.07f;
    public float punchBackDuration = 0.12f;
    public Color colorLow    = Color.white;
    public Color colorMedium = new Color(1f, 0.55f, 0.1f);
    public Color colorHigh   = new Color(1f, 0.15f, 0.15f);
    public Color colorExtreme= new Color(1f, 1f,   0.2f);
    [Tooltip("Combo à partir duquel on atteint la couleur extrême")]
    public int extremeThreshold = 40;

    [Header("--- HitStop scalé ---")]
    public float baseHitStop  = 0.04f;
    public float maxHitStop   = 0.14f;
    [Tooltip("Combo au-delà duquel le hitstop est au max")]
    public int hitStopMaxCombo = 50;

    [Header("--- Camera shake scalé ---")]
    public float baseShakeMag  = 0.06f;
    public float maxShakeMag   = 0.22f;
    public float baseShakeDur  = 0.10f;
    public float maxShakeDur   = 0.20f;
    [Tooltip("Combo au-delà duquel le shake est au max")]
    public int shakeMaxCombo   = 50;

    [Header("--- Combo timeout ---")]
    [Tooltip("Secondes sans hit avant reset du combo")]
    public float comboTimeout  = 3f;
}
