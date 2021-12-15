using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
public class Card : Sprite, GameObject
{
    private CardModel model;

    public Texture frontImage, backImage, currentTexture;
  
    private Queue<Card> eventQueue;
    public enum CardState{
        Flip,
        Draw,
        Discard,
        Drag,
        Drop,
        DropCancel,
        Hover, 
        HoverRemove,
        Default

    }
    
    public CardState cardState;



    private AnimationPlayer animationPlayer;

    private bool flipping = false;

    private CardListener cardListener;

    private TextureRect textureRect;


    public enum CardViewState{
        Pressed,
        Released
    }

    public Vector2 startingPosition {get;set;}
    private CardViewState viewState;

    public Card(){

    }

    public override void _Ready()
    {
        this.animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");   
        this.cardListener = GetNode<CardListener>("CardListener");
        this.cardListener.SetParent(this);
    }


    public void SetTextures(Texture front, Texture back){
        this.frontImage = front;
        this.backImage = back;
        this.currentTexture = back; //give back unless reason not to
        this.Texture = front;
        this.cardState = CardState.Default;
    }

    public void SetEventQueue(Queue<Card> que){
        this.eventQueue = que;
    }

    /// <summary>
    /// Used to reset card variables before moving parents
    /// </summary>
    public void ResetCardState(){
        this.Position = Vector2.Zero;
        this.RotationDegrees = 0;
    }

    public bool LoadObject()
    {
        throw new NotImplementedException();
    }

    public bool SaveObject()
    {
        throw new NotImplementedException();
    }



#region MouseEventHandle

    public object TriggerGetDragDataFunc(Vector2 position ){
        //if there's been no icon or icon texture loaded into inventory slot
        //don't drag anything
        if(Texture ==null)
            return null;
        
        cardState = CardState.Drag;
        
        //var data = 5;
        var dragTexture = new TextureRect();
        dragTexture.Expand = true;
        dragTexture.Texture = this.Texture;
        Vector2 rectSize = this.GetRect().Size;
        dragTexture.RectSize = rectSize;

        var control  = new Control();
        //CardTest c = (CardTest)this.Duplicate([DuplicateFlags.Groups,DuplicateFlags.Scripts,DuplicateFlags.Signals,DuplicateFlags.UseInstancing]);

        //dragTexture.AddChild(c);

        dragTexture.RectPosition = new Vector2(-0.5f * dragTexture.RectSize.x,-0.5f * dragTexture.RectSize.y);

        dragTexture.RectScale = new Vector2(0.5f,0.5f);
        

        this.cardListener.SetDragPreview(dragTexture);
        control.RectScale = new Vector2(Params.GlobalScale, Params.GlobalScale);
        eventQueue.Enqueue(this);

        return this;
    }

    public bool TriggerCanDropDataFunc(Vector2 position, object data){

        return true;
    }

    public bool TriggerDropDataFunc(Vector2 position, object data){

        return true;
    }


    public void _on_CardListener_mouse_entered(){
        this.cardState = CardState.Hover;
        this.eventQueue.Enqueue(this);
    }

    public void _on_CardListener_mouse_exited(){
        this.cardState = CardState.HoverRemove;
        this.eventQueue.Enqueue(this);
    }

   public void _on_CardListener_gui_input(InputEvent inputEvent){
   }

#endregion

#region Animations
    
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
#endregion

}