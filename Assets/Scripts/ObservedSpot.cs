using UnityEngine;

public class ObservedSpot
{
    public bool Spawned { get; set; }
    public GameObject SpawnPoint { get; set; }
    private GameObject Bomb { get; set; }

    private Bomb _currentBombComponent;
    public Bomb bomb => _currentBombComponent;

    private ObservedSpot()
    {
    }

    public static ObservedSpot Create(GameObject spawnPoint)
    {
        return new ObservedSpot
        {
            Spawned = false,
            SpawnPoint = spawnPoint
        };
    }

    public void AddBomb(GameObject bomb)
    {
        _currentBombComponent = bomb.GetComponent<Bomb>();
        _currentBombComponent.Exploded += BombExploded;
        Bomb = bomb;
        Spawned = true;
    }

    private void BombExploded(Explosion explosion)
    {
        _currentBombComponent.Exploded -= BombExploded;
        _currentBombComponent = null;
        Bomb = null;
        Spawned = false;
    }
}