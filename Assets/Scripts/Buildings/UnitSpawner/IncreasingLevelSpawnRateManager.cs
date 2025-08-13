using System.Collections;
using UnityEngine;

public class LevelSpawnRateManager : MonoBehaviour, ISpawnRateManager
{
    public float LevelSpawnRateScalar { get; private set; } = 1f;

    [SerializeField] LevelManager levelManager;

    float startTime;

    void Start()
    {
        if (levelManager == null)
        {
            Debug.LogError("[LevelSpawnRateManager] LevelManager wasn't assigned!");
        }

        startTime = Time.unscaledTime;
    }

    public float GetSpawnRateScalar()
    {
        SetSpawnRateScalar();
        return LevelSpawnRateScalar;
    }

    void SetSpawnRateScalar()
    {
        float timeElapsed = Time.unscaledTime - startTime;
        LevelSpawnRateScalar = levelManager.data.enemySpawnRateScaler * (1 + levelManager.data.inGameSpawnRateSlope * timeElapsed);
    }
}