using Godot;
using System;

using System.Collections.Generic;
using System.Linq;

public class Skeleton : KinematicBody2D, GameObject
{
    public bool hasEnergy = true;
    public int capacity = 0;
    public HexHorizontalTest currentHex;

    public Vector2 targetVec;
    public HexHorizontalTest targetHex;

    private Vector2 currentMovement = Vector2.Zero;


    private static float maxSpeed = 200, friction = 20000, acceleration = 20000;
    
    public Vector2 velocity = Vector2.Zero;
    
    public  HexGrid grid;

    public AnimationPlayer animationPlayer;

    public Queue<Vector2> movementQueue = new Queue<Vector2>();

    //private Vector2 currentMovement = Vector2.Zero;

    public bool DestinationReached = false;


    public override void _Ready()
    {
        animationPlayer = this.GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
        animationPlayer.Play("Idle");
    }

    public override void _PhysicsProcess(float delta)
    {
        ParseHexMovementCommand(delta);
    }
    public void ParseHexMovementCommand(float delta)
    {

        if(targetVec == Position)
            this.DestinationReached = true;
        else
        {
            this.DestinationReached = false;
        }

        //if we are moving, continue moving
        if(currentMovement !=  Vector2.Zero){
           // this.characterTile = (HexHorizontalTest)character.currentTestTile;

            
            if(Position==currentMovement){
                
                //character.animationState.Travel("Idle");              
                currentMovement =Vector2.Zero;
                //animationPlayer.Play("Idle");
            }
            else{
                
                animationPlayer.Play("Walk");
               // var direction = GetVectorAnimationSpace(currentMovement);
                
                //character.SetAnimation("parameters/Walk/blend_position", direction);
                //character.SetAnimation("parameters/Idle/blend_position", direction);

                //character.animationState.Travel("Walk");
                Position = Position.MoveToward(currentMovement,delta*maxSpeed);
                //////GD.Print("current character position: "+character.Position);
                //this.character.move(velocity);
                //MoveToPoint(currentMovement, delta);
            }
        }
        //if we still have movement left to do
        if(currentMovement == Vector2.Zero && this.movementQueue != null && this.movementQueue.Count !=0){   
                          
            currentMovement = this.movementQueue.Dequeue();
            //GD.Print("Dequing vector: ",currentMovement) ; 
        }
        else if(currentMovement == Vector2.Zero && !DestinationReached && targetHex != null)
        {
            //GD.Print("moving to targetHex");
            Position = Position.MoveToward(targetHex.Position,delta*maxSpeed);
        }
        else{
            animationPlayer.Play("Idle");
        }
    }

    public void TakeDamage(int damage)
    {
        this.animationPlayer.Play("TakeDamage");
    }
}
