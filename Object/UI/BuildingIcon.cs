using Godot;
using System;

public class BuildingIcon : VBoxContainer
{
    public TextureButton button;
    public Label label;

    public string BuildingResource;

    [Signal]
    public delegate bool ButtonPress(string button);

    public BuildingTooltip toolTip;

    public string IconName;
    public override void _Ready()
    {

        //////GD.Print("entering ready for Building Icon");
        this.button = GetNode<TextureButton>("TextureButton");
        this.label = GetNode<Label>("Label");
        //toolTip = Params.LoadScene<TextureRect>("res://Object/UI/BuildUIItems/buildingToolTip.tscn");
       // ////GD.Print("button: ",this.button);
        foreach(Node n in this.GetChildren())
        {
           // ////GD.Print("Node n: ", n);
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
        //////GD.Print("button: ",button);
        this.button.TextureNormal = new AtlasTexture(){
            Atlas = ((AtlasTexture)btn.TextureNormal).Atlas,
            Region = ((AtlasTexture)btn.TextureNormal).Region,
        };
        this.button.Disabled = true;
        this.IconName =name;
        this.label.Text = name;
        this.button.SizeFlagsHorizontal = 3;
        this.button.SizeFlagsVertical = 3;
        //this.AddChild(button);
        //this.AddChild(label);

        this.button.Connect("pressed",this, nameof(_on_TextureButton_pressed));
        this.Update();

    }

    public void _on_TextureButton_pressed()
    {
        //EmitSignal(nameof(ButtonPress),this.label.Text);

        //make this into SOMETHING SAFER PLEASE;
        PlayerController.buildingIconQueue.Enqueue(this);
    }

    

    public override void _Process(float delta)
    {   
       
        // Color c = this.button.SelfModulate;
       
        // if(this.button.Disabled){
        //      c.a = 0.5f;
        // }
        // else
        //     c.a = 1f;

        // this.button.SelfModulate= c;
        // this.Update();
    }
    public void on_pressed()
    {
        EmitSignal(nameof(ButtonPress),this.label.Text);
    }

    public void _on_BuildingIcon_mouse_entered()
    {
        toolTip.Visible = true;        
        toolTip.UpdateTooltip(this.RectGlobalPosition - new Vector2(0,150));
    }

    public void _on_BuildingIcon_mouse_exited()
    {
        toolTip.Visible =false;        
        toolTip.UpdateTooltip(this.RectGlobalPosition - new Vector2(0,150));
    }

    






//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
