using Godot;
using System;

public partial class BeekGuy : CharacterBody2D
{
	[Export]
	public int Speed {get; set;} = 400;
	public Vector2 ScreenSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}
		
		Position += velocity * Speed * (float)delta;
		Position = new Vector2(x: Mathf.Clamp(Position.X, -2880, 2880),y: Mathf.Clamp(Position.Y, -1620, 1620));
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("BeekGuySprite");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}
		// sprite selection
		if (velocity.X > 0 && velocity.Y > 0)
		{
			animatedSprite2D.Animation = "FrontRight";
		}
		else if (velocity.X > 0 && velocity.Y < 0)
		{
			animatedSprite2D.Animation = "BackRight";
		}
		else if (velocity.X < 0 && velocity.Y > 0)
		{
			animatedSprite2D.Animation = "FrontLeft";
		}
		else if (velocity.X < 0 && velocity.Y < 0)
		{
			animatedSprite2D.Animation = "BackLeft";
		}
		else if (velocity.X > 0)
		{
			animatedSprite2D.Animation = "Right";
		}
		else if (velocity.X < 0)
		{
			animatedSprite2D.Animation = "Left";
		}
		else if (velocity.Y > 0)
		{
			animatedSprite2D.Animation = "Front";
		}
		else if (velocity.Y < 0)
		{
			animatedSprite2D.Animation = "Back";
		}
	}
}
