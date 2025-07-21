using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns units at a fixed rate from a building.
/// </summary>
[RequireComponent(typeof(Building))]
public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner Profile")]
    [Tooltip("Profile defining unit spawn settings.")]
    public UnitSpawnerProfileSO spawnerProfile;

    [Header("Spawn Point")]
    [Tooltip("Point where units will spawn. Defaults to this transform if not set.")]
    public Transform spawnPoint;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (spawnerProfile == null)
        {
            Debug.LogError("[UnitSpawner] Spawner profile not assigned.", this);
            enabled = false;
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("[UnitSpawner] SpawnPoint not set. Using own transform.", this);
            spawnPoint = transform;
        }

        spawnRoutine = StartCoroutine(SpawnCoroutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnerProfile.spawnRateInSeconds);
            Instantiate(spawnerProfile.unitPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"[UnitSpawner] Spawned unit {spawnerProfile.unitPrefab.name}.", this);
        }
    }
}
