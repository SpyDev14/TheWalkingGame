namespace Game.Arhive.Rendering;

internal readonly record struct DrawFrameData {
	public required Func<int, float[]> GetWallSizes { get; init; }
}

internal interface IRenderer
{
	void DrawFrame(in DrawFrameData data);
}