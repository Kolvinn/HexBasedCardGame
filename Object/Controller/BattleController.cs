public class BattleController : ControllerInstance
{
    public PlayerController player;
    public HexGrid grid;

    public CharacterController skeleton;

    public int Movement = 2;

    public int Range = 1;

    public string turn "player"
    public void LoadCharacters()
    {   
        if (turn == "player")
        {
            
        }
    }

    public void PlayerTurn()
    {
        
    }

    public void SkeletonTurn()
    {
        //this assumes a one range ability
        var vecList = grid.pathFinder.GetPointPath(grid.IndexOfVec(skeleton.character.currentTestTile.Position), grid.IndexOfVec(player.player.currentTestTile.Position));
       
        //if the tile distance between player and skeleton can be crossed in movement
        //then move to the nearest point (without start and end tiles)
        bool canReach = false;
        if(Movement >= (vecList.Length -2) )
        {

            for(int i =1 ;i <vecList.Length-1;i++)
            {
                canReach = true;
                skeleton.movementQueue.Enqueue(vecList[i]);
            }
        }
        else 
        {
            foreach(var vec in vecList)
                skeleton.movementQueue.Enqueue(vec);
        }

        if(canReach)
        {
            //attack
        }
    }
}