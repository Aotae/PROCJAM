using Godot;
using System;

public partial class Queen : Node2D
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
		// moves quickly after player
		// this aint quite it but its pretty funny
		BeekGuy player = GetNode<BeekGuy>("../BeekGuy");
		float speed = 3;
		float moveAmount = speed * (float)delta;
		Position = Position.CubicInterpolate(player.Position, Position, player.Position, moveAmount);
	}
}
