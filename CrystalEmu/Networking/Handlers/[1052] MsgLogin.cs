using System;
using System.Threading.Tasks;
using CrystalEmu.Networking.IPC_Comms;
using CrystalEmu.Networking.Packets;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Sockets;

namespace CrystalEmu.Networking.Handlers
{
    public static class MsgLogin
    {
        public static async Task Handle(Player Player, byte[] Packet)
        {
            if (Player == null || Packet.Length != Packet.Size())
                return;
            
            Player.UID = Packet.ToUInt(4);
            var ServerKey = Packet.ToUInt(8);
            var Language = Packet.StringFrom(14, 2);
            var Version = Packet.ToUInt(24);

            if(!VerifyLanguage(Player, Language))
                Player.Disconnect();
            if(!VerifyKey(Player, ServerKey))
                Player.Disconnect();
            if(!VerifyVersion(Player, Version))
                Player.Disconnect();
            
            await Player.InitializeDatabaseConnection();
            (Player.Socket.Crypto as ConquerStanderedCipher)?.SetKeys(ServerKey, Player.UID);
            Player.Send(CoPacket.MsgText(Player.UID, "SYSTEM", "ALLUSERS", "ANSWER_OK", MsgTextType.LoginInformation));
            Player.Send(CoPacket.MsgHero(Player));
        }

        private static bool VerifyVersion(Player Player,uint Version)
        {
            if (Version == 0)
                return true;

            Player.Send(CoPacket.MsgText(Player.UID, "SYSTEM", "ALLUSERS", "Wrong Version", MsgTextType.LoginInformation));
            return false;
        }

        private static bool VerifyKey(Player Player, uint Key)
        {
            var ComputedKey = Security.Hash((short) (Player.UID - 1000000), (short) (Player.UID - 999999));
            Core.WriteLine("µKey: " + ComputedKey + " | Expected: "+ Key, ConsoleColor.Magenta);
            if (Key == ComputedKey)
                return true;
            Player.Send(CoPacket.MsgText(Player.UID, "SYSTEM", "ALLUSERS", "Wrong Key", MsgTextType.LoginInformation));
            return false;
        }

        private static bool VerifyLanguage(Player Player, string Language)
        {
            if (Language == "En")
                return true;

            Player.Send(CoPacket.MsgText(Player.UID, "SYSTEM", "ALLUSERS", "Wrong Language", MsgTextType.LoginInformation));
            return true;
        }
    }
}
