using System;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public unsafe struct MsgDialog
    {
        public static byte[] NpcSay(string Text)
        {
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
            return Packet;
        }
        public static byte[] NpcLink(string Text, byte DialNr)
        {
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
            return Packet;
        }

        public static byte[] NpcInput(string Text, byte DialNr)
        {
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
            return Packet;
        }

        public static byte[] NpcFace(short Face)
        {
            const ushort packetType = 2032;
            var Packet = new byte[16];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 4) = 10;
                *(P + 6) = 10;
                *((ushort*)(P + 8)) = (ushort)Face;
                *(P + 10) = 0xff;
                *(P + 11) = 4;
            }
            return Packet;
        }

        public static byte[] NpcDone()
        {
            const ushort packetType = 2032;
            var Packet = new byte[16];

            fixed (byte* P = Packet)
            {
                *((ushort*)P) = (ushort)Packet.Length;
                *((ushort*)(P + 2)) = packetType;
                *(P + 10) = 0xff;
                *(P + 11) = 100;
            }

            return Packet;
        }
    }
}
