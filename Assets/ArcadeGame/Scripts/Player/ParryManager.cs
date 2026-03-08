using UnityEngine;
using System.Collections;

public class ParryManager : MonoBehaviour
{
    private Coroutine parryRoutine;

    public bool ParryActive { get; private set; }

    [SerializeField] private float parryWindow = 0.25f;

    public void StartParryWindow()
    {
        if (parryRoutine != null)
            StopCoroutine(parryRoutine);

        parryRoutine = StartCoroutine(ParryCoroutine());
    }

    IEnumerator ParryCoroutine()
    {
        ParryActive = true;
        yield return new WaitForSeconds(parryWindow);
        ParryActive = false;
        parryRoutine = null;
    }
}