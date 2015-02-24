using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgDialog(string Text, byte Index, ushort Face, MsgDialogType Type)
        {
            var P = new CrystalEmuLib.Networking.Packets.Packet(PacketID.MsgDialog, 16 + Text.Length);
            P.Write(0);
            P.Write((short)Face);
            P.Write(Index);
            P.Write((short)Type);
            P.Write(1);
            P.Write(Text);
            return P.Finish();
        }
    }
}
