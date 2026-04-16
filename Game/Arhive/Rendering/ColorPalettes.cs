namespace Game.Arhive.Rendering;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Size = System.Drawing.Point;

[Obsolete]
internal readonly record struct GetColorData(
	double Distance,
	Point ScreenPosition,
	Size ScreenSize
);

[Obsolete]
internal interface IColorPalette<T>
{
	public T Wall(GetColorData data);
	public T Floor(GetColorData data);
	public T Sky(GetColorData data);
}
[Obsolete]
internal class ColorPalette() : IColorPalette<Color>
{
	public Color Sky(GetColorData data)
	{
		double widthPx, heightPx;
		widthPx = data.ScreenPosition.X;
		heightPx = data.ScreenPosition.Y;

		if (widthPx * heightPx % 10 == 0)
			return Color.White;

		return Color.Black;
	}

	public Color Floor(GetColorData data)
	{
		return Color.FromArgb(96, 96, 96);
	}

	public Color Wall(GetColorData data)
	{
		return Color.FromArgb(64, 64, 64);
	}
}