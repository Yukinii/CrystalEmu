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
        Ping = 255
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
        public readonly ConcurrentDictionary<string, IniFile> Cache = new ConcurrentDictionary<string, IniFile>(50, 100);
        public static readonly Timer AutoFlushTimer = new Timer();
        public ExchangeType EType;
        public string Path;
        public string Section;
        public string Key;
        public string Value;
        public string Response;
        public DataExchange()
        {
            if (AutoFlushTimer != null)
                return;
            AutoFlushTimer.Elapsed += AutoFlushAutoFlushTimerElapsed;
            AutoFlushTimer.Interval = 1500;
            AutoFlushTimer.Start();
        }
        public DataExchange(ExchangeType EType, string Path, string Section)
        {
            this.EType = EType;
            this.Path = Path;
            this.Section = Section;
        }

        private void AutoFlushAutoFlushTimerElapsed(object Sender, ElapsedEventArgs E)
        {
            if (Cache == null)
                return;
            foreach (var Kvp in Cache)
            {
                Kvp.Value.Flush();
            }
        }

        // ReSharper disable once PossibleNullReferenceException
        public async Task<string> Execute(DataExchange De) => await Task.Run(() =>
        {
            if (De?.Key == null || De.Path == null)
                return "";

            switch (De.EType)
            {
                case ExchangeType.SaveLocation:
                {
                    if (!File.Exists(AccountDatabasePath + De.Path))
                        return "fail";

                    var Writer = CacheLookup(De);
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

                    var Reader = CacheLookup(De);
                    De.Response = Reader.ReadString(De.Section, De.Key, "");
                    De.EType = ExchangeType.Response;
                    Console.WriteLine(De.Key + " -> " + De.Response + " on " + De.Path);

                    return De.Response;
                }
                case ExchangeType.SaveAccountValue:
                {
                    if (!File.Exists(AccountDatabasePath + De.Path))
                        File.Create(AccountDatabasePath + De.Path).Close();

                    var Writer = CacheLookup(De);
                    Writer.Write(De.Section, De.Key, De.Value);
                    Console.WriteLine(De.Key + " -> " + De.Value + " on " + De.Path);

                    return "done";
                }
                case ExchangeType.GetUsernameByUID:
                {
                    var UID = De.Path;
                    foreach (var Reader in Directory.GetFiles(AccountDatabasePath, "AccountInfo.ini", SearchOption.AllDirectories).Select(PlayerInfo => new IniFile(PlayerInfo)).Where(Reader => Reader?.ReadString("Account", "UID", "") == UID))
                    {
                        return De.Response = Reader?.ReadString("Account", "Username", "0");
                    }
                    return "0";
                }
            }
            return @"INVALID::ETYPE";
        });

        private IniFile CacheLookup(DataExchange De)
        {
            IniFile Writer;
            if (Cache.ContainsKey(AccountDatabasePath + De.Path))
            {
                Core.WriteLine("Cached File Handle: " + De.Path, ConsoleColor.Green);
                Writer = Cache[AccountDatabasePath + De.Path];
            }
            else
            {
                Core.WriteLine("New File Handle: " + De.Path, ConsoleColor.Cyan);
                Writer = new IniFile(AccountDatabasePath + De.Path);
                Cache.TryAdd(AccountDatabasePath + De.Path, Writer);
            }
            return Writer;
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
