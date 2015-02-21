using System;
using System.IO;
using CrystalEmuLib.Enums;

namespace CrystalEmuLib.Networking.Packets
{
    public class Packet : IDisposable
    {
        private readonly byte[] _Buffer;
        private readonly MemoryStream _Stream;
        private readonly BinaryWriter _Writer;

        public Packet(PacketID ID, int Size)
        {
            _Buffer = new byte[Size];
            _Stream = new MemoryStream(_Buffer);
            _Writer = new BinaryWriter(_Stream);
            _Writer.Write((ushort)Size);
            _Writer.Write((ushort)ID);
        }

        public void Dispose()
        {
            _Writer?.Flush();
            _Stream?.Flush();
            _Stream?.Close();
            _Writer?.Close();
            _Stream?.Dispose();
            _Writer?.Dispose();
        }

        public void Write(byte Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(uint Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(ulong Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(int Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(ushort Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(short Value, int Offset = - 1)
        {
            if (Offset != - 1)
            {
                if (_Writer != null)
                    _Writer.BaseStream.Position = Offset;
            }

            _Writer?.Write(Value);
        }

        public void Write(string Value, bool PrefixLenght)
        {
            if (PrefixLenght)
            {
                var Array = Value?.ToCharArray();
                if (Array == null) return;
                _Writer?.Write((byte)Array.Length);
                _Writer?.Write(Array);
            }
            else
                _Writer?.Write(Value?.ToCharArray());
        }

        public byte[] Finish() => _Buffer;
    }
}