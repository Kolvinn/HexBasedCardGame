using Godot;
using System;

public class HexCell1 : Area2D, GameObject
{
    Line2D topBorder;

    public State.MouseEventState eventState;
    public override void _Ready()
    {   
        eventState = State.MouseEventState.Exited;
        topBorder = GetNode<Line2D>("TopBorder"); 
    }

    public void _on_Area2D_mouse_entered(){
        topBorder.Width =6;
        topBorder.DefaultColor = new Color(0.6f,0.27f,0.09f,1);
        this.eventState =State.MouseEventState.Entered;

    }

    public void _on_Area2D_mouse_exited(){
        topBorder.Width =3;
        topBorder.DefaultColor = new Color(0.04f,0.04f,0.04f,1);
        this.eventState =State.MouseEventState.Exited;
        //topBorder.Modulate = new Color(120,51,47);
    }

    public override void _Process(float delta){
        this.Update();//topBorder.Update();

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
