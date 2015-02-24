using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgDialog(string Text, byte Index, ushort Face, MsgDialogType Type)
        {
            var P = new Packet(PacketID.MsgDialog, 16 + Text.Length);

            P.Write(Index, 10);
            switch (Type)
            {
                case MsgDialogType.Face:
                    P.Write((short)10, 4);
                    P.Write((short)10, 6);
                    P.Write((short)Face);
                    break;
            }
            P.Write((short)Type, 11);
            P.Write(Text, true, 13);
            return P.Finish();
        }
    }
}
