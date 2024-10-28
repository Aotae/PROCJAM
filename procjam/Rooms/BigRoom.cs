using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public partial class BigRoom : Node2D
{
	int source_id = 0;
	Vector2I LandAtlas = new Vector2I(0,0);
	Vector2I WaterAtlas = new Vector2I(6,0);
	[Export]
	TileMapPattern[] MapPatterns { get; set;}
	[Export]
	public int MapSizeX{get; set;} = 10;
	[Export]
	public int MapSizeY{get; set;} = 10;
	// Called when the node enters the scene tree for the first time
	[Export]
	TileMapLayer ground{get; set;}
	[Export]
	TileMapLayer walls{get; set;}
	[Export]
	public FastNoiseLite MapNoise = new FastNoiseLite();
	
	void GenerateMap(float[,] noise)
	{
		for (int i = 0; i < MapSizeX;i++){
			for(int j =0; j < MapSizeY;j++){
				Vector2I coords = new Vector2I(i,j);
				if (noise[i,j] >= 0.0){
					// put down a normal walkable tile
					// GD.Print("We got here nerd");
					ground.SetCell(coords,0,LandAtlas,0);
				}else if (noise[i,j] < 0.0)  {
					ground.SetCell(coords,0,WaterAtlas,0);
				}
			}
		}
	}
	
	public override void _Ready()
	{
		float [,] map = new float[MapSizeX,MapSizeY];
		Random seedGen = new Random();
		int seed = seedGen.Next(1000,9999);
		MapNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
		MapNoise.Seed = seed;
		GD.Print("Seed:",MapNoise.Seed);
		for (int i = 0;i<MapSizeX;i++){
			for (int j = 0; j<MapSizeY; j++){
				map[i,j] = MapNoise.GetNoise2D(i,j);
			}
		}
		float maxval = (from float v in map select v).Max();
		float minval = (from float v in map select v).Min();
		GD.Print("Largest Value",maxval);
		GD.Print("Smallest Value",minval);
		GenerateMap(map);
	}

}

// Low Value -> Wall or Water i.e impassable terrain
// High Value -> Dirt Paths or roads i.e. fast moving terrain
// Medium Value -> Middle of the road i.e. grass patches or sand or whatever default ground