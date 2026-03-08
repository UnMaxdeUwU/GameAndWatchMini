using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float ShakeAmount = 0.09f;
    private Vector2 InitialPosition;
    public bool CanShake = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanShake)
        {
            transform.position = InitialPosition + Random.insideUnitCircle * ShakeAmount;
        }

    }
}
