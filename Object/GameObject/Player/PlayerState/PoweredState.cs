public class PoweredState : BaseState
{
    public PoweredState()
    {

    }

    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {
        return base.HandleState(player, delta);
    }
}