using System;

/// <summary>
/// Bus d'events statiques utilisé par tous les systèmes pour déclencher un son
/// sans dépendre directement de l'AudioManager.
/// </summary>
public static class AudioEvents
{
    // ── Ennemis ──────────────────────────────────────────────────────────────
    public static event Action OnEnemyHit;
    public static event Action OnEnemySimpleAttack;
    public static event Action OnBossHit;
    public static event Action OnBossAttack;
    public static event Action OnLaserSpawn;
    public static event Action OnBossAttackAlt;   // alterne entre alt1 et alt2

    // ── Joueur ───────────────────────────────────────────────────────────────
    public static event Action OnPlayerAttack;
    public static event Action OnPlayerParrySuccess;
    public static event Action OnPlayerDeath;

    // ── UI / Combo ────────────────────────────────────────────────────────────
    public static event Action OnRankAppear;
    public static event Action OnRankResult;
    public static event Action OnHitTick;

    // ── Méthodes d'invocation ─────────────────────────────────────────────────
    public static void RaiseEnemyHit()            => OnEnemyHit?.Invoke();
    public static void RaiseEnemySimpleAttack()   => OnEnemySimpleAttack?.Invoke();
    public static void RaiseBossHit()             => OnBossHit?.Invoke();
    public static void RaiseBossAttack()          => OnBossAttack?.Invoke();
    public static void RaiseLaserSpawn()          => OnLaserSpawn?.Invoke();
    public static void RaiseBossAttackAlt()       => OnBossAttackAlt?.Invoke();
    public static void RaisePlayerAttack()        => OnPlayerAttack?.Invoke();
    public static void RaisePlayerParrySuccess()  => OnPlayerParrySuccess?.Invoke();
    public static void RaisePlayerDeath()         => OnPlayerDeath?.Invoke();
    public static void RaiseRankAppear()          => OnRankAppear?.Invoke();
    public static void RaiseRankResult()          => OnRankResult?.Invoke();
    public static void RaiseHitTick()             => OnHitTick?.Invoke();
}
