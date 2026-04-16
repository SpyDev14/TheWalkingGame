using System.Collections.Immutable;
using static Game.Arhive.Shapes.ShapeUtils;
using PointF = System.Drawing.PointF;

namespace Game.Arhive.Shapes;

internal interface IFigure
{
	public bool IsPointIn(PointF point);
}

internal static class ShapeUtils
{
	public const float Epsilon = 1e-6f;
	public static bool IsPointOnLine(PointF point, PointF a, PointF b)
	{
		var p = point;
		float cross = (p.X - a.X) * (b.Y - a.Y) - (p.Y - a.Y) * (b.X - a.X);
		if (Math.Abs(cross) > Epsilon)
			return false;

		float dot = (p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y);
		if (dot < -Epsilon)
			return false;

		float squaredLen = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
		if (dot - squaredLen > Epsilon)
			return false;

		return true;
	}
}

internal class MultipointShape : IFigure
{
	/// <exception cref="ArgumentOutOfRangeException">
	/// Raises when <paramref name="points"/> Length lover than 3.
	/// </exception>
	public MultipointShape(PointF[] points)
	{
		if (points.Length < 3)
			throw new ArgumentOutOfRangeException(nameof(points), $"Minimal 3 points.");
		Points = points.ToImmutableArray();
	}
	public ImmutableArray<PointF> Points { get; }
	public bool IsPointIn(PointF point)
	{
		bool inside = false;
		int n = Points.Length;
		float x = point.X;
		float y = point.Y;

		for (int i = 0, j = n - 1; i < n; j = i++)
		{
			PointF p1 = Points[i];
			PointF p2 = Points[j];

			if (IsPointOnLine(point, p1, p2))
				return true;

			// Условие пересечения горизонтального луча вправо с ребром (p1, p2)
			// Ребро пересекает луч, если:
			//  - y-координата точки между y1 и y2 (строгое сравнение с одним из концов,
			//    чтобы избежать двойного счёта вершин)
			//  - точка пересечения луча с ребром находится правее проверяемой точки
			bool intersect = (
				((p1.Y > y) != (p2.Y > y)) &&
				(x < (p2.X - p1.X) * (y - p1.Y) / (p2.Y - p1.Y) + p1.X)
			);

			if (intersect)
				inside = !inside;
		}

		return inside;
	}
}

internal class Ellipse : IFigure
{
	public float Height { get; }
	public float Width { get; }

	public Ellipse(float height, float width)
	{
		if (height <= 0 || width <= 0)
			throw new ArgumentException("Height and Width must be positive");

		Height = height;
		Width = width;
	}

	public bool IsPointIn(PointF point)
	{
		float a = Width / 2f;
		float b = Height / 2f;
		float cx = a, cy = b; // В текущем контексте, где начало = 0, 0

		float dx = (point.X - cx) / a;
		float dy = (point.Y - cy) / b;

		return dx*dx + dy*dy <= 1 + Epsilon;
	}
}