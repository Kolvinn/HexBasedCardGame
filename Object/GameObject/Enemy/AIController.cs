using System.Collections.Generic;
using Godot;
using System;
using System.Linq;
public class AIController: Node2D
{
    private Queue<Enemy1> enemies;
    public Player player;

    private bool areaJustCleared  = false;
    private Dictionary<Enemy1, EnemyState> states;

    public List<GameObject> MasterList = new List<GameObject>();

    //private List<StaticInteractable> structures = new List<StaticInteractable>();
    public AIController()
    {
        enemies = new Queue<Enemy1>();        
        states = new Dictionary<Enemy1, EnemyState>();
    }

    public void AddEnemy(Enemy1 enemy)
    {
        this.enemies.Enqueue(enemy);
        enemy.Connect(nameof(Enemy1.SwordHit), this, "On_SwordHit");
        this.states.Add(enemy, new EnemyState());
        this.MasterList.Add(enemy);
    }

    public void On_SwordHit(Enemy1 enemy, Node body)
    {
        if(body is Player)
        {
            GD.Print("pushing player hurt onto queue");
            GameUpdateQueue.TryPushUpdate(new CombatInteraction(enemy, false));
        }
    }

    public void HandleInteraction(GameObject target, YSort env)
    {
        if(MasterList.Contains(target)){
            if(target is Enemy1){
                Enemy1 enemy = (Enemy1)target;
                if(!enemy.Alive)
                {
                    var sp = enemy.GetNode<Sprite>("DeathSprite");
                    sp.Visible = true;
                    sp.Scale = enemy.Scale/2;
                    sp.Position = enemy.Position;
                    enemy.RemoveChild(sp);
                    env.AddChild(sp);
                    Kill(enemy);
                        
                }
            }
            else if(target is AnimatedGrave)
            {
                var grave = (AnimatedGrave)target;
                if(grave.Health <=0)
                {
                    MasterList.Remove(grave);
                }
            }

            if(MasterList.Count == 0)
                areaJustCleared = true;
        }
        
    }

    /// <summary>
    /// Only true on the next call the area was cleared of enemies
    /// </summary>
    /// <returns></returns>
    public bool AreaJustCleared()
    {
        if(areaJustCleared)
        {
            areaJustCleared = false;
            return true;
        }
        return false;
    }
    
    public void Kill(Enemy1 enemy)
    {
        enemy.QueueFree();
        this.MasterList.Remove(enemy);
        this.states.Remove(enemy);
    }
    public override void _PhysicsProcess(float delta)
    {
        if(enemies.Count>0){

            Enemy1 enemy = enemies.Dequeue();
            if(!MasterList.Contains(enemy))
                return;

            
            
            try
            {
                if(WeakRef(enemy) != null && IsInstanceValid(enemy)  &&  enemy!= null)
                {
                    //if we are dead, do some clean up
                    

                    if(states[enemy] is TauntState)
                        states[enemy] = states[enemy].HandleState(enemy, player, delta);
                    else if (states[enemy] is EnemyState)
                        states[enemy] = states[enemy].HandleState(enemy, delta);

                    enemies.Enqueue(enemy);
                }
            }
            catch(Exception e)
            {
                GD.Print(e);
            }
            
        }
    }

    public void PushTauntInteraction(Enemy1 enemy1)
    {

    }

    public void SpawnEnemiesForLevel(Biome biome)
    {
        
        var terrainLayer = biome.terrainLayer;
        var hexlayer = biome.hexLayer;
        terrainLayer.Name = "TerrainLayer";
        Enemy1 enemy =Params.LoadScene<Enemy1>("res://Object/GameObject/Enemy/Enemy1.tscn");             
        Enemy1 enemy2 =Params.LoadScene<Enemy1>("res://Object/GameObject/Enemy/Enemy1.tscn");   
        Enemy1 enemy3 =Params.LoadScene<Enemy1>("res://Object/GameObject/Enemy/Enemy1.tscn"); 
        foreach(var entry in hexlayer.GetChildren())
        {
            if(entry is HexHorizontalTest)
            {
                var hex = (HexHorizontalTest)entry;
                if(hex.Visible && hex.HexEnv == null)
                {
                    var grave = Params.LoadScene<AnimatedGrave>("res://Assets/Sprites/Misc/AnimatedGrave.tscn");
                    terrainLayer.AddChild(grave);
                    grave.Position = hex.Position;
                    this.MasterList.Add(grave);
                    enemy.Scale = player.Scale;
                    enemy2.Scale =new Vector2(5,5);
                    enemy3.Scale =new Vector2(4,4);
                    AddEnemy(enemy);
                    AddEnemy(enemy2);
                    AddEnemy(enemy3);
                    var enemies = new Enemy1[3]
                    {
                        enemy,enemy2,enemy3
                    };
                    int i = 0;
                    foreach(var con in hex.connections)
                    {
                        if(con.hex!=null && con.hex.Visible)
                        {
                            if(con.hex.HexEnv != null && terrainLayer.GetChildren().Contains(con.hex.HexEnv))
                            {
                                //enemy.Position = con.hex.HexEnv.Position;
                                terrainLayer.RemoveChild(con.hex.HexEnv);
                            }
                            enemies[i].Position = con.hex.Position;                            
                            i++;

                            if(i==enemies.Count())
                                break;
                        }
                    }
                    //enemy.Position = HexGrid.lastY;
                    break;
                }
            }
            
        } 
    }
}