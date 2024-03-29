using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Matchbox;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour
{
    public int LifeTime = 10; // In seconds
    public float ExplosionForce = 1000f;
    public float ExplosionRadius = 1f;
    public GameObject visuals;
    public float destroyTime = 5f;
    [SerializeField] private float _gravity = 100.0f;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CollisionEventReceiver _charactersReceiver;
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _explosionParticle;

    public event Action<Explosion> Exploded;
    public UnityEvent ExplodedEvent;
    public UnityEvent SpawnEvent;

    private float _currentLifeTime;
    private List<CharacterController> _charactersInBlastZone = new List<CharacterController>();

    private CharacterController _character = null;
    public new Transform transform => _rigidbody.transform;

    public bool IsAttachedOnce = false;

    public float TimerCountdown => _currentLifeTime;
    
    private void Awake()
    {
        OnValidate();
       _charactersReceiver.triggerEnterAction += OnCharacterTriggerEnter;
       _charactersReceiver.triggerExitAction += OnCharacterTriggerExit;
    }

    private void OnValidate()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }
    }
    
    private void OnCharacterTriggerExit(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var otherCharacter = referenceHolder.GetReferenceComponent<CharacterController>();
            _charactersInBlastZone.Remove(otherCharacter);
        }
    }

    private void OnCharacterTriggerEnter(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var otherCharacter = referenceHolder.GetReferenceComponent<CharacterController>();
            if (otherCharacter != this)
            {
                _charactersInBlastZone.Add(otherCharacter);
            }
        }
    }

    private void Start()
    {
        _currentLifeTime = 0f;
        GameObject.Instantiate(_explosionParticle, _rigidbody.transform);
        SpawnEvent?.Invoke();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        Countdown();
        if (_character != null)
        {
            _rigidbody.MovePosition(_character.bombAttackTransform.position);
            _rigidbody.velocity = Vector3.zero;
        }
        else
        {
            _rigidbody.velocity -= Vector3.up * _gravity * deltaTime * _rigidbody.mass;
        }

        if (_rigidbody.position.y < -2.0f)
        {
            Explode();
        }
    }

    private void Countdown()
    {
        var modifier = 1.0f;
        if (IsAttachedOnce == true)
        {
            modifier = 2.0f;
        }
        _currentLifeTime += Time.deltaTime * modifier;
        if (_currentLifeTime >= LifeTime)
        {
            ExplodeInternal();
        }
    }

    public void Explode()
    {
        ExplodeInternal();
    }

    private void ExplodeInternal()
    {
        Exploded?.Invoke(new Explosion
        {
            Force = ExplosionForce,
            Radius = ExplosionRadius,
            Position = transform.position
        });
        
        CameraManager.Instance.ExplosiveEffect();
        var damageTaken = TakeDamage();

        if (AudioManager.Instance != null)
        {
            if (damageTaken)
            {
                AudioManager.Instance.Play(AudioCommand.Hit);
            }
            else
            {
                AudioManager.Instance.Play(AudioCommand.BombExplosion);    
            }            
        }
        
        ExplodedEvent?.Invoke();
        enabled = false;
        GameObject.Instantiate(_explosionParticle, _rigidbody.transform.position, _rigidbody.transform.rotation);
        Destroy(gameObject, destroyTime);
        //Debug.Log("Exploded by count down!");
    }

    public void Attach(CharacterController character)
    {
        if (_character != character)
        {
            Detach();
        }

        //_collider.enabled = false;

        IsAttachedOnce = true;
        _character = character;
    }

    public void Throw(Vector3 direction, float speed)
    {
        Detach();
        _rigidbody.velocity = direction * speed;
    }

    private void Detach()
    {
        //_collider.enabled = true;
        if (_character != null)
        {
            _character.BombDetached(this);
            _character = null;
        }
    }

    private bool TakeDamage()
    {
        var damageTaken = false;
        foreach (var otherCharacter in _charactersInBlastZone)
        {
            damageTaken = otherCharacter.TakeDamage(ExplosionForce);
        }
        return damageTaken;
    }

}