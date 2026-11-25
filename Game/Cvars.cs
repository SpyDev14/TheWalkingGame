namespace Game;

internal static class CVars
{
	public readonly static string ResourcesDirectory = Path.Join(
		AppContext.BaseDirectory, $"Resources{Path.DirectorySeparatorChar}");

	// TODO: сделать
	// 1. ConfigurationManager с методом GetVar(CVarDef)
	// 2. CVarDef<T>
	// Вместо мутируемой глобальной переменной

	/// <summary>
	/// How much tiles in one meter (tiles to meters)
	/// </summary>
	public static float TilesPerMeter = 1f;
}
