﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using CrystalEmu.PlayerFunctions;
using CrystalEmuLib;
using CrystalEmuLib.Extensions;
using CrystalEmuLib.Networking.Packets;

namespace CrystalEmu.Networking.Queue
{
    public static class OutgoingQueue
    {
        public static readonly ConcurrentQueue<PacketInfo> Packets = new ConcurrentQueue<PacketInfo>();
        public static readonly Thread ConsumerThread = new Thread(Loop);
        public static readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

        private static void Loop()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            while (true)
            {
                AutoResetEvent.WaitOne(10);

                while (Packets.Count > 0)
                {
                    PacketInfo Pi;
                    if (!Packets.TryDequeue(out Pi))
                        break;

                    ((Player)Pi.Owner).ForceSend(Pi.Packet);
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
            Core.WriteLine("Outgoing PacketQueue stopped!", ConsoleColor.Red);
        }

        public static void Start()
        {
            ConsumerThread.Start();
            Core.WriteLine("Outgoing PacketQueue started!", ConsoleColor.Green);
        }
    }
}