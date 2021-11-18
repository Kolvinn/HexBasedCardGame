using Godot;
using System;

public class CameraMov : Camera
{   
    private static float maxSpeed = 300, friction = 2500, acceleration = 2000;

    private Vector3 velocity;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        //base._PhysicsProcess(delta);
        GetMovementVector(delta);
        this.Translate(this.velocity);
    }

    public void GetMovementVector(float delta)
    {
        var speedCheck = Vector3.Zero;
        
        speedCheck.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        speedCheck.z = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        speedCheck = speedCheck.Normalized();
           
        if (speedCheck != Vector3.Zero){
            // player.SetAnimation("parameters/Walk/blend_position", speedCheck);
            // player.SetAnimation("parameters/Idle/blend_position", speedCheck);
            // player.animationState.Travel("Walk");
            //velocity += speedCheck * delta * acceleration;
            //velocity = velocity.Clamped(maxSpeed * delta);
            velocity = velocity.MoveToward(speedCheck * maxSpeed, acceleration * delta);
        }
        else {
            
            
            // player.animationState.Travel("Idle");
            velocity = velocity.MoveToward(Vector3.Zero, friction * (delta/2));
            //velocity = Vector2.Zero;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
