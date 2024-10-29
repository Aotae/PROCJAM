using Godot;
using System;
using System.Text.Json;

public partial class BeekGuy : CharacterBody2D
{
	[Export]
	public int Health {get; set;} = 100;
<<<<<<< Updated upstream
	[Export]
	public int MoveSpeed {get; set;} = 400;
	[Export]
=======
	public int MoveSpeed {get; set;} = 1000;
>>>>>>> Stashed changes
	public int AttackSpeed {get; set;} = 100;
	[Export]
	public int Attack {get; set;} = 100;
	[Export]
	public int Defense {get; set;} = 100;
	[Export]
	public Vector2 ScreenSize = new Vector2(2880,1620);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//open Json and get data
<<<<<<< Updated upstream
		using var file = FileAccess.Open("res://player.json", FileAccess.ModeFlags.Read);
		string jsonString = file.GetAsText();
		var details = Json.ParseString(jsonString);
		var data = Json.ParseString(jsonString);
=======
		// using var file = FileAccess.Open("res://stats.json", FileAccess.ModeFlags.Read);
		// string jsonString = file.GetAsText();
		// var details = Json.ParseString(jsonString);
		// var data = Json.ParseString(jsonString);
>>>>>>> Stashed changes
		//allocate data to local variables
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 Velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			Velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			Velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			Velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			Velocity.Y -= 1;
		}
		
		Position += Velocity * MoveSpeed * (float)delta;
		Position = new Vector2(x: Mathf.Clamp(Position.X, -ScreenSize.X, ScreenSize.X),y: Mathf.Clamp(Position.Y, -ScreenSize.Y, ScreenSize.Y));
		// Position = new Vector2(x: Mathf.Clamp(Position.X, -ScreenSize.X, ScreenSize.X),y: Mathf.Clamp(Position.Y, -ScreenSize.Y, ScreenSize.Y));
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("BeekGuySprite");

		if (Velocity.Length() > 0)
		{
			Velocity = Velocity.Normalized() * MoveSpeed;
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
