using Godot;
using System;
using System.Text.Json;

public partial class BeekGuy : CharacterBody2D
{
	[Export]
	// health, moveSpeed, attackSpeed, defense, attack
	public int Health {get; set;} = 100;
	public int MoveSpeed {get; set;} = 100;
	public int AttackSpeed {get; set;} = 100;
	public int Attack {get; set;} = 100;
	public int Defense {get; set;} = 100;
	public Vector2 ScreenSize = new Vector2(2880,1620);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//open Json and get data
		using var file = FileAccess.Open("res://stats.json", FileAccess.ModeFlags.Read);
		string jsonString = file.GetAsText();
		var details = Json.ParseString(jsonString);
		var data = Json.ParseString(jsonString);
		//allocate data to local variables
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.

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
		
		Position += velocity * MoveSpeed * (float)delta;
		Position = new Vector2(x: Mathf.Clamp(Position.X, -ScreenSize.X, ScreenSize.X),y: Mathf.Clamp(Position.Y, -ScreenSize.Y, ScreenSize.Y));
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("BeekGuySprite");

		if (Velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * MoveSpeed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}
		// sprite selection
		if (Velocity.X > 0 && Velocity.Y > 0)
		{
			animatedSprite2D.Animation = "FrontRight";
		}
		else if (Velocity.X > 0 && Velocity.Y < 0)
		{
			animatedSprite2D.Animation = "BackRight";
		}
		else if (Velocity.X < 0 && Velocity.Y > 0)
		{
			animatedSprite2D.Animation = "FrontLeft";
		}
		else if (Velocity.X < 0 && Velocity.Y < 0)
		{
			animatedSprite2D.Animation = "BackLeft";
		}
		else if (Velocity.X > 0)
		{
			animatedSprite2D.Animation = "Right";
		}
		else if (Velocity.X < 0)
		{
			animatedSprite2D.Animation = "Left";
		}
		else if (Velocity.Y > 0)
		{
			animatedSprite2D.Animation = "Front";
		}
		else if (Velocity.Y < 0)
		{
			animatedSprite2D.Animation = "Back";
		}
	}
}
