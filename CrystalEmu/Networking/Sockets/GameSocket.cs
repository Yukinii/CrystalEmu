using System;
using CrystalEmu.Networking.Queue;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Sockets;

namespace CrystalEmu.Networking.Sockets
{
    internal static class GameSocket
    {
        public static YukiServer Socket;

        public static void Open()
        {
            try
            {
                Console.Write("Opening a Map Server Socket...");
                Socket = new YukiServer
                {
                    Port = 5816,
                    OnClientConnect = Handle,
                    OnClientReceive = Handle
                };
                Socket.Enable();
                Core.WriteLine(" [Success]",ConsoleColor.Green);
            }
            catch
            {
                Core.WriteLine(" [Fail]", ConsoleColor.Red);
            }
        }

        private static void Handle(YukiSocket Connection, object Param)
        {
            if (Connection.Ref == null)
                Connection.Ref = new Player(Connection);
            else
                IncomingQueue.Add((Connection.Ref as Player), (byte[])Param);
        }
    }
}
