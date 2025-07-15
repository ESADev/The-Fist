/// <summary>
/// Represents an object that can be interacted with by an <see cref="AutoInteractor"/>.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when an interactor attempts to interact with this object.
    /// </summary>
    /// <param name="interactor">The interacting entity.</param>
    void Interact(AutoInteractor interactor);
}
