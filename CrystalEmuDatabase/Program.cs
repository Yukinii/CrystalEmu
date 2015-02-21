using System;
using System.ServiceModel;
using CrystalEmuLib.IPC_Comms.Database;

namespace CrystalEmuDatabase
{
    internal class Program
    {
        private static ServiceHost _DataExchangeHost;

        private static void Main()
        {
            Console.Title = "CrystalEmu Database Server!";
            CreateService();
            Console.WriteLine("Running & Ready!");
            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void DataExchangeHostClosed(object Sender, EventArgs E) => CreateService();
        private static void DataExchangeHostFaulted(object Sender, EventArgs E) => CreateService();

        private static void CreateService()
        {
            var DataExchangePipe = new NetTcpBinding {ReceiveTimeout = TimeSpan.MaxValue};
            _DataExchangeHost = new ServiceHost(typeof(DataExchange), new Uri("net.tcp://192.168.0.2"));
            _DataExchangeHost.AddServiceEndpoint(typeof(IDataExchange), DataExchangePipe, "Database");
            _DataExchangeHost.Faulted += DataExchangeHostFaulted;
            _DataExchangeHost.Closed += DataExchangeHostClosed;
            _DataExchangeHost.Open();
        }
    }
}