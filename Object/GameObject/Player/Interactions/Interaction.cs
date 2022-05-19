public class Interaction
{
    protected bool HasBeenValidated = false;

    protected bool characterstate =false;

    protected bool finalized;

    /// <summary>
    /// Performs the neccessary checks to make sure the game is in the correct state to perform the interaction.
    /// If the game is in the incorrect state, it will return false.
    /// 
    /// Will default to allow interaction
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual bool ValidateInteraction(PlayerController player)
    {
        HasBeenValidated = true;
        return HasBeenValidated;
    }

    public virtual bool ValidateInteraction(CharacterController character)
    {
        finalized = true;
        return finalized;
    }

    public virtual bool ValidateInteraction(Enemy1 enemy)
    {
        return false;
    }

    public void SetAsCharacterState()
    {
        this.characterstate = true;
    }
    public bool IsCharacterState()
    {
        return characterstate;
    }

    /// <summary>
    /// Completes this interaction once it has been validated.
    /// Will not complete the interaction and return false if it has not validated.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual bool FinalizeInteraction(PlayerController player)
    {
        finalized = true;
        return finalized;
    }

    public virtual bool FinalizeInteraction(CharacterController player)
    {
        return false;
    }

    public virtual bool FinalizeInteraction(Enemy1 enemy)
    {
        return false;
    }

     /// <summary>
    /// Returns an action the player should perform based on the outcome of this interaction.
    /// Will return an empty string if the  interaction has not been validated or if the player should do nothing
    /// </summary>
    /// <returns></returns>
    public virtual string GetValidatedAction()
    {
        return "";
    }

    public virtual bool HasBeenFinalized()
    {
        return this.finalized;
    }

    public virtual bool IsValid()
    {
        return this.HasBeenValidated;
    }

}