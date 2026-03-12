using UnityEngine;

public class Spawner : MonoBehaviour
{
  [SerializeField] private TimeManager _timeManager;
  [SerializeField] private ObjectMovement[] _fallingLines;
  [SerializeField] private GameObject _GoodObjects;
  [SerializeField] private GameObject _BadObjects;
  [SerializeField] private int _spawnTimer = 0;
  [SerializeField] private int _spawnDelayDuration = 3;
  private GameObject ObjectToSpawn;



  private void Start()
  {
    ChangeObject();
  }

private void OnEnable()
  {
    _timeManager.OnTimePassed += TimeGestion;
  }

  private void OnDisable()
  {
    _timeManager.OnTimePassed -= TimeGestion;
  }

  private int random()
  {
    return Random.Range(0, _fallingLines.Length);
  }

  private void TimeGestion()
  {
    _spawnTimer++;
    if (_spawnTimer >= _spawnDelayDuration)
    {
      _spawnTimer = 0;
      _fallingLines[random()].Init(Instantiate(ObjectToSpawn));
      ChangeObject();
    }
  }

  private void ChangeObject()
  {
    int randomObject = Random.Range(0, 2);
    if (randomObject == 0)
    {
      ObjectToSpawn = _GoodObjects;
    }
    else
    {
      ObjectToSpawn = _BadObjects;
    }

  }
}
