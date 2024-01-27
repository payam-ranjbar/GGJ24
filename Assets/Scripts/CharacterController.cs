using System.Collections.Generic;
using Matchbox;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField]
    private float _speed = 10.0f;
    [SerializeField]
    private float _maxHealth = 1.0f;
    [SerializeField]
    private float _rotationSpeed = 500.0f;
    [SerializeField]
    private float _slapCooldown = 1.0f;
    [SerializeField]
    private float _slapDuration = 1.0f;
    [SerializeField]
    private float _slapSpeed = 40.0f;

    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private CollisionEventReceiver _slapEventReceiver;
    
    public Vector2 movementDirection = Vector2.zero;
    public bool applyMovement = true;
    public new Transform transform => _rigidbody.transform;
    public Rigidbody rigidbody => _rigidbody;

    private List<CharacterController> _slapCharacters = new List<CharacterController>();

    private float _slapRemainingTime = 0.0f;
    private Vector2 _slapDirection = Vector2.zero;
    private float _slapCooldownEndTime = -1000.0f;

    private void OnValidate()
    {
        if (_rootIdentifier == null)
        {
            _rootIdentifier = GetComponentInChildren<RootIdentifier>();
        }

        if (_rootIdentifier != null)
        {
            if (_rigidbody == null)
            {
                _rigidbody = _rootIdentifier.GetComponentInChildren<Rigidbody>();
            }

            if (_slapEventReceiver == null)
            {
                _slapEventReceiver = _rootIdentifier.GetComponentInChildren<CollisionEventReceiver>();
            }
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _slapEventReceiver.triggerEnterAction += SlapTriggerEnter;
        _slapEventReceiver.triggerExitAction += SlapTriggerExit;
    }

    private void Update()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        if (_slapRemainingTime > 0)
        {
            _slapRemainingTime -= deltaTime;
            SetVelocity(_slapDirection * _speed);
            UpdateRotation(deltaTime, _slapDirection);
        }
        else if (applyMovement == true)
        {
            UpdateRotation(deltaTime, movementDirection);
            SetVelocity(movementDirection * _speed);
        }
    }

    private void SetVelocity(Vector2 velocity)
    {
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.y);
    }

    private void UpdateRotation(float deltaTime, Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            _rigidbody.rotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.y)),
                deltaTime * _rotationSpeed
            );
        }
    }

    public void TeleportPosition(Vector3 position)
    {
        var interpolation = _rigidbody.interpolation;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        _rigidbody.MovePosition(position);
        transform.position = position;
        _rigidbody.interpolation = interpolation;
    }

    private void TeleportRotation(Quaternion rotation)
    {
        var interpolation = _rigidbody.interpolation;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        _rigidbody.MoveRotation(rotation);
        transform.rotation = rotation;
        _rigidbody.interpolation = interpolation;
    }

    public void Slap()
    {
        var now = Time.time;
        if (now < _slapCooldownEndTime)
        {
            return;
        }
        _slapCooldownEndTime = now + _slapCooldown;
        foreach (var otherCharacter in _slapCharacters)
        {
            otherCharacter.OnSlapped(transform.forward);
        }
    }

    public void OnSlapped(Vector3 direction)
    {
        _slapRemainingTime = _slapDuration;
        _slapDirection = direction;
    }

    private void SlapTriggerEnter(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var otherCharacter = referenceHolder.GetReferenceComponent<CharacterController>();
            if (otherCharacter != this)
            {
                _slapCharacters.Add(otherCharacter);
            }
        }
    }

    private void SlapTriggerExit(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var otherCharacter = referenceHolder.GetReferenceComponent<CharacterController>();
            _slapCharacters.Remove(otherCharacter);
        }
    }

}
