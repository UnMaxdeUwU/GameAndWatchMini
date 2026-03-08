using System;
using System.Collections;
using UnityEngine;
using TMPro;
public class ComboController : MonoBehaviour
{

    private TMP_Text _txt;
    private int _currentCombo;
    [SerializeField] private SwordPlayer _swordPlayer;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _txt = GetComponent<TMP_Text>();
        _txt.alpha = 0f;
    }

    private void OnEnable()
    {
        _swordPlayer.AddCombo += AddComboText;
    }

    private void OnDisable()
    {
        _swordPlayer.AddCombo -= AddComboText;
    }


    public void AddComboText()
    {

        _txt.alpha = 1f;
        _currentCombo++;
        _txt.text = $"Combo x : {_currentCombo}";
        StartCoroutine(ComboVisibility());
    }


    IEnumerator ComboVisibility()
    {
        float duration = 5f; 
        float time = 0f;


        yield return new WaitForSeconds(2f);
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            _txt.alpha = alpha;
            yield return null;
        }

        _txt.alpha = 0f;
        _currentCombo = 0;
        _swordPlayer.ResetCooldownAndSpeed();
    }
}
