using Godot;
using System;

public class CardListener : TextureRect
{   
    [Signal]
    public delegate bool TriggerGetDragData(Vector2 position);

    [Signal]
    public delegate bool TriggerCanDropData(Vector2 position, Texture data);

    [Signal]
    public delegate bool TriggerDropData(Vector2 position, System.Object data);
    private bool isDragging;

    private Texture currentTex;

    private CardView parent;

    public bool canDrop{get;set;}
    public override void _Ready()
    {
        
    }

    public void SetTexture(Texture tex,bool isShowing){
        this.currentTex = tex;
        if(isShowing)
            this.Texture = currentTex;
    }

    public void SetParent(CardView view){
        this.parent = view;
    }

    public override object GetDragData(Vector2 position){
        return this.parent.TriggerGetDragDataFunc(position);
    }
    public override bool CanDropData(Vector2 position, object data) {
        GD.Print("check can drop");
        return this.parent.TriggerCanDropDataFunc(position, data);
        //var array = new object[2]{position, data};

    }
    public override void DropData(Vector2 position, object data){
        this.parent.TriggerDropDataFunc(position, data);
    } 
}   
