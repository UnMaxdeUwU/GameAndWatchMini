using TMPro;
using UnityEngine;

public class LifeChangeUi : MonoBehaviour
{
    private TMP_Text _txt;
    [SerializeField] private int _currentlife = 5;



    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _txt = GetComponent<TMP_Text>();
        _txt.text = _currentlife.ToString();
    }

    private void OnEnable()
    {
         HealthManagerRunner.OnHealthChanged += changelifetext;
    }

    private void OnDisable()
    {
        HealthManagerRunner.OnHealthChanged -= changelifetext;
    }


    public void changelifetext()
    {
        _currentlife--;
        _txt.text = _currentlife.ToString();
    }

}
