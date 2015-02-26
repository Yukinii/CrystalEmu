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
                case MsgActionType.FinishTeleport:
                {
                    ProcessFinishTeleport(Player, Packet);
                    break;
                }
                case MsgActionType.Jump:
                case MsgActionType.Action:
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

        private static void ProcessFinishTeleport(Player Player, MsgAction Packet)
        {
            Selector.LoadCharacters(Player);

            var Dialog = new MsgDialog();
            Dialog.AddText("Noooooooooooo my friend! Its a TOTAL RECALL!");
            Dialog.AddText("No my friend! Have I answered your questions satisfactorily and offered good customer service?");
            Dialog.AddOption("You haven't answered any of my questions!", 255);
            Dialog.AddOption("Fuuuuuuuuuuck youuuu!", 255);
            Dialog.AddFace(194);
            Player.Send(Dialog);
            Player.Send(new MsgText { Message = "Ready!", From = "CrystalEmu", Type = MsgTextType.Action });
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
                Packet.Offset12Big = Player.Location.Z;
                Packet.Offset16 = Player.Location.X;
                Packet.Offset18 = Player.Location.Y;
                Packet.Action = MsgActionType.MapShow;
                Player.Send(Packet);
           }
            else
                Player.Disconnect();

            Player.Send(CoPacket.MsgUpdate(Player.UID, 0x20, MsgUpdateType.StatusEffect));
            Player.Send(new MsgText { Message = "Loading...", From = "CrystalEmu", Type = MsgTextType.Action });
        }
    }
}