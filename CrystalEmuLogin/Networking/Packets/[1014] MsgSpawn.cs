using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;
using CrystalEmuLib.Extensions;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgSpawn(Player Player)
        {
            var P = new Packet(PacketID.MsgSpwan, 100 + Player.Name.Length);
            P.Write(Player.UID);
            P.Write(Player.Model);
            P.Write(Player.StatusEffects);
            P.Write(0);
            P.Write(0);
            P.Write(0);
            var Head = Player.Equipment.GetValueOrNull(MsgItemPosition.Head);
            P.Write(Head?.ID ?? 0);
            var Armor = Player.Equipment.GetValueOrNull(MsgItemPosition.Head);
            P.Write(Armor?.ID ?? 0);
            var Right = Player.Equipment.GetValueOrNull(MsgItemPosition.Head);
            P.Write(Right?.ID ?? 0);
            var Left = Player.Equipment.GetValueOrNull(MsgItemPosition.Head);
            P.Write(Left?.ID ?? 0);
            P.Write(0);
            P.Write(Player.Location.X);
            P.Write(Player.Location.Y);
            P.Write(Player.Hair);
            P.Write(Player.Direction);
            P.Write(Player.Action);
            P.Write(Player.Level);
            P.Write((byte)1);
            P.Write((byte)1);
            P.Write(Player.Name);
            return P.Finish();
        }
    }
}
