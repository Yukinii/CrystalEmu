using System;
using System.Collections.Generic;


namespace CrystalEmuLogin.Networking.Packets
{
    public unsafe struct MsgDialog
    {
        public HashSet<byte[]> Packets; 
        public void AddText(string Text)
        {
            if (Packets == null)
                Packets = new HashSet<byte[]>();
            
            const ushort packetType = 2032;
            var Packet = new byte[16 + Text.Length];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 10) = 0xff;
                *(P + 11) = 1;
                *(P + 12) = 1;
                *(P + 13) = (byte)Text.Length;
                for (var I = 0; I < Text.Length; I++)
                {
                    *(P + 14 + I) = Convert.ToByte(Text[I]);
                }
            }
            Packets.Add(Packet);
        }
        public void AddOption(string Text, byte DialNr)
        {
            if (Packets == null)
                Packets = new HashSet<byte[]>();
            const ushort packetType = 2032;
            var Packet = new byte[16 + Text.Length];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 10) = DialNr;
                *(P + 11) = 2;
                *(P + 12) = 1;
                *(P + 13) = (byte)Text.Length;
                for (var I = 0; I < Text.Length; I++)
                {
                    *(P + 14 + I) = Convert.ToByte(Text[I]);
                }
            }
            Packets.Add(Packet);
        }
        public void AddInputbox(string Text, byte DialNr)
        {
            if (Packets == null)
                Packets = new HashSet<byte[]>();
            const ushort packetType = 2032;
            var Packet = new byte[16 + Text.Length];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 10) = DialNr;
                *(P + 11) = 3;
                *(P + 12) = 1;
                *(P + 13) = (byte)Text.Length;
                for (var I = 0; I < Text.Length; I++)
                {
                    *(P + 14 + I) = Convert.ToByte(Text[I]);
                }
            }
            Packets.Add(Packet);
        }
        public void AddFace(ushort FaceID)
        {
            if (Packets == null)
                Packets = new HashSet<byte[]>();
            const ushort packetType = 2032;
            var Packet = new byte[16];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 4) = 10;
                *(P + 6) = 10;
                *((ushort*)(P + 8)) = FaceID;
                *(P + 10) = 0xff;
                *(P + 11) = 4;
            }
            Packets.Add(Packet);
        }
        public HashSet<byte[]> Finish()
        {
            if (Packets == null)
                Packets = new HashSet<byte[]>();
            const ushort packetType = 2032;
            var Packet = new byte[16];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 10) = 0xff;
                *(P + 11) = 100;
            }
            Packets.Add(Packet);
            return Packets;
        }

        public static implicit operator HashSet<byte[]>(MsgDialog Packet) => Packet.Finish();
    }
}
