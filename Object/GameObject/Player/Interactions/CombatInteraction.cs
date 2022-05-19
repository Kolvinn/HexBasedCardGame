using Godot;
using System;
public class CombatInteraction :Interaction
{
    public GameObject target;
    private bool dealDamage = false;
    public CombatInteraction(GameObject target, bool dealDamage = false)
    {
        this.target = target;
        this.dealDamage = dealDamage;
    }
    public override bool ValidateInteraction(PlayerController player)
    {
        if(target is Enemy1 || target is Grass_Single1 || target is AnimatedGrave)
            this.HasBeenValidated = true;
        return HasBeenValidated;
    }


    public override bool FinalizeInteraction(PlayerController player)
    {
        if(HasBeenValidated){
            finalized = true;
            if(target is Enemy1)
                return HandleEnemyInteraction(player, (Enemy1)target);
            if(target is BasicResource)
                return HandleResourceInteraction(player, (BasicResource)target);
            if(target is AnimatedGrave)
                return HandleItemInteraction(player, (AnimatedGrave)target);
        }
        return finalized;
    }

    private bool HandleItemInteraction(PlayerController player, AnimatedGrave res)
    {
        res.TakeDamage(4);
        if(res.Health <= 0)
        {
            res.QueueFree();

        }
        return true;
    }
    private bool HandleResourceInteraction(PlayerController player, BasicResource res)
    {
        //GD.Print(res.Name);
        if(res.Name.Contains("Grass"))
        { 
            Random r = new Random();
            GD.Print("Player current power: ", player.currentPower);
            if(r.NextDouble() <=0.5 && player.currentPower != null)
            {
                 res.ResourceType = "Mystic Seed";
               
            }
            else {
                res.ResourceType = "Grass Fiber";
            }
            
        }
        player.TryAddToInventory(res,1);
        try{
                //attackObjectQueue.FetchQueue().Remove(obj);
                res.QueueFree();
            }
            catch(Exception e)
            {
                GD.Print("issue removing resource frorm queue and freeing");
                GD.Print(e);
            }
        return true;
    }

    private bool HandleEnemyInteraction(PlayerController player, Enemy1 enemy)
    {
        
        var damageLabel = Params.LoadScene<Node2D>("res://Static/Object/DamageLabel.tscn");
        
        if(dealDamage){
            GD.Print("playing hurt animation");
            //if(player.PowerEffect)
            var damage =12;
            damageLabel.Scale = new Vector2(0.3f,0.3f);
            damageLabel.GetNode<Label>("Label").Text = damage+"";   
            

            if(player.currentPower != null)
            {
                
                var healLabel = Params.LoadScene<Node2D>("res://Static/Object/DamageLabel.tscn");
                
                healLabel.GetNode<Label>("Label").AddColorOverride("font_color",new Color(0.2f,0.8f,0.2f));
                GD.Print("player power is not null, adding new label for adding health");
                player.TakeDamage(-5);
                healLabel.Scale = new Vector2(0.3f,0.3f);
                healLabel.GetNode<Label>("Label").Text = "+5";  
                player.player.AddChild(healLabel); 
                //healLabel.Position = new Vector2(healLabel.Position.x-5, healLabel.Position.y-30);
                
            }

            enemy.AddChild(damageLabel);        
            damageLabel.Position = new Vector2(damageLabel.Position.x-5, damageLabel.Position.y-30);


            enemy.animationPlayer.Play("Hurt");
            enemy.TakeDamage(damage);
            
            if(enemy.Health<= 0)
            {
                enemy.animationPlayer.Play("Death");
                BasicResource r = new BasicResource()
                {
                    ResourceType = "Dark Essence"
                };

                player.TryAddToInventory(r, new Random().Next(1,5));
            }
            else{
                enemy.animationPlayer.Play("Hurt");
            }
            
        }
        else
        {
            GD.Print("Player taking damage");
            var damage  = 5;
            damageLabel.GetNode<Label>("Label").Text = damage+"";
            //damageLabel.Scale = new Vector2(3,3);
            //damageLabel.GlobalPosition = player.GlobalPosition;
            damageLabel.Scale = new Vector2(0.3f,0.3f);
            player.player.AddChild(damageLabel);
            damageLabel.Position = new Vector2(damageLabel.Position.x-5, damageLabel.Position.y-30);
            player.TakeDamage(damage);
        }

        return true;
    }
}