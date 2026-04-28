using System;

/// <summary>
/// Bus d'events statiques pour le GameAndWatch.
/// Chaque système du jeu raise l'event correspondant sans connaître l'AudioManager.
/// </summary>
public static class GameAndWatchAudioEvents
{
    // ── Joueur ───────────────────────────────────────────────────────────────
    public static event Action OnPlayerMove;

    // ── Objets ───────────────────────────────────────────────────────────────
    public static event Action OnObjectChangeLine;
    public static event Action OnObjectCollected;
    public static event Action OnWrongObjectExplode;

    // ── Méthodes d'invocation ─────────────────────────────────────────────────
    public static void RaisePlayerMove()           => OnPlayerMove?.Invoke();
    public static void RaiseObjectChangeLine()     => OnObjectChangeLine?.Invoke();
    public static void RaiseObjectCollected()      => OnObjectCollected?.Invoke();
    public static void RaiseWrongObjectExplode()   => OnWrongObjectExplode?.Invoke();
}
