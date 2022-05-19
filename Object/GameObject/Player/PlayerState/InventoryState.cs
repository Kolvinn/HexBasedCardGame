using Godot;
using System;
public class InventoryState : BaseState
{   
    private BaseState pastState;
    public InventoryState(BaseState pastState)
    {
        this.pastState = pastState;
    }
    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {
       
        bool inventory = Input.IsActionJustPressed("inventory");

        if(inventory)
        {
            player.inventory.UI.Visible = false;
            return pastState;
        }
        else
        {
            
            return this;
        }
    }
}