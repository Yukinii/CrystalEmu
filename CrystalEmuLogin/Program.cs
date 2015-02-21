using System;
using CrystalEmuLib;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLogin.Networking.IPC_Comms;
using CrystalEmuLogin.Networking.Queue;
using CrystalEmuLogin.Networking.Sockets;

namespace CrystalEmuLogin
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "CrystalEmu - Login Server";
            while (!DatabaseConnection.ConnectionOpen)
            {
                DatabaseConnection.Open();
            }
            DataExchangeOverLord.Initialize();
            IncomingQueue.Start();
            OutgoingQueue.Start();
            LoginSocket.Open();
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