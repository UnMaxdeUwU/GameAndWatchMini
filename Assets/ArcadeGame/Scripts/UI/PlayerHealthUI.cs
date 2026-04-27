using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthManagerPlayer playerHealth;

    [Header("Bars")]
    [SerializeField] private Image instantBar; // blanche
    [SerializeField] private Image delayedBar; // rouge

    [Header("Fill Fix")]
    [SerializeField] private float visibleMaxFill = 0.8f;

    [Header("Chip Damage")]
    [SerializeField] private float delayBeforeDrain = 1f;
    [SerializeField] private float drainSpeed = 1.5f;

    Coroutine delayedRoutine;

    private void OnEnable()
    {
        HealthManagerPlayer.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        HealthManagerPlayer.OnHealthChanged -= UpdateHealthBar;
    }

    private void Start()
    {
        UpdateHealthBar();

        // Les deux pleines au départ (0.8 visuel)
        instantBar.fillAmount = visibleMaxFill;
        delayedBar.fillAmount = visibleMaxFill;
    }

    void UpdateHealthBar()
    {
        float normalizedHealth =
            playerHealth.CurrentHealth / playerHealth.MaxHealth;

        // Remap 0-1 vers 0-0.8
        float targetFill = normalizedHealth * visibleMaxFill;

        // Barre principale = immédiate
        instantBar.fillAmount = targetFill;

        // relance le retard rouge
        if(delayedRoutine != null)
            StopCoroutine(delayedRoutine);

        delayedRoutine = StartCoroutine(AnimateDelayedBar(targetFill));
    }

    IEnumerator AnimateDelayedBar(float targetFill)
    {
        yield return new WaitForSeconds(delayBeforeDrain);

        while (delayedBar.fillAmount > targetFill)
        {
            delayedBar.fillAmount = Mathf.MoveTowards(
                delayedBar.fillAmount,
                targetFill,
                drainSpeed * Time.deltaTime
            );

            yield return null;
        }

        delayedBar.fillAmount = targetFill;
    }
}