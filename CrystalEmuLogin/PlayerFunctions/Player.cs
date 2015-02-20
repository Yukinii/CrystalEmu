using System.Threading.Tasks;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLib.Sockets;
using CrystalEmuLogin.Networking.Queue;

namespace CrystalEmuLogin.PlayerFunctions
{
    public class Player
    {
        public readonly YukiSocket Socket;

        public uint UID;
        public string Username = "";
        public string Password = "";
        public ServerInfo ServerInfo;

        public Player(YukiSocket YukiSocket)
        {
            Socket = YukiSocket;
        }

        public void Send(byte[] Packet)
        {
            OutgoingQueue.Add(this, Packet);
        }
        public async Task ForceSend(byte[] Packet)
        {
            await Socket.Send(Packet);
        }

        public void Disconnect()
        {
            Socket.Disconnect();
        }
    }
}
