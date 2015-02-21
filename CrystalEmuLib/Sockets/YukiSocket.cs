using System.Net.Sockets;
using System.Threading.Tasks;

namespace CrystalEmuLib.Sockets
{
    public class YukiSocket
    {
        public readonly byte[] Buffer;
        public bool Connected;
        public IPacketAuthCipher Crypto;
        public int RecvSize;
        public SocketEvent<YukiSocket, object> SocketCorrupt;
        public object Ref;

        public YukiSocket(ServerSocket Server, Socket Connection, int BufferSize)
        {
            this.Server = Server;
            this.Connection = Connection;
            Buffer = new byte[BufferSize];
            RecvSize = 0;
            Ref = null;
        }

        public void Disconnect()
        {
            try
            {
                Connection.Disconnect(false);
            }
            catch
            {
                Server.InvokeDisconnect(this);
            }
        }

        public async Task<bool> Send(byte[] Packet)
        {
            try
            {
                if (Crypto != null)
                {
                    var Out = new byte[Packet.Length];
                    Crypto.Encrypt(Packet, Out, Out.Length);
                    if (Connection != null)
                        await Task.Factory.FromAsync(Connection.BeginSend(Out, 0, Out.Length, SocketFlags.None, null, Connection), Connection.EndSend).ConfigureAwait(false);
                }
                else
                {
                    if (Connection != null)
                        await Task.Factory.FromAsync(Connection.BeginSend(Packet, 0, Packet.Length, SocketFlags.None, null, Connection), Connection.EndSend).ConfigureAwait(false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Socket Connection { get; }

        public ServerSocket Server { get; }

        public void SendClear(byte[] Packet) => Connection.Send(Packet);
    }
}