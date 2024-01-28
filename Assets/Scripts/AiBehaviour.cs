using UnityEngine;
using UnityEngine.AI;

public class AiBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private NavMeshAgent _agent;

    private Bomb _targetBomb = null;

    private void OnValidate()
    {
        if (_rootIdentifier == null)
        {
            _rootIdentifier = GetComponentInChildren<RootIdentifier>();
        }

        if (_rootIdentifier != null)
        {
            if (_characterController == null)
            {
                _characterController = _rootIdentifier.GetComponentInChildren<CharacterController>();
            }

            if (_agent == null)
            {
                _agent = _rootIdentifier.GetComponentInChildren<NavMeshAgent>();
            }
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    public void Update()
    {
        if (_targetBomb == null)
        {
            var filledSpots = BombSpawnSystem.instance.filledSpots;
            if (filledSpots.Count > 0)
            {
                var spot = filledSpots[Random.Range(0, filledSpots.Count)];
                _targetBomb = spot.bomb;
                _targetBomb.Exploded += OnBombExploded;
            }
        }

        if (_targetBomb != null)
        {
            _agent.SetDestination(_targetBomb.transform.position);
        }
    }

    private void OnBombExploded(Explosion explosion)
    {
        _targetBomb = null;
    }

}
