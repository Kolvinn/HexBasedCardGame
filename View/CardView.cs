using Godot;
using System;

public class CardView : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    [Signal]
    public delegate bool TriggerStateChange(CardObject.CardState state);

    [Signal]
    public delegate bool TriggerDrag(CardObject.CardState state);

    //[Signal]
    //public delegate bool TriggerStateChange(CardObject.CardState state);

    private Texture frontImage, backImage, currentTexture;

    private AnimationPlayer animationPlayer;

    private bool flipping = false;

    private CardObject.CardState cardState;

    private CardListener cardListener;

    // public CardView(CardObject.CardState cardState, Texture front = null, Texture back = null, Texture currentTexture = null){
    //     this.frontImage = front;
    //     this.backImage = back;
    //     this.Texture = currentTexture;
    //     this.cardState = cardState;
    // }

    public void SetParams( ref CardObject.CardState cardState, Texture front = null, Texture back = null, Texture currentTexture = null){
        this.frontImage = front;
        this.backImage = back;
        this.currentTexture = currentTexture;
        this.cardState = cardState;
    }
    public CardView(){

    }
    
    public override void _Ready()
    {
        this.animationPlayer = this.GetChild<AnimationPlayer>(0);   
        this.cardListener = GetNode<CardListener>("CardListener");
    }

    public void LoadCardTexture(Texture tex, bool isFront, Texture currentTexture = null){
        if(isFront)
            this.frontImage = tex;
        else
            this.backImage = tex;

        this.Texture = this.frontImage;
    }


    public void _on_AnimationPlayer_animation_finished(String anim_name){
        if(anim_name == "flip" && flipping){
            if(this.Texture == backImage){
                this.Texture = this.frontImage;
            }
            else{
                this.Texture  = backImage;
            }
            FlipCard(false);
        }
    }

    
    /// <summary>
    /// Flips this card and sets the texture
    /// </summary>
    /// <param name="start"> Whether this is a call to start the animation</param>    
    public void FlipCard(bool start){
        
        if(start){
            this.flipping = true; 
            animationPlayer.Play("flip");
        }
        else{
            this.flipping = false;
            animationPlayer.PlayBackwards("flip");
        }
       
        //animationPlayer.PlayBackwards("flip");

    }

    public override object GetDragData(Vector2 position){
        //isDragging = true;
        GD.Print("dragging?");
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

    public void _on_CardListener_mouse_entered(){

        cardState = CardObject.CardState.Hover;
        EmitSignal(nameof(TriggerStateChange), cardState);
    }

    public void _on_CardListener_mouse_exited(){

        cardState = CardObject.CardState.HoverRemove;
        EmitSignal(nameof(TriggerStateChange), cardState);
    }
}
