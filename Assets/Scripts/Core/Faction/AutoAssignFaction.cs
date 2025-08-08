using UnityEngine;

public class AutoAssignFaction : MonoBehaviour
{
    void Start()
    {
        try
        {
            FactionType faction = GetComponentInParent<FactionParent>().CurrentFaction;
            GetComponent<Faction>().SetFaction(faction);
        }
        catch
        {
            Debug.Log("[AutoAssignFaction] FactionParent not found in parent hierarchy. Assigning default faction.");
            Destroy(this);
        }
    }
}
