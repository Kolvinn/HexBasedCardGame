using Godot;
using System;

public class CardListener : TextureRect
{
    
    public override void _Ready()
    {
        
    }
    public override object GetDragData(Vector2 position){
        return null;
    }
    public override bool CanDropData(Vector2 position, object data) {
        return false;
    }
    public override void DropData(Vector2 position, object data){

    } 
}   
