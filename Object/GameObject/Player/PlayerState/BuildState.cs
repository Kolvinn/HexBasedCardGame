using Godot;
using System;
using System.Collections.Generic;
public class BuildState : BaseState
{
    public BuildState(BuildingUIMenu ui, PlayerIcon icon)
    {
        ui.ShowMenu();
        GameUpdateQueue.TryPushUpdate(new BuildingMenuInteraction());            
        icon.Visible =false;
    }
    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {
        player.GetMovementVector(delta);
        if(PlayerController.buildingIconQueue.Count>0)
        {
            BuildingIcon icon = PlayerController.buildingIconQueue.Dequeue();
            var b = Params.LoadScene<Building>(icon.BuildingResource);
            GameUpdateQueue.TryPushUpdate(new BuildingUpdateInteraction(b));
            player.buildingUI.HideMenu();
            player.buildingUI.currentMenu.icons.ForEach(icon => icon.toolTip.Visible = false);
            return new BuildAcceptedState(b, icon.IconName);
        }

        if(Input.IsActionJustPressed("Build"))
        {
            player.buildingUI.HideMenu();
            
            player.playerIcon.Visible =true;
            return new BaseState();
        }


        
        return this;
 
    }

   
}