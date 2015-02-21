using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuLib.IPC_Comms.Shared
{
    public static class IPC
    {
        private static readonly Thread ConsumerThread = new Thread(Loop);
        private static readonly AutoResetEvent ResetEvent = new AutoResetEvent(false);
        public static readonly ConcurrentQueue<DataExchange> PendingOps = new ConcurrentQueue<DataExchange>();

        public static Task<bool> Set(DataExchange Ex, string Key, string Value)
        {
            var Tcs = new TaskCompletionSource<bool>();
            if (Ex == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;
            Enqueue(De);
            Tcs.SetResult(true);

            return Tcs.Task;
        }

        public static Task<bool> Set(DataExchange Ex, string Key, bool Value)
        {
            var Tcs = new TaskCompletionSource<bool>();
            if (Ex == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = "" + (Value ? 1 : 0);
            De.EType = ExchangeType.SaveAccountValue;
            Enqueue(De);
            Tcs.SetResult(true);

            return Tcs.Task;
        }

        public static Task<bool> Set(DataExchange Ex, string Key, ulong Value)
        {
            var Tcs = new TaskCompletionSource<bool>();
            if (Ex == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;
            Enqueue(De);
            Tcs.SetResult(true);

            return Tcs.Task;
        }

        public static Task<bool> Set(DataExchange Ex, string Key, int Value)
        {
            var Tcs = new TaskCompletionSource<bool>();
            if (Ex == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;
            Enqueue(De);
            Tcs.SetResult(true);
            return Tcs.Task;
        }

        public static async Task<string> Get(DataExchange Ex, string Key, string Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();
            Clone.Key = Key;
            var Result = await Core.DbServerConnection.Execute(Clone);
            return Result != "" ? Result : Default;
        }

        public static async Task<ulong> Get(DataExchange Ex, string Key, ulong Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();
            var Result = await Core.DbServerConnection.Execute(Clone);
            ulong Parsed;
            return ulong.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<uint> Get(DataExchange Ex, string Key, uint Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();

            var Result = await Core.DbServerConnection.Execute(Clone);
            uint Parsed;
            return uint.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<ushort> Get(DataExchange Ex, string Key, ushort Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();
            var Result = await Core.DbServerConnection.Execute(Clone);
            ushort Parsed;
            return ushort.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static void Initialize()
        {
            if (!ConsumerThread.IsAlive)
                ConsumerThread.Start();
        }

        public static void Enqueue(DataExchange De)
        {
            PendingOps.Enqueue(De.Clone());
            ResetEvent.Set();
        }

        private static async void Loop()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var Ping = new DataExchange(ExchangeType.Ping, "", "");
            while (true)
            {
                ResetEvent.WaitOne(100);
                if (PendingOps.Count == 0)
                    continue;

                while (PendingOps.Count > 0)
                {
                    try
                    {
                        await Core.DbServerConnection.Execute(Ping);
                        DataExchange Ex;
                        if (PendingOps.TryDequeue(out Ex))
                            await Core.DbServerConnection.Execute(Ex);
                    }
                    catch (Exception)
                    {
                        Core.Write("Trying to restore Database Connection...", ConsoleColor.Red);
                        while (!RestoreDatabaseConnection())
                        {
                            await Task.Delay(100);
                        }
                        break;
                    }
                }
            }
        }

        private static bool RestoreDatabaseConnection()
        {
            Core.Write(".", ConsoleColor.Red);
            try
            {
                var PipeFactory = new ChannelFactory<IDataExchange>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/DataExchange"));
                Core.DbServerConnection = PipeFactory.CreateChannel();
                Core.WriteLine("[Success]", ConsoleColor.Green);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}