using Godot;
public class PlayerStateHandler
{
    private enum PlayerState
    {
        Default,
        Interaction,
        InteractionFinish
    }

    private PlayerState state;

    private BaseState InternalState;

    public PlayerStateHandler()
    {
        InternalState = new BaseState();
    }

    public void HandleInput(PlayerController player, float delta)
    {
       // will iterate over states until satisfied the current state
        InternalState = InternalState.HandleState(player, delta);
      
    }
   
    
    

    
}