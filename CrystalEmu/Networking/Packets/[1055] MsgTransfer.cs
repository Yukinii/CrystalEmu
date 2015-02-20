using System;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgTransfer(Player Player)
        {
            Console.WriteLine("{0} -> transfer to -> {1}:{2}", Player.Username, Player.ServerInfo.IP, Player.ServerInfo.Port);
            var P = new Packet(PacketID.MsgTransfer,32);
            P.Write(Player.UID);
            P.Write(Player.UID);
            P.Write(Player.ServerInfo.IP,false);
            P.Write(Player.ServerInfo.Port);
            return P.Finish();
        }
    }
}
