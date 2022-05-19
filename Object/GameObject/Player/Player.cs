using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

public class Player : KinematicBody2D, GameObject
{
	private Resource arrow;

	public List<GameObject> encounters;
	
    public Vector2 Veloctiy = Vector2.Zero;

	public int Mana =4;
	public int Health = 100;
	public string action;
	
	public string direction;

	public HexCell1 currentTile;

	public HexHorizontalTest currentTestTile;
	public HexHorizontalTest TargetHex;

	private  bool mouseEntered =false;
	public Sprite sprite;

	public Color highlight = new Color(0.6f,0.1f,0,1f);
	public Color outline = new Color(0,0,0,1);

	public ShaderMaterial sm= null;
	
	private float animationDelay = 5;

	[Signal]
	public delegate void ExitMainArea();
	
	[Persist(IsPersist = false)]
	public AnimationPlayer animationPlayer;

	[Persist(IsPersist = false)]
	public AnimationTree animationTree;

	[Persist(IsPersist = false)]
	public AnimationNodeStateMachinePlayback animationState;

	[Signal]
	public delegate void PlayerSwordHit(Node objectHit);

	[Signal]
	public delegate void PowerTileInteraction(bool type);
	

	public override void _Ready(){
		//base._Ready();
		encounters = new List<GameObject>();
		arrow = ResourceLoader.Load("res://arrow.png");
		sprite = GetNode<Sprite>("Sprite");

		Material spMat = sprite.Material;
		

		if(typeof(ShaderMaterial) == spMat.GetType()){
			////GD.Print("is the same type");
			sm = (ShaderMaterial)spMat;
			//sm.SetShaderParam("Color", new Color(0,0,0,1));
		}
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		this.animationTree = GetNode<AnimationTree>("AnimationTree");

		
		this.animationState = (AnimationNodeStateMachinePlayback)this.animationTree.Get("parameters/playback");
		////GD.Print("loaded player: ", this.animationState,this.animationTree, this.animationState);
		
		
	}
	public void _on_SwordHitbox_body_entered(Node body)
	{
		
		GD.Print("Sword body entered hitbox for ", body.Name);
		EmitSignal(nameof(PlayerSwordHit), body);
	}

	public void _on_SwordHitbox_area_entered(Area2D area)
	{
		
	}
	public void _on_PlayerHitbox_body_entered(Node body)
	{
		//////GD.Print("Area entered: ", area.Name + "    with pos: "+area.GlobalPosition);
		if(body?.GetType() == typeof(HexHorizontalTest))
		{
			//currentTile = (HexCell1)area;
			currentTestTile = (HexHorizontalTest)body;
			GD.Print("Setting current hex to ;",currentTestTile.Name);
		}
		else if(body?.Name == "ExitMainArea")
		{
			EmitSignal("ExitMainArea");
		}
		else if(body.Name == "Enemy_SwordHitbox" )
		{
			GD.Print("skndjfkjsndkfjnskjdf");
		}
	}

	public void _on_PlayerHitbox_body_exited(Node body)
	{

	}


	public bool IsAvailable()
	{
		GD.Print(this.animationPlayer.CurrentAnimation);
		return string.IsNullOrEmpty(this.animationPlayer.CurrentAnimation);
	}

	public void _on_PlayerHitbox_area_entered(Area2D area)
	{
		if(area is HexHorizontalTest)
		{
			currentTestTile = (HexHorizontalTest)area;
			
			if(!string.IsNullOrEmpty(currentTestTile.powertilestring))
			{
				((HexHorizontalTest)area).Name = "superduper hex";
				GD.Print("Power Hex ",area.Name, " entered");
				EmitSignal(nameof(PowerTileInteraction), true);
			}
			// else 
			// {
			// 	EmitSignal(nameof(PowerTileInteraction), false);
			// }
		}
	}

	public void _on_PlayerHitbox_area_exited(Area2D area)
	{
		if(area is HexHorizontalTest)
		{
			//currentTestTile = (HexHorizontalTest)area;
			
			
			if(!string.IsNullOrEmpty(((HexHorizontalTest)area).powertilestring))
			{
				GD.Print("Power Hex ",((HexHorizontalTest)area).Name, " exited");
				EmitSignal(nameof(PowerTileInteraction), false);
			}
			// else 
			// {
			// 	EmitSignal(nameof(PowerTileInteraction), false);
			// }
		}
	}

	public void _on_Area2D_area_entered(Area2D area){
		//////GD.Print("Area entered: ", area.Name + "    with pos: "+area.GlobalPosition);
		if(area?.GetType() == typeof(HexHorizontalTest))
		{
			
			//currentTile = (HexCell1)area;
			currentTestTile = (HexHorizontalTest)area;
			//GD.Print("Setting current hex to ;",currentTestTile.Name);
		}
		else if(area?.Name == "ExitMainArea")
		{
			EmitSignal("ExitMainArea");
		}
	}

	public void _on_EncounterArea_area_entered(Area2D area)
	{
		
		if(area is IInteractable){
			GD.Print("Adding interactable: ", area.Name);
			encounters.Add((Interactable)area);
		}
	}

	public void _on_EncounterArea_area_exited(Area2D area){
		if(area is Interactable){
			encounters.Remove((Interactable)area);
		}
	}

	public void _on_EncounterArea_body_entered(Node body){
		//GD.Print("jskndkfjnsjkdf ", body.Name);
		if(typeof(GameObject).IsInstanceOfType(body) && body != this)
		{
			GD.Print("Adding body: ", body.Name);
			encounters.Add((GameObject)body);
		}
	}

	public void _on_EncounterArea_body_exited(Node body){
		if(typeof(GameObject).IsInstanceOfType(body) && body != this)
		{
			encounters.Remove((GameObject)body);
		}
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
