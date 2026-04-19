using System.Collections;

namespace Game.Extensions;

internal static class IterableExt
{
	public static T Choose<T>(this IList<T> list, Random rng) => list[rng.Next(0, list.Count)];
	public static T Choose<T>(this T[] arr, Random rng) => arr[rng.Next(0, arr.Length)];
}
