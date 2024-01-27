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

    [SerializeField] private CollisionEventReceiver _charactersReceiver;

    public event Action<Explosion> Exploded;
    public UnityEvent<Explosion> ExplodedEvent;

    private float _currentLifeTime;
    private List<CharacterController> _charactersInBlastZone = new List<CharacterController>();

    private void Awake()
    {
       _charactersReceiver.triggerEnterAction += OnCharacterTriggerEnter;
       _charactersReceiver.triggerExitAction += OnCharacterTriggerExit;
    }

    private void OnCharacterTriggerExit(Collider collider)
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

    private void OnCharacterTriggerEnter(Collider collider)
    {
        var referenceHolder = collider.GetComponent<ReferenceHolder>();
        if (referenceHolder != null)
        {
            var otherCharacter = referenceHolder.GetReferenceComponent<CharacterController>();
            _charactersInBlastZone.Remove(otherCharacter);
        }
    }

    private void Start()
    {
        _currentLifeTime = 0f;
    }

    private void Update()
    {
        Countdown();
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
        
        foreach (var otherCharacter in _charactersInBlastZone)
        {
            otherCharacter.TakeDamage(ExplosionForce);
        }

        Destroy(gameObject);
        Debug.Log("Exploded by count down!");
    }
}