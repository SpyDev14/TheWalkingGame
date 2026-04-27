using Raylib_cs;
namespace Game.Levels;


internal interface ILevel
{
	public Image ImageForRender { get; }
	public Vector2 SpawnPoint { get; }

	public bool IsOutsideMap(Vector2 pos) => IsOutsideMap((int)pos.X, (int)pos.Y);
	public bool IsOutsideMap(int x, int y);

	public bool IsCollided(Vector2 pos) => IsCollided((int)pos.X, (int)pos.Y);
	public bool IsCollided(int x, int y);

	public bool IsCircleCollided(Vector2 pos, float radius);
}