using System;
using CrystalEmu.Networking.IPC_Comms;
using CrystalEmu.Networking.Queue;
using CrystalEmu.Networking.Sockets;
using CrystalEmuLib;

namespace CrystalEmu
{
    public class Program
    {
        static void Main()
        {
            Console.Title = "CrystalEmu - Map Server: 1010 (Birth Village)";
            while (!DatabaseConnection.Open().Result)
            {
                Console.Write(".");
            }
            IncomingQueue.Start();
            OutgoingQueue.Start();
            GameSocket.Open();
            Core.WriteLine("Online! Type help or ? for available commands!", ConsoleColor.White);

            #region Console Command Listener
            while (true)
            {
                switch (Console.ReadLine()?.ToLowerInvariant())
                {
                    case "help":
                    case "?":
                        {
                            Console.WriteLine("Available Commands: exit (kills the server)");
                            break;
                        }
                    case "exit":
                        {
                            IncomingQueue.Stop();
                            OutgoingQueue.Stop();
                            Environment.Exit(0);
                            break;
                        }
                }
            }
            #endregion
        }
    }
}
