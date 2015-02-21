﻿using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.IPC_Comms
{
    public static class DatabaseConnection
    {
        public static bool ConnectionOpen;

        public static void Open()
        {
            try
            {
                Console.Write("Trying to open Database Connection...");
                var PipeFactory = new ChannelFactory<IDataExchange>(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.0.2/Database"));
                Core.DbServerConnection = PipeFactory.CreateChannel();

                if (!PingDB())
                    return;

                ConnectionOpen = true;
                Core.WriteLine(" [Success]", ConsoleColor.Green);
            }
            catch (Exception)
            {
                ConnectionOpen = false;
                Core.WriteLine(" [Failed]", ConsoleColor.Red);
            }
        }

        public static bool Authenticate(Player Player)
        {
            if (!PingDB())
                return false;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue,Core.AccountDatabasePath + Player.Username + "\\AccountInfo.ini", "Account");

            if (!Directory.Exists(Core.AccountDatabasePath + Player.Username + @"\"))
            {
                LastUID += 1;
                Player.UID = LastUID;
                Directory.CreateDirectory(Core.AccountDatabasePath + Player.Username + @"\");
                IPC.Set(Exchange, "UID", LastUID);
                IPC.Set(Exchange, "Username", Player.Username);
                IPC.Set(Exchange, "Password", Player.Password);
            }

            if (IPC.Get(Exchange, "Username", "") != Player.Username)
                return false;

            if (IPC.Get(Exchange, "LoginType", "ERROR") == "Banned")
                return false;

            var Pass =  IPC.Get(Exchange, "Password", "");

            if (Pass == "")
            {
                Player.UID = LastUID;
                IPC.Set(Exchange, "Password", Player.Password);
                LastUID += 1;
                return (Player.UID != 0);
            }

            if (Pass != Player.Password)
                return false;

            Player.UID = IPC.Get(Exchange, "UID", (uint)0);
            return (Player.UID != 0);
        }

        public static ServerInfo FindServer(Player Player)
        {
            if (!PingDB())
                return null;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath+ Player.Username + "\\PlayerInfo.ini", "Character");

            return new ServerInfo
            {
                IP = IPC.Get(Exchange, "ServerIP", "192.168.0.2"),
                Port = IPC.Get(Exchange, "ServerPort", 5816)
            };
        }

        public static bool PingDB()
        {
            var Ping = new DataExchange(ExchangeType.Ping, "", "");
            try
            {
                Core.DbServerConnection.Execute(Ping);
                return true;
            }
            catch (Exception)
            {
                try
                {
                    var PipeFactory = new ChannelFactory<IDataExchange>(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.0.2/Database"));
                    Core.DbServerConnection = PipeFactory.CreateChannel();
                    Core.DbServerConnection.Execute(Ping);
                    return true;
                }
                catch
                {
                    Core.WriteLine(" [Fail]", ConsoleColor.Red);
                }
            }
            return false;
        }

        private static uint _LastUI;

        private static uint LastUID
        {
            get
            {
                _LastUI = uint.Parse(File.ReadAllText(@"Y:\XioEmu\Database\UIDs.txt"));
                return _LastUI;
            }
            set
            {
                File.WriteAllText(@"Y:\XioEmu\Database\UIDs.txt", Convert.ToString(value));
                _LastUI = value;
            }
        }
    }
}