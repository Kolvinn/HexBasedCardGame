using System;
using Godot;
using System.Linq;
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
        GD.Print("validation res interaction for ",this.Resource.ResourceType);
        GD.Print(Resource.ToolLevel <= player.ToolLevel);
        GD.Print(player.CanAddToInventory(), Resource.TotalResource > 0);
            if(Resource.ToolLevel <= player.ToolLevel)
            {
                if(player.CanAddToInventory() && Resource.TotalResource > 0)
                {
                    //if(Resource.resourceState == GameResource.ResourceState.Available)
                    //{
                        //Resource.resourceState = GameResource.ResourceState.Locked;
                        validCheck = true;
                        // player.DoAction(((GameResource)obj).)
                        // return new InteractionState(new ResourceInteraction((GameResource)obj));
                   // }
                }
            }
        
        HasBeenValidated = validCheck;
        return HasBeenValidated;
    }

    public override bool ValidateInteraction(CharacterController character)
    {
        bool validCheck = false;

        if(character.CanAddToInventory() && Resource != null && Resource.TotalResource > 0)
        {
            validCheck = true;
            // player.DoAction(((GameResource)obj).)
            // return new InteractionState(new ResourceInteraction((GameResource)obj));
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
        finalized = true;
        if(!HasBeenValidated)
        {
            return false;
        }
        return HandleResourceInteraction(player, (BasicResource)this.Resource);                        
        
    }

    private bool HandleResourceInteraction(PlayerController player, BasicResource res)
    {
        GD.Print("Handling resource interaction for ",res.ResourceType);
        int val = Math.Min(GatherAmount(player), Resource.TotalResource);

        Resource.TotalResource -= val;
        ObjectLockQueue.TryRemoveObject((BasicResource)Resource);
        
        player.TryAddToInventory(res,val);
        if(Resource.TotalResource <= 0)
        {
            InteractionQueue.RemoveObject((GameObject)Resource);
            ((Node)Resource).QueueFree();
        }
        // if(res.Name.Contains("Grass"))
        // { 
        //     Random r = new Random();
        //     GD.Print("Player current power: ", player.currentPower);
        //     if(r.NextDouble() <=0.5 && player.currentPower != null)
        //     {
        //          res.ResourceType = "Mystic Seed";
               
        //     }
        //     else {
        //         res.ResourceType = "Grass Fiber";
        //     }
            
        return true;
    }

    public override bool FinalizeInteraction(CharacterController character)
    {
        if(!HasBeenValidated)
        {
            this.finalized = true;
            return false;
        }
        int val = Math.Min(2, Resource.TotalResource);
        if(Resource.TotalResource - val <= 0)
        {
            GD.Print("freeing resource");
            Resource.TotalResource = 0;
            //InteractionQueue.RemoveObject((GameObject)Resource);
            ((Node)Resource).QueueFree();
            //HexGrid.resourcePositions.Remove(HexGrid.resourcePositions.First( i => i.Value))

        }
        else
        {
            character.capacity +=val;
            character.ResourceUpdate[Resource.ResourceType] += val;
            Resource.TotalResource -= val;
            ObjectLockQueue.TryRemoveObject((BasicResource)Resource);
            
            GD.Print("Updating resource values with amoutn left = ", Resource.TotalResource);
        }
        this.finalized = true;
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