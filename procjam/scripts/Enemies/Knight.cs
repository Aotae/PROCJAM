using Godot;
using System;

public partial class Knight : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// jumps toward player
		BeekGuy player = GetNode<BeekGuy>("../BeekGuy");
		float speed = 80;
		float moveAmount = speed * (float)delta;
		Vector2 moveDirection = (player.Position - Position).Normalized();
		Position += moveDirection * moveAmount;
	}
}
