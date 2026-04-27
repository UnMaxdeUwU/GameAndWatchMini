using System.Collections;
using UnityEngine;
using TMPro;

public class ComboUIFeedback : MonoBehaviour
{
    [Header("Texte du chiffre (ex: 75)")]
    [SerializeField] private TMP_Text _comboText;

    [Header("Texte HITS (positionné en dessous dans le Canvas)")]
    [SerializeField] private TMP_Text _hitsText;

    [Header("Rank splash (BRUTAL, SAVAGE…)")]
    [SerializeField] private TMP_Text _rankText;

    [SerializeField] private ComboConfig  _config;
    [SerializeField] private ComboManager _comboManager;

    [Header("Grossissement progressif")]
    [SerializeField] private float _baseScale    = 1f;
    [SerializeField] private float _maxScale     = 2.2f;
    [SerializeField] private int   _maxScaleCombo = 60;

    [Header("Punch par hit")]
    [SerializeField] private float _punchExtra   = 0.25f;
    [SerializeField] private float _punchOutDur  = 0.05f;
    [SerializeField] private float _punchBackDur = 0.10f;

    [Header("Flash blanc au hit")]
    [SerializeField] private float _flashDuration = 0.04f;

    [Header("Clignotement d'avertissement (combo pas encore perdu)")]
    [SerializeField] private int   _blinkCount    = 5;
    [SerializeField] private float _blinkInterval = 0.09f;

    [Header("Animation de fin (combo expiré)")]
    [SerializeField] private float _shrinkDur = 0.25f;

    // ── État interne ─────────────────────────────────────────────────────────
    private Vector3 _rankBaseScale;
    private float   _targetScale;
    private Color   _currentColor;

    private Coroutine _punchRoutine;
    private Coroutine _rankRoutine;
    private Coroutine _endRoutine;
    private Coroutine _flashRoutine;
    private Coroutine _warnRoutine;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _rankBaseScale = _rankText.transform.localScale;
        _comboText.alpha = 0f;
        _hitsText.alpha  = 0f;
        _rankText.alpha  = 0f;
        _targetScale = _baseScale;
        SetComboScale(_baseScale);
    }

    private void OnEnable()
    {
        ComboManager.OnComboIncreased += HandleComboIncreased;
        ComboManager.OnRankChanged    += HandleRankChanged;
        ComboManager.OnComboWarning   += HandleWarning;
        ComboManager.OnComboExpired   += HandleExpired;
    }

    private void OnDisable()
    {
        ComboManager.OnComboIncreased -= HandleComboIncreased;
        ComboManager.OnRankChanged    -= HandleRankChanged;
        ComboManager.OnComboWarning   -= HandleWarning;
        ComboManager.OnComboExpired   -= HandleExpired;
    }

    // ── Hit : compteur ───────────────────────────────────────────────────────

    private void HandleComboIncreased(int combo)
    {
        // Annule tout en cours (avertissement, fin)
        CancelAll();

        _comboText.alpha = 1f;
        _hitsText.alpha  = 1f;
        _comboText.text  = $"{combo}";
        _hitsText.text   = "hits";

        _currentColor = GetComboColor(combo);

        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashWhite());

        _targetScale = Mathf.Lerp(_baseScale, _maxScale, Mathf.Clamp01((float)combo / _maxScaleCombo));

        if (_punchRoutine != null) StopCoroutine(_punchRoutine);
        _punchRoutine = StartCoroutine(PunchRoutine());
    }

    // ── Avertissement : clignote mais le joueur PEUT reprendre ───────────────

    private void HandleWarning()
    {
        if (_warnRoutine != null) StopCoroutine(_warnRoutine);
        _warnRoutine = StartCoroutine(WarnBlink());
    }

    private IEnumerator WarnBlink()
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _comboText.alpha = 0f;
            _hitsText.alpha  = 0f;
            yield return new WaitForSecondsRealtime(_blinkInterval);
            _comboText.alpha = 1f;
            _hitsText.alpha  = 1f;
            yield return new WaitForSecondsRealtime(_blinkInterval);
        }
        _warnRoutine = null;
    }

    // ── Expiration : le combo EST perdu, animation de fin ────────────────────

    private void HandleExpired(int finalCombo, int finalRank)
    {
        CancelAll();
        if (_endRoutine != null) StopCoroutine(_endRoutine);
        _endRoutine = StartCoroutine(ExpireRoutine());
    }

    private IEnumerator ExpireRoutine()
    {
        // Rétrécissement rapide vers la taille de base
        float startScale = _targetScale;
        float t = 0f;
        while (t < _shrinkDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _shrinkDur, 2f);
            SetComboScale(Mathf.Lerp(startScale, _baseScale, ease));
            yield return null;
        }

        SetComboScale(_baseScale);

        // Fade out final
        t = 0f;
        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / 0.15f);
            _comboText.alpha = a;
            _hitsText.alpha  = a;
            yield return null;
        }

        _comboText.alpha = 0f;
        _hitsText.alpha  = 0f;
        _endRoutine = null;

        // Reset gameplay seulement quand tout est invisible
        _comboManager.ResetCombo();
    }

    // ── Couleurs Streets of Rage 4 ────────────────────────────────────────────

    private Color GetComboColor(int combo)
    {
        if (combo <= 10)
            return Color.Lerp(Color.white, new Color(1f, 1f, 0.4f), combo / 10f);
        if (combo <= 20)
            return Color.Lerp(new Color(1f, 1f, 0.4f), new Color(1f, 0.5f, 0f), (combo - 10f) / 10f);
        if (combo <= 21)
            return new Color(0.4f, 1f, 0.4f);
        if (combo <= 33)
            return Color.Lerp(new Color(0.4f, 1f, 0.4f), new Color(0f, 0.5f, 0f), (combo - 21f) / 12f);
        if (combo <= 34)
            return HexColor("4169E1");
        if (combo <= 55)
            return Color.Lerp(HexColor("4169E1"), new Color(0.6f, 0f, 1f), (combo - 34f) / 21f);
        if (combo <= 56)
            return HexColor("FF196B");
        if (combo <= 70)
            return Color.Lerp(HexColor("FF196B"), HexColor("FF0000"), (combo - 55f) / 15f);
        return HexColor("FF0000");
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }

    // ── Flash blanc au hit ───────────────────────────────────────────────────

    private IEnumerator FlashWhite()
    {
        _comboText.color = Color.white;
        _hitsText.color  = Color.white;
        yield return new WaitForSecondsRealtime(_flashDuration);
        _comboText.color = _currentColor;
        _hitsText.color  = _currentColor;
        _flashRoutine = null;
    }

    // ── Punch scale ──────────────────────────────────────────────────────────

    private IEnumerator PunchRoutine()
    {
        float peak = _targetScale + _punchExtra;

        float t = 0f;
        while (t < _punchOutDur)
        {
            t += Time.unscaledDeltaTime;
            SetComboScale(Mathf.Lerp(_targetScale, peak, t / _punchOutDur));
            yield return null;
        }

        t = 0f;
        while (t < _punchBackDur)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _punchBackDur, 3f);
            SetComboScale(Mathf.Lerp(peak, _targetScale, ease));
            yield return null;
        }

        SetComboScale(_targetScale);
        _punchRoutine = null;
    }

    // ── Scale synchronisé nombre + HITS ──────────────────────────────────────

    private void SetComboScale(float s)
    {
        _comboText.transform.localScale = Vector3.one * s;
        _hitsText.transform.localScale  = Vector3.one * (s * 0.65f);
    }

    // ── Rank splash ──────────────────────────────────────────────────────────

    private void HandleRankChanged(int rankIndex, int combo)
    {
        if (rankIndex < 0 || rankIndex >= _config.rankLabels.Length) return;

        _rankText.text  = _config.rankLabels[rankIndex];
        _rankText.color = _config.rankColors[rankIndex];

        if (_rankRoutine != null) StopCoroutine(_rankRoutine);
        _rankRoutine = StartCoroutine(RankSplash(rankIndex));
    }

    private IEnumerator RankSplash(int rankIndex)
    {
        _rankText.alpha = 1f;
        _rankText.transform.localScale = _rankBaseScale;

        float punchMult = 1f + rankIndex * 0.15f;
        float t = 0f;

        while (t < _config.punchOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(0.3f, _config.basePunchScale * punchMult, t / _config.punchOutDuration);
            _rankText.transform.localScale = _rankBaseScale * s;
            yield return null;
        }

        t = 0f;
        while (t < _config.punchBackDuration)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1f - Mathf.Pow(1f - t / _config.punchBackDuration, 3f);
            _rankText.transform.localScale = _rankBaseScale * Mathf.Lerp(_config.basePunchScale * punchMult, 1f, ease);
            yield return null;
        }

        _rankText.transform.localScale = _rankBaseScale;
        yield return new WaitForSecondsRealtime(0.6f);

        t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            _rankText.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
            yield return null;
        }

        _rankText.alpha = 0f;
        _rankRoutine = null;
    }

    // ── Utilitaire ───────────────────────────────────────────────────────────

    private void CancelAll()
    {
        if (_warnRoutine  != null) { StopCoroutine(_warnRoutine);  _warnRoutine  = null; }
        if (_endRoutine   != null) { StopCoroutine(_endRoutine);   _endRoutine   = null; }
        if (_punchRoutine != null) { StopCoroutine(_punchRoutine); _punchRoutine = null; }

        // Restaure l'alpha si le clignotement l'avait mis à 0
        _comboText.alpha = 1f;
        _hitsText.alpha  = 1f;
    }
}