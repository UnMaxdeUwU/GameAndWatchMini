using System;
using UnityEngine;

/// <summary>
/// Fires OnEndReached when the player enters the End trigger zone.
/// </summary>
public class End : MonoBehaviour
{
    /// <summary>Fired once when the player reaches the end of the level.</summary>
    public static event Action OnEndReached;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<InputManagerCustomRunner>() != null)
        {
            OnEndReached?.Invoke();
        }
    }
}
