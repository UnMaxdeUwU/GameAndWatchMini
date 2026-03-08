using System;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms;
    private int _index = -1;
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private GameObject _objectfalling;
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;
    [SerializeField] private AudioType _objectmovement;
    [SerializeField] private AudioType _destruction;
    [SerializeField] private int idLine;
    private PlayerMovement _playerMovement;
    public Action<int,int> indexChange;



    private void Awake()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }
    
    public void Init(GameObject NewObject)
    {
        _objectfalling = NewObject;
        MoveObject();
    }

    private void OnEnable()
    {
        _timeManager.OnTimePassed += MoveObject;
        _playerMovement.ObjectSmash += Destroy;
    }
    
    private void OnDisable()
    {
        _timeManager.OnTimePassed -= MoveObject;
        _playerMovement.ObjectSmash -= Destroy;
    }

    private void MoveObject()
    {
        if (_objectfalling == null)
        {
            return;
        }
        _index++;
        if (_index < _transforms.Length)
        {
            _objectfalling.transform.position = _transforms[_index].position;
            _audioEventDispatcher.Playaudio(_objectmovement);
            indexChange?.Invoke(idLine, _index);
        }
        else
        {
            Destroy(_objectfalling);
            _audioEventDispatcher.Playaudio(_destruction);
            _index = -1;
        }
    }

    private void Destroy()
    {
        Destroy(_objectfalling);
    }
}
