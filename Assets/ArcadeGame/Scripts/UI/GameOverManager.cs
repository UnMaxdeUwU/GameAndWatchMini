using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the Game Over screen lifecycle:
/// listens to the player death event, pauses gameplay,
/// displays the final score, handles name input,
/// saves to the leaderboard, and shows the Top 3.
///
/// Flow imposé :
///   1. Mort du joueur → canvas ouvert, champ actif
///   2. Le joueur saisit son nom et valide (Entrée / Done)
///   3. Score enregistré, champ verrouillé, leaderboard affiché
///   4. Recommencer ou Retour Menu
/// </summary>
public class GameOverManager : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";

    [Header("Canvas")]
    [SerializeField] private GameObject _gameOverCanvas;

    [Header("Score")]
    [SerializeField] private TMP_Text _finalScoreText;

    [Header("Player Name Input")]
    [SerializeField] private TMP_InputField _nameInputField;

    [Header("Leaderboard (Top 3)")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private TMP_Text[] _leaderboardEntryTexts;

    [Header("Action Buttons")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private readonly LeaderboardController _leaderboard = new LeaderboardController();
    private int _finalScore;
    private bool _scoreSubmitted;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _gameOverCanvas.SetActive(false);
        _leaderboardPanel.SetActive(false);
    }

    private void OnEnable()
    {
        HealthManagerPlayer.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        HealthManagerPlayer.OnPlayerDied -= HandlePlayerDied;
    }

    private void Start()
    {
        // La validation se déclenche via Entrée (desktop) ou Done (mobile)
        _nameInputField.onSubmit.AddListener(OnNameSubmitted);
        _restartButton.onClick.AddListener(OnRestart);
        _mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    // ── Death handling ───────────────────────────────────────────────────────

    private void HandlePlayerDied()
    {
        _finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.TotalScore : 0;
        _scoreSubmitted = false;

        Time.timeScale = 0f;

        _finalScoreText.text = _finalScore.ToString("D6");
        _nameInputField.text = string.Empty;
        _nameInputField.interactable = true;
        _leaderboardPanel.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _mainMenuButton.gameObject.SetActive(false);

        _gameOverCanvas.SetActive(true);

        // Focus automatique sur le champ de saisie
        _nameInputField.ActivateInputField();
    }

    // ── Name submission ──────────────────────────────────────────────────────

    /// <summary>
    /// Called when the player validates their name (Enter / mobile Done).
    /// Saves the entry, locks the field, and reveals the leaderboard.
    /// </summary>
    private void OnNameSubmitted(string value)
    {
        if (_scoreSubmitted) return;

        string playerName = value.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonyme";

        _leaderboard.AddEntry(playerName, _finalScore);
        _scoreSubmitted = true;

        // Verrouille le champ — impossible de réécrire
        _nameInputField.interactable = false;

        RefreshLeaderboard();
        _leaderboardPanel.SetActive(true);
        _restartButton.gameObject.SetActive(true);
        _mainMenuButton.gameObject.SetActive(true);
    }

    // ── Action buttons ───────────────────────────────────────────────────────

    private void OnRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }

    // ── Leaderboard display ──────────────────────────────────────────────────

    private void RefreshLeaderboard()
    {
        List<LeaderboardEntry> top = _leaderboard.GetTopEntries(3);

        for (int i = 0; i < _leaderboardEntryTexts.Length; i++)
        {
            _leaderboardEntryTexts[i].text = i < top.Count
                ? $"{i + 1}. {top[i].PlayerName} — {top[i].Score}"
                : $"{i + 1}. ---";
        }
    }
}
