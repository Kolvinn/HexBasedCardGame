using Godot;
using System;
public class BattleController : ControllerInstance
{
    public PlayerController player;
    public HexGrid grid;

    public FireController fireController;
    public SpellController spellController;

    public NewUIController newUIController;

    public Skeleton skeleton;

    public Label TurnLabel;

    public int Movement = 2;

    public int Range = 1;

    bool canReach = false;

    public enum BattleState
    {
        TurnChange,
        EnemyMove,
        Default,
        PlayerInputListen

    }

    public BattleState battleState = BattleState.Default;
    
    public void On_FireSpellCompleted(Card card, HexHorizontalTest hex)
    {   
        GD.Print("On firespell completed with card ", card.model.Name,  " and hex ", hex.Name);
        GD.Print(skeleton.currentHex);
        if(HexGrid.IndexOfVec(skeleton.targetVec) == HexGrid.IndexOfVec(hex.Position))
            skeleton.TakeDamage(20);
        //if(hex == skeleton.currentHex)
            
        
    }
    public string turn= "player";

    public override void _PhysicsProcess(float delta)
    {
        if(battleState == BattleState.TurnChange){
            this.fireController.IncreaseFireStacks();
            if (turn == "player")
            {                
                TurnLabel.Text = "Your Turn";
                newUIController.LoadPlayerTurnUI();
                this.battleState = BattleState.PlayerInputListen;

            }
            else if (turn == "skeleton")
            {
                TurnLabel.Text = "Enemy's Turn";
                
                newUIController.LoadEnemyTurnUI();
                SkeletonMovement();
            }
        }
        else if(battleState == BattleState.EnemyMove)
        {
            if(skeleton.DestinationReached)
            {
                if(canReach)
                {
                    //attack
                }
                else{
                    this.turn = "player";
                    this.battleState = BattleState.TurnChange;
                }
            }

        }
        else if(battleState == BattleState.PlayerInputListen)
        {
        }
    }
    public void LoadCharacters()
    {   
        // if(battleState == BattleState.TurnChange){
        //     if (turn == "player")
        //     {
                
        //     }
        //     else if (turn == "skeleton")
        //     {
        //         SkeletonMovement();
        //     }
        // }
        // else if(battleState == BattleState.EnemyMove)
        // {
        //     if(skeleton.DestinationReached)
        //     {
        //         if(canReach)
        //         {

        //         }
        //     }
        // }
    }

    public void PlayerTurn()
    {
        
    }

    public void SkeletonAttack()
    {
        skeleton.animationPlayer.Play("Attack");
    }

    public void SkeletonMovement()
    {
    //     //this assumes a one range ability
    //    // var vecList = grid.pathFinder.GetPointPath(HexGrid.IndexOfVec(skeleton.currentHex.Position), HexGrid.IndexOfVec(player.player.currentTestTile.Position));
    //     GD.Print("Vector list length  = ", vecList.Length);
    //     //if the tile distance between player and skeleton can be crossed in movement
    //     //then move to the nearest point (without start and end tiles)
    //     canReach = false;

    //     if(vecList.Length == 2)
    //     {
    //         canReach = true;
    //     }
    //     else {
    //         if(Movement >= (vecList.Length -2)) 
    //             canReach = true;
            
    //         for(int i =1 ;i <vecList.Length-2;i++)
    //         {   
    //             skeleton.movementQueue.Enqueue(vecList[i]);

    //             if(Movement == i)
    //             {
    //                 skeleton.targetVec = vecList[i];
    //                 break;
    //             }
    //         }
    //     }
    //      battleState = BattleState.EnemyMove;

        // if(Movement >= (vecList.Length -2) )
        //     canReach = true;

        
        // for(int i =1 ;i <vecList.Length-2;i++)
        // {   
            
        //     GD.Print("enquing vector and can reach");   
        //     skeleton.movementQueue.Enqueue(vecList[i]);

        //     if(i == Movement)
        //     {
        //         break;
        //     }
        //     else if()

        // }
        // if(Movement >= (vecList.Length -2) )
        // {
            
        //     canReach = true;
        //     for(int i =1 ;i <vecList.Length-2;i++)
        //     {   
        //         GD.Print("enquing vector and can reach");   
        //         skeleton.movementQueue.Enqueue(vecList[i]);
        //     }
        // }
        // else 
        // {
        //     skeleton.targetVec = vecList[vecList.Length-2];
        //     for(int i =1 ;i <vecList.Length-2;i++)
        //     {   
        //         GD.Print("enquing vector and can reach");   
        //         skeleton.movementQueue.Enqueue(vecList[i]);
        //     }
        // }
       

    }
}