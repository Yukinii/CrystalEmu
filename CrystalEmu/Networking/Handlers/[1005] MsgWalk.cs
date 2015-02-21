namespace CrystalEmu.Networking.Handlers
{
    using System;
    using System.Threading.Tasks;
    using CrystalEmuLib.Enums;
    using CrystalEmuLib.Extensions;
    using Packets;
    using PlayerFunctions;

    public static class MsgWalk
    {
        public static async Task Handle(Player Player, byte[] Packet)
        {
            var UID = Packet.ToUInt(4);
            var Direction = (byte) (Packet[8]%8);
            var Running = Packet[9];
            int[] Xadd = { 0, -1, -1, -1, 0, 1, 1, 1 };
            int[] Yadd = { 1, 1, 0, -1, -1, -1, 0, 1 };


            if (Player.UID != UID)
                Player.Disconnect();

            if(Running == 0 && (Player.LastWalk + 500 > Environment.TickCount) || Running == 0 && (Player.LastWalk + 400 > Environment.TickCount))
                Player.Disconnect();

            Player.Direction = Direction;

            Player.LastWalk = Environment.TickCount;

            Player.X += Xadd[Direction];
            Player.Y += Yadd[Direction];

            Player.Send(CoPacket.MsgText(Player.UID,Player.Name,Player.Name, "X: "+Player.X + " - Y:" +Player.Y,MsgTextType.Top));
            Player.Send(Packet);
        }
    }
}
