using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    internal partial class CoPacket
    {
        public static byte[] MsgItem(uint UID, uint Value, MsgItemType Type)
        {
            using (var P = new Packet(PacketID.MsgItem, 20))
            {
                P.Write(UID);
                P.Write(Value);
                P.Write((uint) Type);
                return P.Finish();
            }
        }
    }
}
