namespace CrystalEmu.Networking.Packets
{
    using CrystalEmuLib.Enums;
    using CrystalEmuLib.Networking.Packets;

    internal partial class CoPacket
    {
        public static byte[] MsgUpdate(uint UID, ulong Value, MsgUpdateType Type)
        {
            using (var P = new Packet(PacketID.MsgUpdate, 28))
            {
                P.Write(UID);
                P.Write(1);
                P.Write((uint) Type);
                P.Write(Value);
                return P.Finish();
            }
        }
    }
}
