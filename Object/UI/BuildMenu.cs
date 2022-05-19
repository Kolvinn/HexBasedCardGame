using Godot;
using System;
using System.Collections.Generic;

public class BuildMenu : GridContainer
{
    public List<BuildingIcon> icons = new List<BuildingIcon>();
    public override void _Ready()
    {       
        MouseFilter = MouseFilterEnum.Stop;
        this.RectMinSize = new Vector2(86,0);
    }


    public BuildingIcon AddNewIcon(string name, string resourceString, string textureResource, string description, string buildingRes = "")
    {
        BuildingIcon icon  = Params.LoadScene<BuildingIcon>("res://Object/UI/BuildingIcon.tscn");
        icon.toolTip = Params.LoadScene<BuildingTooltip>("res://Object/UI/BuildUIItems/buildingToolTip.tscn");
        icon.toolTip.UpdateTooltip(description, resourceString);
        icon.button = icon.GetNode<TextureButton>("TextureButton");
        icon.label = icon.GetNode<Label>("Label");    
        icon.SetAnchorsPreset(LayoutPreset.Center);
        icon.BuildingResource = buildingRes;
        Control container = new Control()
        {
           RectMinSize = new Vector2(150,0),
           SizeFlagsVertical =3

        };
       // ColorRect cr= new ColorRect();
        //cr.AnchorRight =1;
        //cr.AnchorBottom = 1;
        //container.AddChild(cr);
        container.AddChild(icon);  
        icon.SetAnchorsPreset(LayoutPreset.Center); 
        this.AddChild(container);
        this.Columns = this.GetChildren().Count;
        
        //icon.button.HintTooltip = resourceString;

        // //p.Value.Connect("ButtonPress", this,nameof(ButtonPressed));
        TextureButton b = null;
        try{
            b = Params.LoadScene<TextureButton>(textureResource);
        }catch(Exception e)
        {
            b = Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/ImageNotFound.tscn");
            GD.Print("found this: ",b);
        }
        //GD.Print("B ", b.Name);
        icon.button.Disabled =true;
        //buildingButtons.Add(name, icon.button);
        icon.UpdateIcon(name,b);
        icons.Add(icon);
        return icon;
       
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
