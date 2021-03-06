﻿using System;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Sockets;
using CrystalEmuLogin.Networking.Packets;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Handlers
{
    public static class MsgLogin
    {
        public static void Handle(Player Player, byte[] Packet)
        {
            if (Player == null || Packet.Length != Packet.Size())
                return;

            Player.UID = Packet.ToUInt(4);
            var ServerKey = Packet.ToUInt(8);
            var Language = Packet.StringFrom(14, 2);
            var Version = Packet.ToUInt(24);

            if (!VerifyLanguage(Player, Language))
                Player.Disconnect();
            if (!VerifyKey(Player, ServerKey))
                Player.Disconnect();
            if (!VerifyVersion(Player, Version))
                Player.Disconnect();

            (Player.Socket.Crypto as ConquerStanderedCipher)?.SetKeys(ServerKey, Player.UID);
            var Message = new MsgText {Color =0xFF0000, From = "SYSTEM", To = "ALLUSERS", Message = "ANSWER_OK", Type = MsgTextType.LoginInformation};

            Player.Send(Message);
            Player.Send(CoPacket.MsgHero(Player));
        }

        private static bool VerifyVersion(Player Player, uint Version)
        {
            if (Version == 0)
                return true;
            var Message = new MsgText { Color = 0xFF0000, From = "SYSTEM", To = "ALLUSERS", Message = "Wrong Version", Type = MsgTextType.LoginInformation };
            Player.Send(Message);
            return false;
        }

        private static bool VerifyKey(Player Player, uint Key)
        {
            var ComputedKey = Security.Hash((short)(Player.UID - 1000000), (short)(Player.UID - 999999));
            Core.WriteLine("µKey: " + ComputedKey + " | Expected: " + Key, ConsoleColor.Magenta);
            if (Key == ComputedKey)
                return true;
            var Message = new MsgText { Color = 0xFF0000, From = "SYSTEM", To = "ALLUSERS", Message = "Wrong Key", Type = MsgTextType.LoginInformation };
            Player.Send(Message);
            return false;
        }

        private static bool VerifyLanguage(Player Player, string Language)
        {
            if (Language == "En")
                return true;

            var Message = new MsgText { Color = 0xFF0000, From = "SYSTEM", To = "ALLUSERS", Message = "Wrong Language", Type = MsgTextType.LoginInformation };
            Player.Send(Message);
            return true;
        }
    }
}