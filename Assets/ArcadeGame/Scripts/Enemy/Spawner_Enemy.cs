using UnityEngine;

public class Spawner_Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] spawner;
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private GameObject Enemy;
    [SerializeField] private int _spawnTimer = 0;
    [SerializeField] private int _spawnDelayDuration = 3;

    private int spawnerindex;

    private void OnEnable()
    {
        _timeManager.OnTimePassed += TimeGestion;
    }

    private void OnDisable()
    {
        _timeManager.OnTimePassed -= TimeGestion;
    }

    private void random()
    {
         spawnerindex = Random.Range(0, spawner.Length);
    }

    private void TimeGestion()
    {
        _spawnTimer++;
        if (_spawnTimer >= _spawnDelayDuration)
        {
            _spawnTimer = 0;
            random();
            if (spawnerindex == 0)
            {
                Instantiate(Enemy,  spawner[spawnerindex].position, Quaternion.identity);
            }
            else
            {
                Instantiate(Enemy, spawner[spawnerindex].position, Quaternion.Euler(0f, -180f, 0f));
            }

        }
    }
}


