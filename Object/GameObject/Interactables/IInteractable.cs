public interface IInteractable : GameObject 
{
    void InteractionAreaEntered();

    void ValidateInteraction(string action);
}