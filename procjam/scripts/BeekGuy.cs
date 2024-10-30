using Godot;
using System;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

public partial class BeekGuy : CharacterBody2D
{
	// Apparently garbage issues occur if not using static vars for IsActionPressed
	public static StringName MoveRight = new StringName("move_right");
	public static StringName MoveLeft = new StringName("move_left");
	public static StringName MoveDown = new StringName("move_down");
	public static StringName MoveUp = new StringName("move_up");
	
	[Export]
	public Json playerJSON{get; set;}
	[Export]
	public int Health {get; set;} = 100;
	[Export]
	public int MoveSpeed {get; set;} = 1000;
	[Export]
	public int AttackSpeed {get; set;} = 100;
	[Export]
	public int Attack {get; set;} = 100;
	[Export]
	public int Defense {get; set;} = 100;
	[Export]
	public Vector2 ScreenSize = new Vector2(2880,1620);
	
	public void AttackWithWeapon()
	{
		return;
	}
	
	public void EnemyCollision()
	{
		return;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string JsonData = Json.Stringify(playerJSON.GetData());
		var error = playerJSON.Parse(JsonData);
  		var datareceived = playerJSON.Data;
		Godot.Collections.Dictionary dict = (Godot.Collections.Dictionary)datareceived;
		Godot.Collections.Dictionary dictInDict = (Godot.Collections.Dictionary)dict["Player"];
		Health = (int)dictInDict["Health"];
		MoveSpeed = (int)dictInDict["Speed"];
		AttackSpeed = (int)dictInDict["AttackSpeed"];
		Attack = (int)dictInDict["Attack"];
		Defense = (int)dictInDict["Defense"];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 Velocity = Vector2.Zero; // The player's movement vector.
		
		if (Input.IsActionPressed(MoveRight))
		{
			Velocity.X += 1;
		}
		if (Input.IsActionPressed(MoveLeft))
		{
			Velocity.X -= 1;
		}
		if (Input.IsActionPressed(MoveDown))
		{
			Velocity.Y += 1;
		}
		if (Input.IsActionPressed(MoveUp))
		{
			Velocity.Y -= 1;
		}
		
		Position += Velocity * MoveSpeed * (float)delta;
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
