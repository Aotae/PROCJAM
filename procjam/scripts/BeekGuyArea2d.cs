using Godot;
using System;


public partial class BeekGuyArea2d : Area2D
{
	[Export]
	BeekGuy beekguy{get; set;}
	public override void _Ready()
	{
		BodyEntered += OnAreaShapeEntered;
	}
	
	public void OnAreaShapeEntered(Node2D area)
	{
		//The below doesn't work but it should be close if you need to differentiate enemy types
		//if (area is  RedBlob) // Class of entity, i.e. Rook or Pawn, etc.
		//{
			//GD.Print("Collision with slime");
		//}
		GD.Print("Collide with Enemy");
	}
}
