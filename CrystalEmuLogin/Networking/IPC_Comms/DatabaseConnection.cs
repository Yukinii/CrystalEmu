using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLogin.PlayerFunctions;
using CrystalEmuLogin.World;

namespace CrystalEmuLogin.Networking.IPC_Comms
{
    public static class DatabaseConnection
    {
        public static bool ConnectionOpen;

        public static async Task<bool> Open()
        {
            try
            {
                Console.Write("Trying to open Database Connection...");
                var PipeFactory = new ChannelFactory<IDataExchange>(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.0.4/Database"));
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

        public static async Task<bool> Authenticate(Player Player)
        {
            if (!await PingDB())
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

            if (await IPC.Get(Exchange, "Username", "") != Player.Username)
                return false;

            if (await IPC.Get(Exchange, "LoginType", "ERROR") == "Banned")
                return false;

            var Pass =  await IPC.Get(Exchange, "Password", "");

            if (Pass == "")
            {
                Player.UID = LastUID;
                IPC.Set(Exchange, "Password", Player.Password);
                LastUID += 1;
                return (Player.UID != 0);
            }

            if (Pass != Player.Password)
                return false;

            Player.UID = await IPC.Get(Exchange, "UID", (uint)0);
            return (Player.UID != 0);
        }
        public static async Task<bool> FindSpawnPoint(Player Player)
        {
            if (!await PingDB())
                return false;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath + Player.Username + "\\PlayerInfo.ini", "Account");

            Player.Location = new Vector3(Player, 61, 109, 1010)
            {
                X = await IPC.Get(Exchange, "X", 096),
                Y = await IPC.Get(Exchange, "Y", 181),
                Z = await IPC.Get(Exchange, "Z", 1040)
            };

            return true;
        }

        public static async Task<bool> LoadCharacter(Player Player)
        {
            if (!await PingDB())
                return false;

            if (string.IsNullOrEmpty(Player.Username))
            {
                var TempExchange = new DataExchange(ExchangeType.GetUsernameByUID, Player.UID.ToString(), "");
                Player.Username = await IPC.Get(TempExchange, Player.UID.ToString(), "0");
            }

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath + Player.Username+"\\"+Player.Name +"\\PlayerInfo.ini", "Character");
            Player.Name = await IPC.Get(Exchange, "Name", "SELECTOR");

            Player.InitializeDatabaseConnection();

            Player.Spouse = await IPC.Get(Exchange, "Spouse", "CrystalEmu");
            Player.Model = await IPC.Get(Exchange, "Model", 355  );
            Player.Hair = await IPC.Get(Exchange, "Hair", 310);
            Player.Class = (byte)await IPC.Get(Exchange, "Class", 10);
            Player.Level = (byte)await IPC.Get(Exchange, "Level", 1);
            Player.Direction = 7;
            return true;
        }
        public static async Task<ServerInfo> FindServer(Player Player)
        {
            if (!await PingDB())
                return null;

            var Exchange = new DataExchange(ExchangeType.LoadAccountValue, Core.AccountDatabasePath+ Player.Username + "\\PlayerInfo.ini", "Character");

            return new ServerInfo
            {
                IP =await IPC.Get(Exchange, "ServerIP", "192.168.0.4"),
                Port =await IPC.Get(Exchange, "ServerPort", 5816)
            };
        }

        public static async Task<bool> PingDB()
        {
            var Ping = new DataExchange(ExchangeType.Ping, "", "");
            try
            {
                await Core.DbServerConnection.Execute(Ping);
                return true;
            }
            catch
            {
                await Open();
                return await Core.DbServerConnection.Execute(Ping) != "";
            }
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