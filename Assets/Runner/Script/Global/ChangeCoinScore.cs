using TMPro;
using UnityEngine;

public class ChangeCoinScore : MonoBehaviour
{
    private TMP_Text _txt;
    private int _currentCoin;



    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _txt = GetComponent<TMP_Text>();
        _txt.text = _currentCoin.ToString();
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected += AddCoinScore;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= AddCoinScore;
    }

    private void AddCoinScore()
    {
        _currentCoin++;
        _txt.text = _currentCoin.ToString();
    }


}
