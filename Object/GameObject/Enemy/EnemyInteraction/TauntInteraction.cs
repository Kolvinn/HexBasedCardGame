public class TauntInteraction : Interaction

{   
    Enemy1 enemy;
    public TauntInteraction(Enemy1 enemy)
    {
        this.enemy = enemy;
    }
    

    public override bool ValidateInteraction(Enemy1 enemy)
    {
        this.HasBeenValidated = true;
        return HasBeenValidated;
    }

    public override bool FinalizeInteraction(Enemy1 enemy)
    {
        return true;
    }
}