using UnityEngine;

public class BaseGemMaterialAssigner : MonoBehaviour
{
    [SerializeField] MeshRenderer gemMaterial;
    [SerializeField] Material playerMaterial;
    [SerializeField] Material enemyMaterial;

    void Awake()
    {
        FactionType faction = GetComponentInParent<Entity>().Faction.CurrentFaction;

        switch (faction)
        {
            case FactionType.Player:
                gemMaterial.material = playerMaterial;
                break;
            case FactionType.Enemy:
                gemMaterial.material = enemyMaterial;
                break;
            default:
                // Nothing to do 
                break;
        }
    }
}
