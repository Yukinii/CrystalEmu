using System.Collections.Concurrent;

namespace CrystalEmuLib.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> Self, TKey Key)
        {
            TValue Ignored;
            return Self.TryRemove(Key, out Ignored);
        }
    }
}