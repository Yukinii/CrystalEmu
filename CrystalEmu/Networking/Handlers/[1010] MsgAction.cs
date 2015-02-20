using System;
using System.Threading.Tasks;
using CrystalEmu.CoreSystems;
using CrystalEmu.Networking.IPC_Comms;
using CrystalEmu.Networking.Packets;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;

namespace CrystalEmu.Networking.Handlers
{
    public static class MsgAction
    {
        public static async Task Handle(Player Player, byte[] Packet)
        {
            var SubType = (MsgActionType) Packet[22];

            switch (SubType)
            {
                case MsgActionType.MapShow:
                {
                    await ProcessLogin(Player);
                    break;
                }
                case MsgActionType.Jump:
                {
                    ProcessJump(Player, Packet);
                    break;
                }
                case MsgActionType.Action:
                case MsgActionType.ChangeDirection:
                case MsgActionType.ChangeFace:
                case MsgActionType.ChangeMap:
                case MsgActionType.ChangePkMode:
                case MsgActionType.CompleteMapChange:
                case MsgActionType.ConfirmFriends:
                case MsgActionType.ConfirmGuild:
                case MsgActionType.ConfirmProf:
                case MsgActionType.ConfirmSkills:
                case MsgActionType.CorrectCords:
                case MsgActionType.DeleteChar:
                case MsgActionType.Dialog:
                case MsgActionType.DropMagic:
                case MsgActionType.DropSkill:
                case MsgActionType.EndFly:
                case MsgActionType.EndTransform:
                case MsgActionType.EndXpList:
                case MsgActionType.EntityRemove:
                case MsgActionType.EntitySpawn:
                case MsgActionType.FinishTeleport:
                case MsgActionType.Hotkeys:
                case MsgActionType.Leveled:
                case MsgActionType.Mine:
                case MsgActionType.OnTeleport:
                case MsgActionType.OpenShop:
                case MsgActionType.PickupCashEffect:
                case MsgActionType.Portal:
                case MsgActionType.QueryEnemyInfo:
                case MsgActionType.QueryFriendInfo:
                case MsgActionType.QueryTeamLeader:
                case MsgActionType.QueryTeamMemberPos:
                case MsgActionType.RemoteCommands:
                case MsgActionType.RemoveWeaponMesh:
                case MsgActionType.RemoveWeaponMesh2:
                case MsgActionType.Revive:
                case MsgActionType.SpawnEffect:
                case MsgActionType.StartVending:
                case MsgActionType.StopVending:
                case MsgActionType.Sync:
                case MsgActionType.ViewOthersEquip:
                {
                    Core.WriteLine("Unhandled MsgAction Type: " + (byte) SubType, ConsoleColor.Red);
                    break;
                }
            }
        }

        private static async Task ProcessLogin(Player Player)
        {
            Kernel.Players.TryAdd(Player.UID, Player);

            if (await DatabaseConnection.FindSpawnPoint(Player))
            {
                await DatabaseConnection.LoadCharacer(Player);
                Player.Send(CoPacket.MsgHero(Player));
                Player.Send(CoPacket.GeneralData(Player.UID, (ushort)Player.Z, (ushort)Player.X, (ushort)Player.Y, MsgActionType.MapShow));
            }
            else
            {
                Player.Disconnect();
            }
        }

        private static void ProcessJump(Player Player, byte[] Packet)
        {
            var Timestamp = Packet.ToUInt(4);
            var UID       = Packet.ToUInt(8);
            var X         = Packet.ToUShort(12);
            var Y         = Packet.ToUShort(14);
            var CurrentX  = Packet.ToUShort(16);
            var CurrentY  = Packet.ToUShort(18);

            if (Player.UID != UID)
                Player.Disconnect();
            if (Player.X != CurrentX || Player.Y != CurrentY)
                Player.Disconnect();
            if (Player.LastJump + 300 > Timestamp)
                Player.Disconnect();

            Player.LastJump = Timestamp;
            Player.X = X;
            Player.Y = Y;
            Player.Direction = 0;
        }
    }
}