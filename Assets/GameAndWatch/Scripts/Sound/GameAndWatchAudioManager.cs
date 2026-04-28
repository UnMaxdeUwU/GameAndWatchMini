using UnityEngine;

/// <summary>
/// Singleton audio du GameAndWatch.
/// Écoute les <see cref="GameAndWatchAudioEvents"/> et joue les clips
/// définis dans le <see cref="GameAndWatchAudioConfig"/> assigné.
/// </summary>
public class GameAndWatchAudioManager : MonoBehaviour
{
    public static GameAndWatchAudioManager Instance { get; private set; }

    [SerializeField] private GameAndWatchAudioConfig _config;

    [Header("Source audio")]
    [SerializeField] private AudioSource _sfxSource;

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (_sfxSource != null && _config != null)
            _sfxSource.volume = _config.defaultSfxVolume;
    }

    private void OnEnable()
    {
        GameAndWatchAudioEvents.OnPlayerMove         += PlayPlayerMove;
        GameAndWatchAudioEvents.OnObjectChangeLine   += PlayObjectChangeLine;
        GameAndWatchAudioEvents.OnObjectCollected    += PlayObjectCollected;
        GameAndWatchAudioEvents.OnWrongObjectExplode += PlayWrongObjectExplode;
    }

    private void OnDisable()
    {
        GameAndWatchAudioEvents.OnPlayerMove         -= PlayPlayerMove;
        GameAndWatchAudioEvents.OnObjectChangeLine   -= PlayObjectChangeLine;
        GameAndWatchAudioEvents.OnObjectCollected    -= PlayObjectCollected;
        GameAndWatchAudioEvents.OnWrongObjectExplode -= PlayWrongObjectExplode;
    }

    // ── SFX helpers ───────────────────────────────────────────────────────────

    /// <summary>Joue un clip en one-shot sur la source SFX.</summary>
    private void PlaySfx(AudioClip clip, string debugName = "")
    {
        if (_config == null)
        {
            Debug.LogWarning("[GameAndWatchAudioManager] AudioConfig non assigné dans l'Inspector !");
            return;
        }
        if (clip == null)
        {
            if (!string.IsNullOrEmpty(debugName))
                Debug.LogWarning($"[GameAndWatchAudioManager] Clip manquant : {debugName} — assignez-le dans GameAndWatchAudioConfig.");
            return;
        }
        _sfxSource.PlayOneShot(clip);
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void PlayPlayerMove()         => PlaySfx(_config.playerMove,         nameof(_config.playerMove));
    private void PlayObjectChangeLine()   => PlaySfx(_config.objectChangeLine,   nameof(_config.objectChangeLine));
    private void PlayObjectCollected()    => PlaySfx(_config.objectCollected,    nameof(_config.objectCollected));
    private void PlayWrongObjectExplode() => PlaySfx(_config.wrongObjectExplode, nameof(_config.wrongObjectExplode));

    // ── API publique volume ───────────────────────────────────────────────────

    /// <summary>Règle le volume SFX (0–1).</summary>
    public void SetSfxVolume(float value) => _sfxSource.volume = Mathf.Clamp01(value);
}
