using UnityEngine;

/// <summary>
/// Configuration profile that defines what automatic interactions an
/// <see cref="AutoInteractor"/> is allowed to perform.
/// </summary>
[CreateAssetMenu(fileName = "InteractorProfile", menuName = "TheFist/Interactor Profile")]
public class InteractorProfileSO : ScriptableObject
{
    [Header("Capabilities")]
    [Tooltip("Allows the interactor to issue attacks via its Attacker component.")]
    public bool canAttack = true;
    
    [Space]
    
    [Tooltip("Allows the interactor to perform automatic interactions with targets.")]
    public bool canInteract = false;

    [Tooltip("Allows the interactor to upgrade compatible targets.")]
    public bool canUpgrade = false;

    [Tooltip("Allows the interactor to unlock compatible targets.")]
    public bool canUnlock = false;

    [Tooltip("Allows the interactor to collect compatible items.")]
    public bool canCollect = false;
}
