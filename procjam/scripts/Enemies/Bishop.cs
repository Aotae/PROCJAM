using Godot;
using System;

public partial class Bishop : Node2D
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
		// moves quickly diagonally after player
		BeekGuy player = GetNode<BeekGuy>("../BeekGuy");
		
		float speed = 250;
		float moveAmount = speed * (float)delta;
		timer += delta;
		if(timer > 4){
			movinX = false;
			movinY = false;
			timer = 0;
		}
		
		float xDiff = Mathf.Abs(player.Position.X - Position.X);
		float yDiff = Mathf.Abs(player.Position.Y - Position.Y);
		
		if(xDiff - yDiff > -25 && xDiff - yDiff < 25){
			movinX = false;
			movinY = false;
		}
		if(movinX == true && xDiff < 25){
			movinX = false;
		}
		else if(movinY == true && yDiff < 25){
			movinY = false;
		}
		if(movinX == false && movinY == false){
			if(xDiff > yDiff){
				movinX = true;
			}
			else{
				movinY = true;
			}
			if(player.Position.X > Position.X){
				// player is west
				if(player.Position.Y > Position.Y){
					// player is south
					targetVect = new Vector2(1 ,1);
				}
				else if(player.Position.Y <= Position.Y){
					// player is north
					targetVect = new Vector2(1 ,-1);
				}
			}
			else if(player.Position.X <= Position.X){
				// player is east
				if(player.Position.Y > Position.Y){
					// player is south
					targetVect = new Vector2(-1 ,1);
				}
				else if(player.Position.Y <= Position.Y){
					// player is north
					targetVect = new Vector2(-1 ,-1);
				}
			}
		}
		
		Position += targetVect * moveAmount;
	}
}
