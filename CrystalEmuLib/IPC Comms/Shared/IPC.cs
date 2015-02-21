using System.Globalization;
using System.Threading.Tasks;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuLib.IPC_Comms.Shared
{
    public static class IPC
    {
        public static async Task<bool> Set(DataExchange Ex, string Key, string Value)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return false;

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            await Core.DbServerConnection.Execute(De);
            return true;
        }

        public static async Task<bool> Set(DataExchange Ex, string Key, bool Value)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return false;

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            await Core.DbServerConnection.Execute(De);
            return true;
        }

        public static async Task<bool> Set(DataExchange Ex, string Key, ulong Value)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return false;

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            await Core.DbServerConnection.Execute(De);
            return true;
        }

        public static async Task<bool> Set(DataExchange Ex, string Key, int Value)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return false;

            var De = Ex.Clone();
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            await Core.DbServerConnection.Execute(De);
            return true;
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
            Clone.Key = Key;

            var Result = await Core.DbServerConnection.Execute(Clone);
            ulong Parsed;
            return ulong.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<uint> Get(DataExchange Ex, string Key, uint Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();
            Clone.Key = Key;

            var Result = await Core.DbServerConnection.Execute(Clone);
            uint Parsed;
            return uint.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<ushort> Get(DataExchange Ex, string Key, ushort Default)
        {
            if (Ex == null || string.IsNullOrEmpty(Key))
                return Default;

            var Clone = Ex.Clone();
            Clone.Key = Key;

            var Result = await Core.DbServerConnection.Execute(Clone);
            ushort Parsed;
            return ushort.TryParse(Result, out Parsed) ? Parsed : Default;
        }
    }
}