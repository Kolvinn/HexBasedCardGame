using Godot;
using System;

public class Card : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private Texture frontImage, backImage;

    private AnimationPlayer animationPlayer;

    private bool flipping = false;

    public Card(Texture front = null, Texture back = null, Texture currentTexture = null){
        this.frontImage = front;
        this.backImage = back;
        this.Texture = currentTexture;
    }

    public Card(){

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
}
