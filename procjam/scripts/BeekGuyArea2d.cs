using Godot;
using System;


public partial class BeekGuyArea2d : Area2D
{
	[Export]
	BeekGuy beekguy{get; set;}
	public override void _Ready()
	{
	}
	
	public void OnAreaShapeEntered(CollisionObject2D area)
	{
		//check if enemy then call player hit function
		GD.Print("Collide");
		beekguy.EnemyCollision();
	}
}
