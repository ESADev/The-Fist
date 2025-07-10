/// <summary>
/// Defines behaviour for objects that can take damage and be destroyed.
/// </summary>
public interface IDestructible
{
    /// <summary>
    /// Applies damage to the object from a specified attacker.
    /// </summary>
    /// <param name="amount">The damage amount.</param>
    /// <param name="attacker">The attacking entity.</param>
    void TakeDamage(float amount, Attacker attacker);
}
