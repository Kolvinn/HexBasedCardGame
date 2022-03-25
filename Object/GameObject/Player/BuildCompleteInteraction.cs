using Godot;
using System;
public class BuildCompleteInteraction : Interaction
{
    public Building building;

    public BuildCompleteInteraction(Building building)
    {
        this.building = building;
    }



    public override bool ValidateInteraction(PlayerController player)
    {
        //TODO - do something useful with this
        GD.Print("SDJKFHBJKSDBNF");
        HasBeenValidated = true;
        return true;
    }

    public override bool FinalizeInteraction(PlayerController player)
    {
        if(!HasBeenValidated)
            return false;
        building.Position = building.buildingAsset.Position;
        building.Scale  = building.buildingAsset.Scale;
        
        foreach(var item in building.model?.RequiredResources.GetResourceCosts())
        {
            player.ResourceUpdate[item.Key] -= item.Value;
        }
        return true;
    }
}