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
    public static class DataExchangeOverLord
    {
        internal static volatile bool Initialized = false;
        internal static readonly ConcurrentDictionary<string, IniFile> Cache = new ConcurrentDictionary<string, IniFile>(50, 100);
        internal static readonly ConcurrentDictionary<string, IniFile> Cleanup = new ConcurrentDictionary<string, IniFile>(50, 100);
        internal static readonly Timer AutoFlushTimer = new Timer();
        public static void Initialize()
        {
            AutoFlushTimer.Elapsed += AutoFlushAutoFlushTimerElapsed;
            AutoFlushTimer.Interval = 10 * 1000;
            AutoFlushTimer.Start();
            Initialized = true;
        }

        internal static IniFile CacheLookup(DataExchange De)
        {
            if (!Initialized)
                Initialize();

            IniFile Writer;
            if (Cache.TryGetValue(De.Path, out Writer))
            {
                Core.Write("#", ConsoleColor.Green);
            }
            else
            {
                Core.Write("#", ConsoleColor.Red);
                Writer = new IniFile(De.Path);
                Cache.TryAdd(De.Path, Writer);
            }
            return Writer;
        }
        internal static Task<string> Execute(DataExchange De)
        {
            var Tcs = new TaskCompletionSource<string>();

            switch (De.EType)
            {
                case ExchangeType.SaveLocation:
                {
                    var Writer = CacheLookup(De);
                    Writer.Write(De.Section, "X", De.Key);
                    Writer.Write(De.Section, "Y", De.Value);
                    Tcs.SetResult(null);
                    break;
                }
                case ExchangeType.LoadAccountValue:
                {
                    var Reader = CacheLookup(De);
                    De.Response = Reader.ReadString(De.Section, De.Key, "");
                    Tcs.SetResult(De.Response);
                    break;
                }
                case ExchangeType.SaveAccountValue:
                {
                    var Writer = CacheLookup(De);
                    Writer.Write(De.Section, De.Key, De.Value);
                    Tcs.SetResult(null);
                    break;
                }
                case ExchangeType.GetUsernameByUID:
                {
                    Tcs.SetResult(Directory.GetFiles(Core.AccountDatabasePath, "AccountInfo.ini", SearchOption.AllDirectories).Select(PlayerInfo => new IniFile(PlayerInfo)).Where(Reader => Reader?.ReadString("Account", "UID", "") == De.Path).Select(Reader => De.Response = Reader?.ReadString("Account", "Username", "0")).FirstOrDefault());break;
                }
                default:
                {
                    Tcs.SetResult(null);
                    break;
                }
            }
            return Tcs.Task;
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
        Task<string> Execute(DataExchange De);
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
        public async Task<string> Execute(DataExchange De) => await DataExchangeOverLord.Execute(De);
    }
}