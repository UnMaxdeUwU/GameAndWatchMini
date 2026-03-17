using UnityEngine;

public class MouvementPlayerArcade : MonoBehaviour
{
    [SerializeField] private InputPlayerManagerCustom inputPlayerManagerCustom;

    private void OnEnable()
    {
        inputPlayerManagerCustom.OnMoveLeft += MoveLeft;
        inputPlayerManagerCustom.OnMoveRight += MoveRight;
    }

    private void OnDisable()
    {
        inputPlayerManagerCustom.OnMoveLeft -= MoveLeft;
        inputPlayerManagerCustom.OnMoveRight -= MoveRight;
    }

    private void MoveLeft()
    {
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }

    private void MoveRight()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
