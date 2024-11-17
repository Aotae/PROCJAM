using Godot;
using System;

public partial class RedBlob : Node2D
{
	private bool movinX = false;
	private bool movinY = false;
	private Vector2 targetVect = new Vector2(0, 0);
	private double timer = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// follows player at set speedVector2 targetVect = Vector2.Zero;
		BeekGuy player = GetNode<BeekGuy>("../BeekGuy");
		
		
	}
}
