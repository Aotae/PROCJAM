using Godot;
using System;

public partial class DungeonGen : Node
{
	[Export]
	public int Room_X {get; set;} = 100;
	[Export]
	public int Room_Y {get; set;} = 100;
	
	public override void _Ready()
	{
		var player = GD.Load<PackedScene>("res://BeekGuy.tscn");
		var scene = GD.Load<PackedScene>("res://Rooms/BigRoom.tscn");
		var playerinstance = player.Instantiate();
		var room = scene.Instantiate();
		AddChild(room);
		AddChild(playerinstance);
	}
}
