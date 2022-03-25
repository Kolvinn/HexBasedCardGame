using System;
using Godot;
public class BaseState 
{

    public virtual BaseState HandleState(PlayerController player, float delta)
    {   
        

        bool leftClick = Input.IsActionJustPressed("left_click");
        var interactable = player.GetInteractableObject();
        BaseState state = null;
        //GD.Print("Base state with left click being :" , leftClick);
        if(leftClick)
        {
            GD.Print("interaction accepted3");

            if( interactable != null){
                GD.Print("interaction accepted1");

                    if(player.CanInteractWithObject(interactable))
                    {
                        GD.Print("interaction accepted");
                        state = new InteractionAcceptedState(GetInteraction(interactable));
                    }               
            }
        }

        if(state == null)
        {
            player.GetMovementVector(delta);
            return this;
        }
        else
        {
            state.HandleState(player, delta);
            return state;
        }
        //base state can always move
        
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
        return null;
    }

    



}