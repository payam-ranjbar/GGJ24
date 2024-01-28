using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiBehaviour : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField]
    private float _escapeDuration = 1.0f;
    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private CharacterController _characterController;
    
    private Bomb _targetBomb = null;
    private CharacterController _targetPlayer = null;
    private Transform _freeSpot = null;
    NavMeshPath _path = null;
    private float _escapeRemainingTime = 0.0f;
    private List<CharacterController> _others = new List<CharacterController>();

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
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    private void Start()
    {
        _path = new NavMeshPath();

        _others.Clear();
        var ccs = FindObjectsOfType<CharacterController>();
        foreach (var cc in ccs)
        {
            if (cc != _characterController)
            {
                _others.Add(cc);
            }
        }
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;

        for (int i = _others.Count - 1; i >= 0; --i)
        {
            if (_others[i].destroyed == true)
            {
                _others.RemoveAt(i);
            }
        }

        if (_others.Count <= 0)
        {
            _characterController.movementDirection = Vector3.zero;
            return;
        }

        if (_characterController.canSlap == true && _characterController.hasBomb == false)
        {
            //Debug.Log("Slapping");
            _characterController.Slap();
        }

        var bombs = BombSpawnSystem.instance.bombs;

        var playerPosition = FindPositionOnNavMesh(_characterController.position, out var success2);
        if (success2 == false)
        {
            //Debug.Log("Failed to find player position");
            return;
        }

        Vector3 movementDirection = Vector3.zero;

        if (_characterController.hasBomb == false)
        {
            Bomb closestBomb = null;
            float leastDistance = 0.0f;
            foreach (var bomb in bombs)
            {
                if (bomb.IsAttachedOnce == true)
                {
                    var distance = (bomb.transform.position - playerPosition).magnitude;
                    if (closestBomb == null || distance < leastDistance)
                    {
                        leastDistance = distance;
                        closestBomb = bomb;
                    }
                }
            }

            if (leastDistance < 4.0f && closestBomb != null)
            {
                //Debug.Log("Escaping bomb");
                movementDirection = -CalculateMovementDirection(playerPosition, closestBomb.transform.position, out var success);
                _characterController.movementDirection = new Vector2(movementDirection.x, movementDirection.z);
                return;
            }
        }

        if (_escapeRemainingTime > 0.0f && _targetBomb == null)
        {
            _escapeRemainingTime -= deltaTime;
            //Debug.Log("Moving to a free spot");
            var freeSpots = BombSpawnSystem.instance.freeSpots;
            if (freeSpots.Count > 0)
            {
                if (_freeSpot == null || (playerPosition - _freeSpot.position).sqrMagnitude < 10.0f)
                {
                    _freeSpot = freeSpots[Random.Range(0, freeSpots.Count)].SpawnPoint.transform;
                    return;
                }
                movementDirection = CalculateMovementDirection(playerPosition, _freeSpot.position, out var success);
            }
            if (_escapeRemainingTime <= 0.0f)
            {
                _escapeRemainingTime = 0.0f;
                _targetBomb = null;
                _freeSpot = null;
            }
        }
        else if (_characterController.hasBomb == true)
        {
            //Debug.Log("Has bomb");
            if (_targetPlayer == null || _targetPlayer.destroyed == true)
            {
                _targetPlayer = _others[Random.Range(0, _others.Count)];
            }
            var target = Vector3.zero;
            bool success = false;
            target = FindPositionOnNavMesh(_targetPlayer.position, out success);
            movementDirection = (target - _characterController.position).normalized;
            movementDirection.y = 0.0f;
            if (Vector3.Dot(movementDirection, _characterController.transform.forward) > 0.9)
            {
                _characterController.Slap();
                _escapeRemainingTime = Random.Range(_escapeDuration, _escapeDuration * 2.0f);
                _targetPlayer = null;
            }
        }
        else
        {
            if (_targetBomb == null)
            {
                //Debug.Log("Choosing new bomb");
                if (bombs.Count > 0)
                {
                    var bomb = bombs[Random.Range(0, bombs.Count)];
                    if (bomb != null && bomb.IsAttachedOnce == false)
                    {
                        _targetBomb = bomb;
                        _targetBomb.Exploded += OnBombExploded;
                    }
                }
            }

            if (_targetBomb != null)
            {
                //Debug.Log("Going to bomb");
                movementDirection = CalculateMovementDirection(
                    playerPosition, 
                    _targetBomb.transform.position,
                    out bool success
                );

                if (success == false || _targetBomb.IsAttachedOnce == true)
                {
                    _targetBomb = null;
                }
            }
        }
        //Debug.Log("Movement direction:" + movementDirection);
        _characterController.movementDirection = new Vector2(movementDirection.x, movementDirection.z);
    }

    private void OnBombExploded(Explosion explosion)
    {
        //Debug.Log("OnBomb exploded");
        _targetBomb = null;
    }

    private Vector3 FindPositionOnNavMesh(Vector3 position, out bool success)
    {
        NavMeshHit hit;

        success = false;
        if (NavMesh.SamplePosition(position, out hit, 1000, NavMesh.AllAreas))
        {
            position = hit.position;
            success = true;
        }
        
        return position;
    }

    private Vector3 CalculateMovementDirection(Vector3 myPosition, Vector3 targetPosition, out bool success)
    {
        Vector3 movementDirection = Vector3.zero;
        
        targetPosition = FindPositionOnNavMesh(targetPosition, out success);
        
        NavMesh.CalculatePath(
            myPosition,
            targetPosition,
            NavMesh.AllAreas,
            _path
        );

        if (_path.corners.Length > 1)
        {
            targetPosition = _path.corners[1];
            movementDirection = (targetPosition - myPosition).normalized;
        }

        return movementDirection;
    }

}
