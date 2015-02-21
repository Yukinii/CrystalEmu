using System.Threading.Tasks;
using CrystalEmu.Networking.Packets;
using CrystalEmu.Networking.Queue;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLib.Sockets;

namespace CrystalEmu.PlayerFunctions
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
        #endregion
        public readonly YukiSocket Socket;
        public readonly ServerInfo ServerInfo;
        public DataExchange LoadExchange;
        public DataExchange SaveExchange;
        public uint UID;
        public string Username;
        public uint LastJump;
        public int LastWalk;

        public Player(YukiSocket YukiSocket)
        {
            ServerInfo = new ServerInfo {IP = "192.168.0.2", Port = 5816};
            Socket = YukiSocket;
        }
        public async Task InitializeDatabaseConnection()
        {
            var TempExchange = new DataExchange(ExchangeType.GetUsernameByUID, UID.ToString(), "");
            Username = await IPC.Get(TempExchange, UID.ToString(), "0");
            SaveExchange = new DataExchange(ExchangeType.SaveCharacterValue, Core.AccountDatabasePath + Username + @"\" + Name + @"\PlayerInfo.ini", "Character");
        }
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
            set { _Spouse = value;
                IPC.Set(SaveExchange, "Spouse", value);
            }
        }
        public byte Stamina
        {
            get { return _Stamina; }
            set
            {
                _Stamina = value;
                IPC.Set(SaveExchange, "Stamina", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Stamina));
            }
        }
        public byte XpTimer
        {
            get { return _XpTimer; }
            set
            {
                _XpTimer = value;
                IPC.Set(SaveExchange, "XpTimer", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.XpTimer));
            }
        }
        public byte Level
        {
            get { return _Level; }
            set { _Level = value;
                IPC.Set(SaveExchange, "Level", value);
                Send(CoPacket.MsgUpdate(UID,value,MsgUpdateType.Level));
            }
        }

        public byte Class
        {
            get { return _Class; }
            set { _Class = value;
                IPC.Set(SaveExchange, "Class", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Job));
            }
        }

        public byte Direction
        {
            get { return _Direction; }
            set { _Direction = value;
                IPC.Set(SaveExchange, "Direction", value);
            }
        }

        public ushort PkPoints
        {
            get { return _PkPoints; }
            set { _PkPoints = value;
                IPC.Set(SaveExchange, "PkPoints", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.PKPoints));
            }
        }

        public uint Model
        {
            get { return _Model; }
            set { _Model = value;
                IPC.Set(SaveExchange, "Model", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Model));
            }
        }

        public ushort Hair
        {
            get { return _Hair; }
            set { _Hair = value;
                IPC.Set(SaveExchange, "Hair", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.HairStyle));
            }
        }

        public uint Money
        {
            get { return _Money; }
            set { _Money = value;
                IPC.Set(SaveExchange, "Money", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.InvMoney));
            }
        }

        public uint Cps
        {
            get { return _Cps; }
            set { _Cps = value;
                IPC.Set(SaveExchange, "Cps", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.InvCPoints));
            }
        }

        public uint Exp
        {
            get { return _Exp; }
            set { _Exp = value;
                IPC.Set(SaveExchange, "Exp", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Exp));
            }
        }

        public ushort Strength
        {
            get { return _Strength; }
            set { _Strength = value;
                IPC.Set(SaveExchange, "Strength", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.StrengthStatPoints));
            }
        }

        public ushort Agility
        {
            get { return _Agility; }
            set { _Agility = value;
                IPC.Set(SaveExchange, "Agility", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.DexterityStatPoints));
            }
        }

        public ushort Vitality
        {
            get { return _Vitality; }
            set { _Vitality = value;
                IPC.Set(SaveExchange, "Vitality", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.VitalityStatPoints));
            }
        }

        public ushort Spirit
        {
            get { return _Spirit; }
            set { _Spirit = value;
                IPC.Set(SaveExchange, "Spirit", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.ManaStatPoints));
            }
        }

        public ushort AttributePoints 
        {
            get { return _AttributePoints; }
            set { _AttributePoints = value;
                IPC.Set(SaveExchange, "AttributePoints", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.AttributePoints));
            }
        }

        public uint MaximumHP
        {
            get { return _MaximumHP; }
            set { _MaximumHP = value;
                IPC.Set(SaveExchange, "MaximumHP", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.MaxHP));
            }
        }

        public uint MaximumMP
        {
            get { return _MaximumMP; }
            set { _MaximumMP = value;
                IPC.Set(SaveExchange, "MaximumMP", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.MaxMana));
            }
        }
        public uint CurrentHP
        {
            get { return _CurrentHP; }
            set { _CurrentHP = value;
                IPC.Set(SaveExchange, "CurrentHP", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Hp));
            }
        }

        public uint CurrentMP
        {
            get { return _CurrentMP; }
            set { _CurrentMP = value;
                IPC.Set(SaveExchange, "CurrentMP", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Mp));
            }
        }

        public int X
        {
            get { return _X; }
            set { _X = value;
                IPC.Set(SaveExchange, "X", value);
            }
        }

        public int Y
        {
            get { return _Y; }
            set { _Y = value;
                IPC.Set(SaveExchange, "Y", value);
            }
        }

        public int Z
        {
            get { return _Z; }
            set { _Z = value;
                IPC.Set(SaveExchange, "Z", value);
            }
        }



        public void Send(byte[] Packet) => OutgoingQueue.Add(this, Packet);
        public async Task ForceSend(byte[] Packet) => await Socket.Send(Packet);

        public void Disconnect() => Socket.Disconnect();
    }
}
