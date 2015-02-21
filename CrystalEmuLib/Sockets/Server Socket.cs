using System;
using System.Net;
using System.Net.Sockets;

namespace CrystalEmuLib.Sockets
{
    public abstract class ServerSocket
    {
        private int _Backlog;
        private int _Clientbuffersize = 0xffff;
        private readonly Socket _Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool _Enabled;
        public SocketEvent<YukiSocket, object> OnClientConnect;
        public SocketEvent<YukiSocket, object> OnClientDisconnect;
        public SocketEvent<YukiSocket, SocketError> OnClientError;
        public SocketEvent<YukiSocket, byte[]> OnClientReceive;
        private ushort _Port;

        private void AsyncConnect(IAsyncResult Res)
        {
            try
            {
                var Sender = new YukiSocket(this, _Connection.EndAccept(Res), _Clientbuffersize) {Crypto = MakeCrypto(), Connected = true};
                OnClientConnect?.Invoke(Sender, null);
                _Connection.BeginAccept(AsyncConnect, null);
                Sender.Connection.BeginReceive(Sender.Buffer, 0, Sender.Buffer.Length, SocketFlags.None, AsyncReceive, Sender);
            }
            catch (SocketException)
            {
                if (_Enabled)
                    _Connection.BeginAccept(AsyncConnect, null);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void AsyncReceive(IAsyncResult Res)
        {
            try
            {
                SocketError Error;
                var AsyncState = (YukiSocket)Res.AsyncState;
                AsyncState.RecvSize = AsyncState.Connection.EndReceive(Res, out Error);
                if (((Error == SocketError.Success) && (AsyncState.RecvSize > 0)) && AsyncState.Connection.Connected)
                {
                    var Out = new byte[AsyncState.RecvSize];
                    if (AsyncState.Crypto != null)
                        AsyncState.Crypto.Decrypt(AsyncState.Buffer, Out, Out.Length);
                    else
                        Array.Copy(AsyncState.Buffer, 0, Out, 0, AsyncState.RecvSize);
                    OnClientReceive?.Invoke(AsyncState, Out);
                    AsyncState.Connection.BeginReceive(AsyncState.Buffer, 0, AsyncState.Buffer.Length, SocketFlags.None, AsyncReceive, AsyncState);
                }
                else
                {
                    AsyncState.Connected = false;
                    if ((Error != SocketError.Success))
                        OnClientError?.Invoke(AsyncState, Error);
                    InvokeDisconnect(AsyncState);
                }
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Disable()
        {
            if (!_Enabled) return;
            _Connection.Close();
            _Enabled = false;
        }

        public void Enable()
        {
            if (_Enabled) return;
            _Connection.Bind(new IPEndPoint(IPAddress.Any, _Port));
            _Connection.Listen(_Backlog);
            _Connection.BeginAccept(AsyncConnect, null);
            _Enabled = true;
        }

        private void EnabledCheck(string Variable)
        {
            if (_Enabled)
                throw new Exception("Cannot modify " + Variable + " while socket is enabled.");
        }

        public void InvokeDisconnect(YukiSocket Client)
        {
            if (Client == null) return;
            try
            {
                if (Client.Connected)
                {
                    Client.Connection.Shutdown(SocketShutdown.Both);
                    Client.Connection.Close();
                }
                else
                {
                    OnClientDisconnect?.Invoke(Client, null);
                    Client.Ref = null;
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        protected abstract IPacketAuthCipher MakeCrypto();

        public int Backlog
        {
            get { return _Backlog; }
            set
            {
                EnabledCheck("Backlog");
                _Backlog = value;
            }
        }

        public int ClientBufferSize
        {
            get { return _Clientbuffersize; }
            set
            {
                EnabledCheck("ClientBufferSize");
                _Clientbuffersize = value;
            }
        }

        public bool Enabled => _Enabled;

        public ushort Port
        {
            get { return _Port; }
            set
            {
                EnabledCheck("Port");
                _Port = value;
            }
        }
    }
}