using System;
using Godot;
public class ResourceInteraction : Interaction 
{
    private GameResource Resource;

    public ResourceInteraction(GameResource resource)
    {
        this.Resource = resource;
    }

    


    public override bool ValidateInteraction(PlayerController player)
    {
        bool validCheck = false;
         
            if(Resource.ToolLevel <= player.ToolLevel)
            {
                if(player.CanAddToInventory() && Resource.TotalResource > 0)
                {
                    if(Resource.resourceState == GameResource.ResourceState.Available)
                    {
                        validCheck = true;
                        // player.DoAction(((GameResource)obj).)
                        // return new InteractionState(new ResourceInteraction((GameResource)obj));
                    }
                }
            }
        
        HasBeenValidated = validCheck;
        return HasBeenValidated;
    }

    public override string GetValidatedAction()
    {
        return HasBeenValidated ? Resource.RequiredAction : "";
    }

    public override bool FinalizeInteraction(PlayerController player)
    {
        if(!HasBeenValidated)
            return false;

        int val = Math.Min(GatherAmount(player), Resource.TotalResource);
        if(Resource.TotalResource - val  == 0)
        {
            InteractionQueue.RemoveObject((GameObject)Resource);
            ((Node)Resource).QueueFree();

        }
        else
        {
            player.ResourceUpdate[Resource.ResourceType] += val;
            Resource.TotalResource -= val;
            Resource.resourceState = GameResource.ResourceState.Available;
        }
        return true;
    }

    public string ValidateInteraction(SpellController spell)
    {
        return null;
    }

    public int GatherAmount(PlayerController player)
    {
        if(player.ToolLevel ==0)
            return 2;
        return 1;
    }
}