using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
  [SerializeField] private float _timeStepDuration = 1.0f;

  public event Action OnTimePassed;
  
  IEnumerator SpendingTime()
  {
    while (true)
    {
      yield return new WaitForSeconds(_timeStepDuration);
      OnTimePassed?.Invoke();
    }
  }

  private void Start()
  {
    StartTime();
  }

  private void StartTime()
  {
    StartCoroutine(SpendingTime());
  }

  private void StopTime()
  {
    StopCoroutine(SpendingTime());
  }
}
