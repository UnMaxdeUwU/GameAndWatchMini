using TMPro;
using UnityEngine;

public class LifeChangeUi : MonoBehaviour
{
    private TMP_Text _txt;
    private int _currentlife = 3;



    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _txt = GetComponent<TMP_Text>();
        _txt.text = _currentlife.ToString();
    }

    private void OnEnable()
    {
         HealthManagerPlayer.OnHealthChanged+= changelifetext;
    }

    private void OnDisable()
    {
        HealthManagerPlayer.OnHealthChanged-= changelifetext;
    }


    public void changelifetext()
    {
        _currentlife--;
        _txt.text = _currentlife.ToString();
    }

}
