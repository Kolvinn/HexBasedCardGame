using Godot;
public class CharacterGatherState : CharacterBaseState 
{
    private BasicResource gatherable = null;
    private ResourceInteraction interaction = null;

    private CharacterInteractState interactState =null;
    public CharacterGatherState(BasicResource gatherable)
    {
        this.gatherable = gatherable;
        this.interaction = new ResourceInteraction(gatherable);
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
            GD.Print("Exit state in gather control state");
            return new ExitState();
        }
        
    }

    




}