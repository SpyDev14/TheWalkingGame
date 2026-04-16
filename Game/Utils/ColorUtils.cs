// It is utils with extension syntax for easy using, not extension inmmediatly

using DrawingColor = System.Drawing.Color;
using RaylibColor = Raylib_cs.Color;

namespace Game.Utils;

internal static class ColorUtils
{
	// Microsoft stupid, wtf, why DrawingColor equeals color also check fkcing string Name?!!!!!!
	public static bool IsColorEquals(this DrawingColor a, DrawingColor b) => (
		a.R == b.R &&
		a.G == b.G &&
		a.B == b.B
	);

	public static bool IsColorEquals(this DrawingColor a, RaylibColor b) => (
		a.R == b.R &&
		a.G == b.G &&
		a.B == b.B
	);

	public static bool IsColorEquals(this RaylibColor a, DrawingColor b) => (
		a.R == b.R &&
		a.G == b.G &&
		a.B == b.B
	);

	public static bool IsColorEquals(this RaylibColor a, RaylibColor b) => (
		a.R == b.R &&
		a.G == b.G &&
		a.B == b.B
	);

















	public static DrawingColor ToDrawingColor(this RaylibColor self) => DrawingColor.FromArgb(self.R, self.B, self.G, self.A);
	public static RaylibColor ToRaylibColor(this DrawingColor self) => new RaylibColor(self.R, self.B, self.G, self.A);
}
