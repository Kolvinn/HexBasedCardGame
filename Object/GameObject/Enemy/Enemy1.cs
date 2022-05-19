using Godot;
using System;
using System.Collections.Generic;

public class Enemy1 : KinematicBody2D, GameObject
{


    public AnimationPlayer animationPlayer;


	public AnimationTree animationTree;
    

    private bool playerEncountered = false;
	private bool canAttack = false;
	public AnimationNodeStateMachinePlayback animationState;

	private bool canInteract =true;
	private bool attacking =false;

	public bool Alive
	{
		get 
		{
			return this.Health > 0;
		}
	}

	Timer attackTimer = new Timer();

	public int Health = 30;
	
	[Signal]
	public delegate void SwordHit(Enemy1 enemy,Node objectHit);



	public override void _Ready(){
		foreach(Node n in this.GetChildren())
			GD.Print(n.Name);
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		//this.animationPlayer.Connect("animation_finished", this, nameof(On_AnimationFinished));
		this.animationTree = GetNode<AnimationTree>("AnimationTree");
		GD.Print("animationtree: ", animationTree, "   animationplayer: ", animationPlayer);
		this.animationState = (AnimationNodeStateMachinePlayback)this.animationTree.Get("parameters/playback");
		attackTimer.Connect("timeout", this, "on_attackTimerTimeout");
		this.AddChild(attackTimer);
    }


	public void _on_Enemy_SwordHitbox_body_entered(Node body)
	{
		EmitSignal(nameof(SwordHit), this,body);
	}

	public void TakeDamage(int damage)
	{
		this.Health -=damage;
	//	if
	}

	

	// public void _on_PlayerHitbox_body_entered(Node body)
	// {
	// 	//////GD.Print("Area entered: ", area.Name + "    with pos: "+area.GlobalPosition);
	// 	if(body?.GetType() == typeof(HexHorizontalTest))
	// 	{
	// 		//currentTile = (HexCell1)area;
	// 		currentTestTile = (HexHorizontalTest)body;
	// 		GD.Print("Setting current hex to ;",currentTestTile.Name);
	// 	}
	// 	else if(body?.Name == "ExitMainArea")
	// 	{
	// 		EmitSignal("ExitMainArea");
	// 	}
	// }

	public void PerformAttack(bool continuation)
	{
		if(continuation){
			attacking = true;
		}
		else
		{
			attacking = false;
			StartCoolDown();
		}
		
		canInteract = false;
		animationState.Travel("Attack");
		//this.animationState.Travel
	}

	public void StartCoolDown(int cooldown = 2)
	{
		//GD.Print("Starting attack cooldown");
		this.attackTimer.Start(cooldown);
	}

	public bool IsOnCoolDown()
	{
		return this.attackTimer.TimeLeft == 0;
	}

	public bool IsAttacking()
	{
		return this.attacking;
	}

	public void on_attackTimerTimeout()
	{
		canInteract = true;
	}

	public void _on_AnimationPlayer_animation_finished(string anim_name)
	{
		GD.Print("finished animation: ",anim_name);
		if(anim_name == "Attack")
		{
			if(attacking){
				this.animationState.Travel("Attack");
				attacking = false;
			}
			else
			{
				attackTimer.Start(2);
				//canInteract = true;
			}
		}
	}

	public bool CanInteract()
	{
		return string.IsNullOrEmpty(this.animationPlayer.CurrentAnimation) && canInteract;
	}

	public void _on_AttackRange_body_entered(Node body)
	{
		//GD.Print("Attack range body entered: ", body.Name);
		if(body is Player)
        {
            this.canAttack = true;
        }
	}
	public void _on_AttackRange_body_exited(Node body)
	{
		
		//GD.Print("Attack range body exited: ", body.Name);
		if(body is Player)
        {
            this.canAttack = false;
        }
	}
    public void _on_TauntRange_body_exited(Node body)
    {
		
        if(body is Player)
        {
            this.playerEncountered = false;
        }
    }

    public bool IsTaunted()
    {
        return this.playerEncountered;
    }
	public bool CanAttack()
	{
		return this.canAttack;
	}

	public void _on_TauntRange_body_entered(Node body)
    {
		//GD.Print("Node entered taunt range: ", body.Name);

        if(body is Player)
        {
            this.playerEncountered = true;
        }
    }
	public virtual void SetAnimation(String param, Vector2 vector){
		//this.animationPlayer.Play(animation);
		this.animationTree.Set(param,vector);
	}



}
