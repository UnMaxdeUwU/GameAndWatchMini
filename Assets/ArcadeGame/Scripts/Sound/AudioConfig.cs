using UnityEngine;

/// <summary>
/// ScriptableObject centralisant tous les clips audio du jeu.
/// Créer via Assets > Create > ArcadeGame > Audio Config.
/// </summary>
[CreateAssetMenu(fileName = "AudioConfig", menuName = "ArcadeGame/Audio Config")]
public class AudioConfig : ScriptableObject
{
    [Header("── Ennemis ──────────────────────────────")]
    [Tooltip("Son joué quand l'ennemi simple subit un dégât.")]
    public AudioClip enemyHit;

    [Tooltip("Son joué quand le boss subit un dégât.")]
    public AudioClip bossHit;

    [Tooltip("Son joué quand le boss lance son attaque (Flame Wave).")]
    public AudioClip bossAttack;

    [Tooltip("Son joué quand le projectile laser apparaît.")]
    public AudioClip laserSpawn;

    [Tooltip("Attaque mêlée du boss — alternative 1.")]
    public AudioClip bossAttackAlt1;

    [Tooltip("Attaque mêlée du boss — alternative 2.")]
    public AudioClip bossAttackAlt2;

    [Header("── Joueur ───────────────────────────────")]
    [Tooltip("Son joué quand le joueur attaque.")]
    public AudioClip playerAttack;

    [Tooltip("Son joué quand le joueur réussit une parry.")]
    public AudioClip playerParrySuccess;

    [Tooltip("Son joué quand le joueur meurt.")]
    public AudioClip playerDeath;

    [Tooltip("Son joué quand l'ennemi simple attaque.")]
    public AudioClip enemySimpleAttack;

    [Header("── UI / Combo ──────────────────────────")]
    [Tooltip("Son joué quand le Rank_Text apparaît (changement de rang en cours de combo).")]
    public AudioClip rankAppear;

    [Tooltip("Son joué quand le Rank_result apparaît (résultat du combo expiré).")]
    public AudioClip rankResult;

    [Tooltip("Son joué à chaque fois que le compteur HIT augmente.")]
    public AudioClip hitTick;

    [Header("── Musiques ─────────────────────────────")]
    [Tooltip("Musique de jeu principale, joue dès le lancement de la scène.")]
    public AudioClip musicGame;

    [Tooltip("Musique du Game Over, fade-in progressif à la mort du joueur.")]
    public AudioClip musicGameOver;

    [Header("── Volumes par défaut ───────────────────")]
    [Range(0f, 1f)] public float defaultSfxVolume   = 1f;
    [Range(0f, 1f)] public float defaultMusicVolume  = 0.6f;
}
