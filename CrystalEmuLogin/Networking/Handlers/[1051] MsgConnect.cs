namespace CrystalEmuLogin.Networking.Handlers
{
    using System;
    using System.Threading.Tasks;
    using CrystalEmuLib;
    using CrystalEmuLib.Extensions;
    using CrystalEmuLib.Sockets;
    using IPC_Comms;
    using Packets;
    using PlayerFunctions;

    public static class MsgConnect
    {
        public static async Task Handle(Player Player, byte[] Packet)
        {
            Player.Username = Packet.StringFrom(4, 16);
            Player.Password = Rc5.Decrypt(Packet.ArrayFrom(20, 16));
            var Server = Packet.StringFrom(36, 16);

            Console.WriteLine("{0} : {1} -> {2}", Player.Username, Player.Password, Server);

            if (await DatabaseConnection.Authenticate(Player))
            {
                Player.ServerInfo =await DatabaseConnection.FindServer(Player);
                Player.Send(CoPacket.MsgTransfer(Player));
                return;
            }
            Player.Disconnect();
            Core.WriteLine("Invalid Password",ConsoleColor.Red);
        }
    }
}
