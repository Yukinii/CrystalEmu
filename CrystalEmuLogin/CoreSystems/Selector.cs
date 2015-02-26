using System;
using System.Collections.Generic;
using CrystalEmuLogin.PlayerFunctions;
using CrystalEmuLogin.World;

namespace CrystalEmuLogin.CoreSystems
{
    public static class Selector
    {
        public static List<Vector3> SpawnPoints = new List<Vector3>(); 
        public static void LoadSpawnPoints()
        {
            
        }
        public static void SpawnCharacters(Player Player)
        {
            for (var I = 0; I < Player.Characters.Count; I++)
            {
                
            }
        }
    }
}
