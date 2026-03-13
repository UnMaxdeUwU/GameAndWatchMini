using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator _animator;
    private bool targetCamera = true;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    private void OnEnable()
    {
        CameraZone.CanSwitch += SwitchState;
    }

    private void OnDisable()
    {
        CameraZone.CanSwitch -= SwitchState;
    }

    private void SwitchState()
    {
        if (targetCamera)
        {
            _animator.Play("TargetCamera");
        }
        else
        {
            _animator.Play("ObstacleCamera");
        }
        targetCamera = !targetCamera;
    }
    
    
}
