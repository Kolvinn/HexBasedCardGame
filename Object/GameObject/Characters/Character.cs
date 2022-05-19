using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

public class Character : KinematicInteractable, GameObject
{
    public Vector2 SpriteFrameSize = new Vector2(52,72);
    public Vector2 SpriteFrameOffset = new Vector2(0,10);

	public Timer gatherTimer = new Timer();
	private Resource arrow;

	public static List<GameObject> encounters;


	public string action;
	
	public string direction;

	public HexCell1 currentTile;

	public HexHorizontalTest currentTestTile;

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


	
    private HexHorizontalTest TargetHex;

	[Signal]
	public delegate void TargetHexReached();

	public int InventoryCapacity = 5;

	public float delta;

	public Character()
	{
		GD.Print("GUH");
	}

	public override void _Ready(){
		
		base._Ready();
        this.AddChild(gatherTimer);
		gatherTimer.OneShot = true;
		gatherTimer.Connect("timeout", this, nameof(On_GatherTimerTimeout));
		//base._Ready();
		encounters = new List<GameObject>();
		//arrow = ResourceLoader.Load("res://arrow.png");
		sprite = GetNode<Sprite>("Sprite");
        this.sprite.Texture = GetRandomAtlasTexture();
		//GetRandomAtlasTexture();

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
	public override void _PhysicsProcess(float delta)
	{
		this.delta= delta;
	}

	public bool IsAvailable()
	{
		//GD.Print(this.animationPlayer.CurrentAnimation, "   ",gatherTimer.TimeLeft);
		return string.IsNullOrEmpty(this.animationPlayer.CurrentAnimation) && gatherTimer.TimeLeft==0;
	}

	public void On_GatherTimerTimeout()
	{
		this.animationPlayer.CurrentAnimation = "";
	}

	public void SetTargetHex(HexHorizontalTest hex)
	{
		this.TargetHex = hex;
	}

    public AtlasTexture GetRandomAtlasTexture(){ 
		Texture tex = ResourceLoader.Load<Texture>("res://Assets/Sprites/Characters/motw.png");
        AtlasTexture at = new AtlasTexture();
		
		tex.ResourceLocalToScene = true;
		at.Atlas = tex;
		at.ResourceLocalToScene = true;
		
		
        Random r = new Random();
        float randW = (float)r.Next(0,4), randH = (float)r.Next(0,2);
		float sectionW = 3 * SpriteFrameSize.x, sectionH = 4 * SpriteFrameSize.y;
		randW = randW * sectionW;
		randH = randH * sectionH;
        Vector2 pos = new Vector2(randW, randH +2);
        Vector2 size = new Vector2(sectionW, sectionH);
        at.Region = new Rect2(pos, size);
		//GD.Print("Spawning random aatlas pos at :", tex.Region);
        at.Flags = 1;
		at.FilterClip  =true;
        return at;
    }

	public void _on_Area2D_area_entered(Area2D area){
		//////GD.Print("Area entered: ", area.Name + "    with pos: "+area.GlobalPosition);
		if(area?.GetType() == typeof(HexHorizontalTest))
		{
			if((HexHorizontalTest)area == TargetHex){
				EmitSignal(nameof(TargetHexReached));
			}
			else{
				currentTestTile = (HexHorizontalTest)area;
			}
			//////GD.Print("Setting current hex to ;",currentTestTile.Name);
		}
	}

	public void _on_EncounterArea_area_entered(Area2D area)
	{
		if(area is Interactable){
			////GD.Print("Adding interactable");
			encounters.Add((Interactable)area);
		}
	}

	public void _on_EncounterArea_area_exited(Area2D area){
		if(area is Interactable){
			encounters.Remove((Interactable)area);
		}
	}

	public void _on_EncounterArea_body_entered(Node body){
		if(typeof(GameObject).IsInstanceOfType(body) && body != this)
		{
			////GD.Print("Adding body: ", body.Name);
			encounters.Add((GameObject)body);
		}
	}

	public void _on_EncounterArea_body_exited(Node body){
		if(typeof(GameObject).IsInstanceOfType(body) && body != this)
		{
			encounters.Remove((GameObject)body);
		}
	}
	public virtual void SetAnimation(String param, Vector2 vector, ActionWrapper delegatedMethod = null){
		//this.animationPlayer.Play(animation);
		if(param.Contains("ther"))
		{
			gatherTimer.Start(0.2f);
		}
		else {
			this.animationTree.Set(param,vector);
		}
		if(delegatedMethod != null)
		{
			delegatedMethod.Call();
		}
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
