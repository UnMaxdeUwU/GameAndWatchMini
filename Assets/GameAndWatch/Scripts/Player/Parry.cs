using System;
using UnityEngine;

public class Parry : MonoBehaviour
{
    [SerializeField] private InputPlayerManagerCustom inputPlayerManager;
    [SerializeField] private ParryManager parryManager;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputPlayerManager.OnSwip += TriggerParry;
    }

    private void OnDisable()
    {
        inputPlayerManager.OnSwip -= TriggerParry;
    }

    private void TriggerParry()
    {
        _animator.SetTrigger("Parry");
        parryManager.StartParryWindow();
    }
}
