using UnityEngine;

/// <summary>
/// ScriptableObject centralisant tous les clips audio du GameAndWatch.
/// Créer via Assets > Create > GameAndWatch > Audio Config.
/// </summary>
[CreateAssetMenu(fileName = "GameAndWatchAudioConfig", menuName = "GameAndWatch/Audio Config")]
public class GameAndWatchAudioConfig : ScriptableObject
{
    [Header("── Joueur ───────────────────────────────────")]
    [Tooltip("Son joué à chaque déplacement du joueur.")]
    public AudioClip playerMove;

    [Header("── Objets ───────────────────────────────────")]
    [Tooltip("Son joué quand un objet change de ligne (avance d'un cran).")]
    public AudioClip objectChangeLine;

    [Tooltip("Son joué quand le joueur collecte un bon objet.")]
    public AudioClip objectCollected;

    [Tooltip("Son joué quand le mauvais objet explose (changement de sprite).")]
    public AudioClip wrongObjectExplode;

    [Header("── Volume ───────────────────────────────────")]
    [Range(0f, 1f)] public float defaultSfxVolume = 1f;
}
