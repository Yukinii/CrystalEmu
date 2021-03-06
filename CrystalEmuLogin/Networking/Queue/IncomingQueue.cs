﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using CrystalEmuLib;
using CrystalEmuLib.Enums;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;
using CrystalEmuLogin.Networking.Handlers;
using CrystalEmuLogin.PlayerFunctions;

namespace CrystalEmuLogin.Networking.Queue
{
    public static class IncomingQueue
    {
        private static void Loop()
        {
            while (true)
            {
                AutoResetEvent.WaitOne(10);

                while (Packets.Count > 0)
                {
                    PacketInfo Pi;
                    if (!Packets.TryDequeue(out Pi))
                        break;

                    switch (Pi.Packet.PacketID())
                    {
                        case PacketID.MsgConnect:
                        {
                            MsgConnect.Handle((Player)Pi.Owner, Pi.Packet);
                            break;
                        }
                        case PacketID.MsgAction:
                        {
                            MsgActionHandler.Handle((Player)Pi.Owner, Pi.Packet);
                            break;
                        }
                        case PacketID.MsgItem:
                        {
                            MsgItem.Handle((Player)Pi.Owner, Pi.Packet);
                            break;
                        }
                        case PacketID.MsgLogin:
                        {
                            MsgLogin.Handle((Player)Pi.Owner, Pi.Packet);
                            break;
                        }
                        default:
                        {
                            Core.WriteLine("Unknown Packet: " + Pi.Packet.PacketID(), ConsoleColor.DarkRed);
                            break;
                        }
                    }
                }
            }
        }

        public static void Add(Player P, byte[] Packet)
        {
            if (Packet == null || P == null)
                return;

            if (Packet.Length != Packet.Size())
                return;

            Packets.Enqueue(new PacketInfo {Owner = P, Packet = Packet});
            AutoResetEvent.Set();
        }

        public static void Stop()
        {
            ConsumerThread.Abort();
            Core.WriteLine("Incoming PacketQueue stopped!", ConsoleColor.Red);
        }

        public static void Start()
        {
            ConsumerThread.Start();
            Core.WriteLine("Incoming PacketQueue started!", ConsoleColor.Green);
        }

        public static readonly ConcurrentQueue<PacketInfo> Packets = new ConcurrentQueue<PacketInfo>();
        public static readonly Thread ConsumerThread = new Thread(Loop);
        public static readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
    }
}