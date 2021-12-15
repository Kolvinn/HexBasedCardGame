using Godot;
using System.Collections.Generic;

public class NewHexMapTests : Node2D
{
    private YSort tileMap;

    private AStar2D pathFinder;

    Player player;

    private static float maxSpeed = 300, friction = 2500, acceleration = 2000;
    
    public Vector2 velocity = Vector2.Zero;

    HexCell1 playerTile;

    Line2D path;

    Queue<Vector2> movementQueue;


    private Vector2 currentMovement = Vector2.Zero;

    Dictionary<HexCell1,int> tiles;

    bool isMovement = false;
    public override void _Ready()
    {   
        movementQueue = new Queue<Vector2>();
        tiles = new Dictionary<HexCell1,int>();
        tileMap = GetNode<YSort>("TileLayer1");
        pathFinder = new AStar2D();
        player = GetNode<Player>("EvnLayer/Player");
        path = GetNode<Line2D>("Line2D");
        
        int i =0;
        foreach(Node node in tileMap.GetChildren()){
            tiles.Add((HexCell1)node,i);
            ////GD.print(((Area2D)node).Position);
            if(i==0){
                playerTile = (HexCell1)node;
                //GD.print(playerTile);
            }
            pathFinder.AddPoint(i++,((Area2D)node).GlobalPosition);

            
           
        }

        ConnectPoints();

        
    }

    private void ConnectPoints(){
        pathFinder.ConnectPoints(0,1);
        pathFinder.ConnectPoints(0,2);

        pathFinder.ConnectPoints(1,3);
        pathFinder.ConnectPoints(2,4);

        pathFinder.ConnectPoints(4,5);
        pathFinder.ConnectPoints(4,6);
        pathFinder.ConnectPoints(3,5);

        pathFinder.ConnectPoints(6,7);
        pathFinder.ConnectPoints(6,8);
        pathFinder.ConnectPoints(7,8);

        pathFinder.ConnectPoints(8,10);
        pathFinder.ConnectPoints(10,11);

        pathFinder.ConnectPoints(11,9);

        pathFinder.ConnectPoints(9,3);
        pathFinder.ConnectPoints(5,9);
        pathFinder.ConnectPoints(6,5);
    }
    

    private Vector2 GetVectorAnimationSpace(Vector2 target){
        //GD.print("calc direction from player: "+ player.Position+ "     and target position "+ target);
        float x = 0f, y= 0f;
        if(target.x != player.Position.x)
            x = player.Position.x > target.x ? -1f:1f;

        if(target.y != player.Position.y)
            y = player.Position.y > target.y ? -1f:1f;

        //GD.print("returning vector: ", new Vector2(x,y));
        return new Vector2(x,y);
    }


    public override void _Process(float delta){
        //if we are moving, continue moving
        if(currentMovement !=  Vector2.Zero){
            this.playerTile = (HexCell1)player.currentTile;
            if(player.Position==currentMovement){
                //GD.print("finished current movement");
                player.animationState.Travel("Idle");              
                currentMovement =Vector2.Zero;
            }
            else{
                
                
                var direction = GetVectorAnimationSpace(currentMovement);
                
                player.SetAnimation("parameters/Walk/blend_position", direction);
                player.SetAnimation("parameters/Idle/blend_position", direction);

                player.animationState.Travel("Walk");
                player.Position = player.Position.MoveToward(currentMovement,delta*250);
                //GD.print("current player position: "+player.Position);
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
        else if(Input.IsActionJustPressed("right_click") && (this.movementQueue ==null || this.movementQueue.Count==0)){

            //GD.print("right click found");
            HexCell1 found=null;
            int toindex =0;

            foreach(HexCell1 cell in tiles.Keys){
                path.ClearPoints();
                if(cell.eventState == Params.MouseEventState.Entered){

                    found = cell;
                    tiles.TryGetValue(cell, out toindex);


                    int fromidx;
                    tiles.TryGetValue(playerTile, out fromidx);
                    GD.Print("movementQueue populating");
                    foreach(Vector2 vec in pathFinder.GetPointPath(fromidx, toindex)){
                        GD.Print(vec);
                        path.AddPoint(vec);
                        movementQueue.Enqueue(vec);


                    }

                    this.currentMovement = movementQueue.Dequeue();
                    this.player.GlobalPosition = this.currentMovement;
                    
                    break;
                }
            }  
        }
        //player.MoveAndSlide(this.velocity);
    }

    public void MoveToPoint(Vector2 dest, float delta)
    {       
                      
        if (dest != Vector2.Zero){
            player.SetAnimation("parameters/Walk/blend_position", dest);
            player.SetAnimation("parameters/Idle/blend_position", dest);
            player.animationState.Travel("Walk");
            //velocity += speedCheck * delta * acceleration;
            //velocity = velocity.Clamped(maxSpeed * delta);
            velocity = velocity.MoveToward(dest, acceleration * delta);
        }
        else {
            
            
            player.animationState.Travel("Idle");
            velocity = velocity.MoveToward(Vector2.Zero, friction * (delta/2));
            //velocity = Vector2.Zero;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
