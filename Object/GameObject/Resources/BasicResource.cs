using Godot;
using System;
using System.Text.RegularExpressions;
using System.Linq;
public class BasicResource : StaticBody2D, GameResource
{
    public State.MouseEventState mouseEventState;
    public string ResourceType {
        get;set;
    }
    public int Capacity{
        get;set;
    }

    public AtlasTexture GenTex;

    public Sprite sprite;

    public CollisionPolygon2D collision;
    public BasicResource()
    {
        this.Connect("mouse_entered", this, nameof(_on_mouse_entered));
        this.Connect("mouse_exited", this, nameof(_on_mouse_exited));
        
        this.Capacity = 5;
    }

    public override void _Ready()
    {   
       // ResourceType = this.Name;
        this.ResourceType = new String(this.Name.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
        //Regex.Replace(ResourceType, @"[\d-]", string.Empty);   
        if(ResourceType.Contains("Rock") || ResourceType.Contains("Stone")){
            ////GD.Print("Setting random rock atlas");
            ////GD.Print(this.Scale);
            this.GetNode<Sprite>("Sprite").Texture = this.GenRandomRockSprite();
        }
        
        //GD.Print(this.ResourceType);
    }

    public AtlasTexture GenRandomRockSprite(){
        AtlasTexture atlas = new AtlasTexture();
        atlas.Atlas = ResourceLoader.Load<Texture>("res://Assets/Environment/Rocks/512x1024.png");
        Random r = new Random();
        float randW = (float)r.Next(0,3), randH = (float)r.Next(0,7);
        Vector2 pos = new Vector2(128 * randW,128 * randH);
        Vector2 size = new Vector2(128, 128);
        atlas.Region = new Rect2(pos, size);
        atlas.Flags = 1;
        return atlas;
    }

    public void _on_mouse_entered()
    {
        ////GD.Print("Mouse entered for the wooooooood");
        this.mouseEventState =  State.MouseEventState.Entered;
        PlayerController.TryAddObjectToQueue(this);
    }

    public void _on_mouse_exited()
    {
        this.mouseEventState =  State.MouseEventState.Exited;
        PlayerController.TryremoveObjectFromQueue(this);
    }
}