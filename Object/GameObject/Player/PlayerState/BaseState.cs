using System;
using Godot;
using System.Collections.Generic;
public class BaseState 
{

    public BaseState(BaseState parentState = null)
    {

    }

    public virtual BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {   
        

        bool leftClick = Input.IsActionJustPressed("left_click");
        bool rClick = Input.IsActionJustPressed("rClick");
        var interactable = player.GetInteractableObject();
        bool eClick = Input.IsActionJustPressed("E");
        bool build = Input.IsActionJustPressed("Build");
        
        bool inventory = Input.IsActionJustPressed("inventory");
        BaseState state = null;
        //GD.Print("Base state with left click being :" , leftClick);
        if(leftClick)
        {

            //GD.Print("interaction accepted3");
            if( interactable != null){

                //GD.Print("interaction accepted1");
                    if(player.CanInteractWithObject(interactable))
                    {
                        //GD.Print("interaction accepted");
                        state = new InteractionAcceptedState(GetInteraction(interactable));
                    }               
            }
        }
        if(rClick)
        {
            //GD.Print("Pressed r key");
            player.playerIcon.animationPlayer.PlayBackwards("SwapEquip");
            return new BattleState();
        }

        if(build)
        {
            
            return new BuildState(player.buildingUI, player.playerIcon);
        }

        if(inventory)
        {
            player.inventory.UI.Visible = true;
            return new InventoryState(this);
        }
        // if(eClick)
        // {
        //     player.player.animationPlayer.Play("PowerUp");
        //     player.PowerEffect.Visible = true;
        //     player.currentPower = new WaterPower();
        //     //return new PoweredState();
            
        // }
        if(state == null)
        {
            player.GetMovementVector(delta);
            return this;
        }
        else
        {
            //state.HandleState(player, delta);
            return state;
        }
        //base state can always move
        
    }

    protected List<string> AvailableActions()
    {
        return new List<string>()
        {
            "left_click"
        };
    }
    private Interaction GetInteraction(GameObject obj)
    {
        //GD.Print("sdkfn");
        if(obj is GameResource)
        {
            return new ResourceInteraction(((GameResource)obj));

        }
        else if(obj is Building)
        {    
            GD.Print("Found object");
            return new BuildingInteraction((Building)obj);
        }
        else if(obj is StaticInteractable)
        {
            if(((StaticInteractable)(obj)).Name == "Chest")
            {
                GD.Print("CHEST");
                return new StorageInteraction((Chest)obj);
            }
        }
        return null;
    }

    



}