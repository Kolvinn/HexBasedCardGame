using Godot;
using System;

public class CardTestListener : TextureRect
{   
    private bool isDragging;

    private Texture currentTex;

    public bool canDrop{get;set;}

    private CardTest parent;
    public override void _Ready()
    {
        
    }

    public void SetTexture(Texture tex,bool isShowing){
        this.currentTex = tex;
        if(isShowing)
            this.Texture = currentTex;
    }

    public void SetParent(CardTest view){
        this.parent = view;
    }


    public override object GetDragData(Vector2 position){
        GD.Print("trying to drag in test listener");
        return this.parent.TriggerGetDragDataFunc(position);
    }
    public override bool CanDropData(Vector2 position, object data) {
        return this.parent.TriggerCanDropDataFunc(position, data);
        //var array = new object[2]{position, data};

    }
    public override void DropData(Vector2 position, object data){
        this.parent.TriggerDropDataFunc(position, data);
    }
    public void _on_CardListener_mouse_entered(){

    } 
}   
