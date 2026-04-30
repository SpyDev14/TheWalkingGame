using Raylib_cs;
namespace Game.Levels;


internal interface ILevel
{
	public string Name { get; }

	/// <summary>
	/// Generate new Image for minimap preview and return.
	/// </summary>
	/// <param name="size">Size in tiles</param>
	/// <param name="pos">Pivot map cell (pos in tiles). Be equal to center at image in regular case.</param>
	/// <param name="pixelsPerTile">Pixels per tile</param>
	/// <param name="posOffset">
	/// 	Center position offset at returned Image center.
	/// 	<para>
	/// 		Not equals to zero when <paramref name="size"/> lover than map size,
	/// 		or <paramref name="pos"/> + <paramref name="pos"/> / 2 in map.
	/// 	</para>
	/// </param>
	/// <returns>
	/// 	new Image. Should be manual unloaded from RAM by
	/// 	<see cref="Raylib.UnloadImage(Image)"/> for except memory leack.
	/// </returns>
	public Image GetMinimapImage(Size size, Vector2 pos, int pixelsPerTile, out Vector2 posOffset);
	public Vector2 SpawnPointPos { get; }

	public bool IsOutsideMap(Vector2 pos) => IsOutsideMap((int)pos.X, (int)pos.Y);
	public bool IsOutsideMap(int x, int y);

	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCollided(int x, int y);

	public bool IsCircleCollided(Vector2 pos, float radius);
}
