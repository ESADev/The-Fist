using System.Collections;
using UnityEngine;

public class FixedLevelSpawnRateManager : MonoBehaviour, ISpawnRateManager
{
    [SerializeField] LevelManager levelManager;

    void Awake()
    {
        if (levelManager == null)
        {
            Debug.LogError("[LevelSpawnRateManager] LevelManager wasn't assigned!");
        }
    }

    public float GetSpawnRateScalar()
    {
        return levelManager.data.enemySpawnRateScaler;
    }
}