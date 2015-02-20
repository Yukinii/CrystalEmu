using System;
using System.ServiceModel;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuDatabase
{
    class Program
    {
        public static ServiceHost DataExchangeHost;

        static void Main()
        {
            Console.Title = "CrystalEmu Database Server!";
            CreateService();
            Console.WriteLine("Running & Ready!");
            while (true)
            {
                Console.ReadLine();
            }
        }
        private static void DataExchangeHostClosed(object Sender, EventArgs E)
        {
            CreateService();
        }

        private static void DataExchangeHostFaulted(object Sender, EventArgs E)
        {
            CreateService();
        }
        private static void CreateService()
        {
            var DataExchangePipe = new NetTcpBinding() { ReceiveTimeout = TimeSpan.MaxValue };
            DataExchangeHost = new ServiceHost(typeof(DataExchange), new Uri("net.tcp://192.168.0.2"));
            DataExchangeHost.AddServiceEndpoint(typeof(IDataExchange), DataExchangePipe, "Database");
            DataExchangeHost.Faulted += DataExchangeHostFaulted;
            DataExchangeHost.Closed += DataExchangeHostClosed;
            DataExchangeHost.Open();
        }
    }
}
