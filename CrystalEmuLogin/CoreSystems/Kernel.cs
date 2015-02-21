using System.Collections.Concurrent;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.CoreSystems
{
    public static class Kernel
    {
        public static readonly ConcurrentDictionary<uint, Player> Players = new ConcurrentDictionary<uint, Player>();
    }
}