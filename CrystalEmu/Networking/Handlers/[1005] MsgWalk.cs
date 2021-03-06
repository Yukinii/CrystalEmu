﻿using System;
using CrystalEmu.Networking.Packets;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;

namespace CrystalEmu.Networking.Handlers
{
    public static class MsgWalk
    {
        public static void Handle(Player Player, byte[] Packet)
        {
            var UID = Packet.ToUInt(4);
            var Direction = (byte)(Packet[8] % 8);
            var Running = Packet[9];
            int[] Xadd = {0, - 1, - 1, - 1, 0, 1, 1, 1};
            int[] Yadd = {1, 1, 0, - 1, - 1, - 1, 0, 1};


            if (Player.UID != UID)
                Player.Disconnect();

            if (Running == 0 && (Player.LastWalk + 500 > Environment.TickCount) || Running == 0 && (Player.LastWalk + 400 > Environment.TickCount))
                Player.Disconnect();

            Player.Direction = Direction;

            Player.LastWalk = Environment.TickCount;

            Player.X += Xadd[Direction];
            Player.Y += Yadd[Direction];

            Player.Send(CoPacket.MsgText(Player.UID, Player.Name, Player.Name, "X: " + Player.X + " - Y:" + Player.Y, MsgTextType.Top));
            Player.Send(Packet);
        }
    }
}