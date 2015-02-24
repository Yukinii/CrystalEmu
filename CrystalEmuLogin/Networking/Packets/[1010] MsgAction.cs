using System;
using CrystalEmuLib.Enums;
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
                TimeStamp = BitConverter.ToUInt32(Buffer, 4),
                UID = BitConverter.ToUInt32(Buffer, 8),
                Offset12 = BitConverter.ToUInt16(Buffer, 12),
                Offset14 =  BitConverter.ToUInt16(Buffer, 14),
                Offset16 = BitConverter.ToUInt16(Buffer, 16),
                Offset18 = BitConverter.ToUInt16(Buffer, 18),
                Offset20 = BitConverter.ToUInt16(Buffer, 20),
                Action = (MsgActionType)BitConverter.ToUInt32(Buffer, 22)
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