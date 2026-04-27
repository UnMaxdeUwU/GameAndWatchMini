using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Affiche :
///   - Le score total au format 000000 avec flash blanc sur mise à jour
///   - Un "+{bonus}" animé qui entre depuis le côté à l'expiration du combo
///   - Un texte de félicitation (NICE! → Out Of This World!!!) selon le rank final
/// </summary>
public class ScoreUIFeedback : MonoBehaviour
{
    [Header("Score total")]
    [SerializeField] private TMP_Text _scoreText;

    [Header("Bonus combo (+ {valeur})")]
    [SerializeField] private TMP_Text  _bonusText;
    [SerializeField] private RectTransform _bonusRect;
    [Tooltip("Position de départ du bonus (offset X depuis l'ancre)")]
    [SerializeField] private float _bonusStartOffsetX = -120f;
    [SerializeField] private float _bonusMoveDur      = 0.35f;
    [SerializeField] private float _bonusHoldDur      = 1.0f;
    [SerializeField] private float _bonusFadeDur      = 0.3f;

    [Header("Texte de rank final")]
    [SerializeField] private TMP_Text _rankResultText;
    [Tooltip("Labels par rank index (doit correspondre aux rankThresholds)")]
    [SerializeField] private string[] _rankResultLabels =
        { "NICE!", "GREAT!", "SUPER!", "AMAZING!", "Out Of This World!!!" };
    [SerializeField] private float _rankResultPunchDur  = 0.12f;
    [SerializeField] private float _rankResultPeakScale = 3.5f;
    [SerializeField] private float _rankResultHoldDur   = 1.2f;
    [SerializeField] private float _rankResultFadeDur   = 0.4f;

    [Header("Flash blanc score")]
    [SerializeField] private float _scoreFlashDur = 0.06f;

    private Vector2 _bonusAnchoredPos;
    private Coroutine _bonusRoutine;
    private Coroutine _rankResultRoutine;
    private Coroutine _scoreFlashRoutine;

    private void Awake()
    {
        _bonusAnchoredPos = _bonusRect.anchoredPosition;
        _scoreText.text   = "000000";
        _bonusText.alpha  = 0f;
        _rankResultText.alpha = 0f;
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged    += HandleScoreChanged;
        ScoreManager.OnBonusScoreAdded += HandleBonusAdded;
        ComboManager.OnComboExpired    += HandleComboExpired;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged    -= HandleScoreChanged;
        ScoreManager.OnBonusScoreAdded -= HandleBonusAdded;
        ComboManager.OnComboExpired    -= HandleComboExpired;
    }

    // ── Score total ──────────────────────────────────────────────────────────

    private void HandleScoreChanged(int score)
    {
        _scoreText.text = score.ToString("D6");

        if (_scoreFlashRoutine != null) StopCoroutine(_scoreFlashRoutine);
        _scoreFlashRoutine = StartCoroutine(ScoreFlash());
    }

    private IEnumerator ScoreFlash()
    {
        _scoreText.color = Color.white;
        yield return new WaitForSecondsRealtime(_scoreFlashDur);
        _scoreText.color = Color.white; // le score reste blanc — le flash est subtil via alpha
        // Pour un flash plus visible, on peut booster le glow via material mais ici on pulse l'alpha
        _scoreText.alpha = 0.5f;
        yield return new WaitForSecondsRealtime(_scoreFlashDur);
        _scoreText.alpha = 1f;
        _scoreFlashRoutine = null;
    }

    // ── Bonus combo animé ────────────────────────────────────────────────────

    private void HandleBonusAdded(int bonus, Color rankColor)
    {
        if (_bonusRoutine != null) StopCoroutine(_bonusRoutine);
        _bonusRoutine = StartCoroutine(BonusRoutine(bonus, rankColor));
    }

    private IEnumerator BonusRoutine(int bonus, Color color)
    {
        _bonusText.text  = $"+{bonus}";
        _bonusText.color = color;
        _bonusText.alpha = 0f;

        // Position de départ décalée
        Vector2 startPos = _bonusAnchoredPos + new Vector2(_bonusStartOffsetX, 0f);
        _bonusRect.anchoredPosition = startPos;

        // Entrée : déplacement + fade in simultanés
        float t = 0f;
        while (t < _bonusMoveDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _bonusMoveDur, 3f); // ease out cubic
            _bonusRect.anchoredPosition = Vector2.Lerp(startPos, _bonusAnchoredPos, ease);
            _bonusText.alpha = Mathf.Lerp(0f, 1f, t / _bonusMoveDur);
            yield return null;
        }

        _bonusRect.anchoredPosition = _bonusAnchoredPos;
        _bonusText.alpha = 1f;

        // Maintien
        yield return new WaitForSecondsRealtime(_bonusHoldDur);

        // Fade out
        t = 0f;
        while (t < _bonusFadeDur)
        {
            t += Time.unscaledDeltaTime;
            _bonusText.alpha = Mathf.Lerp(1f, 0f, t / _bonusFadeDur);
            yield return null;
        }

        _bonusText.alpha = 0f;
        _bonusRoutine = null;
    }

    // ── Texte de rank final ──────────────────────────────────────────────────

    private void HandleComboExpired(int finalCombo, int finalRank)
    {
        if (finalRank < 0 || finalRank >= _rankResultLabels.Length) return;

        if (_rankResultRoutine != null) StopCoroutine(_rankResultRoutine);
        _rankResultRoutine = StartCoroutine(RankResultRoutine(finalRank));
    }

    private IEnumerator RankResultRoutine(int rankIndex)
    {
        _rankResultText.text  = _rankResultLabels[rankIndex];
        _rankResultText.alpha = 1f;
        _rankResultText.transform.localScale = Vector3.zero;

        // Grossissement très rapide (punch brutal)
        float t = 0f;
        while (t < _rankResultPunchDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _rankResultPunchDur, 2f);
            float s    = Mathf.Lerp(0f, _rankResultPeakScale, ease);
            _rankResultText.transform.localScale = Vector3.one * s;
            yield return null;
        }

        // Retour à taille normale rapidement
        t = 0f;
        float retractDur = _rankResultPunchDur * 0.8f;
        while (t < retractDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / retractDur, 3f);
            float s    = Mathf.Lerp(_rankResultPeakScale, 1f, ease);
            _rankResultText.transform.localScale = Vector3.one * s;
            yield return null;
        }

        _rankResultText.transform.localScale = Vector3.one;

        // Maintien
        yield return new WaitForSecondsRealtime(_rankResultHoldDur);

        // Fade out
        t = 0f;
        while (t < _rankResultFadeDur)
        {
            t += Time.unscaledDeltaTime;
            _rankResultText.alpha = Mathf.Lerp(1f, 0f, t / _rankResultFadeDur);
            yield return null;
        }

        _rankResultText.alpha = 0f;
        _rankResultRoutine = null;
    }
}
