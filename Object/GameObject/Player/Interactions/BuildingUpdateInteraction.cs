public class BuildingUpdateInteraction : Interaction 
{   
    public Building building;

    public BuildingUpdateInteraction(Building b)
    {
        building  =b;
       
    }
    public override bool ValidateInteraction(PlayerController player)
    {
        return base.ValidateInteraction(player);
    }

    public override bool FinalizeInteraction(PlayerController player)
    {   
        return base.FinalizeInteraction(player);;
    }
}