using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class BigRoom : Node2D
{
    private Vector2I[] neighbors = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };
	private Dictionary<Vector2I, Array<Vector2I>> Patterns = new Dictionary<Vector2I, Array<Vector2I>>();
    Vector2I LandAtlas = new Vector2I(0, 0);
    Vector2I WaterAtlas = new Vector2I(6, 0);

    [Export] private int PatternId { get; set; } = 0;
    [Export] public int MapSizeX { get; set; } = 10;
    [Export] public int MapSizeY { get; set; } = 10;
    [Export] TileMapLayer ground { get; set; }
    [Export] TileMapLayer walls { get; set; }
    [Export] public FastNoiseLite MapNoise = new FastNoiseLite();
    [Export] TileSet oneBit { get; set; }

    // Generate patterns and their neighbor constraints
    void GeneratePatterns()
    {
        TileMapPattern WFCSample = oneBit.GetPattern(PatternId);
        Array<Vector2I> PatternCells = WFCSample.GetUsedCells();
        

        foreach (Vector2I cell in PatternCells)
        {
            Vector2I current = WFCSample.GetCellAtlasCoords(cell);
            Patterns[current] = new Array<Vector2I>();
            foreach (Vector2I neighbour in neighbors)
            {
                Vector2I currentNeighbor = cell + neighbour;
                if (currentNeighbor.X >= 0 && currentNeighbor.Y >= 0 && WFCSample.HasCell(currentNeighbor))
                {
                    Patterns[current].Add(WFCSample.GetCellAtlasCoords(currentNeighbor));
                }
            }
        }
        GD.Print(Patterns);
    }

    // Initialize the map and perform the Wave Function Collapse algorithm
	void GenerateMap(float[,] noise)
	{
		Vector2I[] patternKeys = Patterns.Keys.ToArray();
		Vector2I[][][] sptiles = new Vector2I[MapSizeX][][];

		for (int i = 0; i < MapSizeX; i++)
		{
			sptiles[i] = new Vector2I[MapSizeY][];
			for (int j = 0; j < MapSizeY; j++)
			{
				sptiles[i][j] = patternKeys.Length > 0 ? patternKeys.ToArray() : new Vector2I[0]; // Initialize with pattern keys or empty
			}
		}

		while (true)
		{
			Vector2I? cell = GetLowestEntropy(sptiles);
			if (cell == null) break; // All cells are collapsed

			Collapse(cell.Value, sptiles);
			PropagateConstraints(cell.Value, sptiles);
		}
	}


    // Propagate constraints to reduce possibilities for neighboring tiles
    private void PropagateConstraints(Vector2I cellpos, Vector2I[][][] tiles)
	{
		var ToVisit = new System.Collections.Generic.Queue<Vector2I>();
		ToVisit.Enqueue(cellpos);

		while (ToVisit.Count > 0)
		{
			Vector2I pos = ToVisit.Dequeue();

			// Ensure the current cell is already collapsed (has only one tile)
			if (tiles[pos.X][pos.Y].Length != 1)
				continue;

			Vector2I collapsedTile = tiles[pos.X][pos.Y][0]; // Get the single tile ID for the collapsed cell

			if (!Patterns.ContainsKey(collapsedTile))
			{
				GD.PrintErr($"Tile {collapsedTile} at {pos} does not exist in Patterns.");
				continue; // Skip if collapsedTile is invalid
			}

			Array<Vector2I> validNeighbors = Patterns[collapsedTile];
			Vector2I[] directions = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

			// Process each direction
			for (int i = 0; i < directions.Length; i++)
			{
				Vector2I neighborPos = pos + directions[i];

				// Ensure neighbor position is within bounds
				if (neighborPos.X < 0 || neighborPos.X >= MapSizeX || neighborPos.Y < 0 || neighborPos.Y >= MapSizeY)
					continue;

				// Ensure the neighbor cell exists and has options left
				if (tiles[neighborPos.X][neighborPos.Y] == null)
				{
					GD.PrintErr($"Neighbor cell {neighborPos} was unexpectedly null. Initializing to an empty array.");
					tiles[neighborPos.X][neighborPos.Y] = new Vector2I[0]; // Initialize with an empty array to prevent further issues
					continue;
				}

				System.Collections.Generic.List<Vector2I> neighborOptions = tiles[neighborPos.X][neighborPos.Y].ToList();

				// Remove any options from neighbor that aren't valid
				int originalCount = neighborOptions.Count;
				neighborOptions.RemoveAll(tile => !validNeighbors.Contains(tile));

				// Only update and enqueue if we reduced the number of options
				if (neighborOptions.Count < originalCount)
				{
					tiles[neighborPos.X][neighborPos.Y] = neighborOptions.ToArray();
					ToVisit.Enqueue(neighborPos); // Propagate further if options were reduced
				}
			}
		}
	}


    // Collapse a cell to a single tile choice and update the TileMap
    private void Collapse(Vector2I position, Vector2I[][][] tiles)
    {
        if (tiles[position.X][position.Y].Length <= 1) return; // Already collapsed

        // Choose a random tile from possible options
        Vector2I chosen = tiles[position.X][position.Y][GD.Randi() % tiles[position.X][position.Y].Length];
        Vector2I[] visited = { chosen };
        ground.SetCell(position, 0, chosen, 0); // Set the tile in the TileMap
        tiles[position.X][position.Y] = visited; // Collapse to this tile
    }

    // Find the cell with the lowest entropy (fewest remaining possibilities)
    private Vector2I? GetLowestEntropy(Vector2I[][][] tiles)
    {
        int minEntropy = int.MaxValue;
        Vector2I? selectedCell = null;

        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                int entropy = tiles[x][y].Length;
                if (entropy > 1 && entropy < minEntropy)
                {
                    minEntropy = entropy;
                    selectedCell = new Vector2I(x, y);
                }
            }
        }

        return selectedCell;
    }

    public override void _Ready()
    {
        float[,] map = new float[MapSizeX, MapSizeY];
        Random seedGen = new Random();
        int seed = seedGen.Next(1000, 9999);
        MapNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        MapNoise.Seed = seed;
        GD.Print("Seed:", MapNoise.Seed);

        // Generate noise values
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                map[i, j] = MapNoise.GetNoise2D(i, j);
            }
        }

        // Display max and min noise values for debugging
        float maxval = (from float v in map select v).Max();
        float minval = (from float v in map select v).Min();
        GD.Print("Largest Value", maxval);
        GD.Print("Smallest Value", minval);

        GeneratePatterns();
        GenerateMap(map);
    }
}


// Low Value -> Wall or Water i.e impassable terrain
// High Value -> Dirt Paths or roads i.e. fast moving terrain
// Medium Value -> Middle of the road i.e. grass patches or sand or whatever default ground
