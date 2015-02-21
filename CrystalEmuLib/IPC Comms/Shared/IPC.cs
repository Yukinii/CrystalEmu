namespace CrystalEmuLib.IPC_Comms.Shared
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Database;

    public static class IPC
    {
        private static readonly Thread ConsumerThread = new Thread(Loop);
        private static readonly AutoResetEvent ResetEvent = new AutoResetEvent(false);
        public static readonly ConcurrentQueue<DataExchange> PendingOps = new ConcurrentQueue<DataExchange>();

        public static Task<bool> Set(DataExchange Ex, string Key, string Value)
        {
            var Tcs = new TaskCompletionSource<bool>();
            var De = Ex?.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }
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
            var De = Ex?.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }
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
            var De = Ex?.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }
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
            var De = Ex?.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
            {
                Tcs.SetResult(false);
                return Tcs.Task;
            }
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;
            Enqueue(De);
            Tcs.SetResult(true);
            return Tcs.Task;
        }
        public static async Task<string> Get(DataExchange Ex, string Key, string Default)
        {
            if (Ex == null)
                return Default;
            var De = Ex.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
                return Default;
            var Clone = De.Clone();
            Clone.Key = Key;
            try
            {
                var Result = await Core.DbServerConnection.Execute(Clone);
                return Result != "" ? Result : Default;
            }
            catch (Exception)
            {
                return Default;
            }
        }
        public static async Task<ulong> Get(DataExchange Ex, string Key, ulong Default)
        {
            if (Ex == null)
                return Default;
            var De = Ex.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = De.Clone();
            Clone.Key = Key;
            try
            {
                var Result = await Core.DbServerConnection.Execute(Clone);
                ulong Parsed;
                return ulong.TryParse(Result, out Parsed) ? Parsed : Default;
            }
            catch (Exception)
            {
                return Default;
            }
        }
        public static async Task<uint> Get(DataExchange Ex, string Key, uint Default)
        {
            if (Ex == null)
                return Default;
            var De = Ex.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
                return Default;
            var Clone = De.Clone();
            Clone.Key = Key;
            try
            {
                var Result = await Core.DbServerConnection.Execute(Clone);
                uint Parsed;
                return uint.TryParse(Result, out Parsed) ? Parsed : Default;
            }
            catch (Exception)
            {
                return Default;
            }
        }
        public static async Task<ushort> Get(DataExchange Ex, string Key, ushort Default)
        {
            if (Ex == null)
                return Default;
            var De = Ex.Clone();
            if (De == null || string.IsNullOrEmpty(Key))
                return Default;
            var Clone = De.Clone();
            Clone.Key = Key;
            try
            {
                var Result = await Core.DbServerConnection.Execute(Clone);
                ushort Parsed;
                return ushort.TryParse(Result, out Parsed) ? Parsed : Default;
            }
            catch (Exception)
            {
                return Default;
            }
        }

        public static void Initialize()
        {
            if (!ConsumerThread.IsAlive)
            {
                ConsumerThread.Start();
            }
        }

        public static void Enqueue(DataExchange De)
        {
            PendingOps.Enqueue(De.Clone());
            ResetEvent.Set();
        }
        private static void Loop()
        {
            var Ping = new DataExchange(ExchangeType.Ping, "", "");
            while (true)
            {
                ResetEvent.WaitOne();
                if (PendingOps.Count == 0)
                    continue;

                while (PendingOps.Count > 0)
                {
                    try
                    {
                        Core.DbServerConnection.Execute(Ping);
                        DataExchange Ex;
                        if (PendingOps.TryDequeue(out Ex))
                            Core.DbServerConnection.Execute(Ex);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("DB Server Offline! Pending Writes: " + PendingOps.Count);
                        try
                        {
                            var PipeFactory = new ChannelFactory<IDataExchange>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/DataExchange"));
                            Core.DbServerConnection = PipeFactory.CreateChannel();
                        }
                        catch
                        {
                            Console.WriteLine("Couln't restore the DB Connection.");
                        }
                        break;
                    }
                }
            }
        }
    }
}
