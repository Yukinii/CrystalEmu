using System;
using System.Linq;
using System.Text;
using CrystalEmuLib.Enums;

namespace CrystalEmuLib.Extensions
{
    public static class ByteExtensions
    {
        public static ushort Size(this byte[] Packet)
        {
            return Packet.Length > 2 ? BitConverter.ToUInt16(Packet, 0) : (ushort)0;
        }
        public static PacketID PacketID(this byte[] Packet)
        {
            ushort ID = BitConverter.ToUInt16(Packet, 2);
            return (PacketID)(Packet.Length > 4 ? BitConverter.ToUInt16(Packet, 2) : 0);
        }

        public static uint ToUInt(this byte[] Packet, int Offset)
        {
            return Packet.Length > Offset + 4 ? BitConverter.ToUInt32(Packet, Offset) : 0;
        }
        public static ushort ToUShort(this byte[] Packet, int Offset)
        {
            return Packet.Length > Offset + 2 ? BitConverter.ToUInt16(Packet, Offset) : (ushort)0;
        }
        public static string StringFrom(this byte[] Packet, int Offset, int Count)
        {
            return Packet.Length > Offset + Count-1 ? Encoding.UTF8.GetString(Packet, Offset, Count).Trim((char) 0x0000) : null;
        }
        public static byte[] ArrayFrom(this byte[] Packet, int Offset, int Count)
        {
            return Packet.Length > Offset + Count-1 ? Packet.Skip(Offset).Take(Count).ToArray() : null;
        }
    }
}
