using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Tracks and displays the player score for the GameAndWatch mini-game.
/// Listens to ObjectMovement.OnGoodObjectCollected.
/// </summary>
public class ScoreManagerGameAndWatch : MonoBehaviour
{
    public static ScoreManagerGameAndWatch Instance { get; private set; }

    /// <summary>Fired whenever the score changes, passes the new value.</summary>
    public static event Action<int> OnScoreChanged;

    public int Score { get; private set; }

    [SerializeField] private TMP_Text _scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        ObjectMovement.OnGoodObjectCollected += AddPoint;
    }

    private void OnDisable()
    {
        ObjectMovement.OnGoodObjectCollected -= AddPoint;
    }

    private void Start()
    {
        Score = 0;
        Refresh();
    }

    private void AddPoint()
    {
        Score++;
        Refresh();
        OnScoreChanged?.Invoke(Score);
    }

    private void Refresh()
    {
        if (_scoreText != null)
            _scoreText.text = Score.ToString("D6");
    }
}
