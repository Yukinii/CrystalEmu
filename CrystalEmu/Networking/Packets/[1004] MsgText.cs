using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    internal partial class CoPacket
    {
        public static byte[] MsgText(uint UID, string From = "", string To = "", string Msg = "", MsgTextType MsgTextType = MsgTextType.Talk)
        {
            if (Msg == null || From == null || To == null)
                return null;

            var P = new Packet(PacketID.MsgText, (ushort)(21 + From.Length + To.Length + Msg.Length));
            P.Write(0xFF0000);
            P.Write((ushort)MsgTextType);
            P.Write((ushort)0);
            P.Write(UID);
            P.Write((byte)4); //Unknown
            P.Write(From);
            P.Write(To);
            P.Write((byte)0);
            P.Write(Msg);
            return P.Finish();

        }
    }
}