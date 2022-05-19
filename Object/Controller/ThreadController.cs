using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;  
using System.Linq;
using System;
using System.Timers;


public class ThreadController 
{
    //private Dictionary<HexHorizontalTest, int> HexToInt = new Dictionary<HexHorizontalTest, int>();
    public HexGrid grid;
    //private AStar2D resourceMap;
    //public List<WorkerJob> AvailableJobs = new List<WorkerJob>();
    //public List<WorkerJob> AssignedJobs = new  List<WorkerJob>();
    //public List<CharacterController> AssignedCharacters = new List<CharacterController>();
    public PlayerController player;

    public static System.Threading.Mutex MuTexLock = new System.Threading.Mutex();
    
    public List<CharacterController> AvailableCharacters = new List<CharacterController>();

    public Dictionary<CharacterController,WorkerJob> CharWithJob = new Dictionary<CharacterController,WorkerJob>();

    //public Dictionary<CharacterController,BasicResource> AssignedResources = new Dictionary<CharacterController,BasicResource>();

   // public Dictionary<Building,CharacterController> BuildingToCharacter = new Dictionary<Building, CharacterController>();

    public Dictionary<CharacterController, CharacterBaseState> CharacterStates = new Dictionary<CharacterController, CharacterBaseState>();
    public Dictionary<CharacterController, Building> AssignedBuildings = new Dictionary<CharacterController, Building>();
    System.Threading.Thread tr;
    public ThreadController()
    {
        tr  =  new  System.Threading.Thread(new ThreadStart(BeginThread));
        tr.Start();
    }

    private static bool loopFinish =false;

    public List<string> ToRemove = new List<string>();

    private ObjectLockQueue lockQueue;

    private void BeginThread()
    {
        //Godot.Semaphore s = new Godot.Semaphore();
        lockQueue = new ObjectLockQueue();
        //CheckPlayerInputChange();
        //CheckAvailableJobs();
        var t = new System.Timers.Timer(1000/60);
        t.AutoReset =false;
        t.Start();
        t.Elapsed += OnTimedEvent;


        while (true)
        {
            
            // UpdateWorkerStates();

            // while(!loopFinish)
            // {

            // }
            
            // loopFinish = false;
            // t.AutoReset =false;
            // t.Interval = 1000/60;
            // t.Start();
            // t.Elapsed += OnTimedEvent;
        }
    }

    private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
        loopFinish = true;
    }

    

    // public void CheckAvailableJobs()
    // {
    //     if(AvailableJobs.Count >0)
    //     {
    //         //if we have idle characters
    //         if(AvailableCharacters.Count > 0)
    //         {
    //             WorkerJob job = AvailableJobs[0];
    //             AvailableJobs.RemoveAt(0);
    //             CharacterController character = FindBestSuitedCharacter(job);

    //             //now we have found character to assign, assign all the things.
    //             character.workerJob = job;
    //             character.workerState = State.WorkerState.Default;
    //             AvailableCharacters.Remove(character);
    //             //AssignedCharacters.Add(character);
    //             CharWithJob.Add(character,job);
    //             character.TargetReached = false;
    //         }
    //         //we don't have idle characters but have job prio - build this later
    //         else {

    //         }
    //     }
    // }

    public void ChangeWorkerAmount(Building b, int amount)
    {
        
        if(AvailableCharacters?.Count >0)
        {
            if(amount > 0){
                GD.Print("Assigning all the character things in the thread controller");
                var c = AvailableCharacters[0];
                AssignedBuildings.Add(c,b);
                CharWithJob.Add(c,null);
                b.AssignedWorkers.Add(c.Name);
                AvailableCharacters.Remove(c);
                CharacterStates.Add(c, null);
            }
           
        }
        else if(amount < 0 )
        {
            GD.Print("REMOVING WORKER");
            ToRemove.Add(b.AssignedWorkers[0]);
        }
    }

    // public CharacterController FindBestSuitedCharacter(WorkerJob job)
    // {
        
    //     var found = new CharacterController(null);
    //     var highestPrio = 0;
    
    //     foreach(CharacterController crt in AvailableCharacters)
    //     {
    //         int prio = crt.jobPriority.GetPriority(job.jobType);
    //         if (prio > highestPrio)
    //         {
    //             found = crt;
    //             highestPrio = prio;
    //         }
    //     }
    //     return found;
    // }

    public void UpdateWorkerStates()
    {

        foreach(KeyValuePair<CharacterController,WorkerJob> charJobPair in CharWithJob.ToList())
        {
            var character = charJobPair.Key;
            var job = charJobPair.Value;
            

            if(ToRemove?.Count > 0 && ToRemove.Contains(character?.Name))
            {
                CharWithJob[character] = null;
                AssignedBuildings[character] = null;
                continue;

                // if(CharacterStates[character] is CharacterBaseState)
                // {

                // }

            }

            if(job == null)
            {
                job = CreateNewJob(character);
                CharWithJob[character] = job;
            }

            if(job != null)
            {
                
                CharacterStates[character] = job.HandleCharacterState(character,CharacterStates[character]);
                //GD.Print("Creating new job 1");
                // if(CharacterStates[character] != null)
                // {
                //     //GD.Print("HANDLING THE STATE");
                //     // if(CharacterStates[character] is CharacterInteractState)
                //     // {
                //     //     GD.Print("FOUND THE INTERACTION STATE");
                //     // }
                //     if(!character.IsLocked() && character.CanInteract())
                //     {
                //         CharacterStates[character] = CharacterStates[character].HandleState(character, job);
                //     }
                //     //GD.Print("FINISHED HANDLING THE STATE");
                // }
                // else
                // {
                    
                //     CharacterStates[character] = new CharacterBaseState();
                // }
            }
            else
            {
                //update idle states or something like that
            }

            // switch(character.workerState)
            // {
            //     case State.WorkerState.Idle:
            //         if(job.IsJobStillValid(character))
            //         {
            //             job.PerformJobAction(character);
            //         }
            //         else
            //         {
            //             //delete job
            //         }
            //         break;
            //     case State.WorkerState.WaitingOnMovement:
            //         PopulateTravelPath(character, job);
            //         character.workerState = State.WorkerState.Travel;
            //         break;

            //     case State.WorkerState.Dropping:
            //         job.UpdateStorage(character.capacity);
            //         break;
            //     case State.WorkerState.Gather:
            //         if(character.CanInteract())
            //         {
            //             GameUpdateQueue.TryPushUpdate();
            //         }
            //         break;
            // }
        }
    }

    public WorkerJob CreateNewJob(CharacterController character)
    {
        WorkerJob job = null;
        var assBuild = AssignedBuildings[character];
        if(assBuild != null)
        {   
            // if(assBuild.model.Name == "Gathering Post")
            // {
            //     var tnh = HexGrid.storedHexes[HexGrid.IndexOfVec(((Building)assBuild).Position)];
            //     job = new GatherJob(assBuild,(StorageBuilding)assBuild,true, tnh);
            // }
        }

        return job;
    }

    //checks to see if target resource isn't busy, has resources, and we have enough space in character inven.
    // public bool CanGather(CharacterController character)
    // {
        
    //     BasicResource res = character.workerJob.TargetHex.GetNextNonBusyResource();

    //     if(res !=null){
    //         if(!AssignedResources.Keys.Any(ch => ch == character))
    //         {
    //             AssignedResources.Add(character,res);
    //         }
    //         return true;
    //     }
    //     else 
    //         return false;
    // }


    // public void CheckFinishedRes(CharacterController character, BasicResource res)
    // {
    //     if(res.TotalResource <=0 )
    //     {
    //         character.workerJob.TargetHex.ResourceExhausted = true;
    //         AssignedResources.Remove(character);
    //     }
    // }


   

}