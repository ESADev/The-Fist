using UnityEngine;

/// <summary>
/// Identifies which team a GameObject belongs to.
/// Other components read this to determine friend or foe.
/// </summary>
public class FactionComponent : MonoBehaviour
{
    public enum Team { Player, Enemy }

    [Tooltip("Team allegiance of this unit")]
    public Team unitTeam = Team.Player;
}
