using System;
using CrystalEmuLib.Enums;

namespace CrystalEmuLib.Networking.Packets
{
    public unsafe class Packet
    {
        private int _Offset;
        private byte[] _Buffer;

        public Packet(PacketID PacketID, int Size)
        {
            _Buffer = new byte[Size];
            Write((ushort)Size);
            Write((ushort)PacketID);
        }

        private byte* Ptr
        {
            get
            {
                fixed (byte* P = _Buffer)
                    return P;
            }
        }
        public void Write(bool Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *(Ptr + _Offset) = (byte)(Val ? 1 : 0);
            _Offset++;
        }
        public void Write(byte Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *(Ptr + _Offset) = Val;
            _Offset ++;
        }
        public void Write(ushort Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *((ushort*)(Ptr + _Offset)) = Val;
            _Offset += 2;
        }
        public void Write(short Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *((short*)(Ptr + _Offset)) = Val;
            _Offset += 2;
        }
        public void Write(uint Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *((uint*)(Ptr + _Offset)) = Val;
            _Offset += 4;
        }
        public void Write(int Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *((int*)(Ptr + _Offset)) = Val;
            _Offset += 4;
        }
        public void Write(ulong Val, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            *((ulong*)(Ptr + _Offset)) = Val;
            _Offset += 8;
        }
        public void Write(string Val, bool PrefixLenght = true, int Offset = - 1)
        {
            if (Offset != - 1)
                _Offset = Offset;
            if (PrefixLenght)
            {
                *(Ptr + _Offset) = (byte)Val.Length;
                _Offset++;
            }
            foreach (var Char in Val)
            {
                *(Ptr + _Offset) = Convert.ToByte(Char);
                _Offset++;
            }
        }
        public void Move(int Count) => _Offset += Count;
        public byte[] Finish() => _Buffer;
    }

    //public class Packet : IDisposable
    //{
    //    private readonly byte[] _Buffer;
    //    private readonly MemoryStream _Stream;
    //    private readonly BinaryWriter _Writer;

    //    public Packet(PacketID ID, int Size)
    //    {
    //        _Buffer = new byte[Size];
    //        _Stream = new MemoryStream(_Buffer);
    //        _Writer = new BinaryWriter(_Stream);
    //        _Writer.Write((ushort)Size);
    //        _Writer.Write((ushort)ID);
    //    }

    //    public void Dispose()
    //    {
    //        _Stream?.Dispose();
    //        _Writer?.Dispose();
    //    }

    //    public void Write(byte Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(uint Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(ulong Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(int Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(ushort Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(short Value, int Offset = - 1)
    //    {
    //        if (Offset != - 1)
    //        {
    //            if (_Writer != null)
    //                _Writer.BaseStream.Position = Offset;
    //        }

    //        _Writer?.Write(Value);
    //    }

    //    public void Write(string Value, bool PrefixLenght)
    //    {
    //        if (PrefixLenght)
    //        {
    //            var Array = Value?.ToCharArray();
    //            if (Array == null) return;
    //            _Writer?.Write((byte)Array.Length);
    //            _Writer?.Write(Array);
    //        }
    //        else
    //            _Writer?.Write(Value?.ToCharArray());
    //    }

    //    public byte[] Finish()
    //    {
    //        _Writer.Flush();
    //        _Stream.Flush();
    //        return _Buffer;
    //    }
    //}
}