using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
public class Card : Sprite, GameObject
{

    public List<string> testStringList =  new List<string>();

    public CardListener cardListener;
    public  CardModel model;

    private string objectId;

    private int CardModelId;

    private TextureRect topBanner;



  

    public Texture frontImage, backImage, currentTexture;


    public State.CardState cardState;
    private State.MouseEventState viewState;
    
    //[Signal]
    //public delegate bool TriggerStateChange(Card.CardState state);


    private AnimationPlayer animationPlayer;

    private bool flipping = false;

    

    private TextureRect textureRect;

    public Vector2 startingPosition {get;set;}
    
    [Signal]
    public delegate void CardEvent(Card c );

    public Label Title;
    public Label Cost;
    public TextureRect MidBanner;
    public RichTextLabel BottomText;

    public Card(){

    }

    static ObjectWrapper wrap(object o){
        return new ObjectWrapper(o);
    }
    public override void _Ready()
    {
        testStringList = new List<string>() {{"1"},{"2"},{"3"}};
        this.animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");   
        this.cardListener = GetNode<CardListener>("CardListener");
        this.Title = GetNode<Label>("CardListener/VBoxContainer/MarginContainer/TopBanner/Title");
        this.Cost = GetNode<Label>("CardListener/VBoxContainer/MarginContainer/TopBanner/Mana/Cost");
        this.MidBanner = GetNode<TextureRect>("CardListener/VBoxContainer/MarginContainer2/MidBanner");
        this.BottomText = GetNode<RichTextLabel>("CardListener/VBoxContainer/MarginContainer3/BottomText");
        ////GD.Print(Title, "  ",Cost, "  ",MidBanner);
    }


    public void SetTextures(Texture front, Texture back){
        this.frontImage = front;
        this.backImage = back;
        this.currentTexture = back; //give back unless reason not to
        this.Texture = front;
        this.cardState = State.CardState.Default;
    }

    /// <summary>
    /// Used to reset card variables before moving parents
    /// </summary>
    public void ResetCardState(){
        //this.Position = Vector2.Zero;
        this.RotationDegrees = 0;
    }

    public void LoadModel(CardModel model)
    {
        this.Title.Text = model.Name;
        this.Cost.Text = ""+model.Cost;
        this.MidBanner.Texture = ResourceLoader.Load<Texture>(model.FrontImagePath);
        this.BottomText.BbcodeText = model.Description;
        this.objectId = model.ObjectId;
        this.model = model;
    }

    // public CardView CreateCardView(){
    //     CardView cardView = Params.LoadScene<CardView>("res://View/CardView.tscn");
    //     cardView.SetParams(ref this.cardState,frontImage,backImage,frontImage);
    //     //this.AddChild(cardView);//new CardView(this.cardState,frontImage,backImage,frontImage);
    //     this.view = cardView;
    //     this.view.Connect("TriggerStateChange",this,nameof(TriggerCardStateChange));
    //     return this.view;
    // }

    public void TriggerCardStateChange(State.CardState state){
        this.cardState = state;
        EmitSignal("CardEvent", this);
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
        ////GD.Print("is trying to drag");
        this.TriggerCardStateChange(State.CardState.Drag);

        Control control = new TextureRect();

        //////GD.Print(control.SizeFlagsHorizontal);
        //control.RectSize = this.cardListener.RectSize;
        Card c = (Card)this.Duplicate();
        //control.RectScale = c.Scale;
        control.AddChild(c);
        c.Position = Vector2.Zero;

        //control.GetGlobalRect().Position = this.cardListener.RectPosition;
        Params.Print("Mouse: {0}  Control: {1}  New Card position: {2}",GetGlobalMousePosition(),control.RectPosition, c.Position);


        this.cardListener.SetDragPreview(control);
  
        return this;
    }

    public bool TriggerCanDropDataFunc(Vector2 position, object data){
        return true;
    }

    public bool TriggerDropDataFunc(Vector2 position, object data){
        
        return true;
    }

    public void _on_CardListener_mouse_entered()
    {
        this.TriggerCardStateChange(State.CardState.Hover);
    }

    public void _on_CardListener_mouse_exited()
    {
        this.TriggerCardStateChange(State.CardState.HoverRemove);
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