using Godot;
using System;
public class BuildCompleteInteraction : Interaction
{
    public Building building;
    public string name;

    public BuildCompleteInteraction(Building building, string name)
    {
        this.building = building;
        this.name = name;
    }



    public override bool ValidateInteraction(PlayerController player)
    {
        //TODO - do something useful with this
        HasBeenValidated = true;
        return true;
    }

    public override bool FinalizeInteraction(PlayerController player)
    {
        return base.FinalizeInteraction(player);
       // if(!HasBeenValidated)
          //  return false;
        //building.Position = building.buildingAsset.Position;
        //building.Scale  = building.buildingAsset.Scale;
       // player.RemoveFromInventory(building.Resour)
        
        // foreach(var item in building.model?.RequiredResources.ResourceCostList)
        // {
        //     player.ResourceUpdate[item.Key] -= item.Value;
        // }
       // return true;
    }
}