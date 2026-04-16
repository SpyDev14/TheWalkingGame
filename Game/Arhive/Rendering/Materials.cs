namespace Game.Arhive.Rendering;

using Point = System.Drawing.Point;

internal readonly record struct GetData(
	double Distance,
	Point ScreenPosition,
	Size ScreenSize
);

internal readonly struct Material
{
	//public Color GetColor()
}
