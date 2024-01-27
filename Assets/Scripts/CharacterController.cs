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
    [SerializeField] 
    private float _bombThrowSpeed = 20.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _bombThrowUpFactor = 0.5f;
    
    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private CollisionEventReceiver _slapEventReceiver;
    [SerializeField]
    private CollisionEventReceiver _bombPickupEventReceiver;
    [SerializeField]
    private Transform _bombAttachTransform;
    
    public Vector2 movementDirection = Vector2.zero;
    public new Transform transform => _rigidbody.transform;
    public Rigidbody rigidbody => _rigidbody;
    public Transform bombAttackTransform => _bombAttachTransform;

    private List<CharacterController> _slapCharacters = new List<CharacterController>();

    private float _slapRemainingTime = 0.0f;
    private Vector2 _slapDirection = Vector2.zero;
    private float _slapCooldownEndTime = -1000.0f;

    private Quaternion _rotation = Quaternion.identity;

    private Bomb _bomb = null;
    
    private bool _destroyed = false;

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

        _bombPickupEventReceiver.triggerEnterAction += BombPickupTriggerEnter;
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
            SetVelocity(_slapDirection * _slapSpeed);
            UpdateRotation(deltaTime, _slapDirection);
        }
        else
        {
            UpdateRotation(deltaTime, movementDirection);
            SetVelocity(movementDirection * _speed);
        }
        _rigidbody.rotation = _rotation;
    }

    private void SetVelocity(Vector2 velocity)
    {
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.y);
    }

    private void UpdateRotation(float deltaTime, Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            _rotation = Quaternion.RotateTowards(
                _rotation,
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
        if (_bomb != null)
        {
            _bomb.Throw((transform.forward * (1.0f - _bombThrowUpFactor) + transform.up * _bombThrowUpFactor).normalized, _bombThrowSpeed);
        }
        else
        {
            foreach (var otherCharacter in _slapCharacters)
            {
                otherCharacter.OnSlapped(new Vector2(transform.forward.x, transform.forward.z));
            }
        }
    }

    public void OnSlapped(Vector2 direction)
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

    private void BombPickupTriggerEnter(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var bomb = referenceHolder.GetReferenceComponent<Bomb>();
            if (_bomb == null)
            {
                bomb.Attach(this);
                _bomb = bomb;
            }
        }
    }

    public void BombDetached(Bomb bomb)
    {
        if (_bomb == bomb)
        {
            _bomb = null;
        }
    }

    public bool TakeDamage(float damage)
    {
        if (_destroyed)
        {
            return false;
        }
        
        _maxHealth -= damage;
        if (_maxHealth <= 0)
        {
            Die();
        }
        return true;
    }
    
    private void Die()
    {
        _destroyed = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _slapEventReceiver.triggerEnterAction -= SlapTriggerEnter;
        _slapEventReceiver.triggerExitAction -= SlapTriggerExit;
    }
}
