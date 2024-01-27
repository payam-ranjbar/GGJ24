using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class Bomb : MonoBehaviour
{
    public int LifeTime = 10; // In seconds
    public float ExplosionForce = 1000f;
    public float ExplosionRadius = 1f;

    [SerializeField] private Rigidbody _rigidbody;

    public event Action<Explosion> Exploded;
    public UnityEvent<Explosion> ExplodedEvent;
    
    private float _currentLifeTime;

    private CharacterController _character = null;

    private void OnValidate()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }
    }

    private void Awake()
    {
        OnValidate();;
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

}