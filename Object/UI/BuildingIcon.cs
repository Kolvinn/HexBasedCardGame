using Godot;
using System;

public class BuildingIcon : VBoxContainer
{
    public TextureButton button;
    public Label label;

    [Signal]
    public delegate bool ButtonPress(string button);
    public override void _Ready()
    {

        ////GD.Print("entering ready for Building Icon");
        this.button = GetNode<TextureButton>("TextureButton");
        this.label = GetNode<Label>("Label");
       // //GD.Print("button: ",this.button);
        foreach(Node n in this.GetChildren())
        {
           // //GD.Print("Node n: ", n);
        }
        //this.SizeFlagsHorizontal = 3;
        //this.SizeFlagsVertical = 3;
    }

    public void UpdateIcon(string name,TextureButton btn)
    {
        // foreach(Node n in this.GetChildren())
        // {
        //     this.RemoveChild(n);
        //     n.QueueFree();
        // }
        ////GD.Print("button: ",button);
        this.button.TextureNormal = new AtlasTexture(){
            Atlas = ((AtlasTexture)btn.TextureNormal).Atlas,
            Region = ((AtlasTexture)btn.TextureNormal).Region,
        };
        this.button.Disabled = true;
        this.label.Text = name;
        //this.AddChild(button);
        //this.AddChild(label);

        this.button.Connect("pressed",this, nameof(on_pressed));
        this.Update();

    }
    public override void _Process(float delta)
    {   
        Color c = this.button.SelfModulate;
       
        if(this.button.Disabled)
             c.a = 0.5f;
        else
            c.a = 1f;

        this.button.SelfModulate= c;
        this.Update();
    }
    public void on_pressed()
    {
        EmitSignal(nameof(ButtonPress),this.label.Text);
    }




//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
