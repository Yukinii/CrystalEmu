namespace CrystalEmuLib
{
    using System;
    using IPC_Comms.Database;

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
