using System;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLogin.CoreSystems;
using CrystalEmuLogin.Networking.IPC_Comms;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Handlers
{
    public static class MsgActionHandler
    {
        public static void Handle(Player Player, MsgAction Packet)
        {
            switch (Packet.Action)
            {
                case MsgActionType.MapShow:
                {
                    ProcessLogin(Player, Packet);
                    break;
                }
                case MsgActionType.Jump:
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
                    Core.WriteLine("Unhandled MsgAction Type: " + Packet.Action, ConsoleColor.Red);
                    break;
                }
            }
        }

        private static async void ProcessLogin(Player Player, MsgAction Packet)
        {
            Kernel.Players.TryAdd(Player.UID, Player);

            await DatabaseConnection.LoadCharacter(Player);
            if (await DatabaseConnection.FindSpawnPoint(Player))
            {
                Player.Send(CoPacket.MsgHero(Player));

                Packet.TimeStamp = (uint)Environment.TickCount;
                Packet.UID = Player.UID;
                Packet.Offset12Big = Player.Z;
                Packet.Offset16 = Player.X;
                Packet.Offset18 = Player.Y;
                Packet.Action = MsgActionType.MapShow;

                Player.Send(Packet);
           }
            else
                Player.Disconnect();

            Player.Send(CoPacket.MsgDialog("Hello!",0,10, MsgDialogType.Text));
            Player.Send(CoPacket.MsgDialog("Hi!",1,0, MsgDialogType.Link));
            Player.Send(CoPacket.MsgDialog("",0,10,MsgDialogType.Face));
            Player.Send(CoPacket.MsgDialog("",0,0,MsgDialogType.End));
            Player.Send(CoPacket.MsgTime());
        }
    }
}