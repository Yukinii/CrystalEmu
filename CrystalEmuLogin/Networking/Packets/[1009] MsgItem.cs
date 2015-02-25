using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{ 
    public struct MsgItem
    {
        public uint UID;
        public uint Value;
        public MsgItemType Type;

        public static implicit operator MsgItem(byte[] Buffer)
        {
            var Packet = new MsgItem
            {
                UID = Buffer.ToUInt(4),
                Value = Buffer.ToUInt(8),
                Type = (MsgItemType)Buffer.ToUShort(12)
            };
            return Packet;
        }

        public static implicit operator byte[] (MsgItem Packet)
        {
            var P = new Packet(PacketID.MsgItem, 20);
            P.Write(Packet.UID);
            P.Write(Packet.Value);
            P.Write((uint)Packet.Type);
            return P.Finish();
        }
    }
}