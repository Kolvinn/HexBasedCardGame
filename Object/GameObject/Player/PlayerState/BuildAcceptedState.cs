using Godot;
using System;
using System.Collections.Generic;
public class BuildAcceptedState : BaseState
{
    Building building;
    string name;

    public BuildAcceptedState(Building building, string name)
    {
        this.building  = building;
        this.name = name;

    }
    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {

        player.GetMovementVector(delta);
        if(HexGrid.hoveredHex !=null){
                building.Position = HexGrid.hoveredHex.Position;
        }

        if(Input.IsActionJustPressed("left_click"))
        {
            GameUpdateQueue.TryPushUpdate(new BuildCompleteInteraction(building, name));
            return new BaseState();
        }
        else if(Input.IsActionJustPressed("right_click"))
        {
            building.QueueFree();
            return new BuildState(player.buildingUI, player.playerIcon);
        }
        return this;

 
    }

   
}