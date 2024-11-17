using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BigRoom : Node2D
{
    private Vector2I[] neighbors = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };
    private Godot.Collections.Dictionary<Vector2I, Array<Vector2I>> Patterns = new Godot.Collections.Dictionary<Vector2I, Array<Vector2I>>();
    [Export] private int PatternId { get; set; } = 0;
    [Export] public int MapSizeX { get; set; } = 10;
    [Export] public int MapSizeY { get; set; } = 10;
    [Export] private TileMapLayer ground { get; set; }
    [Export] private TileMapLayer walls { get; set; }
    [Export] public FastNoiseLite MapNoise = new FastNoiseLite();
    [Export] private TileSet oneBit { get; set; }
	[Export] private TileSetAtlasSource Atlas {get; set;}

    private float GetTileWeight(Vector2I atlasCoords)
    {
        // Convert atlas coordinates to a tile ID
		Atlas = (TileSetAtlasSource)oneBit.GetSource(0);
		TileData data = Atlas.GetTileData(atlasCoords,0);
		float weight = (float)data.GetCustomData("Weight");
        return weight;  // Default weight if no custom data found
    }
    private void PrintJaggedArray(Array<Vector2I>[][] jaggedArray)
    {
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            string rowOutput = "";
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                var cellArray = jaggedArray[i][j];
                rowOutput += "[";
                for (int k = 0; k < cellArray.Count; k++)
                {
                    rowOutput += cellArray[k].ToString();
                    if (k < cellArray.Count - 1) rowOutput += ", ";
                }
                rowOutput += "]";
                if (j < jaggedArray[i].Length - 1) rowOutput += ", ";
            }
            GD.Print("Row ", i, ": ", rowOutput);
        }
    }
    
    // Generate patterns and their neighbor constraints
	private void GeneratePatterns()
	{
		var WFCSample = oneBit.GetPattern(PatternId);
		var PatternCells = WFCSample.GetUsedCells();

		foreach (Vector2I cell in PatternCells)
		{
			Vector2I cellAtlasCoords = WFCSample.GetCellAtlasCoords(cell);

			// Initialize the array if the key doesn't exist, otherwise append to the existing array
			if (!Patterns.ContainsKey(cellAtlasCoords))
			{
				Patterns[cellAtlasCoords] = new Array<Vector2I>();
			}

			foreach (Vector2I neighborOffset in neighbors)
			{
				Vector2I neighborCell = cell + neighborOffset;
				if (neighborCell.X >= 0 && neighborCell.Y >= 0 && WFCSample.HasCell(neighborCell))
				{
					Vector2I neighborCoords = WFCSample.GetCellAtlasCoords(neighborCell);
					
					// Add the neighbor coordinates only if they are not already present to avoid duplicates
					if (!Patterns[cellAtlasCoords].Contains(neighborCoords))
					{
						Patterns[cellAtlasCoords].Add(neighborCoords);
					}
				}
			}
		}

		foreach (KeyValuePair<Vector2I, Array<Vector2I>> kvp in Patterns)
		{
			GD.Print("Key: ", kvp.Key, " Value: ", kvp.Value);
		}
	}

    // Initialize the map and perform the Wave Function Collapse algorithm
   private void GenerateMap(float[,] noise)
   {
		Vector2I[] patternKeys = Patterns.Keys.ToArray();
		Array<Vector2I>[][] sptiles = new Array<Vector2I>[MapSizeX][];

		for (int i = 0; i < MapSizeX; i++)
		{
			sptiles[i] = new Array<Vector2I>[MapSizeY];
			for (int j = 0; j < MapSizeY; j++)
			{
				// Initialize each cell with a copy of all pattern options
				sptiles[i][j] = new Array<Vector2I>(patternKeys);
			}
		}

		while (true)
		{
			Vector2I cell = GetLowestEntropy(sptiles);
			// GD.Print("Current cell to collapse: ", cell);
			
			if (cell == new Vector2I(-1, -1))
			{
				// All cells are collapsed
				GD.Print("Map generation completed successfully.");
				break;
			}

			Collapse(cell, sptiles);
			// GD.Print("Cell collapsed at: ", cell);
			PropagateConstraints(cell, sptiles);
		}
	}

	private Vector2I GetLowestEntropy(Array<Vector2I>[][] tiles)
	{
		int minEntropy = int.MaxValue; // Reset minEntropy each time this method is called
		Vector2I minCell = new Vector2I(-1, -1);

		for (int x = 0; x < MapSizeX; x++)
		{
			for (int y = 0; y < MapSizeY; y++)
			{
				if (tiles[x][y] == null || tiles[x][y].Count <= 1)
					continue; // Skip already-collapsed or empty cells

				int entropy = tiles[x][y].Count;
				if (entropy < minEntropy)
				{
					minEntropy = entropy;
					minCell = new Vector2I(x, y);
				}
			}
		}

		// GD.Print("Lowest entropy cell found: ", minCell, " with entropy ", minEntropy);
		return minCell;
	}

	private void Collapse(Vector2I position, Array<Vector2I>[][] tiles)
	{
		var cellTiles = tiles[position.X][position.Y];
		if (cellTiles == null || cellTiles.Count == 0)
		{
			GD.PrintErr("Attempted to collapse an empty cell at ", position);
			return;
		}

		// Calculate cumulative weight and choose based on weighted probability
		float totalWeight = cellTiles.Sum(tile => GetTileWeight(tile));
		float chosenWeight = (float)(GD.Randf() * totalWeight);
		
		Vector2I chosenTile = cellTiles[0];
		foreach (var tile in cellTiles)
		{
			chosenWeight -= GetTileWeight(tile);
			if (chosenWeight <= 0)
			{
				chosenTile = tile;
				break;
			}
		}

		// GD.Print("Chosen weighted tile at ", position, ": ", chosenTile);
		tiles[position.X][position.Y] = new Array<Vector2I> { chosenTile };

		// Place the chosen tile in the TileMapLayer
		ground.SetCell(position, 0, chosenTile);
	}


	private void PropagateConstraints(Vector2I cellPos, Array<Vector2I>[][] tiles)
	{
		var stack = new Stack<Vector2I>();
		stack.Push(cellPos);

		while (stack.Count > 0)
		{
			Vector2I current = stack.Pop();
			Array<Vector2I> currentPossible = tiles[current.X][current.Y];

			if (currentPossible == null || currentPossible.Count == 0)
			{
				GD.PrintErr("No possible tiles to propagate at ", current);
				continue;
			}

			foreach (Vector2I neighbor in neighbors)
			{
				Vector2I currentNeighbor = current + neighbor;
				if (currentNeighbor.X >= 0 && currentNeighbor.Y >= 0 &&
					currentNeighbor.X < MapSizeX && currentNeighbor.Y < MapSizeY)
				{
					Array<Vector2I> neighborTiles = tiles[currentNeighbor.X][currentNeighbor.Y];
					if (neighborTiles == null || neighborTiles.Count == 0)
					{
						GD.PrintErr("Neighbor at ", currentNeighbor, " has no possible tiles.");
						continue;
					}

					bool changed = false;
					foreach (Vector2I tile in neighborTiles.ToList())
					{
						bool isCompatible = currentPossible.Any(possibleTile =>
							Patterns.ContainsKey(possibleTile) && Patterns[possibleTile].Contains(tile));

						if (!isCompatible)
						{
							neighborTiles.Remove(tile);
							changed = true;
						}
					}

					if (changed && neighborTiles.Count > 0)  // Only push if tiles remain
					{
						stack.Push(currentNeighbor);
					}
				}
			}
		}
	}



    public override void _Ready()
    {
        float[,] map = new float[MapSizeX, MapSizeY];
        Random seedGen = new Random();
        int seed = seedGen.Next(1000, 9999);
        MapNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        MapNoise.Seed = seed;
        GD.Print("Seed:", MapNoise.Seed);
		// GetTileDataFromAtlas(new Vector2I(1,1));
        // Generate noise values
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                map[i, j] = MapNoise.GetNoise2D(i, j);
            }
        }

        GeneratePatterns();
        GenerateMap(map);
    }
}
