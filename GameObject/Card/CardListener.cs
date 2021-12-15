using Godot;
using System;

public class CardListener : TextureRect
{   
    private bool isDragging;

    private Texture currentTex;

    public bool canDrop{get;set;}

    private Card parent;

    private CardTest testParent;
    public override void _Ready()
    {
        
    }

    public void SetTexture(Texture tex,bool isShowing){
        this.currentTex = tex;
        if(isShowing)
            this.Texture = currentTex;
    }

    public void SetParent(Card view){
        this.parent = view;
    }

    public void SetParent(CardTest view){
        this.testParent = view;
    }

    public override object GetDragData(Vector2 position){
        return this.parent.TriggerGetDragDataFunc(position);
    }
    public override bool CanDropData(Vector2 position, object data) {
        return this.parent.TriggerCanDropDataFunc(position, data);
        //var array = new object[2]{position, data};

    }
    public override void DropData(Vector2 position, object data){
        this.parent.TriggerDropDataFunc(position, data);
    } 
}   
