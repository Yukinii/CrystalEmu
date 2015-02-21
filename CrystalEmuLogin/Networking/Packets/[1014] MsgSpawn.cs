using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

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
            P.Write((ushort)0);
            P.Write((byte)0);
            if (Player.Equipment.ContainsKey(MsgItemPosition.Head))
                P.Write(Player.Equipment[MsgItemPosition.Head].ID);
            if (Player.Equipment.ContainsKey(MsgItemPosition.Armor))
                P.Write(Player.Equipment[MsgItemPosition.Armor].ID);
            if (Player.Equipment.ContainsKey(MsgItemPosition.Right))
                P.Write(Player.Equipment[MsgItemPosition.Right].ID);
            if (Player.Equipment.ContainsKey(MsgItemPosition.Left))
                P.Write(Player.Equipment[MsgItemPosition.Left].ID);
            P.Write(Player.CurrentHP);
            P.Write(Player.CurrentMP);
            P.Write(Player.X);
            P.Write(Player.Y);
            P.Write(Player.Hair);
            P.Write(Player.Direction);
            P.Write(Player.Action);
            P.Write(Player.Level);
            P.Write((byte)1);
            P.Write(Player.Name);
            return P.Finish();
        }
    }
}
