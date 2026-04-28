using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreUIFeedback : MonoBehaviour
{
    [Header("Score total")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Color    _scoreBaseColor = new Color(1f, 0.9f, 0.2f); // couleur de base réglable
    [SerializeField] private float    _scoreFlashDur  = 0.06f;

    [Header("Bonus combo (+valeur)")]
    [SerializeField] private TMP_Text       _bonusText;
    [SerializeField] private RectTransform  _bonusRect;
    [SerializeField] private float _bonusStartOffsetX = -120f;
    [SerializeField] private float _bonusMoveDur      = 0.35f;
    [SerializeField] private float _bonusHoldDur      = 1.0f;
    [SerializeField] private float _bonusFadeDur      = 0.3f;
    [Tooltip("Scale du texte +bonus au moment de l'entrée")]
    [SerializeField] private float _bonusPeakScale    = 2.2f;

    [SerializeField] private float bonusScale = 1.8f;

    [Header("Texte de rank final (NICE! → Out Of This World!!!)")]
    [SerializeField] private TMP_Text _rankResultText;
    [SerializeField] private string[] _rankResultLabels =
        { "NICE!", "GREAT!", "SUPER!", "AMAZING!", "Out Of This World!!!" };
    [Tooltip("Scale maximum au moment du punch brutal")]
    [SerializeField] private float _rankResultPeakScale = 7f;
    [SerializeField] private float _rankResultPunchDur  = 0.10f;
    [SerializeField] private float _rankResultHoldDur   = 1.2f;
    [SerializeField] private float _rankResultFadeDur   = 0.4f;
    [SerializeField] private float scaleRank = 2.2f;

    // ── État interne ─────────────────────────────────────────────────────────
    private Vector2   _bonusAnchoredPos;
    private Color     _lastComboColor = Color.white; // couleur mémorisée depuis le combo expiré

    private Coroutine _scoreFlashRoutine;
    private Coroutine _bonusRoutine;
    private Coroutine _rankResultRoutine;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _bonusAnchoredPos     = _bonusRect.anchoredPosition;
        _scoreText.text       = "000000";
        _scoreText.color      = _scoreBaseColor;
        _bonusText.alpha      = 0f;
        _rankResultText.alpha = 0f;
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged    += HandleScoreChanged;
        ScoreManager.OnBonusScoreAdded += HandleBonusAdded;
        ComboManager.OnComboExpired    += HandleComboExpired;
        ComboManager.OnComboIncreased  += HandleComboIncreased; // pour mémoriser la couleur courante
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged    -= HandleScoreChanged;
        ScoreManager.OnBonusScoreAdded -= HandleBonusAdded;
        ComboManager.OnComboExpired    -= HandleComboExpired;
        ComboManager.OnComboIncreased  -= HandleComboIncreased;
    }

    // Mémorise la couleur du combo en temps réel pour l'avoir au moment de l'expiration
    private void HandleComboIncreased(int combo)
    {
        _lastComboColor = GetComboColor(combo);
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
        // Flash vers blanc
        _scoreText.color = Color.white;
        yield return new WaitForSecondsRealtime(_scoreFlashDur);
        // Retour à la couleur de base
        _scoreText.color = _scoreBaseColor;
        _scoreFlashRoutine = null;
    }

    // ── Bonus combo animé ────────────────────────────────────────────────────

    private void HandleBonusAdded(int bonus, Color rankColor)
    {
        if (_bonusRoutine != null) StopCoroutine(_bonusRoutine);
        // Le bonus utilise la couleur du rang (déjà correcte depuis ScoreManager)
        _bonusRoutine = StartCoroutine(BonusRoutine(bonus, rankColor));
    }

    private IEnumerator BonusRoutine(int bonus, Color color)
    {
        _bonusText.text  = $"+{bonus}";
        _bonusText.color = color;
        _bonusText.alpha = 0f;
        _bonusText.transform.localScale = Vector3.one * _bonusPeakScale;

        Vector2 startPos = _bonusAnchoredPos + new Vector2(_bonusStartOffsetX, 0f);
        _bonusRect.anchoredPosition = startPos;

        // Entrée : déplacement + fade in + rétrécissement vers scale 1
        float t = 0f;
        while (t < _bonusMoveDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _bonusMoveDur, 3f);
            _bonusRect.anchoredPosition       = Vector2.Lerp(startPos, _bonusAnchoredPos, ease);
            _bonusText.alpha                  = Mathf.Lerp(0f, 1f, t / _bonusMoveDur);
            float s = Mathf.Lerp(_bonusPeakScale, 1f, ease);
            _bonusText.transform.localScale   = Vector3.one * s;
            yield return null;
        }

        _bonusRect.anchoredPosition     = _bonusAnchoredPos;
        _bonusText.alpha                = 1f;
        _bonusText.transform.localScale = Vector3.one * bonusScale;

        yield return new WaitForSecondsRealtime(_bonusHoldDur);

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
        // Utilise la couleur mémorisée du combo au moment où il s'est arrêté
        _rankResultRoutine = StartCoroutine(RankResultRoutine(finalRank, _lastComboColor));
    }

    private IEnumerator RankResultRoutine(int rankIndex, Color color)
    {
        _rankResultText.text  = _rankResultLabels[rankIndex];
        _rankResultText.color = color;
        _rankResultText.alpha = 1f;
        _rankResultText.transform.localScale = Vector3.zero;

        AudioEvents.RaiseRankResult();

        // Punch brutal très rapide vers peak
        float t = 0f;
        while (t < _rankResultPunchDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _rankResultPunchDur, 2f);
            _rankResultText.transform.localScale = Vector3.one * Mathf.Lerp(0f, _rankResultPeakScale, ease);
            yield return null;
        }

        // Retour rapide à taille 1
        t = 0f;
        float retractDur = _rankResultPunchDur * 0.7f;
        while (t < retractDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / retractDur, 3f);
            _rankResultText.transform.localScale = Vector3.one * Mathf.Lerp(_rankResultPeakScale, 1f, ease);
            yield return null;
        }

        _rankResultText.transform.localScale = Vector3.one * scaleRank;

        yield return new WaitForSecondsRealtime(_rankResultHoldDur);

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

    // ── Couleur combo (miroir de ComboUIFeedback) ────────────────────────────

    private static Color GetComboColor(int combo)
    {
        if (combo <= 10) return Color.Lerp(Color.white, new Color(1f, 1f, 0.4f), combo / 10f);
        if (combo <= 20) return Color.Lerp(new Color(1f, 1f, 0.4f), new Color(1f, 0.5f, 0f), (combo - 10f) / 10f);
        if (combo <= 21) return new Color(0.4f, 1f, 0.4f);
        if (combo <= 33) return Color.Lerp(new Color(0.4f, 1f, 0.4f), new Color(0f, 0.5f, 0f), (combo - 21f) / 12f);
        if (combo <= 34) return Hex("4169E1");
        if (combo <= 55) return Color.Lerp(Hex("4169E1"), new Color(0.6f, 0f, 1f), (combo - 34f) / 21f);
        if (combo <= 56) return Hex("FF196B");
        if (combo <= 70) return Color.Lerp(Hex("FF196B"), Hex("FF0000"), (combo - 55f) / 15f);
        return Hex("FF0000");
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}