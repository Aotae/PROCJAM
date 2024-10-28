using Godot;
using System;

public partial class BeekGuy : CharacterBody2D
{
	[Export]
	public int Speed {get; set;} = 100;
	public Vector2 ScreenSize;

	public void GetInput()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		// GD.Print(inputDirection.IsNormalized());
        Velocity = inputDirection * Speed;
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// var Velocity = Vector2.Zero; // The player's movement vector.
		
		// Position += Velocity * Speed * (float)delta;
		// Position = new Vector2(x: Mathf.Clamp(Position.X, -2880, 2880),y: Mathf.Clamp(Position.Y, -1620, 1620));

		GetInput();
		MoveAndSlide();
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("BeekGuySprite");

		if (Velocity.Length() > 0)
		{
			Velocity = Velocity.Normalized() * Speed;
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
