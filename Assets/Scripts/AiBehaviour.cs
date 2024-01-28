using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiBehaviour : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField]
    private float _escapeDuration = 3.0f;
    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private CharacterController _characterController;
    
    private Bomb _targetBomb = null;
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

        if (_others.Count <= 0)
        {
            return;
        }

        for (int i = _others.Count - 1; i >= 0; --i)
        {
            if (_others[i].destroyed == true)
            {
                _others.RemoveAt(i);
            }
        }

        //if (_characterController.canSlap == true && _characterController.hasBomb == false)
        //{
        //    _characterController.Slap();
        //}

        var bombs = BombSpawnSystem.instance.bombs;
        var playerPosition = FindPositionOnNavMesh(_characterController.position, out var success2);
        if (success2 == false)
        {
            return;
        }

        Vector3 movementDirection = Vector3.zero;
        //if (_escapeRemainingTime > 0.0f && _targetBomb == null)
        //{
        //    _escapeRemainingTime -= deltaTime;

        //    //var freeSpots = BombSpawnSystem.instance.freeSpots;
        //    //if (freeSpots.Count > 0)
        //    //{
        //    //    //var targetPosition = freeSpots[Random.Range(0, freeSpots.Count)].SpawnPoint.transform.position;
        //    //    movementDirection = CalculateMovementDirection(playerPosition, targetPosition);
        //    //}
        //    if (_escapeRemainingTime <= 0.0f)
        //    {
        //        _escapeRemainingTime = 0.0f;
        //        _targetBomb = null;
        //    }
        //}
        //else 
        if (_characterController.hasBomb == true)
        {
            var target = Vector3.zero;
            target = _others[Random.Range(0, _others.Count)].position;
            movementDirection = (target - _characterController.position).normalized;
            movementDirection.y = 0.0f;
            if (Vector3.Dot(movementDirection, _characterController.transform.forward) > 0.9)
            {
                _characterController.Slap();
                //movementDirection = -movementDirection;
                _escapeRemainingTime = _escapeDuration;
            }
        }
        else
        {
            if (_targetBomb == null)
            {
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
        _characterController.movementDirection = new Vector2(movementDirection.x, movementDirection.z);
    }

    private void OnBombExploded(Explosion explosion)
    {
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
