
using System.Runtime.Versioning;

namespace Game.Levels;


internal static class CreateLevel
{
	public static GameLevel FromBlueprint(
		string name,
		string[] blueprint,
		Func<char, GameObject> charToObject,
		GameObject fillObject = GameObject.Wall
	)
	{
		int width = blueprint.Max(s => s.Length);
		int height = blueprint.Length;
		var field = new GameObject[width * blueprint.Length];

		for (int y = 0; y < blueprint.Length; y++)
		{
			string line = blueprint[y];
			for (int x = 0; x < width; x++)
				field[y * width + x] = x < line.Length ? charToObject(line[x]) : fillObject;
		}

		return new GameLevel(name, field, new(width, blueprint.Length));
	}

	[SupportedOSPlatform("windows6.1")]
	public static GameLevel FromImage(
		string name, Bitmap img, Func<System.Drawing.Color, GameObject> pixelToObject
	)
	{
		int width = img.Width;
		int height = img.Height;
		var field = new GameObject[width * img.Height];

		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				field[y * width + x] = pixelToObject(img.GetPixel(x, y));

		return new GameLevel(name, field, new(width, height));
	}

	public static GameLevel PlugLevel => FromBlueprint(
		"Plug",
		[
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
}
