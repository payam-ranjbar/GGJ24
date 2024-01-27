using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour
{
    public int LifeTime = 10; // In seconds
    public float ExplosionForce = 1000f;
    public float ExplosionRadius = 1f;
    
    public event Action<Explosion> Exploded;
    public UnityEvent<Explosion> ExplodedEvent;
    
    private float _currentLifeTime;

    private void Awake()
    {

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

        Destroy(gameObject);
        Debug.Log("Exploded by count down!");
    }
}