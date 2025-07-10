/// <summary>
/// Provides healing functionality to an object.
/// </summary>
public interface IHealable
{
    /// <summary>
    /// Heals the object by a specific amount.
    /// </summary>
    /// <param name="healAmount">The amount of health to restore.</param>
    void Heal(float healAmount);
}
