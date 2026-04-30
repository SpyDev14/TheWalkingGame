using static Raylib_cs.Raylib;
using Game.Maths;

namespace Game;

readonly record struct Theme(
	Color WallNear,
	Color WallFar,
	Color FloorNear,
	Color FloorFar,
	Color Sky,
	Func<Direction, Color>? WallTint = null
)
{
	public readonly static Theme MonoLight = new Theme(
		WallNear: new Color(255, 255, 255),
		WallFar: new Color(127, 127, 127),
		FloorNear: new Color(55, 55, 55),
		FloorFar: new Color(15, 15, 15),
		Sky: new Color(0, 0, 0)
	);

	public readonly static Theme ColoredNight = new Theme(
		WallNear: MonoLight.WallNear,
		WallFar: MonoLight.WallFar,
		FloorNear: MonoLight.FloorNear,
		FloorFar: MonoLight.FloorFar,
		Sky: MonoLight.Sky,
		WallTint: static dir => ColorTint(dir switch
		{
			Direction.South => Color.Beige,
			Direction.North => Color.SkyBlue,
			Direction.East => Color.Pink,
			Direction.West or _ => Color.Violet // (Direction)228 - is valid Direction (enum) in C# 😡
		}, Color.White)
	);

	public readonly static Theme ColoredDay = new Theme(
		WallNear: ColoredNight.WallNear,
		WallFar: ColoredNight.WallFar,
		FloorNear: ColoredNight.FloorNear,
		FloorFar: ColoredNight.FloorFar,
		Sky: ColorTint(Color.SkyBlue, Color.White),
		WallTint: ColoredNight.WallTint
	);

	public readonly static Theme MonoDark = new Theme(
		WallNear: new Color(16, 16, 16),
		WallFar: new Color(0, 0, 0),
		FloorNear: new Color(16, 16, 16),
		FloorFar: new Color(0, 0, 0),
		Sky: new Color(0, 0, 0)
	);

	public readonly static Theme Lavaland = new Theme(
		WallNear: new Color(0x60, 0x48, 0x48),
		WallFar: new Color(60, 47, 47),
		FloorNear: new Color(56, 46, 46),
		FloorFar: new Color(35, 30, 30),
		Sky: new Color(100, 11, 11)
	);

	public readonly static Theme Backroums = new Theme(
		WallNear: new Color(172, 165, 53),
		WallFar: new Color(68, 59, 0),
		FloorNear: new Color(114, 92, 17),
		FloorFar: new Color(46, 32, 0),
		Sky: new Color(154, 143, 64)
	);
}
