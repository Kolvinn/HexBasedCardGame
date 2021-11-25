using Godot;
using System;

public class CardListener : TextureRect
{
    private bool isDragging;
    public override void _Ready()
    {
        
    }
    public override object GetDragData(Vector2 position){
        return false;

        //if there's been no icon or icon texture loaded into inventory slot
        // don't drag anything
        // if(icon ==null || icon.Texture == null)
        //     return null;
        
        // //var data = 5;
        // var dragTexture = new TextureRect();
        // dragTexture.Expand = true;
        // dragTexture.Texture = this.icon.Texture;
        // dragTexture.RectSize = new Vector2(100,100);

        // var control  = new Control();
        // control.AddChild(dragTexture);
        // dragTexture.RectPosition = new Vector2(-0.5f * dragTexture.RectSize.x,-0.5f * dragTexture.RectSize.y);
        // SetDragPreview(control);

        // //set the 
        // this.swappingTex = this.icon.Texture;
        // this.icon.Texture = null;


        // GD.Print("mouse pos: ",GetLocalMousePosition());
        // GD.Print("drag pos: ",dragTexture.GetRect().Position);
        // GD.Print("Setting drag texture to: ",dragTexture);
        // return this;
    }
    public override bool CanDropData(Vector2 position, object data) {
        return false;
    }
    public override void DropData(Vector2 position, object data){

    } 
}   
