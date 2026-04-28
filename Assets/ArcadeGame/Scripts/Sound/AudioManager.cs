using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton gérant toute la couche audio du jeu :
/// - Deux AudioSource séparées : SFX et Musique.
/// - Volume SFX et Musique réglables indépendamment.
/// - Musique de jeu → Game Over : blend croisé progressif.
/// - Écoute AudioEvents pour jouer chaque clip au bon moment.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioConfig _config;

    [Header("Sources audio")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSourceA;   // musique principale
    [SerializeField] private AudioSource _musicSourceB;   // musique game over (fade-in)

    [Header("Blend musical")]
    [SerializeField] private float _crossfadeDuration = 2f;

    // ── Volume accessibles depuis d'autres systèmes ───────────────────────────
    public float SfxVolume
    {
        get => _sfxSource.volume;
        set => _sfxSource.volume = Mathf.Clamp01(value);
    }

    public float MusicVolume
    {
        get => _musicSourceA.volume;
        set
        {
            _targetMusicVolume = Mathf.Clamp01(value);
            _musicSourceA.volume = _targetMusicVolume;
        }
    }

    private float _targetMusicVolume;
    private bool _altToggle;
    private bool _crossfadeStarted;

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _targetMusicVolume   = _config.defaultMusicVolume;
        _sfxSource.volume    = _config.defaultSfxVolume;
        _musicSourceA.volume = _config.defaultMusicVolume;
        _musicSourceB.volume = 0f;
        _crossfadeStarted    = false;

        PlayGameMusic();
    }

    private void OnEnable()
    {
        AudioEvents.OnEnemyHit           += PlayEnemyHit;
        AudioEvents.OnEnemySimpleAttack  += PlayEnemySimpleAttack;
        AudioEvents.OnBossHit            += PlayBossHit;
        AudioEvents.OnBossAttack         += PlayBossAttack;
        AudioEvents.OnLaserSpawn         += PlayLaserSpawn;
        AudioEvents.OnBossAttackAlt      += PlayBossAttackAlt;
        AudioEvents.OnPlayerAttack       += PlayPlayerAttack;
        AudioEvents.OnPlayerParrySuccess += PlayParrySuccess;
        AudioEvents.OnPlayerDeath        += PlayPlayerDeath;
        AudioEvents.OnRankAppear         += PlayRankAppear;
        AudioEvents.OnRankResult         += PlayRankResult;
        AudioEvents.OnHitTick            += PlayHitTick;

        HealthManagerPlayer.OnPlayerDied += OnPlayerDied;
    }

    private void OnDisable()
    {
        AudioEvents.OnEnemyHit           -= PlayEnemyHit;
        AudioEvents.OnEnemySimpleAttack  -= PlayEnemySimpleAttack;
        AudioEvents.OnBossHit            -= PlayBossHit;
        AudioEvents.OnBossAttack         -= PlayBossAttack;
        AudioEvents.OnLaserSpawn         -= PlayLaserSpawn;
        AudioEvents.OnBossAttackAlt      -= PlayBossAttackAlt;
        AudioEvents.OnPlayerAttack       -= PlayPlayerAttack;
        AudioEvents.OnPlayerParrySuccess -= PlayParrySuccess;
        AudioEvents.OnPlayerDeath        -= PlayPlayerDeath;
        AudioEvents.OnRankAppear         -= PlayRankAppear;
        AudioEvents.OnRankResult         -= PlayRankResult;
        AudioEvents.OnHitTick            -= PlayHitTick;

        HealthManagerPlayer.OnPlayerDied -= OnPlayerDied;
    }

    // ── Musique ───────────────────────────────────────────────────────────────

    private void PlayGameMusic()
    {
        if (_config.musicGame == null) return;
        _musicSourceA.clip = _config.musicGame;
        _musicSourceA.loop = true;
        _musicSourceA.Play();
    }

    private void OnPlayerDied()
    {
        if (_crossfadeStarted) return;
        _crossfadeStarted = true;
        StartCoroutine(CrossfadeToGameOver());
    }

    private IEnumerator CrossfadeToGameOver()
    {
        if (_config.musicGameOver != null)
        {
            _musicSourceB.clip   = _config.musicGameOver;
            _musicSourceB.loop   = true;
            _musicSourceB.volume = 0f;
            _musicSourceB.Play();
        }

        float elapsed = 0f;
        float startVolA = _musicSourceA.volume;

        while (elapsed < _crossfadeDuration)
        {
            // Utilise unscaledDeltaTime : le crossfade fonctionne même si timeScale = 0
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _crossfadeDuration);

            _musicSourceA.volume = Mathf.Lerp(startVolA,           0f,                    t);
            _musicSourceB.volume = Mathf.Lerp(0f,                  _targetMusicVolume,    t);
            yield return null;
        }

        _musicSourceA.Stop();
    }

    // ── SFX helpers ───────────────────────────────────────────────────────────

    /// <summary>Joue un clip en one-shot sur la source SFX partagée.</summary>
    private void PlaySfx(AudioClip clip, string debugName = "")
    {
        if (_config == null)
        {
            Debug.LogWarning("[AudioManager] AudioConfig non assigné dans l'Inspector !");
            return;
        }
        if (clip == null)
        {
            if (!string.IsNullOrEmpty(debugName))
                Debug.LogWarning($"[AudioManager] Clip manquant : {debugName} — assignez-le dans AudioConfig.");
            return;
        }
        _sfxSource.PlayOneShot(clip);
    }

    // ── Callbacks SFX ────────────────────────────────────────────────────────

    private void PlayEnemyHit()          => PlaySfx(_config.enemyHit,         nameof(_config.enemyHit));
    private void PlayEnemySimpleAttack() => PlaySfx(_config.enemySimpleAttack, nameof(_config.enemySimpleAttack));
    private void PlayBossHit()           => PlaySfx(_config.bossHit,           nameof(_config.bossHit));
    private void PlayBossAttack()        => PlaySfx(_config.bossAttack,        nameof(_config.bossAttack));
    private void PlayLaserSpawn()        => PlaySfx(_config.laserSpawn,        nameof(_config.laserSpawn));
    private void PlayPlayerAttack()      => PlaySfx(_config.playerAttack,      nameof(_config.playerAttack));
    private void PlayParrySuccess()      => PlaySfx(_config.playerParrySuccess, nameof(_config.playerParrySuccess));
    private void PlayPlayerDeath()       => PlaySfx(_config.playerDeath,       nameof(_config.playerDeath));
    private void PlayRankAppear()        => PlaySfx(_config.rankAppear,        nameof(_config.rankAppear));
    private void PlayRankResult()        => PlaySfx(_config.rankResult,        nameof(_config.rankResult));
    private void PlayHitTick()           => PlaySfx(_config.hitTick,           nameof(_config.hitTick));

    /// <summary>Alterne entre les deux clips d'attaque mêlée du boss.</summary>
    private void PlayBossAttackAlt()
    {
        AudioClip clip = _altToggle ? _config.bossAttackAlt1 : _config.bossAttackAlt2;
        _altToggle = !_altToggle;
        PlaySfx(clip, _altToggle ? nameof(_config.bossAttackAlt1) : nameof(_config.bossAttackAlt2));
    }

    // ── API publique volume ───────────────────────────────────────────────────

    /// <summary>Règle le volume SFX (0-1).</summary>
    public void SetSfxVolume(float value)   => SfxVolume   = value;

    /// <summary>Règle le volume musique (0-1). Affecte les deux sources musicales.</summary>
    public void SetMusicVolume(float value)
    {
        _targetMusicVolume   = Mathf.Clamp01(value);
        _musicSourceA.volume = _targetMusicVolume;
        _musicSourceB.volume = _musicSourceB.isPlaying ? _targetMusicVolume : 0f;
    }
}
