using CrystalEmuLib.IPC_Comms.Shared;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.World
{
    public struct Vector3
    {
        #region Privates
        private Player Player;
        private ushort _X;
        private ushort _Y;
        private ushort _Z;
        #endregion
        public ushort X
        {
            get { return _X; }
            set
            {
                _X = value;
                IPC.Set(Player.SaveExchange, "X", value);
            }
        }

        public ushort Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                IPC.Set(Player.SaveExchange, "Y", value);
            }
        }

        public ushort Z
        {
            get { return _Z; }
            set
            {
                _Z = value;
                IPC.Set(Player.SaveExchange, "Z", value);
            }
        }
        public Vector3(Player Player, ushort X, ushort Y, ushort Z)
        {
            this.Player = Player;
            _X = X;
            _Y = Y;
            _Z = Z;
        }
    }
}