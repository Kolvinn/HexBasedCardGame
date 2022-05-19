using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;

/// <summary>
/// Class dedicated to handling character interactions with the world in terms of clicks and collisions 
/// </summary>
public class CharacterController :ControllerInstance
{   

    public Dictionary<string, int> ResourceUpdate = new Dictionary<string, int>(){
        {"Wood", 0},
        {"Leaves", 0},
        {"Stone",0}
    };

    //public JobPriority jobPriority = null;

    public bool TargetReached = false;
    
   // public HexHorizontalTest targetHex = null;

    public bool hasEnergy = true;
    public int capacity = 0;
    HexHorizontalTest characterTile;

    //public static List<GameObject> eventQueue;
    
    //public Queue<Vector2> movementQueue = new Queue<Vector2>();

    //private Vector2 currentMovement = Vector2.Zero;

    private bool isLocked = false;


    private static float maxSpeed = 200, friction = 20000, acceleration = 20000;
    
    //public Vector2 velocity = Vector2.Zero;
    
   // public  HexGrid grid;

    public Character character;

    public bool DestinationReached = false;

    public BasicResource resType;

    // [Signal]
	// public delegate void TargetHexReached(CharacterController c, string task);

    [Signal]
	public delegate void CapacityFull(CharacterController c);

    [Signal]
	public delegate void HexDepleted(HexHorizontalTest hex);
    
    [Signal]
    public delegate void ResourceDepleted(CharacterController c,BasicResource res);

    [Signal]
    public delegate void RestCompleted(CharacterController c);


    public Timer workerTimer;
    public Timer restTimer;
    
    public State.WorkerState workerState = State.WorkerState.Default;

    public string CurrentTask = "null";
    public Node Garbage = null;

    public BasicResource currentTargetResource;
    public int HighestTime =int.MinValue;

    
    public CharacterController(Character c){
        //eventQueue = new List<GameObject>();
        c.Connect("TargetHexReached", this, nameof(On_TargetHexReached));
        this.character = c;
        
    }


    public HexHorizontalTest  GetCurrentTile()
    {
        return character.currentTestTile;
    }
    public void SetDestination(HexHorizontalTest hex)
    {
        character.CallDeferred("SetTargetHex",hex);
    }
    public void Lock()
    {
        //GD.Print("LOCKING");
        this.isLocked = true;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public void UnLock()
    {
        //GD.Print("UNLOCKED");
        this.isLocked = false;
    }
   // public override void _PhysicsProcess(float delta)
   // {
       // this.delta = delta;
     
       // GD.Print("Setting delta to: ", this.delta);   // if(this.workerState == State.WorkerState.Gather)
        // {
        //     //trigger gather action
        //     //character.animationPlayer.Play("GatherResource");
        //    // character.animationPlayer.Connect("animation_finished", this,nameof(AnimationComplete));

        // }
        // else if(this.workerState == State.WorkerState.Dropping)
        // {
        //     //character.animationPlayer.Play("DropResource");
        //     //character.animationPlayer.Connect("animation_finished", this,nameof(AnimationComplete));
        // }
        // else if(this.workerState == State.WorkerState.Travel)
        // {
        //     ParseHexMovementCommand(delta);
        // }
        
        // if(this.workerState != State.WorkerState.Default){
        //     // if(hasEnergy && workerTimer == null)
        //     // {
        //     //     workerTimer = new Timer();
        //     //     this.AddChild(workerTimer);
        //     //     workerTimer.Connect("timeout", this, nameof(On_Timeout));
        //     //     workerTimer.Start(5);
        //     // }
        //     // else if(!hasEnergy && restTimer == null)
        //     // {
        //     //     restTimer = new Timer();
        //     //     this.AddChild(restTimer);
        //     //     restTimer.Connect("timeout", this, nameof(On_Timeout));
        //     //     restTimer.Start(5);

        //     // }
        // }
        
        // if(Garbage!= null)
        // {
        //     //GD.Print("Removing garbage node from parent: " ,Garbage.GetParent().GetType());
        //     Garbage.GetParent().RemoveChild(Garbage);
            
        //     Garbage.QueueFree();
        //     Garbage = null;
            
        // }
       
        
        //ParseHexMovementCommand(delta);

   // }

     
    public bool CanAddToInventory()
    {
        return this.capacity < 10;
    }
    public void DoAction(string action)
    {
        Lock();
        Action unlockCharacter = delegate() {UnlockCharacter();};
        
        ActionWrapper wrap = new ActionWrapper(unlockCharacter);
        character.CallDeferred("SetAnimation",action, Vector2.Zero, wrap);
    }

    public void UnlockCharacter()
    {
        UnLock();
        //GD.Print("UNLOCKED BBY");
    }
    public bool CanInteract()
    {
        return character.IsAvailable() && character.gatherTimer.TimeLeft ==0;
    }
    public void AnimationComplete(string anim_name)
    {
        
        // if(anim_name == "GatherResource")
        // {

        // }

    }
    // public void Rest()
    // {
    //     CharacterController c = null;
    //     Building b = null;
    //     foreach(var v in GameController.RestSpots)
    //     {
    //         if(v.Value == null || v.Value == this)
    //         {
    //             //c = null;
    //             b = v.Key;
    //             break;
    //         }
    //         // else if()
    //         // {
    //         //     b = v.Key;
    //         //     c= this;
    //         //     break;
    //         // }
    //     }
        
    //     //if no building, then we need to trigger warning message
    //     if(b == null){
    //         GameController.ShowWarning(this);
    //     }
        
    //     if(b!=null)
    //     {
    //         //if empty
    //         if(c == null)
    //         {
    //             GameController.RestSpots[b] = this;
    //         }
            
    //         //GD.Print("Bulding asset is: ",grid);
    //         //GD.Print("Bulding asset postion is: ",HexGrid.IndexOfVec(b.buildingAsset.Position));
    //         foreach(var v in grid.pathFinder.GetPointPath(HexGrid.IndexOfVec(character.currentTestTile.Position),HexGrid.IndexOfVec(b.buildingAsset.Position)))
    //         {
    //            // GD.Print("Populating movement vector to rest spot: ", v);
    //             this.movementQueue.Enqueue(v);
    //         }
    //     }
    // }
    public void On_Timeout()
    {
        
        if(workerTimer!= null)
        {
            GD.Print("Working time timeout");
            hasEnergy = false;
            this.RemoveChild(workerTimer);
            workerTimer.QueueFree();
            workerTimer = null;
        }

        else if(restTimer != null)
        {
            
            GD.Print("rest time timeout");
            hasEnergy = true;
            this.RemoveChild(restTimer);
            restTimer.QueueFree();
            restTimer = null;
            EmitSignal(nameof(RestCompleted), this);
        }
    }
    public bool HasReachedDestination()
    {
        return this.DestinationReached;
    }
    public void On_TargetHexReached(){
        //GD.Print("Destination reached in charcter controller");
        this.DestinationReached = true;
        //currentMovement = Vector2.Zero;
        //movementQueue= new Queue<Vector2>();
        //this.workerState = State.WorkerState.Idle;
        this.TargetReached = true;
        //EmitSignal("TargetHexReached", this, CurrentTask);
    }

    public void MoveToPoint(Vector2 dest)
    {
        var direction = GetVectorAnimationSpace(dest);
        
       // character.CallDeferred("SetAnimation",action, Vector2.Zero, wrap);
                
        character.CallDeferred("SetAnimation","parameters/Walk/blend_position", direction,null);
        character.CallDeferred("SetAnimation","parameters/Idle/blend_position", direction,null);
        
        //character.animationState.Travel() 
        character.animationState.Travel("Walk");
       // GD.Print("DELTA: ", character.delta);
        var thing = character.Position.MoveToward(dest,character.delta*maxSpeed*2);
        //GD.Print(character.delta,"   ",dest, "   ",thing);
        character.Position = thing;

    }


    // public void GatherResource(){
    //     //this.character.TargetHex.Get
    //     int i = 0;
        
        
    //     BasicResource res = null;

    //     if(this.character.TargetHex.HexEnv is YSort)
    //     {   
    //         Node n = null;
    //         //GD.Print("is rock or leaves");
    //         while(n == null){
    //             n = this.character.TargetHex.HexEnv.GetChild(i);
    //         }
           
    //         res = (BasicResource)n;
    //     }
    //     else if(this.character.TargetHex.HexEnv is BasicResource)
    //     {
    //         //GD.Print("is wood");
    //         res = (BasicResource)this.character.TargetHex.HexEnv;
   
    //     }
      
    //     resType = new BasicResource();
    //     resType.ResourceType = res.ResourceType;

    //     // if(!res.IsBusy)
    //     //     res.IsBusy = true;
    //     // else
    //     // {
    //     //     GatherResource();
    //     //     return;

    //     // }
    //     //GD.Print("Gathering Resource ", res.ResourceType);

    //     while(res.TotalResource != 0 && capacity != character.InventoryCapacity)
    //     {
    //         res.TotalResource -= 1;
    //         capacity +=1;

    //         //hex is now out of resources
    //     }

        

    //     if(res.TotalResource == 0){
    //         //GD.Print("one insataance of bassiced resource depleted ");
    //         //Garbage = res;

    //         EmitSignal("ResourceDepleted",this, res);
            
    //         if(!this.character.TargetHex.HasAvailableResource())
    //         {
    //             //GD.Print("dealing with depleted hex");
    //             EmitSignal("HexDepleted", this.character.TargetHex);
    //         }           
            
    //         //this.character.TargetHex?.HexEnv?.RemoveChild(n);  
            
    //     }
    //     // if(this.character.TargetHex.HexEnv == null || this.character.TargetHex.HexEnv.GetChildren().Count==0)
    //     // {
    //     //     //GD.Print("dealing with depleted hex");
    //     //     EmitSignal("HexDepleted", this.character.TargetHex);
    //     // }   

    //         ////GD.Print("Capacity reached");
    //         //EmitSignal(nameof(CapacityFull),this);
            
        
                     
        
    // }

    public virtual Vector2 GetVectorAnimationSpace(Vector2 target){
        //////GD.Print("calc direction from character: "+ character.Position+ "     and target position "+ target);
        float x = 0f, y= 0f;
        if(target.x != character.Position.x)
            x = character.Position.x > target.x ? -1f:1f;

        if(target.y != character.Position.y)
            y = character.Position.y > target.y ? -1f:1f;

        //////GD.Print("returning vector: ", new Vector2(x,y));
        return new Vector2(x,y);
    }

    // public Vector2 FetchNextMovement()
    // {
    //     if(movementQueue?.Count > 0)
    //     {
    //         return movementQueue.Dequeue();
    //     }
    //     return Vector2.Zero;
    // }

    // public void SetVectorTarget(Vector2 vec)
    // {
    //     this.currentMovement = vec;
    // }

    // public Vector2 GetVectorTarget()
    // {
    //     return this.currentMovement;
    // }

    // public bool HasReachedDestination()
    // {
    //     return currentMovement == character.Position;
    // }

    // public void MoveToPoint(Vector2 dest)
    // {
    //     var direction = GetVectorAnimationSpace(currentMovement);
                
    //     character.SetAnimation("parameters/Walk/blend_position", direction);
    //     character.SetAnimation("parameters/Idle/blend_position", direction);

    //     character.animationState.Travel("Walk");
    //     character.Position = character.Position.MoveToward(currentMovement,delta*maxSpeed);

    // }
    
    // public virtual void ParseHexMovementCommand(float delta)
    // {

        

    //     //if we are moving, continue moving
    //     if(currentMovement !=  Vector2.Zero){
    //         this.characterTile = (HexHorizontalTest)character.currentTestTile;

            
    //         if(character.Position==currentMovement){
                
    //             //character.animationState.Travel("Idle");              
    //             currentMovement =Vector2.Zero;
    //         }
    //         else{
                
                
    //             var direction = GetVectorAnimationSpace(currentMovement);
                
    //             character.SetAnimation("parameters/Walk/blend_position", direction);
    //             character.SetAnimation("parameters/Idle/blend_position", direction);

    //             character.animationState.Travel("Walk");
    //             character.Position = character.Position.MoveToward(currentMovement,delta*maxSpeed);
    //             //this.character.move(velocity);
    //             //MoveToPoint(currentMovement, delta);
    //         }
    //     }
    //     //if we still have movement left to do
    //     if(currentMovement == Vector2.Zero && this.movementQueue != null && this.movementQueue.Count !=0){   
                          
    //         currentMovement = this.movementQueue.Dequeue();
    //     }
    //     // else if(currentMovement == Vector2.Zero && !DestinationReached && character.TargetHex != null)
    //     // {
    //     //     character.Position = character.Position.MoveToward(character.TargetHex.Position,delta*maxSpeed);
    //     // }
    //     else if(currentMovement == Vector2.Zero && this.movementQueue?.Count == 0 && this.DestinationReached) {
    //         character.animationState.Travel("Idle");    
    //         this.workerState = State.WorkerState.Idle;
    //     }
        //if we have a movement command pressed and are not currently moving,
        //create new movement queue and add to current movement
        // else if(this.character != null && Input.IsActionJustPressed("right_click") 
        // && (this.movementQueue ==null || this.movementQueue.Count==0))
        // {
        //     //////GD.Print("right click found");
        //     HexHorizontalTest found=null;
        //     int toindex =-1, fromidx = -1;


        //     foreach(KeyValuePair<int,HexHorizontalTest> cell in grid.storedHexes){

        //         //get the last tile that the mouse was in
        //         if(cell.Value.eventState == State.MouseEventState.Entered){
        //             ////GD.Print("Entered cell: ", cell);

        //             found = cell.Value;
        //             toindex = cell.Key;
        //         }
                
        //         //get the index of the current character tile
        //         if(cell.Value == character.currentTestTile)
        //         {
        //             fromidx = cell.Key;
        //         }

        //         if(toindex >=0 && fromidx >=0)
        //             break;
        //     }

        //     if(found == null || !found.Visible)
        //             return;

        //     //tiles.TryGetValue(characterTile, out fromidx);
        //     if(toindex >=0 && fromidx >=0)
        //     {
        //         ////GD.Print("movementQueue populating");
        //         foreach(Vector2 vec in grid.pathFinder.GetPointPath(fromidx, toindex))
        //         {
        //             ////GD.Print(vec);
        //             //path.AddPoint(vec);
        //             movementQueue.Enqueue(vec);


        //         }
        //     }
        //     if(movementQueue.Count >0)
        //     {   this.currentMovement = movementQueue.Dequeue();
        //         this.character.Position = this.currentMovement;                    
        //     }            
        // }
        //character.MoveAndSlide(this.velocity);
   // }

    

    
}