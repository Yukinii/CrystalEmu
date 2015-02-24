using CrystalEmuLib.Enums;

namespace CrystalEmuLogin.Networking.Packets
{
    public unsafe struct MsgAction
    {
        public uint TimeStamp { get; set; }
        public uint UID { get; set; }
        public uint Data1 { get; set; }
        public uint Data2 { get; set; }
        public ushort Data3 { get; set; }
        public MsgActionType Action { get; set; }
        #region Data1
        /// <summary>
        /// Offset [12]
        /// </summary>
        public ushort Data1Low
        {
            get { return (ushort)Data1; }
            set { Data1 = (uint)((Data1High << 16) | value); }
        }

        /// <summary>
        /// Offset [14]
        /// </summary>
        public ushort Data1High
        {
            get { return (ushort)(Data1 >> 16); }
            set { Data1 = (uint)((value << 16) | Data1Low); }
        }

        #endregion
        #region Data2
        /// <summary>
        /// Offset [16]
        /// </summary>
        public ushort Data2Low
        {
            get { return (ushort)Data2; }
            set { Data2 = (uint)((Data2High << 16) | value); }
        }

        /// <summary>
        /// Offset [18]
        /// </summary>
        public ushort Data2High
        {
            get { return (ushort)(Data2 >> 16); }
            set { Data2 = (uint)((value << 16) | Data2Low); }
        }

        #endregion
        public static MsgAction Create(uint Offset4, uint Offset8, uint Offset12, uint Offset16, ushort Offset20, MsgActionType Type)
        {
            var Packet = new MsgAction
            {
                TimeStamp = Offset4,
                UID = Offset8,
                Data1 = Offset12,
                Data2 = Offset16,
                Data3 = Offset20,
                Action = Type
            };
            return Packet;
        }
        public static MsgAction Create(uint Offset4, uint Offset8, uint Offset12, ushort Offset16, ushort Offset20, MsgActionType Type)
        {
            var Packet = new MsgAction
            {
                TimeStamp = Offset4,
                UID = Offset8,
                Data1 = Offset12,
                Data2Low = Offset16,
                Data2High = Offset20,
                Action = Type
            };
            return Packet;
        }

        public static implicit operator MsgAction(byte[] Buffer)
        {
            var Packet = new MsgAction();
            fixed (byte* Pointer = Buffer)
            {
                Packet.TimeStamp = *((uint*)(Pointer + 4));
                Packet.UID = *((uint*)(Pointer + 8));
                Packet.Data1 = *((uint*)(Pointer + 12));
                Packet.Data2 = *((uint*)(Pointer + 16));
                Packet.Data3 = *((ushort*)(Pointer + 20));
                Packet.Action = *(MsgActionType*)(Pointer + 22);
            }
            return Packet;
        }

        public static implicit operator byte[] (MsgAction Packet)
        {
            var Buffer = new byte[24];
            fixed (byte* Pointer = Buffer)
            {
                *((ushort*)(Pointer + 0)) = (ushort)Buffer.Length;
                *((ushort*)(Pointer + 2)) = 1010;
                *((uint*)(Pointer + 4)) = Packet.TimeStamp;
                *((uint*)(Pointer + 8)) = Packet.UID;
                *((uint*)(Pointer + 12)) = Packet.Data1;
                *((uint*)(Pointer + 16)) = Packet.Data2;
                *((ushort*)(Pointer + 20)) = Packet.Data3;
                *((MsgActionType*)(Pointer + 22)) = Packet.Action;
            }
            return Buffer;
        }
    }
}