using static Game.Maths.Angle.MathOperations;

namespace Game.Maths;

internal struct Angle : IFormattable
{
	public static class MathOperations
	{
		public static float RadiansToDegrees(float radians) => 180 / MathF.PI * radians;
		public static float DegreesToRadians(float degrees) => MathF.PI / 180 * degrees;

		public static float NormalizeDegrees(float degrees) => (degrees %= 360) switch
		{
			< -180 => degrees + 360,
			> 180 => degrees - 360,
			_ => degrees
		};

		public static float NormalizeRadians(float radians) => (radians %= (MathF.PI * 2)) switch
		{
			< -MathF.PI => radians + MathF.PI * 2,
			> MathF.PI => radians - MathF.PI * 2,
			_ => radians
		};
	}

	private Angle(float radians) => Radians = radians;
	public Angle() { }

	/// <summary>90 degrees angle</summary>
	public static Angle Right => FromDegrees(90);

	public static Angle FromRadians(float radians) => new Angle(radians);
	public static Angle FromDegrees(float degrees) => new Angle(DegreesToRadians(degrees));

	public float Radians { readonly get; set; } = 0;
	public float Degrees
	{
		readonly get => RadiansToDegrees(Radians);
		set => Radians = DegreesToRadians(value);
	}

	public float NormalizedDegrees
	{
		readonly get => NormalizeDegrees(Degrees);
		set => Radians = DegreesToRadians(NormalizeDegrees(value));
	}

	public float NormalizedRadians
	{
		readonly get => NormalizeRadians(Radians);
		set => Radians = NormalizeRadians(value);
	}

	public readonly Vector2 AsDirection() => new Vector2(
		MathF.Cos(Radians),
		MathF.Sin(Radians)
	);

	public readonly Angle Normalized() => FromRadians(NormalizedRadians);
	public readonly bool IsNormalized() => Radians == NormalizedRadians;

	public readonly override string ToString() => ToString(null, null);
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> $"{Degrees.ToString(format)}° ({Radians.ToString(format)} R)";

	public static Angle operator +(Angle a, Angle b) => new Angle(a.Radians + b.Radians);
	public static Angle operator -(Angle a, Angle b) => new Angle(a.Radians - b.Radians);
	public static Angle operator *(Angle a, float mul) => new Angle(a.Radians * mul);
	public static Angle operator /(Angle a, float dividor) => new Angle(a.Radians / dividor);
	public static Angle operator %(Angle a, float dividor) => new Angle(a.Radians % dividor);
}
