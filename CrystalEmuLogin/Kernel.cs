using System.Collections.Concurrent;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin
{
    public static class Kernel
    {
        public static readonly ConcurrentDictionary<uint, Player> Players = new ConcurrentDictionary<uint, Player>();
    }
}