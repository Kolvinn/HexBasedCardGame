using Godot;
public class InteractionAcceptedState : BaseState
    {
        Interaction interaction;
        string action;
        public InteractionAcceptedState(Interaction interaction)
        {
            this.interaction = interaction;
           // this.action = action;
        }

        public override BaseState HandleState(PlayerController player, float delta)
        {
            if(!interaction.IsValid())
            {
                if(interaction.ValidateInteraction(player))
                {
                    //doactions
                    var action = interaction.GetValidatedAction();
                    if(!string.IsNullOrEmpty(action))
                        player.DoAction(action);
                }
                else
                {
                    return new BaseState();
                }
                
            }
            //importantly --  we dont do if/else because the action the player takes may take no time, so we don't want to wait for next loop to pick it up.
            if(interaction.IsValid())
            {
                //we've finished whatever the interaction action is, so we can push our update
                if(player.CanInteract())
                {
                    GameUpdateQueue.TryPushUpdate(interaction);
                    return new BaseState();
                }
            }        
            return this;
        }
    }