namespace CrystalEmuLib.Sockets
{
    public class ConquerStanderedCipher : IPacketAuthCipher
    {
        private ushort _InCounter;
        private ushort _OutCounter;
        private byte[] _Key3;
        private byte[] _Key4;
        private bool _Server;

        public ConquerStanderedCipher()
        {
            _InCounter = _OutCounter = 0;
        }

        public void Encrypt(byte[] In, byte[] Out, int Size)
        {
            unchecked
            {
                for (var I = 0; I < Size; I++)
                {
                    Out[I] = (byte)(In[I] ^ 0xAB);
                    Out[I] = (byte)((Out[I] << 4) | (Out[I] >> 4));
                    Out[I] = (byte)(ConquerKeys.Key2[_OutCounter >> 8] ^ Out[I]);
                    Out[I] = (byte)(ConquerKeys.Key1[_OutCounter & 0xFF] ^ Out[I]);
                    _OutCounter = (ushort)(_OutCounter + 1);
                }
            }
        }

        public void Decrypt(byte[] In, byte[] Out, int Size)
        {
            for (ushort I = 0; I < Size; I++)
            {
                unchecked
                {
                    Out[I] = (byte)(In[I] ^ 0xAB);
                    Out[I] = (byte)((Out[I] << 4) | (Out[I] >> 4));
                    if (_Server)
                    {
                        Out[I] = (byte)(_Key4[_InCounter >> 8] ^ Out[I]);
                        Out[I] = (byte)(_Key3[_InCounter & 0xFF] ^ Out[I]);
                    }
                    else
                    {
                        Out[I] = (byte)(ConquerKeys.Key2[_InCounter >> 8] ^ Out[I]);
                        Out[I] = (byte)(ConquerKeys.Key1[_InCounter & 0xFF] ^ Out[I]);
                    }
                    _InCounter = (ushort)(_InCounter + 1);
                }
            }
        }

        public unsafe void SetKeys(uint InKey1, uint InKey2)
        {
            unchecked
            {
                var DwKey1 = ((InKey1 + InKey2) ^ 0x4321) ^ InKey1;
                var DwKey2 = DwKey1 * DwKey1;
                _Key3 = new byte[256];
                _Key4 = new byte[256];
                fixed (void* UKey1 = ConquerKeys.Key1, UKey3 = _Key3,
                    UKey2 = ConquerKeys.Key2, UKey4 = _Key4)
                {
                    const byte dwKeyLoop = 256 / 4;
                    for (byte I = 0; I < dwKeyLoop; I++)
                    {
                        *(((uint*)UKey3) + I) = DwKey1 ^ *(((uint*)UKey1) + I);
                        *(((uint*)UKey4) + I) = DwKey2 ^ *(((uint*)UKey2) + I);
                    }
                }
                _OutCounter = 0;
                _Server = true;
            }
        }
    }
}