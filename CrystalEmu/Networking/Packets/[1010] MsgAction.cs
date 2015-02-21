using System;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Packets
{
    internal partial class CoPacket
    {
        public static byte[] GeneralData(uint UID, uint Value1, ushort Value2, ushort Value3, MsgActionType Type)
        {
            using (var P = new Packet(PacketID.MsgAction, 24))
            {
                P.Write(Environment.TickCount);
                P.Write(UID);
                P.Write(Value1);
                P.Write(Value2);
                P.Write(Value3);
                P.Write((ushort)0);
                P.Write((short)Type);
                return P.Finish();
            }
        }
    }
}