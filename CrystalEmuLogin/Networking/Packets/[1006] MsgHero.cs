using CrystalEmuLib.Enums;
using CrystalEmuLib.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Packets
{
    public partial class CoPacket
    {
        public static byte[] MsgHero(Player Player)
        {
            if (Player?.Spouse == null || Player.Name == null)
                return null;

            var P = new Packet(PacketID.MsgHero, 68 + Player.Name.Length + Player.Spouse.Length);
            P.Write(Player.UID);
            P.Write(Player.Model);
            P.Write(Player.Hair);
            P.Write((ushort)0);
            P.Write(Player.Money);
            P.Write(Player.Cps);
            P.Write(Player.Exp);
            P.Write(0);
            P.Write(0);
            P.Write(0);
            P.Write(0);
            P.Write(Player.Strength);
            P.Write(Player.Agility);
            P.Write(Player.Vitality);
            P.Write(Player.Spirit);
            P.Write(Player.AttributePoints);
            P.Write((short)Player.CurrentHP);
            P.Write((short)Player.CurrentMP);
            P.Write(Player.PkPoints);
            P.Write(Player.Level);
            P.Write(Player.Class);
            P.Write((short)0);
            P.Write((byte)1);
            P.Write((byte)2);
            P.Write((byte) (Player.IsReborn ? 1 : 0));
            //P.Zerofill(3);
            P.Write(Player.Name);
            P.Write(Player.Spouse);
            return P.Finish();
        }
    }
}