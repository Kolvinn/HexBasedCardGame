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

	
	public override void _Ready(){
		base._Ready();
		arrow = ResourceLoader.Load("res://arrow.png");
		
	}
	

}
