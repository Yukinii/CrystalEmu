using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;

namespace CrystalEmuLib.IPC_Comms.Database
{
    public static class DataExchangeOverLord
    {
        internal static readonly ConcurrentDictionary<string, IniFile> Cache = new ConcurrentDictionary<string, IniFile>(50, 100);
        internal static readonly ConcurrentDictionary<string, IniFile> Cleanup = new ConcurrentDictionary<string, IniFile>(50, 100);
        internal static readonly Timer AutoFlushTimer = new Timer();
        public static void Initialize()
        {
            AutoFlushTimer.Elapsed += AutoFlushAutoFlushTimerElapsed;
            AutoFlushTimer.Interval = 10 * 1000;
            AutoFlushTimer.Start();
        }

        internal static IniFile CacheLookup(DataExchange De)
        {
            IniFile Writer;
            //if (!De.Path.Contains(Core.AccountDatabasePath))
            //    De.Path = De.Path.Insert(0, Core.AccountDatabasePath);

            if (Cache.ContainsKey(De.Path))
            {
                Core.Write("#", ConsoleColor.Green);
                Writer = Cache[De.Path];
            }
            else
            {
                Core.Write("#", ConsoleColor.Cyan);
                Writer = new IniFile(De.Path);
                Cache.TryAdd(De.Path, Writer);
            }
            return Writer;
        }
        internal static string Execute(DataExchange De)
        {
            if (De.Key == null || De.Path == null)
                return null;

            switch (De.EType)
            {
                case ExchangeType.SaveLocation:
                    {
                        var Writer = CacheLookup(De);
                        Writer.Write(De.Section, "X", De.Key);
                        Writer.Write(De.Section, "Y", De.Value);
                        return null;
                    }
                case ExchangeType.LoadAccountValue:
                    {
                        var Reader = CacheLookup(De);
                        De.Response = Reader.ReadString(De.Section, De.Key, "");
                        De.EType = ExchangeType.Response;
                        return De.Response;
                    }
                case ExchangeType.SaveAccountValue:
                    {
                        var Writer = CacheLookup(De);
                        Writer.Write(De.Section, De.Key, De.Value);
                        return null;
                    }
                case ExchangeType.GetUsernameByUID:
                    {
                        return Directory.GetFiles(Core.AccountDatabasePath, "AccountInfo.ini", SearchOption.AllDirectories).Select(PlayerInfo => new IniFile(PlayerInfo)).Where(Reader => Reader?.ReadString("Account", "UID", "") == De.Path).Select(Reader => De.Response = Reader?.ReadString("Account", "Username", "0")).FirstOrDefault();
                    }
            }
            return null;
        }
        internal static void AutoFlushAutoFlushTimerElapsed(object Sender, ElapsedEventArgs E)
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
    }

    [ServiceContract]
    public interface IDataExchange
    {
        [OperationContract]
        string Execute(DataExchange De);
    }

    [Serializable]
    public class DataExchange : IDataExchange
    {
        public ExchangeType EType;
        public string Path;
        public string Key;
        public string Value;
        public string Response;
        public readonly string Section;

        public DataExchange()
        {
            
        }
        public DataExchange(ExchangeType EType, string Path, string Section)
        {
            this.EType = EType;
            this.Path = Path;
            this.Section = Section;
        }

        public string Execute(DataExchange De) => DataExchangeOverLord.Execute(De);
    }
}