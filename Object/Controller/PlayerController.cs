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
public class PlayerController : Node2D, ControllerBase
{

    [Signal]
    public delegate void TriggerInteractive(Interactable itv);

    [Signal]
    public delegate void TriggerBuildingAction(Building model);
    HexHorizontalTest playerTile;

    public UpdateQueue UpdateUIQueue;


    //public int WoodAmount, StoneAmount, EssenceAmount,LeaveAmount;

    public TextureRect HealthBar;
    public TextureRect ManaBar;

    public Area2D SwordHitbox;

    public Dictionary<string, int> ResourceUpdate = new Dictionary<string, int>(){

    };

    public bool IsEmpowered
    {
        get;set;
    }

    public enum PlayerState 
    {
        WaitOnInputCommand,
        Default,
        AcceptAllInput,
        AcceptPartialInput,
        AcceptBattleInput
    }

    public Queue<GameObject> combatHits = new Queue<GameObject>();

    public AttackObjectQueue attackObjectQueue;

    public InventoryController inventory;


    private static float maxSpeed = 600, friction = 20000, acceleration = 20000;
    
    public InteractionQueue interactionQueue;
    

   // public bool isMovement = false;

    public Power currentPower = null;

    public Player player;

    public HealthBar healthBar;

    public State.ToolLevel ToolLevel = State.ToolLevel.Basic;

    //public bool isBusy = false;

    public static Vector2 lastMove = Vector2.Zero;

    private int health = 50;
    
    PlayerStateHandler stateHandler;

    public TextureRect PowerAbility;
    public TextureRect PowerEffect;

    public BuildingUIMenu buildingUI;

    public PlayerIcon playerIcon;


    public static Queue<BuildingIcon> buildingIconQueue = new Queue<BuildingIcon>();


    public PlayerController(){
        interactionQueue = new InteractionQueue();
        stateHandler = new PlayerStateHandler();
        attackObjectQueue = new AttackObjectQueue();
    }

    public override void _Ready()
    {
        this.SwordHitbox = this.GetNode<Area2D>("SwordHitbox");
        //this.state = ControllerState.AcceptAllInput;
    }

    public void PowerUp(Power power)
    {
        ReferenceTimer powerTimer = new ReferenceTimer(){
            Name = "Finish"            
        };
        powerTimer.Connect(nameof(ReferenceTimer.ReferenceTimerTimeout), this, nameof(On_PowerUpTimeout));
        
        ReferenceTimer flashTimer = new ReferenceTimer(){
            Name = "BeginFinish"
        };        
        flashTimer.Connect(nameof(ReferenceTimer.ReferenceTimerTimeout), this, nameof(On_PowerUpTimeout));
        this.AddChild(powerTimer);
        this.AddChild(flashTimer);
        powerTimer.Start(10);
        flashTimer.Start(5);
        Color c = PowerAbility.SelfModulate;
        c = new Color(0,82,255,255);
        PowerAbility.SelfModulate = c;

        if(power is WaterPower)
        {
            player.animationPlayer.Play("PowerUp");
            currentPower = new WaterPower();          
            PowerEffect.Visible = true;
        }
    }

    public void On_PowerUpTimeout(ReferenceTimer timer)
    {
        if(timer.Name == "BeginFinish")
        {  
            //
            PowerEffect.GetNode<AnimationPlayer>("AnimationPlayer").Play("Flash");
        }
        else if(timer.Name == "Finish")
        {
            currentPower = null;
            PowerEffect.Visible = false;
            Color c = PowerAbility.SelfModulate;
            c.a =0.5f;
            PowerAbility.SelfModulate = new Color(1,1,1,0.5f);
            PowerEffect.GetNode<AnimationPlayer>("AnimationPlayer").Stop();
        }
        
        this.RemoveChild(timer);
        timer.QueueFree();
    }

    public void RemovefromInventory(ResourceCost cost){
        foreach(var entry in cost.ResourceCostList)
        {
            //GD.Print("removing ", entry.Key, " ", this.ResourceUpdate.ContainsKey(entry.Key));
            this.ResourceUpdate[entry.Key] -= entry.Value;
            inventory.Remove(entry.Key, entry.Value);
        }
    }
    public void On_PowerTileInteraction(bool type)
    {
        Color c = PowerAbility.SelfModulate;
        if(type)
            c.a =1;
        else
            c.a = 0.5f;

        GD.Print("Setting colour a to: ", c.a);
        PowerAbility.SelfModulate = c;
    }

    public bool CanPowerup()
    {
        return this.PowerAbility.SelfModulate.a == 1f;
    }

    public void DoAction(string action)
    {
        player.SetAnimation(action, Vector2.Zero);
    }

    public void SetHealth(HealthBar health)
    {
        this.healthBar = health;
        this.healthBar.SetData(50, 1,50);
    }

    public void TakeDamage(int amount)
    {
        this.health -=amount;
        this.healthBar.UpdateHealth(-amount);
    }

    private Vector2 GetVectorAnimationSpace(Vector2 target){
        //////GD.Print("calc direction from player: "+ player.Position+ "     and target position "+ target);
        float x = 0f, y= 0f;
        if(target.x != player.Position.x)
            x = player.Position.x > target.x ? -1f:1f;

        if(target.y != player.Position.y)
            y = player.Position.y > target.y ? -1f:1f;

        //////GD.Print("returning vector: ", new Vector2(x,y));
        return new Vector2(x,y);
    }


    public bool CanInteractWithObject(GameObject obj)
    {
        return CanInteract() && (player.encounters.Contains(obj) || obj is Building);
    }

    public bool CanInteract()
    {
        return this.player.IsAvailable();
    }


    public void TryAddToInventory(GameObject obj, int amount)
    {
        if(inventory.CanStoreObject(obj)) 
        {
            if(obj is BasicResource)
            {
                var newObj = ((BasicResource)obj);
                TextureRect g = new TextureRect();
                if(newObj.ResourceType == "Grass Fiber")
                    g  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/Grass Fiber Icon.tscn");
                else if (newObj.ResourceType == "Dark Essence")
                    g  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/Dark Essence Icon.tscn");
                else if (newObj.ResourceType == "Mystic Seed")
                    g  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/MysticSeedIcon.tscn");
                else if(newObj.ResourceType.ToLower().Contains("rock") || newObj.ResourceType.ToLower().Contains("stone"))
                    g  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/StoneIcon.tscn");
                else if(newObj.ResourceType.ToLower().Contains("wood"))
                    g  = Params.LoadScene<TextureRect>("res://Object/UI/Icons/WoodIcon.tscn");
                    
                Texture tex = g.Texture;
                
                string resName = newObj.ResourceType;
                
                GD.Print("storing ", resName, " with texture: ", tex, "from ", newObj.ResourceType);
                UpdateUIQueue?.PushUpdate(g,amount, resName);

                if (ResourceUpdate.ContainsKey(resName))
                    ResourceUpdate[resName] +=amount;
                else
                    ResourceUpdate.Add(resName, amount);

                inventory.AddItem(tex,amount,resName);
                //player.inventory
                               
            }  
        }
    }

    public void On_Player_SwordHit(Node body)
    {
        
        if(body is GameObject && body!= this)
        {
            //GD.Print("ksljdnfksndkfaskdfj j sdfbgka kf;g akfds gb;kafej bka dfbkja jdfkb ");
            combatHits.Enqueue((GameObject)body);
        }
    }

    public bool CanAddToInventory()
    {
        return true;
    }
    public override void _PhysicsProcess(float delta)
    {
        stateHandler.HandleInput(this,delta);
        // switch(playerState)
        // {
        //     case PlayerState.Default:

        //         break;
        //     case PlayerState.AcceptAllInput:
        //         ParseEventQueue();
        //         GetMovementVector(delta);                
        //         break;
        //     case PlayerState.AcceptPartialInput:
                
        //         break;
        //     case PlayerState.AcceptBattleInput:
                
        //         break;
        //     case PlayerState.WaitOnInputCommand:
        //         //here we don't actually do anything because we wait for GameController to tell us what to do.
        //         break;
        // }
        // if(this.state == ControllerState.Wait)
        // {

        // }
        // else if(this.state == ControllerState.AcceptAllInput)
        // {
        //     ParseInputCommand();
        //     ParseEventQueue();
        //     GetMovementVector(delta);

        //     if(this.velocity != Vector2.Zero){

        //         player.MoveAndCollide(this.velocity/10);

        //     }
        // }
        // //block interactive buildings and tiles, but allow movement
        // else if(this.state == ControllerState.AcceptPartialInput)
        // {
        //     GetMovementVector(delta);

        //     if(this.velocity != Vector2.Zero){

        //         player.MoveAndCollide(this.velocity/10);
 
        //     }
        // }
        // else if(this.state == ControllerState.BattleInput)
        // {
        //     ParseHexMovementCommand(delta);
        // }
            
    }

    // public void ParseInputCommand()
    // {
    //     if(Input.IsActionJustPressed("left_click"))
    //     {
    //         if(interactionQueue.FetchQueue().Count > 0)
    //         {
    //             playerState = PlayerState.WaitOnInputCommand;
    //             //return;
    //             //HandleClickedObject(interactionQueue.FetchQueue()[0]);
    //             //interactionQueue.FetchQueue().RemoveAt(0);
    //         }
    //     }
    // }

    public GameObject GetInteractableObject()
    {
        if(this.interactionQueue.FetchQueue().Count>0)
        {
            return interactionQueue.FetchQueue()[0];
        }
        return null;
    }

   // public void HandleClickedObject(GameObject obj)
    //{
     //   if(player.encounters.Contains(obj))
   // }


    // public static void TryAddObjectToQueue(GameObject obj)
    //  {
    //     GD.Print("trying to add object to the player queue of type: ", obj.GetType());
    //     if(!eventQueue.Contains(obj) )//&& .encounters.Contains(obj))
    //         eventQueue.Add(obj);
    //  }
    
    // public static void TryremoveObjectFromQueue(GameObject obj)
    // {
    //     ////GD.Print("trying to remove object from the queue");
    //     eventQueue.Remove(obj);
    // }
    public void ParseEventQueue()
    {
        // bool action = Input.IsActionJustPressed("left_click");
        // if(this.)
        // if(eventQueue.Count > 0)
        // {
        //     //float action = Input.GetActionStrength("left_click");
        //     bool action = Input.IsActionJustPressed("left_click");
        //     if(action)
        //     {

        //             for(int i =0; i < eventQueue.Count;i++)
        //             {//foreach(GameObject o in eventQueue){
        //                 //////GD.Print("Parsing event from Queue");
        //                 GameObject o = eventQueue[i];
        //                 //eventQueue.Remove(o);
        //                 if(IsInstanceValid((Node)o))
        //                     HandleRightClick(o);
        //             }
        //             //}
        //             //eventQueue.Remove(o);
        //     }

        // }
        // else {
        //     bool action = Input.IsActionJustPressed("left_click");
        //     if(action){
        //         ////GD.Print("haandling left click for hex cos nothing in queue");
        //         HandleRightClick(HexGrid.hoveredHex);
        //     }
        // }
            
        
    }

    public void HandleRightClick(GameObject gameObject)
    {
        // ////GD.Print("handling gameObject: ",gameObject.GetType());
        // if(gameObject is GameResource)
        // {
        //     //if we aren't trying to get something that's busy
        //     //if(gameObject)
        //     GatherResource((GameResource)gameObject);
        //     if(((GameResource)gameObject).Capacity<=0)
        //     {
        //         eventQueue.Remove(gameObject);
        //         ((Node2D)gameObject).QueueFree();
        //     }
        //     //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart());
        //     //Node2D woodp = Params.LoadScene<Node2D>
        // }
        // else if(gameObject is HexHorizontalTest)
        // {
        //     //if we are clicking on a building
        //     if(((HexHorizontalTest)gameObject).building != null)
        //     {
        //         this.buildingAction = ((HexHorizontalTest)gameObject).building;
        //         EmitSignal(nameof(TriggerBuildingAction), this.buildingAction);
        //     }
        // }
        // else if(gameObject is Interactable)
        // {
        //     EmitSignal(nameof(TriggerInteractive), (Interactable)gameObject);
        // }
    }

    

    // public void GatherResource(GameResource gameObject)
    // {
    //     //////GD.Print("Gathering resource and playing animations");
    //     this.isBusy = true;
    //     var woodp = Params.LoadScene<WoodAnim>("res://Assets/Environment/WoodAnim.tscn");
    //     //this.AddChild(woodp);
    //     Node2D node = ((Node2D)gameObject);
    //     node.AddChild(woodp);
    //     woodp.ZIndex =3;
    //     //////GD.Print(((Node2D)gameObject).GlobalPosition, "   ", ((Node2D)gameObject).Position);
    //     woodp.GlobalPosition = ((Node2D)gameObject).GlobalPosition;

    //     woodp.GetNode<AnimationPlayer>("AnimationPlayer").Play("HoverAndFaade");
    //     gameObject.Capacity--;

    //     if(typeof(BasicResource) == gameObject.GetType())
    //     {   
    //         GD.Print("looking for resource ", gameObject.ResourceType);
    //         //int amount = -1;
    //         if(ResourceUpdate.ContainsKey(gameObject.ResourceType))
    //         {
    //             //int count = 0;
    //             GD.Print("Adding resource type ",gameObject.ResourceType);
    //             //ResourceUpdate.TryGetValue(gameObject.ResourceType, out count);
    //             ++ResourceUpdate[gameObject.ResourceType];//== ++count;
    //         }
    //         else
    //             ResourceUpdate.Add(gameObject.ResourceType, 1);
    //     }

    //     woodp.GetNode<Label>("Label").Text = "+1 " +gameObject.ResourceType;
    //     ResourcesGathered = true;
        
    // }
    
    public void GetMovementVector(float delta)
    {
        // if(currentMovement != Vector2.Zero || (this.movementQueue != null && this.movementQueue.Count !=0))
        //     return;

        var speedCheck = Vector2.Zero;
        
        //these will return 1s and 0s as it's a keyboard input
        speedCheck.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        speedCheck.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        speedCheck = speedCheck.Normalized();
        lastMove = speedCheck;
        
       
            
        
        if (speedCheck != Vector2.Zero){
            player.SetAnimation("parameters/Walk/blend_position", speedCheck);
            player.SetAnimation("parameters/Idle/blend_position", speedCheck);            
            
            player.animationState.Travel("Walk");
            //velocity += speedCheck * delta * acceleration;
            //velocity = velocity.Clamped(maxSpeed * delta);

            //player.Veloctiy = player.Veloctiy.MoveToward(speedCheck * maxSpeed, acceleration * delta);
            player.MoveAndCollide(speedCheck *  delta *maxSpeed);
        }
        else {
            
            
            player?.animationState.Travel("Idle");
            player.Veloctiy = player.Veloctiy.MoveToward(Vector2.Zero, friction * (delta/2));
            //velocity = Vector2.Zero;
        }
    }

    
}