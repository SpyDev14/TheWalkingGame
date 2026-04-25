namespace Game.Types;

// It's cursed
/*
internal readonly struct Meters<T> where T : INumber<T>
{
	public readonly T Value;
	public Meters(T value) => Value = value;

	public static implicit operator Meters<T>(T value) => new(value);
	public static implicit operator T(Meters<T> meters) => meters.Value;

	public static implicit operator Tiles<T>(Meters<T> meters)
	{
		float newValue = float.CreateChecked(meters.Value) * REWORK_IT.TILES_PER_METER;
		return new Tiles<T>(T.CreateChecked(newValue));
	}

	public static Meters<T> operator +(Meters<T> a, Meters<T> b) => new(a.Value + b.Value);
	public static Meters<T> operator -(Meters<T> a, Meters<T> b) => new(a.Value - b.Value);
	public static Meters<T> operator /(Meters<T> a, Meters<T> b) => new(a.Value / b.Value);
	public static Meters<T> operator *(Meters<T> a, Meters<T> b) => new(a.Value * b.Value);
	public static Meters<T> operator %(Meters<T> a, Meters<T> b) => new(a.Value % b.Value);
}

internal struct Tiles<T> where T : INumber<T>
{
	public readonly T Value;
	public Tiles(T value) => Value = value;

	public static implicit operator T(Tiles<T> meters) => meters.Value;
	public static implicit operator Tiles<T>(T value) => new(value);

	public static implicit operator Meters<T>(Tiles<T> meters)
	{
		float newValue = float.CreateChecked(meters.Value) / REWORK_IT.TILES_PER_METER;
		return new Tiles<T>(T.CreateChecked(newValue));
	}

	public static Tiles<T> operator +(Tiles<T> a, Tiles<T> b) => new(a.Value + b.Value);
	public static Tiles<T> operator -(Tiles<T> a, Tiles<T> b) => new(a.Value - b.Value);
	public static Tiles<T> operator /(Tiles<T> a, Tiles<T> b) => new(a.Value / b.Value);
	public static Tiles<T> operator *(Tiles<T> a, Tiles<T> b) => new(a.Value * b.Value);
	public static Tiles<T> operator %(Tiles<T> a, Tiles<T> b) => new(a.Value % b.Value);

	void Test()
	{
	}
}
*/

internal static class UnitConverations
{
	/// <summary>
	/// How much tiles in one meter (tiles to meters)
	/// </summary>
	public const float TILES_PER_METER = 1f;

	/// <summary>Convert meters to tiles</summary>
	public static float ToTiles(this float meters) => meters * TILES_PER_METER;

	/// <summary>Convert tiles to meters</summary>
	public static float ToMeters(this float tiles) => tiles / TILES_PER_METER;
}