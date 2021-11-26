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

    private CardListener cardListener;

    private TextureRect textureRect;
    private CardObject.CardState cardState;


    public enum CardViewState{
        Pressed,
        Released
    }

    private CardViewState viewState;

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
        //this.textureRect = GetNode<TextureRect>("TextureRect");
        this.cardListener = GetNode<CardListener>("CardListener");
        //this.viewState = null;
        // this.cardListener.Connect("TriggerGetDragData",this,nameof(TriggerGetDragDataFunc));
        // this.cardListener.Connect("TriggerCanDropData",this,nameof(TriggerCanDropDataFunc));
        // this.cardListener.Connect("TriggerDropData",this,nameof(TriggerDropDataFunc));
        //this.cardListener.SetParent(this);

        //this.cardListener.Texture = this.Texture;
        //cardListener.Visible =false;
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

    
    // public void _on_Hitbox_mouse_entered(){

    //     cardState = CardObject.CardState.Hover;
    //     EmitSignal(nameof(TriggerStateChange), cardState);
    // }

    // public void _on_Hitbox_mouse_exited(){

    //     cardState = CardObject.CardState.HoverRemove;
    //     EmitSignal(nameof(TriggerStateChange), cardState);
    // }


    // public void _on_Hitbox_input_event(Node viewport, InputEvent inputEvent, int shape_idx){
    //     if (inputEvent is InputEventMouseButton){
    //         GD.Print("sldjnklsndfjklns");
    //         InputEventMouseButton emb = (InputEventMouseButton)inputEvent;
    //         if (emb.ButtonIndex == (int)ButtonList.Left){
    //             if (emb.IsPressed()){  
    //                 GD.Print("pressed");
    //             }                          
    //             //if (emb.ButtonIndex == (int)ButtonList.WheelUp && currentzoom >= upperLimit){
    //             //}
            
    //             if(emb.IsActionReleased("left_click")){
    //                 GD.Print("releasd");
            
    //             }
    //         }
            
    //     }
    //     else if (inputEvent is InputEventMouseMotion && this.viewState == CardViewState.Pressed){
    //         cardState = CardObject.CardState.Drag;
    //         EmitSignal(nameof(TriggerStateChange), cardState);
    //         //var data = 5;
    //         var dragTexture = new TextureRect();
    //         dragTexture.Expand = true;
    //         dragTexture.Texture = this.Texture;
    //         Vector2 rectSize = this.GetRect().Size;
    //         dragTexture.RectSize = rectSize;

    //         var control  = new Control();
    //         control.AddChild(dragTexture);
            
    //         dragTexture.RectPosition = new Vector2(-0.5f * dragTexture.RectSize.x,-0.5f * dragTexture.RectSize.y);
    //         GetViewport().GuiGetDragData();
    //         this.textureRect.SetDragPreview(control);

    //     }
    // }




    public object TriggerGetDragDataFunc(Vector2 position ){
        //if there's been no icon or icon texture loaded into inventory slot
        //don't drag anything
        if(Texture ==null)
            return null;
        
        cardState = CardObject.CardState.Drag;
        EmitSignal(nameof(TriggerStateChange), cardState);
        //var data = 5;
        var dragTexture = new TextureRect();
        dragTexture.Expand = true;
        dragTexture.Texture = this.Texture;
        Vector2 rectSize = this.GetRect().Size;
        dragTexture.RectSize = rectSize;

        var control  = new Control();
        control.AddChild(dragTexture);
        dragTexture.RectPosition = new Vector2(-0.5f * dragTexture.RectSize.x,-0.5f * dragTexture.RectSize.y);
        this.cardListener.SetDragPreview(control);

        //set the 
        //this.swappingTex = this.icon.Texture;
        //this.icon.Texture = null;


        GD.Print("mouse pos: ",GetLocalMousePosition());
        GD.Print("drag pos: ",dragTexture.GetRect().Position);
        GD.Print("Setting drag texture to: ",dragTexture);
        return control;
    }
    public bool TriggerCanDropDataFunc(Vector2 position, object data){
        GD.Print("trigger can drop");
        return false;
    }
    public bool TriggerDropDataFunc(Vector2 position, object data){
        GD.Print("trigger  drop");
        return false;
    }
}
