using System;
using System.Threading.Tasks;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;

namespace CrystalEmu.Networking.Handlers
{
    public static class MsgItem
    {
        public static async Task Handle(Player Player, byte[] Packet)
        {
            var SubType = (MsgItemType)Packet[12];

            switch (SubType)
            {
                case MsgItemType.Ping:
                {
                    ProcessPing(Player, Packet);
                    break;
                }
                case MsgItemType.AddVendingItem:
                case MsgItemType.BoothAddCP:
                case MsgItemType.BuyItem:
                case MsgItemType.BuyVendingItem:
                case MsgItemType.DepositWarehouseMoney:
                case MsgItemType.DropGold:
                case MsgItemType.Enchant:
                case MsgItemType.EquipItem:
                case MsgItemType.ParticleEffect:
                case MsgItemType.RemoveEquipment:
                case MsgItemType.RemoveInventory:
                case MsgItemType.RemoveVendingItem:
                case MsgItemType.RepairItem:
                case MsgItemType.SellItem:
                case MsgItemType.SetEquipPosition:
                case MsgItemType.ShowVendingList:
                case MsgItemType.ShowWarehouseMoney:
                case MsgItemType.UnEquipItem:
                case MsgItemType.UpdateArrowCount:
                case MsgItemType.UpdateDurability:
                case MsgItemType.UpgradeDragonball:
                case MsgItemType.UpgradeMeteor:
                case MsgItemType.WithdrawWarehouseMoney:
                {
                    Core.WriteLine("Unhandled MsgItem Type: " + (byte)SubType, ConsoleColor.Red);
                    break;
                }
            }
        }

        private static void ProcessPing(Player Player, byte[] Packet)
        {
            var UID = Packet.ToUInt(4);

            if (UID != Player.UID)
                Player.Disconnect();

            Player.Send(Packet);
        }
    }
}