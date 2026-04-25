using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class ComboController : MonoBehaviour
{
    [SerializeField] private SwordPlayer _swordPlayer;

    [Header("Feedback visuel")]
    [SerializeField] private float punchScale = 1.4f;       // grossissement au hit
    [SerializeField] private float punchDuration = 0.08f;   // durée du punch out
    [SerializeField] private float returnDuration = 0.12f;  // durée du retour à taille normale

    private TMP_Text _txt;
    private int _currentCombo;
    private Coroutine _visibilityRoutine;
    private Coroutine _punchRoutine;
    private Vector3 _baseScale;

    void Start()
    {
        _txt = GetComponent<TMP_Text>();
        _txt.alpha = 0f;
        _baseScale = transform.localScale;
    }

    private void OnEnable()  => _swordPlayer.AddCombo += AddComboText;
    private void OnDisable() => _swordPlayer.AddCombo -= AddComboText;

    public void AddComboText()
    {
        _currentCombo++;
        _txt.alpha = 1f;
        _txt.text = $"Combo x{_currentCombo}";

        // ── Reset le timer de disparition à chaque nouveau hit ──────────────
        if (_visibilityRoutine != null)
            StopCoroutine(_visibilityRoutine);
        _visibilityRoutine = StartCoroutine(ComboVisibility());
        // ────────────────────────────────────────────────────────────────────

        // ── Punch scale : grossit puis revient, annule le précédent ─────────
        if (_punchRoutine != null)
            StopCoroutine(_punchRoutine);
        _punchRoutine = StartCoroutine(PunchScale());
        // ────────────────────────────────────────────────────────────────────

        // ── Couleur qui s'intensifie avec le combo ───────────────────────────
        float t = Mathf.InverseLerp(1f, 20f, _currentCombo);
        _txt.color = Color.Lerp(Color.white, new Color(1f, 0.4f, 0f), t); // blanc → orange feu
        // ────────────────────────────────────────────────────────────────────
    }

    // Attend 2s puis fade out sur 5s
    IEnumerator ComboVisibility()
    {
        yield return new WaitForSeconds(2f);

        float duration = 5f;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            _txt.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }

        ResetCombo();
    }

    // Grossit le texte d'un coup puis revient doucement
    IEnumerator PunchScale()
    {
        // Punch out
        float t = 0f;
        while (t < punchDuration)
        {
            t += Time.unscaledDeltaTime;
            float scale = Mathf.Lerp(1f, punchScale, t / punchDuration);
            transform.localScale = _baseScale * scale;
            yield return null;
        }

        // Retour fluide
        t = 0f;
        while (t < returnDuration)
        {
            t += Time.unscaledDeltaTime;
            float scale = Mathf.Lerp(punchScale, 1f, t / returnDuration);
            transform.localScale = _baseScale * scale;
            yield return null;
        }

        transform.localScale = _baseScale;
        _punchRoutine = null;
    }

    private void ResetCombo()
    {
        _txt.alpha = 0f;
        _txt.color = Color.white;
        transform.localScale = _baseScale;
        _currentCombo = 0;
        _visibilityRoutine = null;
        _swordPlayer.ResetCooldownAndSpeed();
    }
}