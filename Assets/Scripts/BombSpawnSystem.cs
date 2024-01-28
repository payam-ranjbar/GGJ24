using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombSpawnSystem : MonoBehaviour
{
    public GameObject BombPrefab;
    public List<GameObject> SpawningPoints;
    public float Radius = 3f;
    
    public int Frequency = 2;

    private List<ObservedSpot> _observedSpots;
    private float _lastSpawnTime;

    private List<ObservedSpot> _freeSpots = new List<ObservedSpot>();
    private List<ObservedSpot> _filledSpots = new List<ObservedSpot>();

    public List<ObservedSpot> freeSpots => _freeSpots;
    public List<ObservedSpot> filledSpots => _filledSpots;

    public static BombSpawnSystem instance = null;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
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
        UpdateSpots();
        _lastSpawnTime += Time.deltaTime;
        if (_lastSpawnTime >= Frequency)
        {
            TryGenerateObject();
            _lastSpawnTime = 0;
        }
    }

    private void UpdateSpots()
    {
        _freeSpots.Clear();
        _filledSpots.Clear();
        foreach (var spot in _observedSpots)
        {
            if (spot.Spawned == false)
            {
                _freeSpots.Add(spot);
            }
            else
            {
                _filledSpots.Add(spot);
            }
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
            var y = center.y;

            var bomb = Instantiate(BombPrefab, new Vector3(x, y, z), Quaternion.identity);
            spot.AddBomb(bomb);
            Debug.Log("Bomb spawned at " + x + ", " + z);
        }
    }

    private bool TryGetAvailableSpot(out ObservedSpot spot)
    {
        if (_freeSpots.Count == 0)
        {
            spot = null;
            return false;
        }

        var randomSpotIndex = Random.Range(0, _freeSpots.Count());
        spot = _freeSpots[randomSpotIndex];
        return true;
    }


}