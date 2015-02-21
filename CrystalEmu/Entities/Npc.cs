namespace CrystalEmu.Entities
{
    internal class Npc
    {
        private uint _UID;
        private uint _MaxHP;
        private uint _CurHP;
        private ushort _X;
        private ushort _Y;
        private uint _Model;
        private ushort _Type;
        private uint _Base;

        public uint UID
        {
            get { return _UID; }
            set { _UID = value; }
        }

        public uint MaxHP
        {
            get { return _MaxHP; }
            set { _MaxHP = value; }
        }

        public uint CurHP
        {
            get { return _CurHP; }
            set { _CurHP = value; }
        }

        public ushort X
        {
            get { return _X; }
            set { _X = value; }
        }

        public ushort Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public uint Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        public ushort Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public uint Base
        {
            get { return _Base; }
            set { _Base = value; }
        }
    }
}