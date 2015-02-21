using System.Collections.Concurrent;
using CrystalEmu.PlayerFunctions;

namespace CrystalEmu.CoreSystems
{
    public static class Kernel
    {
        public static readonly ConcurrentDictionary<uint, Player> Players = new ConcurrentDictionary<uint, Player>();
    }
}