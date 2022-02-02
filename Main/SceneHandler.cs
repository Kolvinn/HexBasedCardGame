using Godot;
using System;

public class SceneHandler : Node
{
    GameController controller;
    public override void _Ready()
    {
        controller = GetNode<GameController>("GameController");
        controller.Connect("ChangeScene", this, "On_ChangeScene");
    }

    public void On_ChangeScene()
    {
        var controller2 = Params.LoadScene<GameController>("res://Main/GameController2.tscn");
        this.RemoveChild(controller);
        //controller.QueueFree(); 
        this.CallDeferred("add_child", controller2);
        this.CallDeferred("UpdateParameters", controller2);
        //this.call(controller2);
    }

    public void UpdateParameters(GameController newControl){
        controller.RemoveChild(controller.buildController);
        newControl.RemoveChild(newControl.buildController);
        newControl.buildController = controller.buildController;
        newControl.AddChild(newControl.buildController);

        
        controller.RemoveChild(controller.cardController);
        newControl.RemoveChild(newControl.cardController);
        newControl.cardController = controller.cardController;
        newControl.AddChild(newControl.cardController);

        
        controller.RemoveChild(controller.newUIController);
        newControl.RemoveChild(newControl.newUIController);
        newControl.newUIController = controller.newUIController;
        newControl.AddChild(newControl.newUIController);

        
        controller.RemoveChild(controller.playerController);
        newControl.RemoveChild(newControl.playerController);
        newControl.playerController = controller.playerController;
        newControl.AddChild(newControl.playerController);

        
        controller.RemoveChild(controller.spellController);
        newControl.RemoveChild(newControl.spellBookController);
        newControl.spellBookController = controller.spellBookController;
        newControl.AddChild(newControl.spellBookController);

        
        controller.RemoveChild(controller.spellController);
        newControl.RemoveChild(newControl.spellController);
        newControl.spellController = controller.spellController;
        newControl.AddChild(newControl.spellController);
        
        controller.QueueFree(); 
    }

}
