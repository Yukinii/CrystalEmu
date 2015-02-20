using System;
using System.IO;
using CrystalEmuLib.Enums;

namespace CrystalEmuLib.Networking.Packets
{
    public class Packet : IDisposable
    {
        public readonly MemoryStream Stream;
        public readonly BinaryWriter Writer;
        public readonly byte[] Buffer;
        public Packet(PacketID ID, int Size)
        {
            Buffer = new byte[Size];
            Stream = new MemoryStream(Buffer);
            Writer = new BinaryWriter(Stream);
            Writer.Write((ushort)Size);
            Writer.Write((ushort)ID);
        }

        public void Zerofill(int Count)
        {
            for (var I = 0; I < Count; I++)
            {
                Writer.Write((byte)0);
            }
        }
        public void Write(byte Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }
        public void Write(uint Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }
        public void Write(ulong Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }
        public void Write(int Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }

        public void Write(ushort Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }

        public void Write(short Value, int Offset = -1)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            Writer.Write(Value);
        }

        public void Write(string Value, int Offset = -1, bool PrefixLenght = false)
        {
            if (Offset != -1)
                Writer.BaseStream.Position = Offset;

            if (PrefixLenght)
            {
                var Array = Value.ToCharArray();
                Writer.Write((byte)Array.Length);
                Writer.Write(Array);
            }
            else
            {
                Writer.Write(Value.ToCharArray());
            }
        }
        public void Write(string Value, bool PrefixLenght)
        {
            if (PrefixLenght)
            {
                var Array = Value.ToCharArray();
                Writer.Write((byte) Array.Length);
                Writer.Write(Array);
            }
            else
            {
                Writer.Write(Value.ToCharArray());
            }
        }

        public byte[] Finish()
        {
            return Buffer;
        }

        public void Dispose()
        {
            Writer.Flush();
            Stream.Flush();
            Stream.Close();
            Writer.Close();
            Stream.Dispose();
            Writer.Dispose();
        }
    }
}
