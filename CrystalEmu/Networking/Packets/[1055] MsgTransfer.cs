namespace CrystalEmu.Networking.Packets
{
    using System;
    using CrystalEmuLib.Enums;
    using CrystalEmuLib.Networking.Packets;
    using PlayerFunctions;

    internal partial class CoPacket
    {
        public static byte[] MsgTransfer(Player Player)
        {
            if (Player?.ServerInfo == null)
                return null;
            Console.WriteLine("{0} -> transfer to -> {1}:{2}", Player.Username, Player.ServerInfo?.IP, Player.ServerInfo.Port);
            using (var P = new Packet(PacketID.MsgTransfer, 32))
            {
                P.Write(Player.UID);
                P.Write(Player.UID);
                P.Write(Player.ServerInfo.IP, false);
                P.Write(Player.ServerInfo.Port);
                return P.Finish();
            }
        }
    }
}
