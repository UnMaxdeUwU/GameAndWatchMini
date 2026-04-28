using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the Game Over screen for the GameAndWatch mini-game.
/// Opens when LifeSystemGameAndWatch fires OnPlayerDeath.
/// Stops all sounds, displays the score, handles name input, saves to the leaderboard, shows Top 3.
/// </summary>
public class GameOverManagerGameAndWatch : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";

    [Header("Canvas")]
    [SerializeField] private GameObject _gameOverCanvas;
    [SerializeField] private GameObject _gameUICanvas;

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

    [SerializeField] private LifeSystemGameAndWatch _lifeSystem;

    private readonly LeaderboardControllerGameAndWatch _leaderboard = new LeaderboardControllerGameAndWatch();
    private bool _scoreSubmitted;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _gameOverCanvas.SetActive(false);
        _leaderboardPanel.SetActive(false);
    }

    private void OnEnable()
    {
        _lifeSystem.OnPlayerDeath += OpenCanvas;
    }

    private void OnDisable()
    {
        _lifeSystem.OnPlayerDeath -= OpenCanvas;
    }

    private void Start()
    {
        _nameInputField.onSubmit.AddListener(OnNameSubmitted);
        _restartButton.onClick.AddListener(OnRestart);
        _mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    // ── Canvas ───────────────────────────────────────────────────────────────

    private void OpenCanvas()
    {
        _scoreSubmitted = false;

        StopAllActiveSounds();
        Time.timeScale = 0f;

        if (_gameUICanvas != null)
            _gameUICanvas.SetActive(false);

        int finalScore = ScoreManagerGameAndWatch.Instance != null ? ScoreManagerGameAndWatch.Instance.Score : 0;
        _finalScoreText.text         = finalScore.ToString("D6");
        _nameInputField.text         = string.Empty;
        _nameInputField.interactable = true;
        _leaderboardPanel.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _mainMenuButton.gameObject.SetActive(false);

        _gameOverCanvas.SetActive(true);
        _nameInputField.ActivateInputField();
    }

    // ── Name submission ──────────────────────────────────────────────────────

    /// <summary>Called when the player validates their name (Enter / mobile Done).</summary>
    private void OnNameSubmitted(string value)
    {
        if (_scoreSubmitted) return;

        string playerName = string.IsNullOrWhiteSpace(value) ? "Anonyme" : value.Trim();
        int finalScore    = ScoreManagerGameAndWatch.Instance != null ? ScoreManagerGameAndWatch.Instance.Score : 0;

        _leaderboard.AddEntry(playerName, finalScore);
        _scoreSubmitted = true;

        _nameInputField.interactable = false;

        RefreshLeaderboard();
        _leaderboardPanel.SetActive(true);
        _restartButton.gameObject.SetActive(true);
        _mainMenuButton.gameObject.SetActive(true);
    }

    // ── Action buttons ───────────────────────────────────────────────────────

    private void OnRestart()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenu()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }

    // ── Audio ─────────────────────────────────────────────────────────────────

    private void StopAllActiveSounds()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            source.Stop();

        AudioListener.pause = true;
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
