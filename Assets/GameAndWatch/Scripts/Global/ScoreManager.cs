using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private TMP_Text _txt;
    private int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Start()
    {
        _txt = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        ObjectMovement.OnGoodObjectCollected += UpdateScore;
    }

    private void OnDisable()
    {
        ObjectMovement.OnGoodObjectCollected -= UpdateScore;
    }
    private void UpdateScore()
    {
        score++;
        _txt.text = $"{score}";
    }
}
