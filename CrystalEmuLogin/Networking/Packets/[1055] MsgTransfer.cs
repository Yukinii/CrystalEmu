using System;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgTransfer(Player Player)
        {
            if (Player?.ServerInfo == null)
                return null;
            Console.WriteLine("{0} -> transfer to -> {1}:{2}", Player.Username, Player.ServerInfo?.IP, Player.ServerInfo.Port);
            var P = new CrystalEmuLib.Networking.Packets.Packet(PacketID.MsgTransfer, 32);
            P.Write(Player.UID);
            P.Write(Security.Hash((short)(Player.UID - 1000000), (short)(Player.UID - 999999)));
            P.Write(Player.ServerInfo.IP, false);
            P.Write(Player.ServerInfo.Port, 28);
            return P.Finish();
        }
    }
}