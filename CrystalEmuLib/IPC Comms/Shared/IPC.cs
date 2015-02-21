using System.Globalization;
using System.Threading.Tasks;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuLib.IPC_Comms.Shared
{
    public static class IPC
    {
        public static bool Set(DataExchange De, string Key, string Value)
        {
            if (De==null || string.IsNullOrEmpty(Key))
            {
                return false;
            }
            
            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            Core.DbServerConnection.Execute(De);
            return true;
        }

        public static bool Set(DataExchange De, string Key, bool Value)
        {
            if (De == null || string.IsNullOrEmpty(Key))
            {
                return false;
            }

            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            Core.DbServerConnection.Execute(De);
            return true;
        }

        public static bool Set(DataExchange De, string Key, ulong Value)
        {
            if (De == null || string.IsNullOrEmpty(Key))
            {
                return false;
            }

            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            Core.DbServerConnection.Execute(De);
            return true;
        }

        public static bool Set(DataExchange De, string Key, int Value)
        {
            if (De == null || string.IsNullOrEmpty(Key))
            {
                return false;
            }

            De.Key = Key;
            De.Value = Value.ToString(CultureInfo.InvariantCulture);
            De.EType = ExchangeType.SaveAccountValue;

            Core.DbServerConnection.Execute(De);
            return true;
        }

        public static async Task<string> Get(DataExchange Clone, string Key, string Default)
        {
            if (Clone == null || string.IsNullOrEmpty(Key))
                return Default;
            
            Clone.Key = Key;
            var Result = await Core.DbServerConnection.Execute(Clone);
            return Result != "" ? Result : Default;
        }

        public static async Task<ulong> Get(DataExchange Clone, string Key, ulong Default)
        {
            if (Clone == null || string.IsNullOrEmpty(Key))
                return Default;

            Clone.Key = Key;
            var Result = await Core.DbServerConnection.Execute(Clone);
            ulong Parsed;
            return ulong.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<uint> Get(DataExchange Clone, string Key, uint Default)
        {
            if (Clone == null || string.IsNullOrEmpty(Key))
                return Default;

            Clone.Key = Key;
            var Result = await Core.DbServerConnection.Execute(Clone);
            uint Parsed;
            return uint.TryParse(Result, out Parsed) ? Parsed : Default;
        }

        public static async Task<ushort> Get(DataExchange Clone, string Key, ushort Default)
        {
            if (Clone == null || string.IsNullOrEmpty(Key))
                return Default;

            Clone.Key = Key;
            var Result = await Core.DbServerConnection.Execute(Clone);
            ushort Parsed;
            return ushort.TryParse(Result, out Parsed) ? Parsed : Default;
        }
    }
}