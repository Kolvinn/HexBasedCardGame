
using Godot;
public class BuildingInteraction : Interaction
{   
    public Building building;
    public BuildingInteraction(Building b)
    {
        this.building = b;
    }

    public override bool ValidateInteraction(PlayerController player)
    {
        this.HasBeenValidated = true;
        return true;
    }

    public override bool FinalizeInteraction(PlayerController player)
    {   
        GD.Print("HAs been validated for bulding interaection: ", HasBeenValidated);
        if(!HasBeenValidated)
            return false;
        
        building.OpenUI();
        return true;
    }
}