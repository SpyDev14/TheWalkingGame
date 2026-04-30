using Raylib_cs;
namespace Game.Levels;

internal class GameLevel : ILevel
{
	public string Name { get; }
	public bool IsOutsideSolid { get; set; } = true;
	public Vector2 SpawnPointPos { get; }

	private readonly ImmutableArray<bool> _collisionField;
	private readonly Size _size;
	/// <summary>
	/// Map as image for create clippet image for draw minimap.
	/// Size euals to <see cref="_size"/>.
	/// </summary>
	public readonly Image _mapImage;

	// private Dictionary<int, Image> _scaledMapImages = new();
	public GameLevel(string name, GameObject[] field, Size size)
	{
		Name = name;
		SpawnPointPos = FindSpawnPoint(field, size);

		_size = size;
		_collisionField = field.Select(x => x == GameObject.Wall).ToImmutableArray();
		_mapImage = GenerateMapImage(field, size);
	}

	// Helper-funcs
	private static Image GenerateMapImage(GameObject[] field, Size size)
	{
		Image image = Raylib.GenImageColor(size.Width, size.Height, Color.Blank);
		for (int y = 0; y < size.Height; y++)
			for (int x = 0; x < size.Width; x++)
			{
				var cell = field[y * size.Width + x];
				Color color = cell.GetColor();
				Raylib.ImageDrawPixel(ref image, x, y, color);
			}

		return image;
	}
	private static Vector2 FindSpawnPoint(GameObject[] field, Size size)
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

	public Image GetMinimapImage(Size size, Vector2 pos, int pixelsPerTile, out Vector2 posOffset)
	{
		Image img = _mapImage;
		bool needDispose = false;
		if (pixelsPerTile > 1)
		{
			img = Raylib.ImageCopy(_mapImage);
			int newWidth = img.Width * pixelsPerTile;
			int newHeight = img.Height * pixelsPerTile;
			Raylib.ImageResizeNN(ref img, newWidth, newHeight);
			needDispose = true;
		}

		// (Given size can be greater than map) (in pixels)
		int rectWidth = Math.Min(size.Width, _size.Width) * pixelsPerTile;
		int rectHeight = Math.Min(size.Height, _size.Height) * pixelsPerTile;

		// Requested center pos. Can be off at map
		float centerX = pos.X * pixelsPerTile;
		float centerY = pos.Y * pixelsPerTile;

		float rectX = Math.Clamp(
			centerX - rectWidth / 2,
			0, img.Width - rectWidth
		);
		float rectY = Math.Clamp(
			centerY - rectHeight / 2,
			0, img.Height - rectHeight
		);

		float actualCenterX = rectX + rectWidth / 2f;
		float actualCenterY = rectY + rectHeight / 2f;
		posOffset = new Vector2(
			centerX - actualCenterX,
			centerY - actualCenterY
		);

		var cropRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
		var resut = Raylib.ImageFromImage(img, cropRect);
		if (needDispose) Raylib.UnloadImage(img);

		return resut;
	}

	// Collision checks
	public bool IsOutsideMap(Vector2 pos) => IsOutsideMap((int)pos.X, (int)pos.Y);
	public bool IsOutsideMap(int x, int y) => (
		x < 0 || x >= _size.Width ||
		y < 0 || y >= _size.Height
	);

	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCollided(int x, int y)
	{
		if (IsOutsideMap(x, y))
			return IsOutsideSolid;

		int idx = y * _size.Width + x;
		return _collisionField[idx];
	}

	public bool IsCircleCollided(Vector2 pos, float radius)
	{
		if (IsOutsideMap(pos + new Vector2(radius)) ||
			IsOutsideMap(pos - new Vector2(radius)))
			return IsOutsideSolid;

		// Check rectangle points
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
