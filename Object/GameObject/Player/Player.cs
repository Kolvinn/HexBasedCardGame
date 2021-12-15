using Godot;
using System;

public class Player : KinematicBody2D
{
	private Resource arrow;


	public string action {
		get; set;
	}
	
	public string direction {
		get; set;
	}

	public HexCell1 currentTile{
		get;set;
	}

	private  bool mouseEntered =false;
	Sprite sprite;

	Color highlight = new Color(0.6f,0.1f,0,1f);
	Color outline = new Color(0,0,0,1);

	ShaderMaterial sm= null;
	
	private float animationDelay = 5;
	
	private AnimationPlayer animationPlayer;

	public AnimationTree animationTree
	{
		get; set;
	}



	public AnimationNodeStateMachinePlayback animationState;

	public override void _Ready(){
		base._Ready();
		arrow = ResourceLoader.Load("res://arrow.png");
		sprite = GetNode<Sprite>("Sprite");

		Material spMat = sprite.Material;
		

		if(typeof(ShaderMaterial) == spMat.GetType()){
			GD.Print("is the same type");
			sm = (ShaderMaterial)spMat;
			//sm.SetShaderParam("Color", new Color(0,0,0,1));
		}
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		this.animationTree = GetNode<AnimationTree>("AnimationTree");

		
		this.animationState = (AnimationNodeStateMachinePlayback)this.animationTree.Get("parameters/playback");
		
		
	}

	public void _on_Area2D_area_entered(Area2D area){
		GD.Print("Area entered: ", area.Name + "    with pos: "+area.GlobalPosition);
		if(area?.GetType() == typeof(HexCell1))
			currentTile = (HexCell1)area;
	}
	public virtual void SetAnimation(String param, Vector2 vector){
		//this.animationPlayer.Play(animation);
		this.animationTree.Set(param,vector);
	}
	public void _on_Player_mouse_entered(){
		this.mouseEntered = true;
		sm?.SetShaderParam("line_color", highlight);
		sm?.SetShaderParam("line_thickness", 2);
	}

	public void _on_Player_mouse_exited(){
		this.mouseEntered = false;
		sm?.SetShaderParam("line_color", outline);
		sm?.SetShaderParam("line_thickness", 1);
	}

}
