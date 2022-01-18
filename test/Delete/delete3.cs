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
public class delete3 : Node2D
{
    private CardController cardController;
    private CanvasLayer canvasLayer;

    public Dictionary<int,HexHorizontalTest> storedHexes = new Dictionary<int, HexHorizontalTest>();
    private NewHexMapTests hexMap;
    private Dictionary<string,string> dict;
    
    private Polygon2D poly;
    private Card c;

    private YSort map, env;

    private SaveInstance s;

    private Button saveButton;

    private HexGrid grid;


#region hexmapvars
 public Player player;

    [PostLoad]
    private YSort tileMap;

    [PostLoad]
    public Dictionary<HexCell1,int> tiles;

    //[PostLoad]
    //Line2D path;

    [PostLoad]
    private HexCell1 startingTile;

    public  AStar2D pathFinder;




    private static float maxSpeed = 300, friction = 2500, acceleration = 2000;
    
    public Vector2 velocity = Vector2.Zero;

    HexHorizontalTest playerTile;
    

    public Queue<Vector2> movementQueue = new Queue<Vector2>();


    private Vector2 currentMovement = Vector2.Zero;

    
    

    public bool isMovement = false;
#endregion
    
    public override void _Ready()
    {
        
        this.pathFinder  =new AStar2D();
        this.player = this.GetNode<Player>("Environment/Player");
        pathFinder.AddPoint(4, this.GetNode<HexHorizontalTest>("YSort/Hex4").Position);
        
        pathFinder.AddPoint(5, this.GetNode<HexHorizontalTest>("YSort/Hex5").Position);
        
        pathFinder.AddPoint(6, this.GetNode<HexHorizontalTest>("YSort/Hex6").Position);
        pathFinder.ConnectPoints(5,6);
        pathFinder.ConnectPoints(4,5);
        pathFinder.ConnectPoints(4,6);
        pathFinder.AddPoint(7, this.GetNode<HexHorizontalTest>("YSort/Hex7").Position);
        pathFinder.ConnectPoints(5,7);
        pathFinder.ConnectPoints(6,7);
        
        pathFinder.AddPoint(10, this.GetNode<HexHorizontalTest>("YSort/Hex10").Position);
        pathFinder.ConnectPoints(10,7);
        pathFinder.ConnectPoints(10,6);
        
        pathFinder.AddPoint(13, this.GetNode<HexHorizontalTest>("YSort/Hex13").Position);
        player.currentTestTile = this.GetNode<HexHorizontalTest>("YSort/Hex13");
        player.Position = this.GetNode<HexHorizontalTest>("YSort/Hex13").Position;
        pathFinder.ConnectPoints(10,13);
        
        pathFinder.AddPoint(12, this.GetNode<HexHorizontalTest>("YSort/Hex12").Position);
        pathFinder.ConnectPoints(12,13);
        
        pathFinder.AddPoint(9, this.GetNode<HexHorizontalTest>("YSort/Hex9").Position);
        pathFinder.ConnectPoints(9,12);
        
        pathFinder.AddPoint(8, this.GetNode<HexHorizontalTest>("YSort/Hex8").Position);
        pathFinder.ConnectPoints(9,8);

        pathFinder.AddPoint(2, this.GetNode<HexHorizontalTest>("YSort/Hex2").Position);
        pathFinder.ConnectPoints(8,2);


        for(int i=2; i<14;i++)
        {
            HexHorizontalTest hex = this.GetNode<HexHorizontalTest>("YSort/Hex"+i+"");
            if(hex !=null)
            {
                storedHexes.Add(i,hex);
                //GD.Print(hex + "   ",i);
            }
        }
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(5,this.GetNode<HexHorizontalTest>("YSort/Hex5"));
        
        // storedHexes.Add(6,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));
        
        // storedHexes.Add(4,this.GetNode<HexHorizontalTest>("YSort/Hex4"));


        
    }


    public void _on_Explore_pressed()
    {
        this.GetNode<TextureRect>("CanvasLayer/Control/ExploreUI").Visible = true;
        this.GetNode<Sprite>("MapIconHover").Visible = true;
        this.GetNode<Sprite>("MapIconHover").GetNode<AnimationPlayer>("AnimationPlayer").Play("Default");
        this.GetNode<Button>("CanvasLayer/Control/Explore").Disabled = true;
        this.GetNode<Label>("CanvasLayer/DeadBodyImage").Visible =true;
    }

    public void _on_Continue_pressed()
    {
        this.GetNode<TextureRect>("CanvasLayer/Control/ExploreUI").Visible = false;
    }
  
    
    

    

    public void RecursiveChildPrint(Node node){
        foreach(Node n in node.GetChildren()){
            RecursiveChildPrint(n);
            if(n.GetType() == typeof(TextureRect)){
                //GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

            }
            if(n.GetType() == typeof(CardListener)){
                //GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

            }
       }
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


    public override void _Process(float delta){
        //if we are moving, continue moving
        if(currentMovement !=  Vector2.Zero){
            this.playerTile = (HexHorizontalTest)player.currentTestTile;
            if(playerTile.Name == "Hex4")
            {
                this.GetNode<TextureRect>("CanvasLayer/Control/Letter").Visible = true;
            }
            
            if(player.Position==currentMovement){
                
                player.animationState.Travel("Idle");              
                currentMovement =Vector2.Zero;
            }
            else{
                
                
                var direction = GetVectorAnimationSpace(currentMovement);
                
                player.SetAnimation("parameters/Walk/blend_position", direction);
                player.SetAnimation("parameters/Idle/blend_position", direction);

                player.animationState.Travel("Walk");
                player.Position = player.Position.MoveToward(currentMovement,delta*250/2);
                ////GD.Print("current player position: "+player.Position);
                //this.player.move(velocity);
                //MoveToPoint(currentMovement, delta);
            }
        }
        //if we still have movement left to do
        if(currentMovement == Vector2.Zero && this.movementQueue != null && this.movementQueue.Count !=0){                    
            currentMovement = this.movementQueue.Dequeue();
        }
        //if we have a movement command pressed and are not currently moving,
        //create new movement queue and add to current movement
        else if(this.player != null && Input.IsActionJustPressed("right_click") 
        && (this.movementQueue ==null || this.movementQueue.Count==0))
        {
            ////GD.Print("right click found");
            HexHorizontalTest found=null;
            int toindex =-1, fromidx = -1;


            foreach(KeyValuePair<int,HexHorizontalTest> cell in storedHexes){
                //path?.ClearPoints();

                //get the last tile that the mouse was in
                if(cell.Value.eventState == State.MouseEventState.Entered){
                    //GD.Print("Entered cell: ", cell);

                    found = cell.Value;
                    toindex = cell.Key;
                }
                
                //get the index of the current player tile
                if(cell.Value == player.currentTestTile)
                {
                    fromidx = cell.Key;
                }

                if(toindex >=0 && fromidx >=0)
                    break;
            }

            if(found == null || !found.Visible)
                    return;

            //tiles.TryGetValue(playerTile, out fromidx);
            if(toindex >=0 && fromidx >=0)
            {
                //GD.Print("movementQueue populating");
                foreach(Vector2 vec in pathFinder.GetPointPath(fromidx, toindex))
                {
                    //GD.Print(vec);
                    //path.AddPoint(vec);
                    movementQueue.Enqueue(vec);


                }
            }
            if(movementQueue.Count >0)
            {   this.currentMovement = movementQueue.Dequeue();
                this.player.Position = this.currentMovement;                    
            }            
        }
        // //player.MoveAndSlide(this.velocity);
    }




    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
