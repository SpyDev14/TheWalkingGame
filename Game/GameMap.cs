using Raylib_cs;
using System.Runtime.Versioning;

namespace Game;
enum GameObject : byte
{
	None,
	Wall,
	SpawnPoint,
}

internal class GameMap
{
	public readonly Size Size;
	public ImmutableArray<bool> CollisionField { get; }
	public Texture2D TextureForRender { get; }

	public Vector2 SpawnPoint => new Vector2(_spawnCell.x, _spawnCell.y) + new Vector2(0.5f);
	private (int x, int y) _spawnCell;

	public GameMap(GameObject[] field, Size size)
	{
		Size = size;
		CollisionField = field.Select(x => x == GameObject.Wall).ToImmutableArray();

		_spawnCell = (size.Width / 2, size.Height / 2);
		

		for (int y = 0; y < Size.Height; y++)
			for (int x = 0; x < Size.Width; x++)
				if (field[y*size.Width+x] == GameObject.SpawnPoint)
					_spawnCell = (x, y);


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

	public static GameMap PlugMap => FromBlueprint([
			"              ##",
			"               #",
			"  #####         ",
			"  #   #         ",
			"  # x #         ",
			"  #   #         ",
			"  ## ##         ",
			"                ",
			"                ",
		],
		cell => cell switch {
			'#' => GameObject.Wall,
			'x' => GameObject.SpawnPoint,
			_ => GameObject.None
		}
	);

	// Collision check's
	private bool IsOutsideMap(Vector2 pos) => IsOutsideMap((int)pos.X, (int)pos.Y);
	private bool IsOutsideMap(int x, int y) => (
		x < 0 || x >= Size.Width ||
		y < 0 || y >= Size.Height
	);

	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCollided(int x, int y)
	{
		if (IsOutsideMap(x, y)) return true;

		int idx = y * Size.Width + x;
		return CollisionField[idx];
	}

	public bool IsCircleCollided(Vector2 pos, float radius)
	{
		if (IsOutsideMap(pos + new Vector2(radius)) ||
			IsOutsideMap(pos - new Vector2(radius)))
			return true;

		// Check rectangle size (points positions)
		int minX = (int)MathF.Floor(pos.X - radius);
		int maxY = (int)MathF.Ceiling(pos.Y + radius);
		int maxX = (int)MathF.Ceiling(pos.X + radius);
		int minY = (int)MathF.Floor(pos.Y - radius);


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
