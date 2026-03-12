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

    private bool _isGoodObject; 

    private PlayerMovement _playerMovement;

    public Action<int,int> indexChange;

    // EVENT SCORE //
    public static Action OnGoodObjectCollected;

    // EVENT EXPLOSION //
    
    public static Action<int> OnExplosion;

    private const int EXPLOSION_INDEX = 4;

    private void Awake()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void Init(GameObject NewObject)
    {
        _objectfalling = NewObject;

        // détecte si c'est un bon ou mauvais objet
        if (_objectfalling.CompareTag("Good"))
            _isGoodObject = true;
        else
            _isGoodObject = false;

        MoveObject();
    }

    private void OnEnable()
    {
        _timeManager.OnTimePassed += MoveObject;
        _playerMovement.ObjectSmash += DestroyObject;
    }

    private void OnDisable()
    {
        _timeManager.OnTimePassed -= MoveObject;
        _playerMovement.ObjectSmash -= DestroyObject;
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

            // EXPLOSION SI MAUVAIS OBJET ARRIVE AU PLAYER
            if (!_isGoodObject && _index == EXPLOSION_INDEX)
            {
                Explode();
            }
        }
        else
        {
            Destroy(_objectfalling);
            _audioEventDispatcher.Playaudio(_destruction);
            _index = -1;
        }
    }

    private void DestroyObject()
    {
        if (_objectfalling == null)
            return;

        if (_isGoodObject)
        {
            // EVENT SCORE
            OnGoodObjectCollected?.Invoke();
        }

        Destroy(_objectfalling);
        _index = -1;
    }

    private void Explode()
    {
        if (_objectfalling == null)
            return;

        Destroy(_objectfalling);

        _audioEventDispatcher.Playaudio(_destruction);

        // EXPLOSION SUR LA LIGNE
        OnExplosion?.Invoke(idLine);

        // EXPLOSION LIGNE GAUCHE
        if (idLine - 1 >= 0)
            OnExplosion?.Invoke(idLine - 1);

        // EXPLOSION LIGNE DROITE
        if (idLine + 1 < 4) // 4 = nombre max de lignes (à adapter)
            OnExplosion?.Invoke(idLine + 1);

        _index = -1;
    }
}