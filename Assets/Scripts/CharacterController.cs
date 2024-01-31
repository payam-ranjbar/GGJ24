using System.Collections.Generic;
using DefaultNamespace;
using Matchbox;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterAnimation))]
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

    [SerializeField] private bool _isPlayer;
    public float speed => _speed;

    public Vector2 movementDirection = Vector2.zero;
    public new Transform transform => _rigidbody.transform;
    public Vector3 position => _rigidbody.position;
    public Quaternion rotation => _rigidbody.rotation;
    public Rigidbody rigidbody => _rigidbody;
    public Transform bombAttackTransform => _bombAttachTransform;

    private List<CharacterController> _slapCharacters = new List<CharacterController>();
    public bool canSlap => _slapCharacters.Count > 0;

    private float _slapRemainingTime = 0.0f;
    private Vector2 _slapDirection = Vector2.zero;
    private float _slapCooldownEndTime = -1000.0f;

    private Quaternion _rotation = Quaternion.identity;

    private Bomb _bomb = null;
    public bool hasBomb => _bomb != null;

    private bool _destroyed = false;
    public bool destroyed => _destroyed;
    
    private CharacterAnimation _characterAnimation;

    public UnityEvent CharacterDie;
    
    [SerializeField] private float footStepInterval;
    [SerializeField] private float footStepTimer;
    [SerializeField] private AudioSource footStepAudio;

    
    private void PlayFootstep()
    {


        if(footStepAudio ==  null) return;
        Debug.Log("not");

        if (footStepTimer <= footStepInterval)
        {
            footStepTimer += Time.deltaTime;
        }
        else
        {
            footStepAudio.Play();
            Debug.Log("played");
            footStepTimer = 0f;
        }
        
    }
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
        _characterAnimation = GetComponent<CharacterAnimation>();
        OnValidate();
        _characterAnimation.SetOffestCycle();
    }

    public void RotateRandomDirection()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep the rotation in the horizontal plane

        Quaternion targetRotation = Quaternion.LookRotation(randomDirection, Vector3.up);

        // Snap to the target rotation immediately
        _rotation = targetRotation;
    }
    // Start is called before the first frame update
    private void Start()
    {
        _slapEventReceiver.triggerEnterAction += SlapTriggerEnter;
        _slapEventReceiver.triggerExitAction += SlapTriggerExit;

        _bombPickupEventReceiver.triggerEnterAction += BombPickupTriggerEnter;
        if (!_isPlayer)
        {
            RotateRandomDirection();
        }
    }

    private void Update()
    {
        _characterAnimation.IsPicking = _bomb != null;
        if (_isRunning)
        {
            PlayFootstep();
        }

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

        if (_rigidbody.position.y < -2.0f)
        {
            TakeDamage(_maxHealth + 10.0f);
        }
    }

    private bool _isRunning;
    private void SetVelocity(Vector2 velocity)
    {
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.y);
        if (velocity.sqrMagnitude > 0.0f)
        {
            _isRunning = true;
            _characterAnimation.Run();
        }
        else
        {
            _isRunning = false;
            _characterAnimation.Idle();
        }
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

    public void Slap(bool isPlayer)
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
            if (_slapCharacters.Count > 0 || isPlayer == true)
            {
                _characterAnimation.Slap(_isPlayer);

            }

            if (_slapCharacters.Count > 0)
            {
                foreach (var otherCharacter in _slapCharacters)
                {
                    otherCharacter.OnSlapped(new Vector2(transform.forward.x, transform.forward.z));
                }
                _slapCharacters.Clear();
            }
        }
    }

    public void OnSlapped(Vector2 direction)
    {
        _slapRemainingTime = _slapDuration;
        _slapDirection = direction;
        _characterAnimation.Pushed(_isPlayer);
        
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
        _characterAnimation.Die(_rigidbody.position, _rigidbody.rotation);
        _destroyed = true;
        _rootIdentifier.gameObject.SetActive(false);
        CharacterDie?.Invoke();
        //Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _slapEventReceiver.triggerEnterAction -= SlapTriggerEnter;
        _slapEventReceiver.triggerExitAction -= SlapTriggerExit;
    }
}
