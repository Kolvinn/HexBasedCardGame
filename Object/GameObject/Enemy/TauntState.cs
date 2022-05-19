using System;
using Godot;
public class TauntState : EnemyState
{
    public override EnemyState HandleState(Enemy1 enemy, Player player, float delta)
    {

        
        if(enemy.IsTaunted())
        {
            //free to attack
            if(enemy.CanInteract())
            {
                var playerPos = player.Position;

                var absX = Math.Abs(playerPos.x) - Math.Abs(enemy.Position.x);                
                var absY = Math.Abs(playerPos.y) - Math.Abs(enemy.Position.y);

                
                var vecTranslate = new Vector2(0,0);
                var vecY =0f;
                var vecX = 0f;

                //weight towards x scale
                // if(Math.Abs(absX) >= Math.Abs(absY) )
                // {
                    //player is further to the left (smaller x)
                    if(absX < 0)
                        vecX = -1;
                    else
                        vecX = 1;
                // }
                //weight towards y scale
                // else
                // {                    
                    if(absY < 0)
                        vecY = -1;
                    else
                        vecY = 1;
                // }
                //weight towards x scale
                if(Math.Abs(absX) >= Math.Abs(absY) )
                {
                    vecTranslate.x = vecX;
                }
                else
                {
                    vecTranslate.y = vecY;
                }

                
                enemy.SetAnimation("parameters/Walk/blend_position", vecTranslate);
                enemy.SetAnimation("parameters/Idle/blend_position", vecTranslate);   
                enemy.SetAnimation("parameters/Attack/blend_position", vecTranslate); 
                //GD.Print("vector translate animation: ", vecTranslate);

                if(enemy.CanAttack())
                {
                    enemy.PerformAttack(true);
                    //enemy.animationState.Travel("Attack");

                }
                else
                {
                    enemy.animationState.Travel("Walk");
                    // GD.Print(new Vector2(vecX,vecY).Normalized());
                    enemy.MoveAndCollide(new Vector2(vecX,vecY).Normalized() *  delta * 500);
                }
                
            }
            else 
            {
                //enemy.animationState.Travel("Idle");
                //GD.Print(enemy.animationState.GetCurrentNode());
                //enemy.animationState.Travel("Idle");
                if(enemy.IsAttacking())
                {
                   //enemy.animationState.Travel("Idle");
                    //GD.Print("is attacking, checking to see if animation is free");
                    if(enemy.animationState.GetCurrentNode() == "Idle")
                    {
                        //GD.Print(enemy.animationState.GetCurrentPlayPosition());
                        
                        //GD.Print(enemy.animationState.GetCurrentNode());
                        
                        //GD.Print(enemy.animationState.GetCurrentLength());
                        //GD.Print("performing attack");
                        
                       // enemy.animationState.Travel("Attack");
                        enemy.PerformAttack(false);
                    }
                }
                
                // else if (!enemy.IsOnCoolDown())
                // {
                //     GD.Print("is on cooldown");
                //     enemy.StartCoolDown();
                // }
            }
           
            return this;
        }
        else
        {
            
            
            
            enemy.animationState.Travel("Idle");
            return new EnemyState();
        }
    }
}