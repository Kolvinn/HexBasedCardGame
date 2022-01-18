using Godot;
using System;

public class OptionBox : Control
{
    [Signal]
    public delegate void OnResponse(bool accepted, OptionBox option);

    [Signal]
    public delegate void WorkerAmountChange(int amount);
    public override void _Ready()
    {
        this.GetNode<HBoxContainer>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2").Visible = false;
    }

    public void UpdateType(string type)
    {
        if(type == "worker")
        {
            this.GetNode<HBoxContainer>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer").Visible = false;
            this.GetNode<HBoxContainer>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2").Visible = true;
            this.GetNode<Button>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/HBoxContainer/Add").Connect("pressed",this,nameof(On_Add_Pressed));
            
            this.GetNode<Button>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/HBoxContainer/Remove").Connect("pressed",this,nameof(On_Remove_Pressed));
        }
    }
    public void _on_Accept_pressed()
    {
        GD.Print("Emitting signal of true");
        EmitSignal(nameof(OnResponse), true, this);
       // this.QueueFree();
    }

    public void On_Add_Pressed()
    {
        EmitSignal(nameof(WorkerAmountChange), 1);
    }

    public void _on_Finish_pressed()
    {
        GD.Print("Emitting finished pressed of false and null");
        EmitSignal(nameof(OnResponse), false, null);
    }

    public void On_Remove_Pressed()
    {
        
        EmitSignal(nameof(WorkerAmountChange), -1);
    }

    public void _on_Decline_pressed()
    {
        GD.Print("decline pressed");
        EmitSignal(nameof(OnResponse), false, this);
        //this.QueueFree();
    }


    public void SetOptionText(string s){
        this.GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/Label").Text = s;
    }

}
