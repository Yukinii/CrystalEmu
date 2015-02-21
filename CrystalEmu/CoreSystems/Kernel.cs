namespace CrystalEmu.CoreSystems
{
    using System.Collections.Concurrent;
    using PlayerFunctions;

    public static class Kernel
    {
        public static readonly ConcurrentDictionary<uint, Player> Players = new ConcurrentDictionary<uint, Player>(); 
    }
}
