using Godot;
public class CharacterInteractState : CharacterBaseState 
{
    private Interaction interaction;
    private bool updated = false;
    public CharacterInteractState(Interaction interaction)
    {
        this.interaction = interaction;
        interaction.SetAsCharacterState();
    }
    public override CharacterBaseState HandleState(CharacterController character, WorkerJob job)
    {   
        if(character.CanInteract())
        {
            //now we wait for the information to be parsed by the main loop, then return back to normal
            if(interaction.HasBeenFinalized() && updated)
            {
                GD.Print("finalised");
                return new ExitState();
            }
            else if(!updated)
            {
                GD.Print("pushing update to interaction");
                updated  = true;
                GameUpdateQueue.TryPushCharacterUpdate(interaction, character);
            }
        }
        return this;
        // return this;
        // if(!interaction.IsValid())
        // {
            
        //     if(interaction.ValidateInteraction(character))
        //     {
        //         //doactions
        //         var action = interaction.GetValidatedAction();
        //         if(!string.IsNullOrEmpty(action))
        //         {
        //             GD.Print("DOING ACTION: ", action);
        //             character.DoAction(action);
        //         }
        //     }
        //     else
        //     {
        //         return new CharacterBaseState();
        //     }
            
        // }
        // if(interaction.IsValid())
        // {
        //     //we've finished whatever the interaction action is, so we can push our update
        //     if(character.CanInteract())
        //     {
        //         //now we wait for the information to be parsed by the main loop, then return back to normal
        //         if(interaction.HasBeenFinalized())
        //         {
        //             GD.Print("has been finalized, now returning back to character base");
        //             return new CharacterBaseState();
        //         }
        //         else if(!updated)
        //         {
        //             GD.Print("pushing update");
        //             updated  = true;
        //             GameUpdateQueue.TryPushCharacterUpdate(interaction, character);
        //         }
        //         //character.Lock();
        //         //return new CharacterBaseState();
        //     }
        // }        
        // return this;
        
    }
}