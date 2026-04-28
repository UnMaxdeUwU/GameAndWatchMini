using System;
using UnityEngine;

public class ChangeOpacity : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        HealthManagerRunner.OnHealthChanged += ChangeOpacityImage;
    }

    private void OnDisable()
    {
        HealthManagerRunner.OnHealthChanged -= ChangeOpacityImage;
    }

    private void ChangeOpacityImage()
    {
        _animator.SetTrigger("ChangeOpacity");
    }
    
}
