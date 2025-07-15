/// <summary>
/// Defines an item that can be collected by an <see cref="AutoInteractor"/>.
/// </summary>
public interface ICollectible
{
    /// <summary>
    /// Called when the item is collected.
    /// </summary>
    /// <param name="collector">The interactor collecting the item.</param>
    void Collect(AutoInteractor collector);
}
