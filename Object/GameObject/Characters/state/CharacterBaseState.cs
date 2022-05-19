using System;
using Godot;
using System.Collections.Generic;
public class CharacterBaseState 
{

    public virtual CharacterBaseState HandleState(CharacterController character, WorkerJob job)
    {  

                //GD.Print("Job is valid");
            //     var action = job.GetNextJobAction(character);

            //     if(action == "GoToGather")
            //     if(action  == "travel")
            //     {
            //         //GD.Print("TRavel");
            //         //do something better
            //         var path = HexGrid.PopulateTravelPath(character.character.currentTestTile.Position,job.FetchTargetVector());
            //         character.SetDestination(job.FetchTargetHex());
            //         return new TravelState(path);

            //     }
            //     //gatherjob
            //     if(action == "gather")
            //     {
                    
            //        // GD.Print("gather");
            //         Interaction c =  new ResourceInteraction(((GatherJob)job).GetResource());
            //         return new CharacterInteractState(c);

            //     }
            //     if(action == "drop")
            //     {
            //         //GD.Print("drop");
            //         Interaction c =  new ResourceDropInteraction(((GatherJob)job).GetStorage());
            //         return new CharacterInteractState(c);
            //     }
            
            
            
            // //set the job for deletion
            // else
            // {
            //     //should probably queuefree the job and set an idle state of some kind

            // }                

        return this;
    }
    



}