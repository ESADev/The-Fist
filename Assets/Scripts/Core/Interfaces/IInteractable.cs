/// <summary>
/// Represents an object that can be interacted with by an <see cref="Interactor"/>.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when an interactor attempts to interact with this object.
    /// </summary>
    /// <param name="interactor">The interacting entity.</param>
    void Interact(Interactor interactor);
}
