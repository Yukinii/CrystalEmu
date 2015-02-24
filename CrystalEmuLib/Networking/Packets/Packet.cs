using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalEmuLib.Enums;
using  CrystalEmuLib.Extensions;

namespace CrystalEmuLib.Networking.Packets
{
    public unsafe class Packet
    {
        private int _Offset;
        private readonly byte[] _Buffer;

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
        public void Write(string Val, bool PrefixLength = true, int Offset = - 1)
        {
            if (Offset != - 1)
                _Offset = Offset;
            if (PrefixLength)
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

        public string ReadString(int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            var Len = *(Ptr + _Offset);
            var Sb = new StringBuilder(Len);

            for (var I = 0; I < Len; I++)
            {
                Sb.Append((char)*(Ptr + _Offset + I));
            }

            return Sb.ToString();
        }
        public string ReadString(int Len, int Offset = -1)
        {
            if (Offset != -1)
                _Offset = Offset;

            var Sb = new StringBuilder(Len);

            for (var I = 0; I < Len; I++)
            {
                Sb.Append((char)*(Ptr + _Offset + I));
            }

            return Sb.ToString();
        }
        public void Move(int Count) => _Offset += Count;
        public byte[] Finish() => _Buffer;
    }
}