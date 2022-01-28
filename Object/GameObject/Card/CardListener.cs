using Godot;
using System;

public class CardListener : TextureRect, GameObject
{   
    private bool isDragging;

    private Texture currentTex;

    public bool canDrop{get;set;}
    
    public override void _Ready()
    {
        
    }

    public void SetTexture(Texture tex,bool isShowing){
        this.currentTex = tex;
        if(isShowing)
            this.Texture = currentTex;
    }

    public void SetParent(Card card){
        //this.parent = card;
    }


    public override object GetDragData(Vector2 position){
        ////GD.Print("trying to drag in test listener");
        Card c = (Card)this.GetParent();
        return c.TriggerGetDragDataFunc(position);
    }
    public override bool CanDropData(Vector2 position, object data) {
        Card c = (Card)this.GetParent();
        return c.TriggerCanDropDataFunc(position, data);
        //var array = new object[2]{position, data};

    }
    public override void DropData(Vector2 position, object data){
        Card c = (Card)this.GetParent();
        c.TriggerDropDataFunc(position, data);
    }

}   
