using Godot;
public class StorageInteraction : Interaction
{
    Chest chest;
    public StorageInteraction(Chest chest)
    {
        this.chest = chest;
        //thoughthough

        
    }

    public override bool ValidateInteraction(PlayerController player)
    {
        this.HasBeenValidated = !chest.IsOpen();
        return HasBeenValidated;
        
    }

    public override bool FinalizeInteraction(PlayerController player)
    {
        GD.Print("finalizing storage interaction");
        var sword  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/SwordIcon.tscn");
       
        player.UpdateUIQueue.PushUpdate(sword,1, "Sword");        
        this.chest.OpenChest();
        return true;
    }
}