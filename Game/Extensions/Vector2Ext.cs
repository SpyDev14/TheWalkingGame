namespace Game.Extensions;

internal static class Vector2Ext
{
	public static bool IsZero(this Vector2 self) => self.X == 0 && self.Y == 0;

	public static Vector2 Normalized(this Vector2 self) => Vector2.Normalize(self);
}
