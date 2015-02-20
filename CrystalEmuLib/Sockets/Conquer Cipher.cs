namespace CrystalEmuLib.Sockets
{
    public interface IPacketAuthCipher
    {
        void Decrypt(byte[] In, byte[] Out, int Size);
        void Encrypt(byte[] In, byte[] Out, int Size);
    }
}