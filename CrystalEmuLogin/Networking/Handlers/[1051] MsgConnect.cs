﻿using System;
using CrystalEmuLib;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Sockets;
using CrystalEmuLogin.Networking.IPC_Comms;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Handlers
{
    public static class MsgConnect
    {
        public static async void Handle(Player Player, byte[] Packet)
        {
            Player.Username = Packet.StringFrom(4, 16);
            Player.Password = Rc5.Decrypt(Packet.ArrayFrom(20, 16));
            var Server = Packet.StringFrom(36, 16);

            Console.WriteLine("{0} : {1} -> {2}", Player.Username, Player.Password, Server);

            if (await DatabaseConnection.Authenticate(Player))
            {
                Player.ServerInfo = await DatabaseConnection.FindServer(Player);
                Player.Send(CoPacket.MsgTransfer(Player));
                return;
            }
            Player.Disconnect();
            Core.WriteLine("Invalid Password", ConsoleColor.Red);
        }
    }
}