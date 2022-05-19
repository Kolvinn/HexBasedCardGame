using System;
using Godot;
using System.Collections.Generic;
public class EnemyState 
{

    public virtual EnemyState HandleState(Enemy1 enemy, float delta = 0)
    { 
        if(enemy.IsTaunted())
        {
            GameUpdateQueue.TryPushUpdate(new TauntInteraction(enemy));

            return new TauntState();
        }
        return this;
        
    }

    public virtual EnemyState HandleState(Enemy1 enemy, Player player,  float delta = 0)
    {
        GD.Print("handling parent state for enemy");
        return this;
    }
}