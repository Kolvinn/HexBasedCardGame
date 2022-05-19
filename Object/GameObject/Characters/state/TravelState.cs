using System.Collections.Generic;
using Godot;
public class TravelState : CharacterBaseState 
{  
    private Queue<Vector2> movementQueue;
    private Vector2 currentMovement = Vector2.Zero;
    public TravelState(Queue<Vector2> movementQueue)
    {
        this.movementQueue = movementQueue;
    }
    public override CharacterBaseState HandleState(CharacterController character, WorkerJob job)
    {
        if(character.HasReachedDestination())
        {
            character.character.animationState.Travel("Idle");
            character.DestinationReached = false;
            character.SetDestination(null);
            return new ExitState();
        }
        //if we are moving, continue moving
        if(currentMovement !=  Vector2.Zero){
           
            if(character.character.Position==currentMovement){              
                character.character.animationState.Travel("Idle");              
                currentMovement =Vector2.Zero;
            }
            else{
               
                character.MoveToPoint(currentMovement);
                //this.character.move(velocity);
                //MoveToPoint(currentMovement, delta);
            }
        }
        //if we still have movement left to do
        if(currentMovement == Vector2.Zero && this.movementQueue != null && this.movementQueue.Count > 0){   
            //GD.Print("moving to");
            currentMovement = this.movementQueue.Dequeue();
        }
        // else if(currentMovement == Vector2.Zero && !DestinationReached && character.TargetHex != null)
        // {
        //     character.Position = character.Position.MoveToward(character.TargetHex.Position,delta*maxSpeed);
        // }
        else if(currentMovement == Vector2.Zero && this.movementQueue?.Count == 0 && character.HasReachedDestination()) 
        {
            //we've reached destination and can now interact with things
            character.character.animationState.CallDeferred("Travel","Idle");
            return new ExitState();

            //this.workerState = State.WorkerState.Idle;
        }
        return this;
    }
}