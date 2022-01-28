using Godot;
using System.Collections.Generic;
using System;
public class FireController: ControllerInstance
{

    public YSort environmentLayer;

    public YSort fireAssets;

    public Dictionary<HexHorizontalTest, int> FireStacks = new Dictionary<HexHorizontalTest, int>();

    public Dictionary<FireTimer, HexHorizontalTest> FireTimers = new Dictionary<FireTimer,HexHorizontalTest>();
    
    public FireController()
    {
        //FireTimer timer = new FireTimer();
        //timer.Connect("FireTimeeTimeout", this, nameof(On_FireTimerTimeout));
        //timer.Start(10);
    } 

    public override void _Ready()
    {
        fireAssets = Params.LoadScene<YSort>("res://Assets/Particles/firespritesFX_PIPO/FireTile.tscn");
        //fireAssets.Position = fireTile.Position;
    }

    public void On_FireTimerTimeout(FireTimer t)
    {
        HexHorizontalTest hex =  FireTimers[t];
        GD.Print("On firetimeout with stacks: ",FireStacks[FireTimers[t]]);
        if(FireStacks[FireTimers[t]]  < 5)
        {
            FireStacks[FireTimers[t]] = FireStacks[FireTimers[t]] +1;
            foreach(Node n in FireTimers[t].EnvironmentAffect.GetChildren())
            {
                if(!((Node2D)n).Visible)
                {
                    ((Node2D)n).Visible = true;
                    break; 
                }
            }
            
            if(FireStacks[FireTimers[t]]  == 3)
            {
                Node2D node = hex.EnvironmentAffect;
                hex.EnvironmentAffect.GetParent().RemoveChild(node);
                hex.EnvironmentAffect = Params.LoadScene<YSort>("Assets/Particles/firespritesFX_PIPO/FireTile_stack3.tscn");
                
                node.QueueFree();
                this.environmentLayer.AddChild(hex.EnvironmentAffect);
                hex.EnvironmentAffect.Position = node.Position;
            }
        
            
            
            this.RemoveChild(t);
            FireTimers.Remove(t);
            FireTimers.Add(CreateFireTimer(2),hex);           

        }
        //at five stacks we spread that fire like them cheeks boi
        else
        {
            Node2D node = hex.EnvironmentAffect;
            hex.EnvironmentAffect.GetParent().RemoveChild(node);
            hex.EnvironmentAffect = Params.LoadScene<YSort>("Assets/Particles/firespritesFX_PIPO/FireTile_stack5.tscn");
            
            node.QueueFree();
            this.environmentLayer.AddChild(hex.EnvironmentAffect);
            hex.EnvironmentAffect.Position = node.Position;
            this.RemoveChild(t);
            FireTimers.Remove(t);
            SpreadFire(hex);
        }
    }

    public void StartNewFire(HexHorizontalTest onTile)
    {
        FireStacks.Add(onTile, 1);
        YSort asset = Params.LoadScene<YSort>("res://Assets/Particles/firespritesFX_PIPO/FireTile.tscn");
        asset.Position = onTile.Position;
        onTile.EnvironmentAffect = asset;
        environmentLayer.AddChild(asset);
        FireTimers.Add(CreateFireTimer(2),onTile);
        
    }

    public void SpreadFire(HexHorizontalTest hex)
    {
        foreach(HexHorizontalTest neighbour in hex.connections)
        {   
            GD.Print("checking neighbour: ", neighbour, " with hex: ", hex);
            //is grasssssssss
            if(!neighbour.isBasicResource && neighbour.HexEnv != null && neighbour!=hex && neighbour.EnvironmentAffect == null)
            {
                StartNewFire(neighbour);
            }
        }
    }
 

    public FireTimer CreateFireTimer(int time)
    {       
        FireTimer timer = new FireTimer();
        timer.Connect("FireTimeTimeout", this, nameof(On_FireTimerTimeout));
        this.AddChild(timer);
        timer.Start(time);
        
        return timer;
        
    }

    public class FireTimer : Timer
    {
        [Signal]
        public delegate void FireTimeTimeout(FireTimer t);
        public FireTimer(){
            this.Connect("timeout", this, nameof(On_Timeout));
        }

        public void On_Timeout()
        {
            EmitSignal("FireTimeTimeout", this);
        }
    }
}