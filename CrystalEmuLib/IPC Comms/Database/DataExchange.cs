using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;

namespace CrystalEmuLib.IPC_Comms.Database
{
    [ServiceContract]
    public interface IDataExchange
    {
        [OperationContract]
        Task<string> Execute(DataExchange De);
    }

    [Serializable]
    public class DataExchange : IDataExchange
    {
        public readonly ConcurrentDictionary<string, IniFile> Cache = new ConcurrentDictionary<string, IniFile>(50, 100);
        public readonly ConcurrentDictionary<string, IniFile> Cleanup = new ConcurrentDictionary<string, IniFile>(50, 100);
        public static readonly Timer AutoFlushTimer = new Timer();
        public ExchangeType EType;
        public string Path;
        public string Section;
        public string Key;
        public string Value;
        public string Response;
        public const string AccountDatabasePath = @"Y:\XioEmu\Database\Accounts\";

        public DataExchange()
        {
            AutoFlushTimer.Elapsed += AutoFlushAutoFlushTimerElapsed;
            AutoFlushTimer.Interval = 10 * 1000;
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
                if (Kvp.Value.CacheModified)
                {
                    Kvp.Value.Flush();
                    if (Cleanup.ContainsKey(Kvp.Key))
                        Cleanup.TryRemove(Kvp.Key);
                }
                else
                    Cleanup.TryAdd(Kvp.Key, Kvp.Value);
            }
            foreach (var IniFile in Cleanup)
            {
                Cache.TryRemove(IniFile.Key);
            }
            Cleanup.Clear();
        }

        public async Task<string> Execute(DataExchange De)
        {
            // ReSharper disable once ConvertToExpressionBodyWhenPossible
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
                        if (!File.Exists(AccountDatabasePath + De.Path.Replace(AccountDatabasePath, "")))
                            File.Create(AccountDatabasePath + De.Path.Replace(AccountDatabasePath, "")).Close();

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
        }

        private IniFile CacheLookup(DataExchange De)
        {
            IniFile Writer;
            if (!De.Path.Contains(AccountDatabasePath))
                De.Path = De.Path.Insert(0, AccountDatabasePath);

            if (Cache.ContainsKey(De.Path))
            {
                Core.WriteLine("Cached File Handle: " + De.Path, ConsoleColor.Green);
                Writer = Cache[De.Path];
            }
            else
            {
                Core.WriteLine("New File Handle: " + De.Path, ConsoleColor.Cyan);
                Writer = new IniFile(De.Path);
                Cache.TryAdd(De.Path, Writer);
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