using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns units at a fixed rate from a building.
/// </summary>
[RequireComponent(typeof(Entity))]
public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner Profile")]
    [Tooltip("Profile defining unit spawn settings.")]
    public UnitSpawnerProfileSO spawnerProfile;

    [Header("Spawn Point")]
    [Tooltip("Point where units will spawn. Defaults to this transform if not set.")]
    public Transform spawnPoint;

    private Coroutine spawnRoutine;

    private ISpawnRateManager spawnRateManager;

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

        spawnRateManager = GetComponentInParent<ISpawnRateManager>();

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
            float spawnRate = spawnerProfile.spawnRate * spawnRateManager.GetSpawnRateScalar();
            yield return new WaitForSeconds(1f / spawnRate);

            CharacterDefinitionSO unitToSpawn = ReturnUnitToSpawn(spawnerProfile.units);

            Entity instantiatedEntity = Instantiate(unitToSpawn.characterPrefab, spawnPoint.position, spawnPoint.rotation);

            instantiatedEntity.Faction.SetFaction(GetComponentInParent<Entity>().Faction.CurrentFaction);

            Debug.Log($"[UnitSpawner] Spawned unit {unitToSpawn.characterName}.", this);

            // SFX
            SFXManager.Instance.PlaySound("unit generation", spawnPoint.position);

            // VFX
            VFXManager.Instance.PlayEffect("unit generation", spawnPoint.position);
        }
    }

    private CharacterDefinitionSO ReturnUnitToSpawn(List<UnitSpawnerUnit> list)
    {
        float totalProbability = 0;
        foreach (var unit in list)
        {
            totalProbability += unit.probability;
        }

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;
        foreach (var unit in list)
        {
            cumulativeProbability += unit.probability;
            if (randomValue < cumulativeProbability)
            {
                return unit.unit;
            }
        }

        return null;
    }
}
