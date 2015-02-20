using System;
using System.Net.Sockets;

namespace CrystalEmuLib.Sockets
{
    public abstract class ClientSocket :IDisposable
    {
        private byte[] _Buffer;
        private Socket _Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        protected IPacketAuthCipher Crypto;
        private bool _Enabled;
        public SocketEvent<ClientSocket, object> OnConnect;
        public SocketEvent<ClientSocket, object> OnConnecting;
        public SocketEvent<ClientSocket, object> OnDisconnect;
        public SocketEvent<ClientSocket, SocketError> OnError;
        public SocketEvent<ClientSocket, byte[]> OnReceive;
        private string _Remoteip;
        private ushort _Remoteport;

        protected ClientSocket()
        {
            BufferSize = 0xffff;
        }

        private void AsyncReceive(IAsyncResult Res)
        {
            try
            {
                SocketError Error;
                var AsyncState = (YukiSocket)Res.AsyncState;
                AsyncState.RecvSize = AsyncState.Connection.EndReceive(Res, out Error);
                if ((Error == SocketError.Success) && (AsyncState.RecvSize > 0))
                {
                    var Buff = new byte[AsyncState.RecvSize];
                    if (Crypto != null)
                    {
                        Crypto.Decrypt(AsyncState.Buffer, Buff, Buff.Length);
                    }
                    else
                    {
                        Buff = new byte[AsyncState.RecvSize];
                        AsyncState.Buffer.CopyTo(Buff, 0);
                        //NativeMethods.memcpy(buff, asyncState.Buffa, asyncState.RecvSize);
                    }
                    OnReceive?.Invoke(this, Buff);
                    _Connection.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, AsyncReceive, null);
                }
                else
                {
                    _Enabled = false;
                    if ((Error != SocketError.Success))
                    {
                        OnError?.Invoke(this, Error);
                    }
                    OnDisconnect?.Invoke(this, null);
                }
            }
            catch (SocketException Exception)
            {
                OnError?.Invoke(this, Exception.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Disable()
        {
            if (_Enabled)
            {
                _Connection.Shutdown(SocketShutdown.Both);
                _Enabled = false;
                OnDisconnect?.Invoke(this, null);
                _Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }

        public void Enable()
        {
            if (!_Enabled)
            {
                try
                {
                    OnConnecting?.Invoke(this, null);
                    _Connection.Connect(_Remoteip, _Remoteport);
                    if (_Connection.Connected)
                    {
                        OnConnect?.Invoke(this, null);
                    }
                    _Connection.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, AsyncReceive, null);
                    _Enabled = true;
                }
                catch (SocketException Exception)
                {
                    OnError?.Invoke(this, Exception.SocketErrorCode);
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void EnabledCheck(string Variable)
        {
            if (_Enabled)
            {
                throw new Exception("Cannot modify " + Variable + " while socket is enabled.");
            }
        }

        public void Send(byte[] Bytes)
        {
            if (!_Enabled)
                return;

            if (Crypto != null)
            {
                var Out = new byte[Bytes.Length];
                Crypto.Encrypt(Bytes, Out, Out.Length);
                _Connection.Send(Out);
            }
            else
            {
                _Connection.Send(Bytes);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool Disposing)
        {
            _Connection?.Dispose();
        }

        public int BufferSize
        {
            get
            {
                return _Buffer.Length;
            }
            set
            {
                EnabledCheck("BufferSize");
                _Buffer = new byte[value];
            }
        }

        public Socket Connection => _Connection;

        public bool Enabled => _Enabled;

        public string RemoteIP
        {
            get
            {
                return _Remoteip;
            }
            set
            {
                EnabledCheck("RemoteIP");
                _Remoteip = value;
            }
        }

        public ushort RemotePort
        {
            get
            {
                return _Remoteport;
            }
            set
            {
                EnabledCheck("RemotePort");
                _Remoteport = value;
            }
        }
    }
}