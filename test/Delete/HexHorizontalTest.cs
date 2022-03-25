using Godot;
using System;
using System.Collections.Generic;

public class HexHorizontalTest: Area2D, GameObject
{
    public Line2D topBorder, botBorder;
    
    public bool isBasicResource = false;

    public State.MouseEventState eventState;

    public Polygon2D topPoly, botPoly;

    public CollisionPolygon2D hitBox;

    public Node2D HexEnv;

    public Building building;
    
    public BasicResource GatherableResource;

    public YSort EnvironmentAffect;

    public HashSet<HexHorizontalTest> connections = new HashSet<HexHorizontalTest>();

    public bool ResourceExhausted = false;

    

    public StaticBody2D staticHitBox;
    public override void _Ready()
    {   
        //objects = new List<GameObject>();
        eventState = State.MouseEventState.Exited;
        topBorder = GetNode<Line2D>("TopBorder"); 
        botBorder = GetNode<Line2D>("BottomBorder"); 
        topPoly = GetNode<Polygon2D>("TopPoly"); 
        botPoly = topPoly.GetNode<Polygon2D>("BotPoly"); 
        hitBox = GetNode<CollisionPolygon2D>("HitBox");
        staticHitBox = this.GetNode<StaticBody2D>("StaticBody2D");

        GenOutLine();
    }

    private void GenOutLine()
    {
        //////GD.Print("gen outlines and hitbox for ",this);
        topBorder.Points = topPoly.Polygon;
        hitBox.Polygon = topPoly.Polygon;
        //staticHitBox.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Polygon = topPoly.Polygon;
        // staticHitBox.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Visible = false;
        //staticHitBox.Visible = false;// foreach(Vector2 vec in topBorder.Points){
        //     ////GD.Print(vec);
        // }
        //botBorder.Points = botPoly.Polygon;
        //topBorder.Update();
        this.Update();
    }

    public void _on_Hex_mouse_entered(){
        //////GD.Print("mouse entered for: ",this);
        topBorder.Width =6;
        //topBorder.ZIndex =5;
        topBorder.DefaultColor = new Color(0.6f,0.27f,0.09f,0.5f);
        this.eventState =State.MouseEventState.Entered;
        HexGrid.hoveredHex = this;

    }

    public BasicResource GetNextNonBusyResource(){
        if(HexEnv is YSort)
        {   
            foreach(Node n in HexEnv.GetChildren())
            {
                if(((BasicResource)n).TotalResource >0 && ((BasicResource)n).resourceState !=0)
                {
                    return ((BasicResource)n);
                }
            }
            return null;
         
        }
        else if(HexEnv is BasicResource)
        {   
            if(((BasicResource)HexEnv).TotalResource > 0 && ((BasicResource)HexEnv).resourceState !=0)
                return ((BasicResource)HexEnv);
        }
        return null;
    }
    
    public bool HasAvailableResource(){
        
        if(HexEnv is YSort)
        {   
            foreach(Node n in HexEnv.GetChildren())
            {
                if(((BasicResource)n).TotalResource >0)
                {
                    return true;
                }
            }
            return false;
         
        }
        else if(HexEnv is BasicResource)
        {
            
            //GD.Print("returning wood: ",((BasicResource)HexEnv).Capacity > 0);
            return ((BasicResource)HexEnv).TotalResource > 0;
        }
        //GD.Print("returning end ",HexEnv == null); 
        return HexEnv == null;
    }

    public  void _on_Hex_mouse_exited(){
        topBorder.Width =3;
        //topBorder.ZIndex =5;
        topBorder.DefaultColor = new Color(0.04f,0.04f,0.04f,0.5f);
        this.eventState =State.MouseEventState.Exited;
        //topBorder.Modulate = new Color(120,51,47);
    }

    

    public override void _PhysicsProcess(float delta){
        //this.Update();//topBorder.Update();
        if(isBasicResource && ResourceExhausted)
        {
            isBasicResource = false;
            ResourceExhausted =false;
            HexEnv.QueueFree();
        }

    }
}
