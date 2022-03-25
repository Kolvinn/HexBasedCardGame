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
public class JobPriority
{
    // 0 = Not assigned
    // 1 = Low prio
    // 2 = med prio
    // 3 = high prio
    // 4 = drop everything
    public Dictionary<State.WorkerJobType, int> jobPrio;
    public JobPriority(){
        jobPrio = new Dictionary<State.WorkerJobType, int>()
        {
            {State.WorkerJobType.Gather,1},
            {State.WorkerJobType.Build,1}
        };
    } 

    public void SetPriority(State.WorkerJobType jobType, int prio)
    {
        this.jobPrio[jobType] = prio;
    }

    public int GetPriority(State.WorkerJobType jobType)
    {
        return this.jobPrio[jobType];
    }
}