using UnityEngine;

public class TempUnitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private float spawnRate = 1f; // Units per second
    [SerializeField] Transform strategicTarget;
    
    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + (1f / spawnRate);
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && unitPrefabs != null && unitPrefabs.Length > 0)
        {
            SpawnUnit();
            nextSpawnTime = Time.time + (1f / spawnRate);
        }
    }

    private void SpawnUnit()
    {
        GameObject randomUnitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
        GameObject unit = Instantiate(randomUnitPrefab, transform.position, transform.rotation);
        unit.GetComponentInChildren<AIMovementBrain>().strategicTarget = strategicTarget;
    }
}