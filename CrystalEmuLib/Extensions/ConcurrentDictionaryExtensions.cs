using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CrystalEmuLib.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> Self, TKey Key)
        {
            TValue Ignored;
            return Self.TryRemove(Key, out Ignored);
        }

        public static TValue GetValueOrNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> Self, TKey Key)
        {
            TValue Value;
            return Self.TryGetValue(Key, out Value) ? Value : default(TValue);
        }
    }
}