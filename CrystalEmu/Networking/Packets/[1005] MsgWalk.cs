using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgWalk(uint UID, byte Dir, bool Run = false)
        {
            var P = new Packet(PacketID.MsgWalk, 10);
            P.Write(UID);
            P.Write(Dir);
            P.Write((byte) (Run ? 1 : 0));
            return P.Finish();
        }
    }
}
