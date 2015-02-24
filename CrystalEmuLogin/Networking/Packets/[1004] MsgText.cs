using System;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public struct MsgText
    {
        public uint Color;
        public MsgTextType Type;
        public uint Unknown;
        public uint TimeStamp;
        public uint ToFace;
        public uint FromFace;
        public string From;
        public string To;
        public string Message;

        public static implicit operator MsgText(byte[] Buffer)
        {
            var Packet = new MsgText
            {
                Color = BitConverter.ToUInt32(Buffer, 4),
                Type = (MsgTextType)BitConverter.ToUInt16(Buffer, 8),
                Unknown = BitConverter.ToUInt32(Buffer, 10),
                TimeStamp = BitConverter.ToUInt32(Buffer, 12),
                ToFace = BitConverter.ToUInt32(Buffer, 16),
                FromFace = BitConverter.ToUInt32(Buffer, 20),
                From = Buffer.StringFrom(26, (int)Buffer.ToUInt(25))
            };
            Packet.To = Buffer.StringFrom(27 + Packet.From.Length, (int)Buffer.ToUInt(26 + Packet.From.Length));
            Packet.Message = Buffer.StringFrom(29 + Packet.From.Length + Packet.To.Length, (int)Buffer.ToUInt(28 + Packet.From.Length + Packet.To.Length));
            return Packet;
        }

        public static implicit operator byte[] (MsgText Packet)
        {
            var P = new Packet(PacketID.MsgText, (ushort)(21 + Packet.From.Length + Packet.To.Length + Packet.Message.Length));
            P.Write(Packet.Color);
            P.Write((ushort)Packet.Type);
            P.Write((ushort)0);
            P.Write(Packet.TimeStamp);
            P.Write((byte)4); //Unknown
            P.Write(Packet.From);
            P.Write(Packet.To);
            P.Write((byte)0);
            P.Write(Packet.Message);
            return P.Finish();
        }
    }
}