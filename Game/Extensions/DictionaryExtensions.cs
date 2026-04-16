// It is extensions

namespace Game.Extensions;

internal static class DictionaryExts
{
	public static TValue GetOrDefault<TKey, TValue>(
		this Dictionary<TKey, TValue> self,
		TKey key, 
		TValue defaultValue
	) where TKey : notnull
	{
		if (self.TryGetValue(key, out var value))
			return value;
		return defaultValue;
	}
}
