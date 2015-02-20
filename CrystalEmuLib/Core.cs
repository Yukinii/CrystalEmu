using System;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuLib
{
    public static class Core
    {
        public static IDataExchange DbServerConnection;
        public const string AccountDatabasePath = @"Y:\XioEmu\Database\Accounts\";

        public static void WriteLine(object Text, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine(Text);
            Console.ResetColor();
        }
    }
}
