using UnityEngine;

public class Spawner_Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] spawner;
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private GameObject Enemy;
    [SerializeField] private int _spawnTimer = 0;
    [SerializeField] private int _spawnDelayDuration = 3;

    [Header("--- Boss ---")]
    [SerializeField] private GameObject bossEnemy;
    [SerializeField] private int killsRequiredForBoss = 10;

    private int spawnerindex;
    private int _killCount = 0;
    private bool _bossSpawned = false;

    private void OnEnable()
    {
        _timeManager.OnTimePassed += TimeGestion;
        HealthManagerEnemy.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        _timeManager.OnTimePassed -= TimeGestion;
        HealthManagerEnemy.OnEnemyKilled -= OnEnemyKilled;
    }

    /// <summary>
    /// À appeler depuis HealthManagerEnemy quand un ennemi basique meurt.
    /// </summary>
    public void OnEnemyKilled()
    {
        if (_bossSpawned) return;

        _killCount++;
        if (_killCount >= killsRequiredForBoss)
            SpawnBoss();
    }

    private void random()
    {
        spawnerindex = Random.Range(0, spawner.Length);
    }

    private void TimeGestion()
    {
        // Plus de spawn basique si le boss est là
        if (_bossSpawned) return;

        _spawnTimer++;
        if (_spawnTimer >= _spawnDelayDuration)
        {
            _spawnTimer = 0;
            random();
            if (spawnerindex == 0)
                Instantiate(Enemy, spawner[spawnerindex].position, Quaternion.identity);
            else
                Instantiate(Enemy, spawner[spawnerindex].position, Quaternion.Euler(0f, -180f, 0f));
        }
    }

    private void SpawnBoss()
    {
        _bossSpawned = true;
        // Toujours sur le spawner[0], face droite
        Instantiate(bossEnemy, spawner[0].position, Quaternion.identity);
        Debug.Log("Boss spawné !");
    }
}