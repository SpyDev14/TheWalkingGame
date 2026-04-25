using Raylib_cs;
namespace Game.Levels;

internal class Level
{
	public readonly Size Size;
	private ImmutableArray<bool> CollisionField { get; }
	public Texture2D TextureForRender { get; }
	public bool OutsideIsSolid { get; set; }

	public Vector2 SpawnPoint { get; }

	public Level(GameObject[] field, Size size)
	{
		Size = size;
		CollisionField = field.Select(x => x == GameObject.Wall).ToImmutableArray();
		TextureForRender = GenerateRenderTexture(field, size);
		SpawnPoint = FindSpawnPoint(field, size);
	}

	private Texture2D GenerateRenderTexture(GameObject[] field, Size size)
	{
		Image image = Raylib.GenImageColor(size.Width, size.Height, Color.Blank);
		for (int y = 0; y < size.Height; y++)
			for (int x = 0; x < size.Width; x++)
			{
				var cell = field[y * size.Width + x];
				Color color = cell switch
				{
					GameObject.Wall => Color.White,
					GameObject.SpawnPoint => Raylib.ColorBrightness(Color.Red, 0.5f),
					GameObject.None => Color.Black,
					_ => Color.Magenta // Unknown
				};

				Raylib.ImageDrawPixel(ref image, x, y, color);
			}

		var texture = Raylib.LoadTextureFromImage(image);
		Raylib.UnloadImage(image);

		return texture;
	}

	private Vector2 FindSpawnPoint(GameObject[] field, Size size)
	{
		Vector2? pos = null;
		for (int y = 0; y < size.Height; y++)
			for (int x = 0; x < size.Width; x++)
				if (field[y * size.Width + x] == GameObject.SpawnPoint)
					pos = new(x, y);

		if (!pos.HasValue) pos = new(
			size.Width  / 2 + (size.Width  % 2 == 0 ? 0 : 0.5f),
			size.Height / 2 + (size.Height % 2 == 0 ? 0 : 0.5f)
		);

		return pos.Value;
	}


	// Collision check's
	public bool IsOutsideMap(Vector2 pos) => IsOutsideMap((int)pos.X, (int)pos.Y);
	public bool IsOutsideMap(int x, int y) => (
		x < 0 || x >= Size.Width ||
		y < 0 || y >= Size.Height
	);

	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCollided(int x, int y)
	{
		if (IsOutsideMap(x, y))
			return OutsideIsSolid;

		int idx = y * Size.Width + x;
		return CollisionField[idx];
	}

	public bool IsCircleCollided(Vector2 pos, float radius)
	{
		if (IsOutsideMap(pos + new Vector2(radius)) ||
			IsOutsideMap(pos - new Vector2(radius)))
			return OutsideIsSolid;

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
