namespace CrystalEmuLib.Sockets
{
    public delegate void SocketEvent<in T, in T2>(T Sender, T2 Arg);

    public class YukiServer : ServerSocket
    {
        protected override IPacketAuthCipher MakeCrypto() => new ConquerStanderedCipher();
    }
}