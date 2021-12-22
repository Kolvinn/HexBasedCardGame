using Godot;
using System;

public class Player : KinematicBody2D, GameObject
{
	private Resource arrow;


	public string action;
	
	public string direction;

	public HexCell1 currentTile;

	private  bool mouseEntered =false;
	public Sprite sprite;

	public Color highlight = new Color(0.6f,0.1f,0,1f);
	public Color outline = new Color(0,0,0,1);

	public ShaderMaterial sm= null;
	
	private float animationDelay = 5;



	
	[Persist(IsPersist = false)]
	public AnimationPlayer animationPlayer;

	[Persist(IsPersist = false)]
	public AnimationTree animationTree;

	[Persist(IsPersist = false)]
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
		GD.Print("loaded player: ", this.animationState,this.animationTree, this.animationState);
		
		
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
