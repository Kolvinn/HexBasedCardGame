using Godot;
public class BattleState : BaseState
{
    public BattleState()
    {

    }


    public override BaseState HandleState(PlayerController player, float delta, BaseState parentState = null)
    {
        
        bool leftClick = Input.IsActionJustPressed("left_click");
        bool rClick = Input.IsActionJustPressed("rClick");
        bool inventory = Input.IsActionJustPressed("inventory");
        
        bool rollClick = Input.IsKeyPressed(((int)KeyList.Space));
        var interactable = player.GetInteractableObject();
        bool eClick = Input.IsActionJustPressed("E");
        bool build = Input.IsActionJustPressed("Build");
        
        if(eClick)
        {
            if(player.CanPowerup())
            {
                player.PowerUp(new WaterPower());
            }
            //return new PoweredState();
            
        }
        BaseState state = null;

        if(player.attackObjectQueue.FetchQueue().Count>0)
        {
            var obj =   player.attackObjectQueue.FetchQueue()[0];
            // player.TryAddToInventory(obj,1);
            var s = new CombatInteraction(obj, true);
            player.attackObjectQueue.FetchQueue().Remove(obj);
            GameUpdateQueue.TryPushUpdate(s);
            
            
        }

        if(build)
        {
            return new BuildState(player.buildingUI, player.playerIcon);
        }

        if(player.combatHits?.Count > 0)
        {
            var s = new CombatInteraction(player.combatHits.Dequeue(), true);
            GameUpdateQueue.TryPushUpdate(s);
        }
        else 
        {            

            if(player.player.animationState.GetCurrentNode() != "Attack" && player.player.animationState.GetCurrentNode() != "RollWest")
            {

                player.GetMovementVector(delta);
                
            }

            if(rollClick)
            {
                var mouseVec = player.GetViewport().GetMousePosition();
                GD.Print("mouse pos : ", mouseVec);
                var playerVec = new Vector2(1920/2,1080/2);
                GD.Print("player pos : ", new Vector2(1920/2,1080/2));
                mouseVec = mouseVec - playerVec;
                player.Position.MoveToward(mouseVec,delta*100);
                
                //player.player.SetAnimation("parameters/Attack/blend_position", mouseVec);
                //player.player.animationState.Travel("Attack");
                player.player.animationPlayer.Play("RollWest");
            }

            //GD.Print("Handling battle state : " , player.player.animationState.GetCurrentNode());
            if(leftClick)
            {
                var mouseVec = player.GetViewport().GetMousePosition();
                var playerVec = new Vector2(1920/2,1080/2);
                mouseVec = mouseVec - playerVec;

                player.player.SetAnimation("parameters/Attack/blend_position", mouseVec);
                player.player.animationState.Travel("Attack");

            }
            if(rClick)
            {
                
                player.playerIcon.animationPlayer.Play("SwapEquip");
                return new BaseState();
            }
            if(inventory)
            {
                player.inventory.UI.Visible = true;
                return new InventoryState(this);
            }

        }
        return this;
        
            //state.HandleState(player, delta);
           // return this;
        
    
    }
}