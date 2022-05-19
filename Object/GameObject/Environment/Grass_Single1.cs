using Godot;
using System;

public class Grass_Single1  : BasicResource
{
    public int Hits = 2;
    public AnimationPlayer player;
    
    public override void _Ready()
    {
        player = this.GetNode<AnimationPlayer>("GrassArea/AnimationPlayer");
        this.ResourceType = "Grass Fiber";
    }

    public void _on_GrassArea_area_entered(Area2D area)
    {
        //GD.Print(area);
        if(area.Name == "PlayerHitbox")
            this.player.Play("Shake");
        if(area.Name == "SwordHitbox" && Hits >0)
        {
            this.player.Play("Shake");
            this.Hits --;
            if(Hits==0)
            {
                GD.Print("adding grass to attack queue");
                AttackObjectQueue.AddObject(this);
            }
        }
    }
}
