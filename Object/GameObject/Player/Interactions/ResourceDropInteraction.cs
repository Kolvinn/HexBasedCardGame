using System.Collections.Generic;
public class ResourceDropInteraction : Interaction
{   
    private StorageBuilding building;

    //private int TotalResource = 0;
    public ResourceDropInteraction(StorageBuilding b)
    {
        this.building = b;
    }

    public override string GetValidatedAction()
    {
        return "drop";
    }

    public override bool ValidateInteraction(CharacterController character)
    {
        var valdiated = false;
        int total = 0;
        foreach(var entry in character.ResourceUpdate)
        {
            total +=entry.Value;
        }
        if(total > 0 && building.CanDropResources(total))
        {
            valdiated =true;
        }
        
        this.HasBeenValidated = valdiated;
        return this.HasBeenValidated;

    }

    public override bool FinalizeInteraction(CharacterController character)
    {
        if(!HasBeenValidated)
        {
            this.finalized = true;
            return false;
        }
        building.UpdateStorage(character.capacity);
        character.capacity =0;
        character.ResourceUpdate = new Dictionary<string, int>(){
            {"Wood", 0},
            {"Leaves", 0},
            {"Stone",0}
        };
        this.finalized = true;
        return true;
    }
}