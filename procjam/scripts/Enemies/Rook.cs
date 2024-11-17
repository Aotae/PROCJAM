using Godot;
using System;

public partial class Rook : Node2D
{
	private bool movinX = false;
	private bool movinY = false;
	private Vector2 targetVect = Vector2.Zero;
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
		// moves quickly up or down after player
		BeekGuy player = GetNode<BeekGuy>("../BeekGuy");
		
		float speed = 400;
		float moveAmount = speed * (float)delta;
		timer += delta;
		
		//-------------------------------------------
		float xDiff = Mathf.Abs(player.Position.X - Position.X);
		float yDiff = Mathf.Abs(player.Position.Y - Position.Y);
		
		if(movinX == true){
			if(xDiff <= yDiff && timer >= 2){
				movinX = false;
				movinY = true;
			}
			else if(xDiff < 25){
				movinX = false;
			}
		}
		else if(movinY == true){
			if(xDiff > yDiff && timer >= 2){
				movinY = false;
				movinX = true;
			}
			else if(yDiff < 25){
				movinY = false;
			}
		}
		if(movinX == false && movinY == false){
			if(xDiff > yDiff){
				movinX = true;
			}
			else{
				movinY = true;
			}
		}
		if(movinX == true){
			targetVect = new Vector2(player.Position.X, Position.Y);
		}
		else if(movinY == true){
			targetVect = new Vector2(Position.X, player.Position.Y);
		}
		if(timer >= 2)
			timer = 0;
		
		Position = Position.MoveToward(targetVect, moveAmount);
	}
}
