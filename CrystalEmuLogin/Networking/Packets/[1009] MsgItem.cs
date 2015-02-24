using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgItem(uint UID, uint Value, MsgItemType Type)
        {
            var P = new CrystalEmuLib.Networking.Packets.Packet(PacketID.MsgItem, 20);
            P.Write(UID);
            P.Write(Value);
            P.Write((uint)Type);
            return P.Finish();
        }
    }
}