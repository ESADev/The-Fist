using UnityEngine;

public class AutoAssignStrategicTarget : MonoBehaviour
{
    void Start()
    {
        Entity entity = GetComponentInParent<Entity>();
        AIMovementBrain brain = GetComponentInParent<AIMovementBrain>();
        LevelManager manager = FindFirstObjectByType<LevelManager>();

        switch (entity.Faction.CurrentFaction)
        {
            case FactionType.Player:
                brain.strategicTarget = manager.enemyMainBase.transform;
                break;
            case FactionType.Enemy:
                brain.strategicTarget = manager.playerMainBase.transform;
                break;
            default:
                Debug.LogWarning($"[AutoAssignStrategicTarget] Unexpected faction: {entity.Faction.CurrentFaction}");
                break;
        }
    }
}
