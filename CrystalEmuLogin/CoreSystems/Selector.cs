using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CrystalEmuLib;
using CrystalEmuLogin.Networking.IPC_Comms;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;
using CrystalEmuLogin.World;

namespace CrystalEmuLogin.CoreSystems
{
    public static class Selector
    {
        public static readonly List<Vector3> SpawnPoints = new List<Vector3>(); 
        public static void LoadSpawnPoints()
        {
            var SpawnPoint2 = new Vector3(null, 87, 185, 1040);
            SpawnPoints.Add(SpawnPoint2);
            var SpawnPoint3 = new Vector3(null, 94, 168, 1040);
            SpawnPoints.Add(SpawnPoint3);
            var SpawnPoint4 = new Vector3(null, 95, 186, 1040);
            SpawnPoints.Add(SpawnPoint4);
            var SpawnPoint5 = new Vector3(null, 100, 179, 1040);
            SpawnPoints.Add(SpawnPoint5);
        }

        public static async void LoadCharacters(Player Player)
        {
            var Dir = Core.AccountDatabasePath + Player.Username + "\\";
            var Files = Directory.GetDirectories(Dir).ToList();
            Files.Remove(Dir + "SELECTOR");
            int I = 0;
            foreach (var P in Files.Select(T => new Player(null) {Username = Player.Username, Name = T.Replace(Dir, "")}))
            {
                await DatabaseConnection.LoadCharacter(P);
                P.UID = (uint)(2000000 + I);
                P.Location = SpawnPoints[I];
                Player.Send(CoPacket.MsgSpawn(P));
                I++;
            }
        } 
        public static void SpawnCharacters(Player Player)
        {
            for (var I = 0; I < Player.Characters.Count; I++)
            {
                var P = Player.Characters[I];
                P.Location = SpawnPoints[I];
            }
        }
    }
}
