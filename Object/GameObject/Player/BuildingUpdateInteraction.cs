public class BuildingUpdateInteraction : Interaction 
{   
    public Building building;
    public BuildingUpdateInteraction(Building b)
    {
        building  =b;
    }
    public override bool ValidateInteraction(PlayerController player)
    {
        this.HasBeenValidated = true;
        return true;
    }

    public override bool FinalizeInteraction(PlayerController player)
    {   
        
        if(!HasBeenValidated)
            return false;
        
        building.OpenUI();
        return true;
    }
}