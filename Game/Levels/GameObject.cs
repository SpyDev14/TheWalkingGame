using Raylib = Raylib_cs.Raylib;

namespace Game.Levels;

enum GameObject : byte
{
	None,
	Wall,
	SpawnPoint,
}

internal static class impl_GameObject
{
	public static Color GetColor(this GameObject self) => self switch
	{
		GameObject.Wall => Color.White,
		GameObject.SpawnPoint => Raylib.ColorBrightness(Color.Red, 0.5f),
		GameObject.None => Color.Black,
		_ => Color.Magenta // Unknown
	};
}
