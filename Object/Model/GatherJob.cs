using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
public class GatherJob : WorkerJob
{
    public string GatherState = "Gather";

    [Signal]
    public delegate void ResourceUpdate(StorageBuilding building);

    public GatherJob()
    {

    }

    public override bool IsJobStillValid(CharacterController character)
    {
        //need to account for building restrictions and for character/tool restrictions
        if(TargetHex.HasAvailableResource())
        {
            return true;
        }
        //check inventory!
        else if(character.capacity>0)
        {
            return true;
        }
        //check for other resource squares
        else if(isPersistant && StorageBuilding !=null && AssignedBuilding !=null)
        {
            if(grid.resourcePositions.ContainsKey(TargetHex.Position)){
                grid.resourcePositions.Remove(TargetHex.Position);
                grid.ResourceMap.RemovePoint(grid.ResourceHexToInt[TargetHex]);
            }
            
            if(grid.resourcePositions.Count >0 )
            {
                // foreach(KeyValuePair<Godot.Vector2, Godot.Node2D> p in grid.resourcePositions)
                // {
                    
                // }
                int index = grid.ResourceMap.GetClosestPoint(AssignedBuilding.buildingAsset.Position);
                TargetHex = grid.ResourceHexToInt.First(item => item.Value ==index).Key;
                return true;
                
            }
        }
        
        return false;
    }

    public override void PerformJobAction(CharacterController character)
    {
        if(GatherState == "Gather")
        {
            //
            // && TargetHex.HasAvailableResource() && character.capacity <5)

            //if we have met conditions of finishing gather
            if(!TargetHex.HasAvailableResource() || character.capacity ==5)
            {
                int ind = grid.IndexOfVec(StorageBuilding.buildingAsset.Position);

                //TargetHex = grid.storedHexes[ind]; //change target hex to
                GatherState = "Drop";
                character.workerState = State.WorkerState.WaitingOnMovement;
            }
            else {
                character.workerState = State.WorkerState.Gather;
            }
        }
        else if(GatherState == "Drop")
        {
            if(character.capacity > 0)
            {
                character.workerState = State.WorkerState.Dropping;

                StorageBuilding.UpdateStorage(character.capacity);
                character.capacity =0;

            }

            GatherState = "Gather";
            character.workerState = State.WorkerState.WaitingOnMovement;
        }
    }

    public override Vector2 FetchTargetVector()
    {
        if(GatherState == "Gather")
        {
            return TargetHex.Position;
        }
        else
        {
            return StorageBuilding.buildingAsset.Position;
        }
    }
    
}