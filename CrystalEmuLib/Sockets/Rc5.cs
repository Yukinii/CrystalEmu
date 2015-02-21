using System.IO;
using System.Text;

namespace CrystalEmuLib.Sockets
{
    public static class Rc5
    {
        private static readonly uint[] Key =
        {
            0xEBE854BC, 0xB04998F7, 0xFFFAA88C, 0x96E854BB,
            0xA9915556, 0x48E44110, 0x9F32308F, 0x27F41D3E,
            0xCF4F3523, 0xEAC3C6B4, 0xE9EA5E03, 0xE5974BBA,
            0x334D7692, 0x2C6BCF2E, 0xDC53B74, 0x995C92A6,
            0x7E4F6D77, 0x1EB2B79F, 0x1D348D89, 0xED641354,
            0x15E04A9D, 0x488DA159, 0x647817D3, 0x8CA0BC20,
            0x9264F7FE, 0x91E78C6C, 0x5C9A07FB, 0xABD4DCCE,
            0x6416F98D, 0x6642AB5B
        };

        private static uint RightRotate(uint DwVar, uint DwOffset)
        {
            DwOffset = DwOffset & 0x1F;
            var DwTemp1 = DwVar << (int)(32 - DwOffset);
            var DwTemp2 = DwVar >> (int)DwOffset;
            DwTemp2 = DwTemp2 | DwTemp1;
            return DwTemp2;
        }

        public static string Decrypt(byte[] Bytes)
        {
            using (var Reader = new BinaryReader(new MemoryStream(Bytes, false)))
            {
                var PassInts = new uint[4];
                for (uint I = 0; I < 4; I++)
                    PassInts[I] = (uint)Reader.ReadInt32();

                for (var I = 1; I >= 0; I--)
                {
                    var Temp1 = PassInts[(I * 2) + 1];
                    var Temp2 = PassInts[I * 2];
                    for (var J = 11; J >= 0; J--)
                    {
                        Temp1 = RightRotate(Temp1 - Key[J * 2 + 7], Temp2) ^ Temp2;
                        Temp2 = RightRotate(Temp2 - Key[J * 2 + 6], Temp1) ^ Temp1;

                    }
                    PassInts[I * 2 + 1] = Temp1 - Key[5];
                    PassInts[I * 2] = Temp2 - Key[4];
                }
                var Writer = new BinaryWriter(new MemoryStream(Bytes, true));
                for (uint I = 0; I < 4; I++)
                    Writer.Write((int)PassInts[I]);
                for (var I = 0; I < 16; I++)
                    if (Bytes[I] == 0)
                        return Encoding.ASCII.GetString(Bytes, 0, I);
                return Encoding.ASCII.GetString(Bytes);
            }
        }
    }
}