using Godot;
using System;

public class Camera2D : Godot.Camera2D
{
    private float zoomspeed = 0.1f, upperLimit = 0.1f, lowerLimit = 2f, currentzoom = 1;

    private bool rightClick;
    public override void _Ready()
    {
        this.SetProcessUnhandledInput(true);
    }

    public override void _Process(float delta)
    {
        //base._Process(delta);
        //float Input.GetActionStrength("right_click");
    }

    public override void _UnhandledInput(InputEvent @event){

        if (@event is InputEventMouseButton){
            InputEventMouseButton emb = (InputEventMouseButton)@event;
            if (emb.IsPressed()){
                if (emb.ButtonIndex == (int)ButtonList.WheelUp && currentzoom >= upperLimit){
                    currentzoom -= zoomspeed;
                    this.Zoom = new Vector2(currentzoom,currentzoom);
                    ////GD.Print(currentzoom);
                }
                if (emb.ButtonIndex == (int)ButtonList.WheelDown && currentzoom < lowerLimit){
                    currentzoom += zoomspeed;
                    this.Zoom = new Vector2(currentzoom,currentzoom);
                    ////GD.Print(currentzoom);
                }
            }
        }
     }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
