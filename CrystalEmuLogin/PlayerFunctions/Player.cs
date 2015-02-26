using System.Collections.Concurrent;
using System.Collections.Generic;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.IPC_Comms.Database;
using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLib.Sockets;
using CrystalEmuLogin.Items;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.Networking.Queue;
using CrystalEmuLogin.World;

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
        private uint _MaximumHP;
        private uint _MaximumMP;
        private byte _Action;
        #endregion
        //public MsgSpawn SpawnPacket;
        public readonly YukiSocket Socket;
        public ServerInfo ServerInfo;
        public DataExchange LoadExchange;
        public DataExchange SaveExchange;
        public readonly ConcurrentDictionary<uint, Player> Characters; 
        public readonly ConcurrentDictionary<MsgItemPosition, Item> Equipment;
        public Vector3 Location;
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

        public byte Stamina
        {
            get { return _Stamina; }
            set
            {
                _Stamina = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Stamina));
                IPC.Set(SaveExchange, "Stamina", value);
            }
        }

        public byte XpTimer
        {
            get { return _XpTimer; }
            set
            {
                _XpTimer = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.XpTimer));
                IPC.Set(SaveExchange, "XpTimer", value);
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

        public uint Money
        {
            get { return _Money; }
            set
            {
                _Money = value;
                IPC.Set(SaveExchange, "Money", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.InvMoney));
            }
        }

        public uint Cps
        {
            get { return _Cps; }
            set
            {
                _Cps = value;
                IPC.Set(SaveExchange, "Cps", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.InvCPoints));
            }
        }

        public uint Exp
        {
            get { return _Exp; }
            set
            {
                _Exp = value;
                IPC.Set(SaveExchange, "Exp", value);
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Exp));
            }
        }

        public ushort Strength
        {
            get { return _Strength; }
            set
            {
                _Strength = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.StrengthStatPoints));
                IPC.Set(SaveExchange, "Strength", value);
            }
        }

        public ushort Agility
        {
            get { return _Agility; }
            set
            {
                _Agility = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.DexterityStatPoints));
                IPC.Set(SaveExchange, "Agility", value);
            }
        }

        public ushort Vitality
        {
            get { return _Vitality; }
            set
            {
                _Vitality = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.VitalityStatPoints));
                IPC.Set(SaveExchange, "Vitality", value);
            }
        }

        public ushort Spirit
        {
            get { return _Spirit; }
            set
            {
                _Spirit = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.ManaStatPoints));
                IPC.Set(SaveExchange, "Spirit", value);
            }
        }

        public ushort AttributePoints
        {
            get { return _AttributePoints; }
            set
            {
                _AttributePoints = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.AttributePoints));
                IPC.Set(SaveExchange, "AttributePoints", value);
            }
        }

        public uint MaximumHP
        {
            get { return _MaximumHP; }
            set
            {
                _MaximumHP = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.MaxHP));
                IPC.Set(SaveExchange, "MaximumHP", value);
            }
        }

        public uint MaximumMP
        {
            get { return _MaximumMP; }
            set
            {
                _MaximumMP = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.MaxMana));
                IPC.Set(SaveExchange, "MaximumMP", value);
            }
        }

        public uint CurrentHP
        {
            get { return _CurrentHP; }
            set
            {
                _CurrentHP = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Hp));
                IPC.Set(SaveExchange, "CurrentHP", value);
            }
        }

        public uint CurrentMP
        {
            get { return _CurrentMP; }
            set
            {
                _CurrentMP = value;
                Send(CoPacket.MsgUpdate(UID, value, MsgUpdateType.Mp));
                IPC.Set(SaveExchange, "CurrentMP", value);
            }
        }

        public byte Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        public Player(YukiSocket YukiSocket)
        {
            ServerInfo = new ServerInfo { IP = "192.168.0.4", Port = 5816 };
            Socket = YukiSocket;
            Equipment = new ConcurrentDictionary<MsgItemPosition, Item>();
            Characters = new ConcurrentDictionary<uint, Player>();
            Location = new Vector3(this, 63, 109, 1010);
        }
        public void InitializeDatabaseConnection() => SaveExchange = new DataExchange(ExchangeType.SaveCharacterValue, Core.AccountDatabasePath + Username + @"\" + Name + @"\PlayerInfo.ini", "Character");
        public void Send(byte[] Packet) => OutgoingQueue.Add(this, Packet);
        public void Send(HashSet<byte[]> Packet) => OutgoingQueue.Add(this, Packet);
        public void ForceSend(byte[] Packet) => Socket.Send(Packet);
        public void Disconnect() => Socket.Disconnect();

    }
}