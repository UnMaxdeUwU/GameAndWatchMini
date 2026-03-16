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
        HealthManagerPlayer.OnHealthChanged += ChangeOpacityImage;
    }

    private void OnDisable()
    {
        HealthManagerPlayer.OnHealthChanged -= ChangeOpacityImage;
    }

    private void ChangeOpacityImage()
    {
        _animator.SetTrigger("ChangeOpacity");
    }
    
}
