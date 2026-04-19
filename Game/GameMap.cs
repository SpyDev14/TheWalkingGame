using System.Runtime.Versioning;

namespace Game;
enum GameObject : byte
{
	None,
	Wall,
}

internal class GameMap
{
	public readonly Size Size;
	public Vector2 PositionInCenter => new Vector2(Size.Width / 2, Size.Height / 2);
	public ImmutableArray<bool> CollisionField { get; }

	public GameMap(GameObject[] field, Size size)
	{
		Size = size;
		CollisionField = field.Select(x => x == GameObject.Wall).ToImmutableArray();
	}

	public static GameMap FromBlueprint(
		string[] blueprint,
		Func<char, GameObject> charToObject,
		GameObject fillObject = GameObject.Wall
	)
	{
		int width = blueprint.Select(s => s.Length).Max();
		int height = blueprint.Length;
		var field = new GameObject[width * blueprint.Length];

		for (int y = 0; y < blueprint.Length; y++)
		{
			string line = blueprint[y];
			for (int x = 0; x < width; x++)
				field[y * width + x] = x < line.Length ? charToObject(line[x]) : fillObject;
		}

		return new GameMap(field, new(width, blueprint.Length));
	}

	[SupportedOSPlatform("windows")]
	public static GameMap FromImage(Bitmap img, Func<System.Drawing.Color, GameObject> pixelToObject)
	{
		int width = img.Width;
		int height = img.Height;
		var field = new GameObject[width * img.Height];

		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				field[y * width + x] = pixelToObject(img.GetPixel(x, y));

		return new GameMap(field, new(width, height));
	}

	private const char PLUG_MAP_SPAWNPOINT_CHAR = 'x';
	private static readonly string[] _plugBlueprint = [
		"              ##",
		"               #",
		"  #####         ",
		"  #   #         ",
		"  # x #         ",
		"  #   #         ",
		"  ## ##         ",
		"                ",
		"                ",
	];
	public static GameMap PlugMap => FromBlueprint(
		_plugBlueprint,
		cell => cell switch {
			'#' => GameObject.Wall,
			_ => GameObject.None
		}
	);

	public static Vector2 PlugMapSpawnPoint
	{
		get
		{
			for (int y = 0; y < _plugBlueprint.Length; y++)
			{
				string line = _plugBlueprint[y];
				for (int x = 0; x < line.Length; x++)
					if (_plugBlueprint[y][x] == PLUG_MAP_SPAWNPOINT_CHAR)
						return new Vector2(x, y);
			}
			return new Vector2(0, 0);
		}
	}

	private bool IsOutsideMap()

	public bool IsCollided(int x, int y)
	{
		if (x < 0 || x >= Size.Width ||
			y < 0 || y >= Size.Height)
			return true;

		int idx = y * Size.Width + x;
		return CollisionField[idx];
	}
	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCircleCollided(Vector2 pos, float radius)
	{
		// Check rectangle size (points positions)
		int minX = (int)Math.Clamp(MathF.Floor(pos.X - radius),   0, Size.Width - 1);
		int maxY = (int)Math.Clamp(MathF.Ceiling(pos.Y + radius), 0, Size.Height - 1);
		int maxX = (int)Math.Clamp(MathF.Ceiling(pos.X + radius), 0, Size.Width - 1);
		int minY = (int)Math.Clamp(MathF.Floor(pos.Y - radius),   0, Size.Height - 1);


		for (int y = minY; y <= maxY; y++)
		for (int x = minX; x <= maxX; x++)
			{
				if (!IsCollided(x, y)) continue;

				// Closest cell to check
				float closestX = Math.Clamp(pos.X, x, x + 1);
				float closestY = Math.Clamp(pos.Y, y, y + 1);

				// for check circle
				float dx = pos.X - closestX;
				float dy = pos.Y - closestY;
				float distanceSquared = (dx * dx) + (dy * dy);

				// Check intersection
				if (distanceSquared < radius * radius)
					return true;
			}
		return false;
	}
}
