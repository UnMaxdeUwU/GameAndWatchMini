using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public static event Action CanSwitch;
    [SerializeField] CinemachineCamera _camera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<MovementPlayerRunner>() != null)
        {
            Debug.LogWarning("Enter");
            _camera.Priority = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<MovementPlayerRunner>() != null)
        {
            CanSwitch?.Invoke();
            Debug.LogWarning("Exit");
            _camera.Priority = 0;
        }
    }
}
