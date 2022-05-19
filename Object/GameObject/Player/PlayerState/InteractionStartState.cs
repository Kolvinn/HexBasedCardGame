public class InteractionStartState :BaseState
{
    GameObject Interactable;
    public InteractionStartState(GameObject Interactable)
    {
        this.Interactable = Interactable;
    }

    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {
        BaseState st = ParseInteraction(player);
        
        return st == null ? new BaseState() : st;
    }


    private BaseState ParseInteraction(PlayerController player)
    {
        Interaction intr = new Interaction();
        BaseState newState;
        string action = "";

        
        if(typeof(GameResource).IsInstanceOfType(Interactable))
        {
            //change for resourceInteractionstate which will trigger an update itself
            intr = new ResourceInteraction(((GameResource)Interactable));
            //newState = new InteractionAcceptedState(intr);

        }
        else if(typeof(Building).IsInstanceOfType(Interactable))
        {
            intr = new BuildingInteraction((Building)Interactable);
            //newState = new BuildingInteractState((Interaction)intr);
        }

        if(!intr.IsValid())
        {
            if(intr.ValidateInteraction(player))
            {
                //doactions
            }
            else
            {
                return new BaseState();
            }
            
        }
        else
        {
            //we've finished whatever the interaction action is, so we can push our update
            if(player.CanInteract())
            {

            }
        }
        if(intr.ValidateInteraction(player))
        {
            newState = new InteractionAcceptedState(intr);
            action = intr.GetValidatedAction();
            if(action != "")
                player.DoAction(action);

            return newState;
        
        }
        
        return null;
    }

}