using Godot;
using System;

public partial class BeekGuyArea2d : Area2D
{
	private Timer CollisionCooldownTimer;
	private bool CanCollide = true;
	[Export] private float CooldownTime = 1.0f; //* Cooldown time, change for different i-frame windows

	[Export] BeekGuy beekguy{get; set;}
	public override void _Ready()
	{
		// Instantiate collision cooldown timer to prevent insta-death to some extent
		CollisionCooldownTimer = new Timer();
		CollisionCooldownTimer.WaitTime = CooldownTime;
		CollisionCooldownTimer.Timeout += OnCollisionCooldownTimeout;
		AddChild(CollisionCooldownTimer);

		//TODO: ONE OF THESE HAS TO SEND WHEN SOMETHING TOUCHES YOU, RIGHT???
		AreaEntered += OnAreaShapeEntered;
		
		BodyEntered += OnAreaShapeEntered;
		AreaExited += OnAreaShapeEntered;
		BodyExited += OnAreaShapeEntered;
	}
	
	public void OnAreaShapeEntered(Node2D area)
	{
		if (!CanCollide)
		{
			// GD.Print("Collision on cooldown");
			return;
		}
		CanCollide = false;
		CollisionCooldownTimer.Start();
		

		//The below doesn't work but it should be close if you need to differentiate enemy types
		//if (area is  RedBlob) // Class of entity, i.e. Rook or Pawn, etc.
		//{
			//GD.Print("Collision with slime");
		//}
		GD.Print("Collide with Enemy");
	}

	public void OnCollisionCooldownTimeout()
	{
		GD.Print("Collision cooldown reset");
		CanCollide = true;
	}
}
