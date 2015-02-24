using System;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgTime()
        {
            var P = new Packet(PacketID.MsgTime, 36);
            var Time = DateTime.Now;
            P.Write(Time.Year - 1900);
            P.Write(Time.Month-1);
            P.Write(Time.DayOfYear);
            P.Write(Time.Day);
            P.Write(Time.Hour);
            P.Write(Time.Minute);
            P.Write(Time.Second);
            return P.Finish();
        }
    }
}
