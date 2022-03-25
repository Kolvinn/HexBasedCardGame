using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;  
using System.Linq;


public class ThreadController 
{
    private Dictionary<HexHorizontalTest, int> HexToInt = new Dictionary<HexHorizontalTest, int>();
    public HexGrid grid;
    private AStar2D resourceMap;
    public List<WorkerJob> AvailableJobs = new List<WorkerJob>();
    public List<WorkerJob> AssignedJobs = new  List<WorkerJob>();
    public List<CharacterController> AssignedCharacters = new List<CharacterController>();
    
    public List<CharacterController> AvailableCharacters = new List<CharacterController>();

    public Dictionary<CharacterController,WorkerJob> CharWithJob = new Dictionary<CharacterController,WorkerJob>();

    public Dictionary<CharacterController,BasicResource> AssignedResources = new Dictionary<CharacterController,BasicResource>();

    public Dictionary<Building,CharacterController> BuildingToCharacter = new Dictionary<Building, CharacterController>();

    //public Dictionary<CharacterController, BuildController> AssignedBuildings = new Dictionary<CharacterController, BuildController>();
    public ThreadController()
    {
        System.Threading.Thread tr  = new  System.Threading.Thread(new ThreadStart(BeginThread));
    }

    public void BeginThread()
    {
        //CheckPlayerInputChange();
        CheckAvailableJobs();
        UpdateWorkerStates();
    }

    

    public void CheckAvailableJobs()
    {
        if(AvailableJobs.Count >0)
        {
            //if we have idle characters
            if(AvailableCharacters.Count > 0)
            {
                WorkerJob job = AvailableJobs[0];
                AvailableJobs.RemoveAt(0);
                CharacterController character = FindBestSuitedCharacter(job);

                //now we have found character to assign, assign all the things.
                character.workerJob = job;
                character.workerState = State.WorkerState.Default;
                AvailableCharacters.Remove(character);
                //AssignedCharacters.Add(character);
                CharWithJob.Add(character,job);
                character.TargetReached = false;
            }
            //we don't have idle characters but have job prio - build this later
            else {

            }
        }
    }

    public CharacterController FindBestSuitedCharacter(WorkerJob job)
    {
        
        var found = new CharacterController();
        var highestPrio = 0;
    
        foreach(CharacterController crt in AvailableCharacters)
        {
            int prio = crt.jobPriority.GetPriority(job.jobType);
            if (prio > highestPrio)
            {
                found = crt;
                highestPrio = prio;
            }
        }
        return found;
    }

    public void UpdateWorkerStates()
    {
        foreach(KeyValuePair<CharacterController,WorkerJob> charJobPair in CharWithJob)
        {
            var character = charJobPair.Key;
            var job = charJobPair.Value;
            switch(character.workerState)
            {
                case State.WorkerState.Idle:
                    if(job.IsJobStillValid(character))
                    {
                        job.PerformJobAction(character);
                    }
                    else
                    {
                        //delete job
                    }
                    break;
                case State.WorkerState.WaitingOnMovement:
                    PopulateTravelPath(character, job);
                    character.workerState = State.WorkerState.Travel;
                    break;
            }
        }
    }

    //checks to see if target resource isn't busy, has resources, and we have enough space in character inven.
    public bool CanGather(CharacterController character)
    {
        
        BasicResource res = character.workerJob.TargetHex.GetNextNonBusyResource();

        if(res !=null){
            if(!AssignedResources.Keys.Any(ch => ch == character))
            {
                AssignedResources.Add(character,res);
            }
            return true;
        }
        else 
            return false;
    }


    public void CheckFinishedRes(CharacterController character, BasicResource res)
    {
        if(res.TotalResource <=0 )
        {
            character.workerJob.TargetHex.ResourceExhausted = true;
            AssignedResources.Remove(character);
        }
    }

    public void PopulateTravelPath(CharacterController character, WorkerJob job)
    {
        Vector2 targetVec = job.FetchTargetVector();
        int toIndex = grid.IndexOfVec(targetVec);
        int fromIndex = grid.IndexOfVec(character.character.currentTestTile.Position);
        //HexHorizontalTest hex=  grid.storedHexes[toIndex];
        if(toIndex >-1 && fromIndex >-1)
        {
            Queue<Vector2> qT = new Queue<Vector2>();
            foreach( Vector2 v in grid.pathFinder.GetPointPath(fromIndex, toIndex))
            {             
                qT.Enqueue(v);                
            }
            character.movementQueue = qT;
        }
    }

   

}