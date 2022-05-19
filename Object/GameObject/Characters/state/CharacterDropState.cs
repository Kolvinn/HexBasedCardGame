public class CharacterDropState : CharacterBaseState 
{   
    private StorageBuilding building;
    private ResourceDropInteraction interaction = null;

    private CharacterInteractState interactState =null;
    
    public CharacterDropState(StorageBuilding building)
    {
        this.building  = building;
        interaction = new ResourceDropInteraction(building);
    }
    public override CharacterBaseState HandleState(CharacterController character, WorkerJob job)
    {
        if(interactState != null)
        {
            var returnState = interactState.HandleState(character,job);

            if(returnState is  ExitState)
            {
                interactState = null;
            }
            else
            {
                return this;
            }
          
        }
        if(interaction.ValidateInteraction(character))
        {
            character.DoAction(interaction.GetValidatedAction());
            interactState = new CharacterInteractState(interaction);
            return this;
        }
        else
        {
            return new ExitState();
        }
    }
}