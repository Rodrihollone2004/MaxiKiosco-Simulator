
public interface IInteractable
{
    void Interact();
    void Highlight();
    void Unhighlight();
    bool CanBePickedUp { get; }
}
