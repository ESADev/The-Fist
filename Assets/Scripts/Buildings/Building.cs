using UnityEngine;

/// <summary>
/// Simple component representing a constructed building.
/// </summary>
[DisallowMultipleComponent]
public class Building : MonoBehaviour
{
    [Tooltip("Data describing this building.")]
    public BuildingDataSO buildingData;

    private BuildingSlot ownerSlot;

    /// <summary>
    /// Sets the slot that spawned this building.
    /// </summary>
    /// <param name="owner">Owning slot.</param>
    public void SetOwner(BuildingSlot owner)
    {
        ownerSlot = owner;
    }
}

