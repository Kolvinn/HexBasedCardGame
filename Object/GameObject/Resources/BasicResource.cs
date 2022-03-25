using Godot;
using System;
using System.Text.RegularExpressions;
using System.Linq;
public class BasicResource : StaticInteractable, GameResource
{
    public State.MouseEventState mouseEventState;

    public int TotalResource{
        get;set;
    }

    public string ResourceType {
        get;set;
    }

    public string RequiredAction
    {
        get;set;
    }

    public State.ToolLevel ToolLevel 
    {
        get;set;
    }
    public GameResource.ResourceState resourceState
    {
        get;set;
    }


    public BasicResource()
    {
        this.TotalResource = 15;
        resourceState = GameResource.ResourceState.Available;
    }

    public override void _Ready()   
    {
        base._Ready();   
       // ResourceType = this.Name;
        this.ResourceType = new String(this.Name.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());

        if(this.ResourceType.Contains("@"))
            this.ResourceType = "Wood";
        //Regex.Replace(ResourceType, @"[\d-]", string.Empty);   
        if(ResourceType.Contains("Rock") || ResourceType.Contains("Stone")){
            //////GD.Print("Setting random rock atlas");
            //////GD.Print(this.Scale);
            this.GetNode<Sprite>("Sprite").Texture = this.GenRandomRockSprite();
        }

        ////GD.Print(this.ResourceType);
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

    public override void ValidateInteraction(string action)
    {
        
    }
    // public void _on_mouse_entered()
    // {
    //     //////GD.Print("Mouse entered for the wooooooood");
    //     this.mouseEventState =  State.MouseEventState.Entered;
    //     PlayerController.TryAddObjectToQueue(this);
    // }

    // public void _on_mouse_exited()
    // {
    //     this.mouseEventState =  State.MouseEventState.Exited;
    //     PlayerController.TryremoveObjectFromQueue(this);
    // }
}