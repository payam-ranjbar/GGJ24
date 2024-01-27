using System;
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

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CollisionEventReceiver _charactersReceiver;

    public event Action<Explosion> Exploded;
    public UnityEvent<Explosion> ExplodedEvent;

    private float _currentLifeTime;
    private List<CharacterController> _charactersInBlastZone = new List<CharacterController>();

    private CharacterController _character = null;

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
    }

    private void Update()
    {
        Countdown();
        if (_character != null)
        {
            _rigidbody.MovePosition(_character.bombAttackTransform.position);
            _rigidbody.velocity = Vector3.zero;
        }
    }

    private void Countdown()
    {
        _currentLifeTime += Time.deltaTime;
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(AudioCommand.BombExplosion);
        }
        
        TakeDamage();

        Destroy(gameObject);
        Debug.Log("Exploded by count down!");
    }

    public void Attach(CharacterController character)
    {
        if (_character != character)
        {
            Detach();
        }
        _character = character;
    }

    public void Throw(Vector3 direction, float speed)
    {
        Detach();
        _rigidbody.velocity = direction * speed;
    }

    private void Detach()
    {
        if (_character != null)
        {
            _character.BombDetached(this);
            _character = null;
        }
    }

    private void TakeDamage()
    {
        foreach (var otherCharacter in _charactersInBlastZone)
        {
            var damageTaken = otherCharacter.TakeDamage(ExplosionForce);
        }
    }
}