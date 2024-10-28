using Godot;
using System;

public partial class MainGameScreen : Node2D
{
	[Export]
	public PackedScene RedBlobScene { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RedBlobScene = GD.Load<PackedScene>("res://scenes/RedBlob.tscn");
		Timer timer = GetNode<Timer>("MobTimer");
		timer.Timeout += OnMobTimerTimeout;
	}
	
	public void OnMobTimerTimeout()
	{
		RedBlob mob = RedBlobScene.Instantiate<RedBlob>();

		PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawn");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		mob.Position = mobSpawnLocation.Position;
	
		AddChild(mob);
	}
}
