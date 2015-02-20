using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgUpdate(uint UID, ulong Value, MsgUpdateType Type)
        {
            var P = new Packet(PacketID.MsgUpdate, 28);
            P.Write(UID);
            P.Write(1);
            P.Write((uint) Type);
            P.Write(Value);
            return P.Finish();
        }
    }
}
