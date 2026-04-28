using UnityEngine;

public class Spawner_Enemy : MonoBehaviour
{
    [Header("Normal Enemies")]
    [SerializeField] private Transform[] spawner;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameObject enemy;

    [SerializeField] private int spawnDelayDuration = 9;
    [SerializeField] private int minimumSpawnDelay = 3;


    [Header("Elite")]
    [SerializeField] private GameObject eliteEnemy;
    [SerializeField] private int killsRequiredForElite = 10;

    [SerializeField] private int eliteOnlySpawnDelay = 8;


    private int spawnTimer;
    private int spawnerIndex;
    private int killCount;

    private bool eliteAlive;
    private bool eliteOnlyMode;



    private void OnEnable()
    {
        timeManager.OnTimePassed += TimeGestion;
        HealthManagerEnemy.OnEnemyKilled += OnEnemyKilled;
        HealthManagerBoss.OnEliteKilled   += OnEliteKilled;
        HealthManagerPlayer.OnPlayerDied  += OnPlayerDied;
    }

    private void OnDisable()
    {
        if (timeManager != null)
            timeManager.OnTimePassed -= TimeGestion;

        HealthManagerEnemy.OnEnemyKilled -= OnEnemyKilled;
        HealthManagerBoss.OnEliteKilled  -= OnEliteKilled;
        HealthManagerPlayer.OnPlayerDied -= OnPlayerDied;
    }

    /// <summary>Désactive le spawner dès que le joueur est mort.</summary>
    private void OnPlayerDied()
    {
        enabled = false;
    }


    private void OnEnemyKilled()
    {
        if (eliteAlive || eliteOnlyMode)
            return;

        killCount++;

        if (killCount >= killsRequiredForElite)
        {
            killCount = 0;

            eliteAlive = true;
            SpawnElite();
        }
    }


    private void OnEliteKilled()
    {
        eliteAlive = false;

        // baisse le nombre requis à chaque élite vaincue
        if (killsRequiredForElite > 1)
            killsRequiredForElite--;

        // accélère le spawn
        spawnDelayDuration -= 2;
        spawnDelayDuration =
            Mathf.Max(spawnDelayDuration, minimumSpawnDelay);


        Debug.Log("Kills requis : " + killsRequiredForElite);


        // mode fin de run
        if (killsRequiredForElite <= 1)
        {
            eliteOnlyMode = true;
            spawnTimer = 0;
            Debug.Log("MODE ELITE ONLY");
        }
    }



    private void TimeGestion()
    {
        spawnTimer++;

        //----------------------------------
        // Mode elite infini
        //----------------------------------
        if (eliteOnlyMode)
        {
            if (spawnTimer >= eliteOnlySpawnDelay)
            {
                spawnTimer = 0;
                SpawnElite();
            }

            return;
        }

        //----------------------------------
        // Pause si élite vivante
        //----------------------------------
        if (eliteAlive)
            return;


        //----------------------------------
        // Spawn normal
        //----------------------------------
        if (spawnTimer >= spawnDelayDuration)
        {
            spawnTimer = 0;
            SpawnRegularEnemy();
        }
    }



    private void SpawnRegularEnemy()
    {
        spawnerIndex = Random.Range(0, spawner.Length);

        Quaternion rot =
            spawnerIndex == 0
                ? Quaternion.identity
                : Quaternion.Euler(0,-180,0);

        Instantiate(
            enemy,
            spawner[spawnerIndex].position,
            rot
        );
    }


    private void SpawnElite()
    {
        Instantiate(
            eliteEnemy,
            spawner[0].position,
            Quaternion.identity
        );

        Debug.Log("Elite Spawned");
    }
}