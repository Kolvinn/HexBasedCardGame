using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

/// <summary>
/// Class dedicated to handling player interactions with the world in terms of clicks and collisions 
/// </summary>
public class PlayerController : CharacterController
{

    [Signal]
    public delegate void TriggerInteractive(Interactable itv);

    [Signal]
    public delegate void TriggerBuildingAction(Building model);
    HexHorizontalTest playerTile;

    //public int WoodAmount, StoneAmount, EssenceAmount,LeaveAmount;

    public Dictionary<string, int> ResourceUpdate = new Dictionary<string, int>(){
        {"Wood", 50},
        {"Leaves", 50},
        {"Stone", 50}
    };
    public Vector2 mousePressPosition;


    public Building buildingAction;
    

    private Vector2 currentMovement = Vector2.Zero;

    public bool ResourcesGathered = false;

    private static float maxSpeed = 100, friction = 20000, acceleration = 20000;
    

    

    public bool isMovement = false;

    public Player player;

    public bool isBusy = false;


    public PlayerController(){
        eventQueue = new List<GameObject>();
    }

    public override void _Ready()
    {
        this.state = ControllerState.AcceptAllInput;
    }
    private Vector2 GetVectorAnimationSpace(Vector2 target){
        ////GD.Print("calc direction from player: "+ player.Position+ "     and target position "+ target);
        float x = 0f, y= 0f;
        if(target.x != player.Position.x)
            x = player.Position.x > target.x ? -1f:1f;

        if(target.y != player.Position.y)
            y = player.Position.y > target.y ? -1f:1f;

        ////GD.Print("returning vector: ", new Vector2(x,y));
        return new Vector2(x,y);
    }


    public override void _Process(float delta)
    {
        
        
    }
    public override void _PhysicsProcess(float delta)
    {
        if(this.state == ControllerState.Wait){

        }
        else if(this.state == ControllerState.AcceptAllInput)
        {
            ParseInputCommand();
            ParseEventQueue();
            GetMovementVector(delta);

            if(this.velocity != Vector2.Zero){

                player.MoveAndCollide(this.velocity/10);

            }
        }
        //block interactive buildings and tiles, but allow movement
        else if(this.state == ControllerState.AcceptPartialInput)
        {
            GetMovementVector(delta);

            if(this.velocity != Vector2.Zero){

                player.MoveAndCollide(this.velocity/10);

            }
        }
            
    }

    public void ParseInputCommand()
    {
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {

    }


    public static void TryAddObjectToQueue(GameObject obj)
     {
         //GD.Print("trying to add object to the queue of type: ", obj.GetType());
         if(!eventQueue.Contains(obj) && Player.encounters.Contains(obj))
            eventQueue.Add(obj);
     }
    
    public static void TryremoveObjectFromQueue(GameObject obj)
    {
        //GD.Print("trying to remove object from the queue");
        eventQueue.Remove(obj);
    }
    public void ParseEventQueue()
    {
        if(eventQueue.Count >0)
        {
            //float action = Input.GetActionStrength("left_click");
            bool action = Input.IsActionJustPressed("left_click");
            if(action)
            {

                    for(int i =0; i<eventQueue.Count;i++)
                    {//foreach(GameObject o in eventQueue){
                        ////GD.Print("Parsing event from Queue");
                        GameObject o = eventQueue[i];
                        //eventQueue.Remove(o);
                        HandleRightClick(o);
                    }
                    //}
                    //eventQueue.Remove(o);
            }

        }
        else {
            bool action = Input.IsActionJustPressed("left_click");
            if(action){
                //GD.Print("haandling left click for hex cos nothing in queue");
                HandleRightClick(HexGrid.hoveredHex);
            }
        }
            
        
    }

    public void HandleRightClick(GameObject gameObject)
    {
        //GD.Print("handling gameObject: ",gameObject.GetType());
        if(gameObject is GameResource)
        {
            
            GatherResource((GameResource)gameObject);
            if(((GameResource)gameObject).Capacity<=0)
            {
                eventQueue.Remove(gameObject);
                ((Node2D)gameObject).QueueFree();
            }
            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart());
            //Node2D woodp = Params.LoadScene<Node2D>
        }
        else if(gameObject is HexHorizontalTest)
        {
            //if we are clicking on a building
            if(((HexHorizontalTest)gameObject).building != null)
            {
                this.buildingAction = ((HexHorizontalTest)gameObject).building;
                EmitSignal(nameof(TriggerBuildingAction), this.buildingAction);
            }
        }
        else if(gameObject is Interactable)
        {
            EmitSignal(nameof(TriggerInteractive), (Interactable)gameObject);
        }
    }

    

    public void GatherResource(GameResource gameObject)
    {
        ////GD.Print("Gathering resource and playing animations");
        this.isBusy = true;
        var woodp = Params.LoadScene<WoodAnim>("res://Assets/Environment/WoodAnim.tscn");
        //this.AddChild(woodp);
        Node2D node = ((Node2D)gameObject);
        node.AddChild(woodp);
        woodp.ZIndex =3;
        ////GD.Print(((Node2D)gameObject).GlobalPosition, "   ", ((Node2D)gameObject).Position);
        woodp.GlobalPosition = ((Node2D)gameObject).GlobalPosition;

        woodp.GetNode<AnimationPlayer>("AnimationPlayer").Play("HoverAndFaade");
        gameObject.Capacity--;

        if(typeof(BasicResource) == gameObject.GetType())
        {   
            //int amount = -1;
            if(ResourceUpdate.ContainsKey(gameObject.ResourceType))
            {
                //int count = 0;
                //ResourceUpdate.TryGetValue(gameObject.ResourceType, out count);
                ++ResourceUpdate[gameObject.ResourceType];//== ++count;
            }
            else
                ResourceUpdate.Add(gameObject.ResourceType, 1);
        }

        woodp.GetNode<Label>("Label").Text = "+1 " +gameObject.ResourceType;
        ResourcesGathered = true;
        
    }
    
    public void GetMovementVector(float delta)
    {
        if(currentMovement != Vector2.Zero || (this.movementQueue != null && this.movementQueue.Count !=0))
            return;
        var speedCheck = Vector2.Zero;
        
        speedCheck.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        speedCheck.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        speedCheck = speedCheck.Normalized();
        
       
            
        
        if (speedCheck != Vector2.Zero){
            player.SetAnimation("parameters/Walk/blend_position", speedCheck);
            player.SetAnimation("parameters/Idle/blend_position", speedCheck);
            player.animationState.Travel("Walk");
            //velocity += speedCheck * delta * acceleration;
            //velocity = velocity.Clamped(maxSpeed * delta);
            velocity = velocity.MoveToward(speedCheck * maxSpeed, acceleration * delta);
        }
        else {
            
            
            player?.animationState.Travel("Idle");
            velocity = velocity.MoveToward(Vector2.Zero, friction * (delta/2));
            //velocity = Vector2.Zero;
        }
    }

    
}