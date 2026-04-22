using Point = System.Drawing.Point;

namespace Game.Arhive;


internal readonly record struct Camera(
	Point position,
	Angle rotate,
	Angle angle
);
internal interface IRaycaster
{
	/// <summary>
	/// Returns array with size given in <paramref name="raysToThrown"/>
	/// </summary>
	/// <param name="camera">Place from </param>
	/// <param name="angle"></param>
	/// <param name="map"></param>
	/// <param name="raysToThrown"></param>
	/// <returns>
	/// Returns array with <paramref name="raysToThrown"/> size with
	/// distances. Return -1 as distance if distance get over <paramref name="maxDistance"/>.
	/// </returns>
	public float[] GetDistances(
		Camera camera,
		bool[] map,
		int raysToThrown,
		float maxDistance
	);
}

internal class Raycaster : IRaycaster
{
	public float[] GetDistances(
		Camera camera,
		bool[] map,
		int raysToThrown,
		float maxDistance
	)
	{
		return new float[raysToThrown];
	}
}
