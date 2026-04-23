namespace Game.Types;

internal readonly struct Meters<T> where T : INumber<T>
{
	public readonly T Value;
	public Meters(T value) => Value = value;

	public static implicit operator T(Meters<T> value) => value.Value;
	public static implicit operator Meters<T>(T value) => new(value);//😡😡😡
	public static implicit operator Tiles<T>(Meters<T> value) => new((T)((float)value.Value * Constants.TILES_PER_METER));

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

	public static implicit operator T(Tiles<T> value) => value.Value;
	public static implicit operator Tiles<T>(T value) => new(value);
	public static implicit operator Meters<T>(Tiles<T> value) => new((T)((float)value.Value / Constants.TILES_PER_METER));

	public static Tiles<T> operator +(Tiles<T> a, Tiles<T> b) => new(a.Value + b.Value);
	public static Tiles<T> operator -(Tiles<T> a, Tiles<T> b) => new(a.Value - b.Value);
	public static Tiles<T> operator /(Tiles<T> a, Tiles<T> b) => new(a.Value / b.Value);
	public static Tiles<T> operator *(Tiles<T> a, Tiles<T> b) => new(a.Value * b.Value);
	public static Tiles<T> operator %(Tiles<T> a, Tiles<T> b) => new(a.Value % b.Value);
}