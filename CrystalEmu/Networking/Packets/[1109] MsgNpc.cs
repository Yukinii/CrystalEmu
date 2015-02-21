using CrystalEmu.Entities;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    internal partial class CoPacket
    {
        public static byte[] MsgNpc(Npc Npc)
        {
            var P = new Packet(PacketID.MsgNpc, 28);
            P.Write(Npc.UID);
            P.Write(Npc.MaxHP);
            P.Write(Npc.CurHP);
            P.Write(Npc.X);
            P.Write(Npc.Y);
            P.Write(Npc.Model);
            P.Write(Npc.Type);
            P.Write(Npc.Base);
            return P.Finish();
        }
    }
}
