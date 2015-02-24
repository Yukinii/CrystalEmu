using System.Collections.Concurrent;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLib.Sockets;
using CrystalEmuLogin.Items;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.Networking.Queue;

namespace CrystalEmuLogin.PlayerFunctions
{
    public class Player
    {
        #region private
        private string _Name = "ERROR";
        private string _Spouse = "None";
        private byte _Stamina;
        private byte _XpTimer;
        private byte _Level;
        private byte _Direction;
        private byte _Class;
        private ushort _PkPoints;
        private uint _Model;
        private ushort _Hair;
        private uint _Money;
        private uint _Cps;
        private uint _Exp;
        private ushort _Strength;
        private ushort _Agility;
        private ushort _Vitality;
        private ushort _Spirit;
        private ushort _AttributePoints;
        private uint _CurrentHP;
        private uint _CurrentMP;
        private int _X;
        private int _Y;
        private int _Z;
        private uint _MaximumHP;
        private uint _MaximumMP;
        private byte _Action;
        #endregion
        public readonly YukiSocket Socket;
        public ServerInfo ServerInfo;
        public DataExchange LoadExchange;
        public DataExchange SaveExchange;
        public readonly ConcurrentDictionary<MsgItemPosition, Item> Equipment;
        public uint UID;
        public string Username;
        public string Password;
        public uint LastJump;
        public int LastWalk;
        public ulong StatusEffects;
        public bool IsReborn;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                IPC.Set(SaveExchange, "Name", value);
            }
        }
        public string Spouse
        {
            get { return _Spouse; }
            set
            {
                _Spouse = value;
                IPC.Set(SaveExchange, "Spouse", value);
            }
        }
        public byte Level
        {
            get { return _Level; }
            set
            {
                _Level = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Level));
                IPC.Set(SaveExchange, "Level", value);
            }
        }
        public byte Class
        {
            get { return _Class; }
            set
            {
                _Class = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Job));
                IPC.Set(SaveExchange, "Class", value);
            }
        }
        public byte Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value;
                IPC.Set(SaveExchange, "Direction", value);
            }
        }
        public ushort PkPoints
        {
            get { return _PkPoints; }
            set
            {
                _PkPoints = value;
                IPC.Set(SaveExchange, "PkPoints", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.PKPoints));
            }
        }
        public uint Model
        {
            get { return _Model; }
            set
            {
                _Model = value;
                IPC.Set(SaveExchange, "Model", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Model));
            }
        }
        public ushort Hair
        {
            get { return _Hair; }
            set
            {
                _Hair = value;
                IPC.Set(SaveExchange, "Hair", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.HairStyle));
            }
        }
        public int X
        {
            get { return _X; }
            set
            {
                _X = value;
                IPC.Set(SaveExchange, "X", value);
            }
        }
        public int Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                IPC.Set(SaveExchange, "Y", value);
            }
        }
        public int Z
        {
            get { return _Z; }
            set
            {
                _Z = value;
                IPC.Set(SaveExchange, "Z", value);
            }
        }
        public byte Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        public Player(YukiSocket YukiSocket)
        {
            ServerInfo = new ServerInfo { IP = "192.168.0.2", Port = 5816 };
            Socket = YukiSocket;
            Equipment = new ConcurrentDictionary<MsgItemPosition, Item>();
        }
        public void InitializeDatabaseConnection() => SaveExchange = new DataExchange(ExchangeType.SaveCharacterValue, Core.AccountDatabasePath + Username + @"\" + Name + @"\PlayerInfo.ini", "Character");
        public void Send(byte[] Packet) => OutgoingQueue.Add(this, Packet);
        public void ForceSend(byte[] Packet) => Socket.Send(Packet);
        public void Disconnect() => Socket.Disconnect();

    }
}