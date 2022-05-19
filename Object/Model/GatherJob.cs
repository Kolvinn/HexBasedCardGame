using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
public class GatherJob : WorkerJob
{
    //private string GatherState = "Begin";

    [Signal]
    public delegate void ResourceUpdate(StorageBuilding building);

    public HexHorizontalTest ResourceHex = null;

    private HexHorizontalTest StorageHex = null;

    private BasicResource targetResource = null;

    enum GatherState 
    {
        Gather,
        GoToGather,
        GoToDrop,
        Drop
    }

    private GatherState gatherState = GatherState.GoToGather;
    public GatherJob(Building assigned, StorageBuilding storage, bool persistant, HexHorizontalTest StorageHex)
    {
        this.AssignedBuilding = assigned;
        this.StorageBuilding = storage;
        this.isPersistant = persistant;
        this.StorageHex = StorageHex;


    }

    // private HexHorizontalTest FindNextBestHex(HexGrid grid)
    // {
        
    //     int index = grid.ResourceMap.GetClosestPoint(AssignedBuilding.Position);
        
    //     return grid.ResourceHexToInt.First(item => item.Value ==index).Key;
    // }


    public override bool IsJobStillValid(CharacterController character, HexGrid grid)
    {
        //job is always valid if there is a hex with a resource that is unlocked
        // if(ResourceHex != null && ResourceHex.HasAvailableResource())
        // {
        //     return true;
        // }
        return HexGrid.resourcePositions.ToList().Count > 1;




        //GD.Print("Checking if job is valid");
        // if(ResourceHex == null)
        // {
        //     ResourceHex = FindNextBestHex(grid);
        // }
        // //need to account for building restrictions and for character/tool restrictions
        // if(ResourceHex.HasAvailableResource())
        // {
        //     return true;
        // }
        // //check inventory!
        // // else if(character.capacity>0)
        // // {
        // //     return true;
        // // }
        // //check for other resource squares
        // else if(isPersistant && StorageBuilding !=null && AssignedBuilding !=null)
        // {
        //     // if(grid.resourcePositions.ContainsKey(ResourceHex.Position)){
        //     //     grid.resourcePositions.Remove(ResourceHex.Position);
        //     //     grid.ResourceMap.RemovePoint(grid.ResourceHexToInt[ResourceHex]);
        //     // }
            
        //     // if(grid.resourcePositions.Count >0 )
        //     // {
        //     //     // foreach(KeyValuePair<Godot.Vector2, Godot.Node2D> p in grid.resourcePositions)
        //     //     // {
                    
        //     //     // }
        //     //     int index = grid.ResourceMap.GetClosestPoint(AssignedBuilding.Position);
        //     //     ResourceHex = grid.ResourceHexToInt.First(item => item.Value ==index).Key;
        //     //     return true;
                
        //     // }
        // }
        
        // GD.Print("returning job is invalid");
        // return false;
    }
    
    public void ReassignResourceTargets()
    {
        //GD.Print("reassining things");
        if(ResourceHex == null || ObjectLockQueue.Contains(ResourceHex))
        {
            ResourceHex = HexGrid.GetClosestResourceHex(AssignedBuilding.Position);
            
        }
        YSort t = new YSort();
        t.QueueFree();
        if(targetResource == null)
        {               
            targetResource = GetNextNonBusyResource(ResourceHex.GetHexEnvinronment());
            
            if(targetResource == null)
            {
                ObjectLockQueue.TryAddObject(ResourceHex);
                HexGrid.ResourceMap.SetPointDisabled(HexGrid.ResourceHexToInt[ResourceHex]);               
                HexGrid.resourcePositions.Remove(ResourceHex.Position);
                if(Node2D.IsInstanceValid(ResourceHex.HexEnv))
                    ResourceHex.HexEnv.CallDeferred("QueueFree");
                //ObjectLockQueue.TryAddObject(ResourceHex);
                ResourceHex = null;
                ReassignResourceTargets();
            }
            ObjectLockQueue.TryAddObject(targetResource);
            GD.Print("          new target Resource is: ",targetResource);
        }
    }



    private BasicResource GetNextNonBusyResource(Node2D HexEnv){
        //if()
        //GD.Print("hex env name: ", HexEnv.Name);
        if(HexEnv is YSort)
        {   
            foreach(Node n in HexEnv.GetChildren())
            {
                GD.Print("node is: ", n.GetType(), "   ", n.Name, "    totalres: ",((BasicResource)n).TotalResource, "  ",!ObjectLockQueue.Contains(((BasicResource)n)));
                if(((BasicResource)n).TotalResource > 0 && !ObjectLockQueue.Contains(((BasicResource)n))  && BasicResource.IsInstanceValid(((BasicResource)n)))
                {
                    return ((BasicResource)n);
                }
            }
            return null;
         
        }
        else if(HexEnv is BasicResource)
        {   
            var res = ((BasicResource)HexEnv);
            if(BasicResource.IsInstanceValid(res) && res.TotalResource > 0 && !ObjectLockQueue.Contains(res))
                return res;
        }
        return null;
    }



    public override CharacterBaseState HandleCharacterState(CharacterController character, CharacterBaseState state)
    {
        //if we have no state then we need to get a new action
        if(state == null || state is ExitState)
        {
            return GetNextJobAction(character);
        }
        else
        {
            //GD.Print("handling state");
            return state.HandleState(character, this);
        }
    }

    public override CharacterBaseState GetNextJobAction(CharacterController character)
    {
        
        if(gatherState == GatherState.GoToGather)
        {
            if(ResourceHex == null || ObjectLockQueue.Contains(ResourceHex) ||  targetResource == null)
            {   
                //GD.Print("the things in the gather job that Ihave to see: ", ResourceHex, "    ",targetResource); 
                ReassignResourceTargets();
            
                //GD.Print("the changed resources: ", ResourceHex, "    ",targetResource);
            }
            ObjectLockQueue.TryAddObject(targetResource);
            
            var path = HexGrid.PopulateTravelPath(character.GetCurrentTile().Position, ResourceHex.Position);
            character.SetDestination(ResourceHex);
            gatherState = GatherState.Gather;
            GD.Print("Going to Gather with ", path.Count, "  ",character.GetCurrentTile().Position, "  ", ResourceHex.Position);
            return new TravelState(path);
        }
        else if(gatherState == GatherState.Gather)
        {
            gatherState = GatherState.GoToDrop;
            return new CharacterGatherState(targetResource);
        }
        else if(gatherState == GatherState.GoToDrop)
        {   
            ObjectLockQueue.TryRemoveObject(targetResource);
            this.targetResource = null; // <- we should be able to release this from lock as well


            GD.Print("Going to drop", StorageBuilding.Position);
            var path2 = HexGrid.PopulateTravelPath(character.GetCurrentTile().Position, StorageBuilding.Position);
            
            var hex = HexGrid.FetchHexAtIndex(HexGrid.IndexOfVec(StorageBuilding.Position));
            character.SetDestination(hex);
            gatherState = GatherState.Drop;
            return new TravelState(path2);
        }
        else 
        {       
            GD.Print("drop state");
            gatherState = GatherState.GoToGather;
            return new CharacterDropState(StorageBuilding);
            
        }
        // if(gatherState == "Begin")
        // {
        //     GatherState = "Gather";
        //     return "travel";
        // }
        // else if(GatherState == "Gather")
        // {
        //     // if(!character.HasReachedDestination())
        //     // {
        //     //     return "travel";
        //     // }
        //     // && TargetHex.HasAvailableResource() && character.capacity <5)

        //     //if we have met conditions of finishing gather
        //     if(!ResourceHex.HasAvailableResource() || character.capacity >=5)
        //     {
        //         //int ind = HexGrid.IndexOfVec(StorageBuilding.buildingAsset.Position);

        //         //TargetHex = grid.storedHexes[ind]; //change target hex to
        //         GatherState = "Drop";
        //         return "travel";
        //     }
        //     else 
        //     {   
        //         //character.DoAction("Gather");
        //         return "gather";
        //     }
        // }
        // else if(GatherState == "Drop")
        // {
        //     if(character.capacity > 0)
        //     {
                
        //         return "drop";

        //     }
            
        //     GatherState = "Gather";
        //     return "travel";
        // }
        // GD.Print("returning an empty action to perform");
        // return "";
    }
    
    public StorageBuilding GetStorage()
    {
        return this.StorageBuilding;
    }

    /// <summary>
    /// Returns the Vector2 of the destination depending on the job state.
    /// If Gather state, then the target is the gatherable resource, otherwise the target is the drop building
    /// </summary>
    /// <returns></returns>
    public override Vector2 FetchTargetVector()
    {
        if(gatherState == GatherState.Gather)
        {
            return ResourceHex.Position;
        }
        else
        {
            return StorageBuilding.buildingAsset.Position;
        }
    }

    public override HexHorizontalTest FetchTargetHex()
    {
        return gatherState  == GatherState.Drop ? StorageHex : ResourceHex; 
    }
    public BasicResource GetResource()
    {
        return ResourceHex.GetNextNonBusyResource();
    }
    
}