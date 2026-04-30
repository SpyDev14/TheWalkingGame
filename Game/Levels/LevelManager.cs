namespace Game.Levels;

internal interface ILevelManager
{
	public GameLevel CurrentLevel { get; }
}

internal class LevelManager : ILevelManager
{
	public GameLevel CurrentLevel { get; }

	public LevelManager(GameLevel initialLevel)
	{
		CurrentLevel = initialLevel;
	}
}
