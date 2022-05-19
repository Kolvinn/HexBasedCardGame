public class PowerUpInteraction : Interaction
{
    public PowerUpInteraction()
    {
        
    }
    public override bool ValidateInteraction(PlayerController player)
    {
        this.HasBeenValidated = true;
        return HasBeenValidated;
    }


    public override bool FinalizeInteraction(PlayerController player)
    {

        return false;
    }
}