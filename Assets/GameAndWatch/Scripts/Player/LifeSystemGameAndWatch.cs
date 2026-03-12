using System;
using UnityEngine;

public class LifeSystemGameAndWatch : MonoBehaviour
{
   [SerializeField] private int m_maxLives = 3;
    
    private int m_currentLives;

    public Action<int> OnLifeChanged;
    public Action OnPlayerDeath;

    private void Start()
    {
        m_currentLives = m_maxLives;
        OnLifeChanged?.Invoke(m_currentLives);
    }

    private void OnEnable()
    {
        ObjectMovement.OnExplosion += CheckExplosion;
    }

    private void OnDisable()
    {
        ObjectMovement.OnExplosion -= CheckExplosion;
    }

    private void CheckExplosion(int explosionLine)
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();

        if (player == null)
            return;

        if (player.m_currentIndex == explosionLine)
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        m_currentLives--;

        OnLifeChanged?.Invoke(m_currentLives);

        if (m_currentLives <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("GAME OVER");

        OnPlayerDeath?.Invoke();
    }
}
