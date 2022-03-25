public class Interaction
{
    protected bool HasBeenValidated = false;

    /// <summary>
    /// Performs the neccessary checks to make sure the game is in the correct state to perform the interaction.
    /// If the game is in the incorrect state, it will return false
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual bool ValidateInteraction(PlayerController player)
    {
        return false;
    }

    /// <summary>
    /// Completes this interaction once it has been validated.
    /// Will not complete the interaction and return false if it has not validated.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual bool FinalizeInteraction(PlayerController player)
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

    public virtual bool IsValid()
    {
        return this.HasBeenValidated;
    }

}