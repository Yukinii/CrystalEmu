using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public struct MsgAction
    {
        public uint TimeStamp;
        public uint UID;
        public uint Offset12Big;
        public uint Offset16Big;
        public ushort Offset20;
        public MsgActionType Action;
        public ushort Offset12
        {
            get { return (ushort)Offset12Big; }
            set { Offset12Big = (uint)((Offset14 << 16) | value); }
        }
        public ushort Offset14
        {
            get { return (ushort)(Offset12Big >> 16); }
            set { Offset12Big = (uint)((value << 16) | Offset12); }
        }
        public ushort Offset16
        {
            get { return (ushort)Offset16Big; }
            set { Offset16Big = (uint)((Offset18 << 16) | value); }
        }
        public ushort Offset18
        {
            get { return (ushort)(Offset16Big >> 16); }
            set { Offset16Big = (uint)((value << 16) | Offset16); }
        }

        public static implicit operator MsgAction(byte[] Buffer)
        {
            var Packet = new MsgAction
            {
                TimeStamp = Buffer.ToUInt(4),
                UID = Buffer.ToUInt(8),
                Offset12 = Buffer.ToUShort(12),
                Offset14 = Buffer.ToUShort(14),
                Offset16 = Buffer.ToUShort(16),
                Offset18 = Buffer.ToUShort(18),
                Offset20 = Buffer.ToUShort(20),
                Action = (MsgActionType)Buffer[22]
            };
            return Packet;
        }

        public static implicit operator byte[] (MsgAction Packet)
        {
            var P = new Packet(PacketID.MsgAction, 28);
            P.Write(Packet.TimeStamp);
            P.Write(Packet.UID);
            P.Write(Packet.Offset12Big);
            P.Write(Packet.Offset16Big);
            P.Write(Packet.Offset20);
            P.Write((ushort)Packet.Action);
            return P.Finish();
        }
    }
}