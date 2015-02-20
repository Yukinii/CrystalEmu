using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;

namespace CrystalEmuLib.IPC_Comms.Database
{
    public enum ExchangeType
    {
        SaveAccountValue = 0,
        LoadAccountValue = 1,
        LoadCharacterValue = 2,
        SaveCharacterValue = 3,

        SaveLocation = 4,

        GetUsernameByUID = 5,
        Response = 6,
        Ping = 255,
    }
    [ServiceContract]
    public interface IDataExchange
    {
        [OperationContract]
        Task<string> Execute(DataExchange De);
    }
    [Serializable]
    public class DataExchange : IDataExchange
    {
        public const string AccountDatabasePath = @"Y:\XioEmu\Database\Accounts\";
        private readonly ConcurrentDictionary<string, IniFile> _Files = new ConcurrentDictionary<string, IniFile>(50, 100);
        private static Timer _Timer;
        public ExchangeType EType;
        public string Path;
        public string Section;
        public string Key;
        public string Value;
        public string Response;
        public DataExchange()
        {
            if (_Timer != null)
                return;

            _Timer = new Timer();
            _Timer.Elapsed += T_Elapsed;
            _Timer.Interval = 1500;
            _Timer.Start();
        }

        private void T_Elapsed(object Sender, ElapsedEventArgs E)
        {
            foreach (var Kvp in _Files)
            {
                Kvp.Value.Flush();
            }
        }

        public async Task<string> Execute(DataExchange De)
        {
            return await Task.Run(() =>
            {
                if (De?.Key == null || De.Path == null)
                    return "";

                switch (De.EType)
                {
                    case ExchangeType.SaveLocation:
                    {
                        if (!File.Exists(AccountDatabasePath + De.Path))
                            return "fail";
                        IniFile Writer;
                        if (_Files.ContainsKey(AccountDatabasePath + De.Path))
                        {
                            Writer = _Files[AccountDatabasePath + De.Path];
                        }
                        else
                        {
                            Console.WriteLine("New File Handle: " + De.Path);
                            Writer = new IniFile(AccountDatabasePath + De.Path);
                            _Files.TryAdd(AccountDatabasePath + De.Path, Writer);
                        }
                        Writer.Write(De.Section, "X", De.Key);
                        Writer.Write(De.Section, "Y", De.Value);
                        Console.WriteLine("X -> " + De.Key + " on " + De.Path);
                        Console.WriteLine("Y -> " + De.Value + " on " + De.Path);
                        return "done";
                    }
                    case ExchangeType.LoadAccountValue:
                    {
                        if (!File.Exists(AccountDatabasePath + De.Path))
                            return "";

                        IniFile Reader;
                        if (_Files.ContainsKey(AccountDatabasePath + De.Path))
                        {
                            Reader = _Files[AccountDatabasePath + De.Path];
                        }
                        else
                        {
                            Console.WriteLine("New File Handle: " + De.Path);
                            Reader = new IniFile(AccountDatabasePath + De.Path);
                            _Files.TryAdd(AccountDatabasePath + De.Path, Reader);
                        }
                        De.Response = Reader.ReadString(De.Section, De.Key,"");
                        De.EType = ExchangeType.Response;
                        Console.WriteLine(De.Key + " -> " + De.Response + " on " + De.Path);
                        return De.Response;
                    }
                    case ExchangeType.SaveAccountValue:
                    {
                        if (!File.Exists(AccountDatabasePath + De.Path))
                            File.Create(AccountDatabasePath + De.Path).Close();
                        IniFile Writer;
                        if (_Files.ContainsKey(AccountDatabasePath + De.Path))
                        {
                            Writer = _Files[AccountDatabasePath + De.Path];
                        }
                        else
                        {
                            Console.WriteLine("New File Handle: " + De.Path);
                            Writer = new IniFile(AccountDatabasePath + De.Path);
                            _Files.TryAdd(AccountDatabasePath + De.Path, Writer);
                        }
                        Writer.Write(De.Section, De.Key, De.Value);
                        Console.WriteLine(De.Key + " -> " + De.Value + " on " + De.Path);
                        return "done";
                    }
                    case ExchangeType.GetUsernameByUID:
                    {
                        var UID = De.Path;
                        foreach (var Reader in Directory.GetFiles(AccountDatabasePath, "AccountInfo.ini", SearchOption.AllDirectories).Select(PlayerInfo => new IniFile(PlayerInfo)).Where(Reader => Reader.ReadString("Account", "UID", "") == UID))
                        {
                            return De.Response = Reader.ReadString("Account", "Username", "0");
                        }
                        return "0";
                    }
                }
                return "INVALID::ETYPE";
            });
        }
        public DataExchange(ExchangeType EType, string Path, string Section)
        {
            this.EType = EType;
            this.Path = Path;
            this.Section = Section;
        }



        public DataExchange Clone()
        {
            var Copy = new DataExchange
            {
                EType = EType,
                Key = Key,
                Path = Path,
                Response = Response,
                Section = Section,
                Value = Value
            };
            return Copy;
        }
    }
}
