using Godot;
using System;

public class CardView : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    [Signal]
    public delegate bool TriggerStateChange(CardObject.CardState state);

    private Texture frontImage, backImage;

    private AnimationPlayer animationPlayer;

    private bool flipping = false;

    private CardObject.CardState cardState;

    public CardView(CardObject.CardState cardState, Texture front = null, Texture back = null, Texture currentTexture = null){
        this.frontImage = front;
        this.backImage = back;
        this.Texture = currentTexture;
        this.cardState = cardState;
    }

    public void SetParams( ref CardObject.CardState cardState, Texture front = null, Texture back = null, Texture currentTexture = null){
        this.frontImage = front;
        this.backImage = back;
        this.Texture = currentTexture;
        this.cardState = cardState;
    }
    public CardView(){

    }
    
    public override void _Ready()
    {
        this.animationPlayer = this.GetChild<AnimationPlayer>(0);   
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

    public void _on_CardListener_mouse_entered(){
        GD.Print("setting card state to hover");
        cardState = CardObject.CardState.Hover;
        EmitSignal(nameof(TriggerStateChange), cardState);
    }

    public void _on_CardListener_mouse_exited(){
        GD.Print("setting card state to hover removed");
        cardState = CardObject.CardState.HoverRemove;
        EmitSignal(nameof(TriggerStateChange), cardState);
    }
}
