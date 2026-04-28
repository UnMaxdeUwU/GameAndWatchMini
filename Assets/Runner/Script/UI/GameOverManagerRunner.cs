using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the end-of-run screen for the Runner mode.
/// Opens when the player dies (HealthManagerRunner.OnPlayerDied)
/// or reaches the end (End.OnEndReached).
/// Hides the Health canvas, displays the coin score, handles name input,
/// saves to the Runner-specific leaderboard, and shows the Top 3.
/// </summary>
public class GameOverManagerRunner : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";

    [Header("Canvas")]
    [SerializeField] private GameObject _gameOverCanvas;
    [SerializeField] private GameObject _healthCanvas;

    [Header("Title")]
    [SerializeField] private TMP_Text _titleText;

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

    private const string TitleDead = "GAME OVER";
    private const string TitleWin  = "YOU WIN!";

    private readonly LeaderboardControllerRunner _leaderboard = new LeaderboardControllerRunner();
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
        HealthManagerRunner.OnPlayerDied += HandlePlayerDied;
        End.OnEndReached                 += HandleEndReached;
    }

    private void OnDisable()
    {
        HealthManagerRunner.OnPlayerDied -= HandlePlayerDied;
        End.OnEndReached                 -= HandleEndReached;
    }

    private void Start()
    {
        _nameInputField.onSubmit.AddListener(OnNameSubmitted);
        _restartButton.onClick.AddListener(OnRestart);
        _mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    private void HandlePlayerDied()
    {
        OpenCanvas(TitleDead);
    }

    private void HandleEndReached()
    {
        OpenCanvas(TitleWin);
    }

    // ── Canvas ───────────────────────────────────────────────────────────────

    private void OpenCanvas(string title)
    {
        _finalScore    = CoinScoreManager.Instance != null ? CoinScoreManager.Instance.TotalCoins : 0;
        _scoreSubmitted = false;

        StopAllActiveSounds();
        Time.timeScale = 0f;

        if (_healthCanvas != null)
            _healthCanvas.SetActive(false);

        _titleText.text              = title;
        _finalScoreText.text         = _finalScore.ToString("D6");
        _nameInputField.text         = string.Empty;
        _nameInputField.interactable = true;
        _leaderboardPanel.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _mainMenuButton.gameObject.SetActive(false);

        _gameOverCanvas.SetActive(true);
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

    /// <summary>
    /// Stops and destroys all active AudioSource GameObjects spawned by SoundFXManager,
    /// then pauses the AudioListener so no new sounds play while the menu is open.
    /// Must be called before setting Time.timeScale = 0.
    /// </summary>
    private void StopAllActiveSounds()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            source.Stop();
            // Destroy only the transient GOs spawned by SoundFXManager (not persistent ones)
            if (source.gameObject != SoundFXManager.instance?.gameObject &&
                source.gameObject.scene.IsValid())
            {
                Destroy(source.gameObject);
            }
        }

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
