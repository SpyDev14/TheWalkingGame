using static Game.Utils.Angle.MathOperations;

namespace Game.Utils;

internal struct Angle : IFormattable{
	public static class MathOperations
	{
		public static float RadiansToDegrees(float radians) => 180 / MathF.PI * radians;
		public static float DegreesToRadians(float degrees) => MathF.PI / 180 * degrees;

		public static float NormalizeDegrees(float degrees) => degrees % (180 * MathF.Sign(degrees));
		public static float NormalizeRadians(float radians) => radians % (MathF.PI * MathF.Sign(radians));
	}

	private Angle(float radians) => Radians = radians;
	public Angle() { }
	public static Angle FromRadians(float radians) => new Angle(radians);
	public static Angle FromDegrees(float degrees) => new Angle(DegreesToRadians(degrees));

	public float Radians { get; set; }
	public float Degrees
	{
		get => RadiansToDegrees(Radians);
		set => Radians = DegreesToRadians(value);
	}

	public float NormalizedDegrees
	{
		get => NormalizeDegrees(Degrees);
		set => Radians = DegreesToRadians(NormalizeDegrees(value));
	}

	public float NormalizedRadians
	{
		get => NormalizeRadians(Radians);
		set => Radians = NormalizeRadians(value);
	}

	public Vector2 AsDirection() => new Vector2(
		MathF.Cos(Radians),
		MathF.Sin(Radians)
	);

	public Angle Normalized() => FromRadians(NormalizedRadians);

	public override string ToString() => ToString(null, null);
	public string ToString(string? format, IFormatProvider? formatProvider)
		=> $"{Degrees.ToString(format)}° ({Radians.ToString(format)} R)";


	public static Angle operator +(Angle a, Angle b) => new Angle(a.Radians + b.Radians);
	public static Angle operator -(Angle a, Angle b) => new Angle(a.Radians - b.Radians);
	public static Angle operator *(Angle a, float mul) => new Angle(a.Radians * mul);
	public static Angle operator /(Angle a, float dividor) => new Angle(a.Radians / dividor);
	public static Angle operator %(Angle a, float dividor) => new Angle(a.Radians % dividor);

	public static bool operator >(Angle a, Angle b) => a.Radians > b.Radians;
	public static bool operator <(Angle a, Angle b) => a.Radians < b.Radians;
}
