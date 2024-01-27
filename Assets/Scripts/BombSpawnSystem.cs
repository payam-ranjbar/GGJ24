using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombSpawnSystem : MonoBehaviour
{
    public GameObject Bomb;
    public List<GameObject> SpawningPoints;
    public float Radius = 3f;
    
    public int Frequency = 2;

    private List<ObservedSpot> _observedSpots;
    private float _lastSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the list of observed spots
        _observedSpots = new List<ObservedSpot>(SpawningPoints.Count);
        foreach (var spawningPoint in SpawningPoints)
        {
            _observedSpots.Add(ObservedSpot.Create(spawningPoint));
        }
    }

    // Update is called once per frame
    void Update()
    {
        _lastSpawnTime += Time.deltaTime;
        if (_lastSpawnTime >= Frequency)
        {
            TryGenerateObject();
            _lastSpawnTime = 0;
        }
    }

    private void TryGenerateObject()
    {
        var pointWithinRadius = Random.insideUnitCircle * Radius;
        if (TryGetAvailableSpot(out var spot))
        {
            var center = spot.SpawnPoint.transform.position;
            var x = center.x + pointWithinRadius.x;
            var z = center.z + pointWithinRadius.y;

            var bomb = Instantiate(Bomb, new Vector3(x, transform.position.y, z), Quaternion.identity);
            spot.AddBomb(bomb);
            Debug.Log("Bomb spawned at " + x + ", " + z);
        }
    }

    private bool TryGetAvailableSpot(out ObservedSpot spot)
    {
        var freeSpots = _observedSpots.Where(x => x.Spawned == false).ToArray();

        if (freeSpots.Length == 0)
        {
            spot = null;
            return false;
        }

        var randomSpotIndex = Random.Range(0, freeSpots.Count());
        spot = freeSpots[randomSpotIndex];
        return true;
    }
}