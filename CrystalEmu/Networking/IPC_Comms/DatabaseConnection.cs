using System;
using System.ServiceModel;
using System.Threading.Tasks;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;

namespace CrystalEmu.Networking.IPC_Comms
{
    public static class DatabaseConnection
    {
        public static async Task<bool> Open()
        {
            try
            {
                Console.Write("Trying to open Database Connection...");
                var PipeFactory = new ChannelFactory<IDataExchange>(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.0.2/Database"));
                Core.DbServerConnection = PipeFactory.CreateChannel();

                if (!await PingDB())
                    return false;

                Core.WriteLine(" [Success]", ConsoleColor.Green);
                return true;
            }
            catch
            {
                Core.WriteLine(" [Failed]", ConsoleColor.Red);
                return false;
            }
        }

        public static async Task<bool> FindSpawnPoint(Player Player)
        {
            if (!await PingDB())
                return false;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath+ Player.Username + "\\PlayerInfo.ini", "Account");
            Player.X = IPC.Get(Exchange, "TransferX", 61);
            Player.Y = IPC.Get(Exchange, "TransferY", 109);
            Player.Z = IPC.Get(Exchange, "TransferMap", 1010);

            return true;
        }

        public static async Task<bool> LoadCharacer(Player Player)
        {
            if (!await PingDB())
                return false;
            var TempExchange = new DataExchange(ExchangeType.GetUsernameByUID, Player.UID.ToString(), "");
            Player.Username = IPC.Get(TempExchange, Player.UID.ToString(), "0");

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath + Player.Username + "\\PlayerInfo.ini", "Character");
            Player.Name = IPC.Get(Exchange, "Name", "ERROR");

            Player.InitializeDatabaseConnection();

            Player.Spouse = IPC.Get(Exchange, "Spouse", "None");
            Player.Model = IPC.Get(Exchange, "Model", 1003);
            Player.Hair = IPC.Get(Exchange, "Hair", 1);
            Player.Class = (byte)IPC.Get(Exchange, "Class", 10);
            Player.Level = (byte)IPC.Get(Exchange, "Level", 1);
            Player.Cps = IPC.Get(Exchange, "Cps", 0);
            Player.Money = IPC.Get(Exchange, "Money", 1000);
            Player.CurrentHP = IPC.Get(Exchange, "CurrentHP", 1);
            Player.CurrentMP = IPC.Get(Exchange, "CurrentMP", 0);

            return true;
        }

        public static async Task<ServerInfo> FindServer(Player Player)
        {
            if (!await PingDB())
                return null;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath + Player.Username + "\\PlayerInfo.ini", "Character");

            return new ServerInfo
            {
                IP = IPC.Get(Exchange, "ServerIP", "192.168.0.2"),
                Port = IPC.Get(Exchange, "ServerPort", 5816)
            };
        }

        public static async Task<bool> PingDB()
        {
            var Ping = new DataExchange(ExchangeType.Ping, "", "");
            try
            {
                Core.DbServerConnection.Execute(Ping);
                return true;
            }
            catch
            {
                await Open();
                return Core.DbServerConnection.Execute(Ping) != "";
            }
        }
    }
}