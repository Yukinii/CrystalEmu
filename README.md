# CrystalEmu
A Conquer Online Emulator written in C# 6.0 using .NET 4.6
First open source Emulator to include Map Servers - Each Map has its own server. 
Fully managed code, no native libraries needed. Fully mono compatible.

#Introduction
After finishing the XioClassic source and giving up development on it (now belongs to the XioOnline Staff) I've decided to restart from zero once more. The only thing I'm using so far is Hybrids SocketNetwork.dll which was modified to make use of C# 6.0 language features and the async/await pattern. I don't really feel confident in writing my own Sockets. Xio has been using my modified version of hybrids for 12 straight months without any issue what so ever. 

#Main Features
I'll be using the latest framework / language features once again. C# 6.0 on Visual Studio 2015 with RyuJIT and .NET4.6. I always start a new conquer server with every new version of .NET cause I found a Conquer Server to be the best way to apply the majority of the new features in the past. I did this three times already - .NET 4.0, 4.5 and 4.5.2 - I usually spend 3-6 months on the project before discontinuing it. I have 'wasted' 12 months on XioClassic, built with .NET 4.5.2 and Async/Await. XioClassic was an experiment that turned out really well, but I'm sure I can do better now.
* Decentralized buffered Flatfile (INI) Server - WCF TCP Binding - Async/Await due to latency. We don't need to be stuck waiting for the data to come back
* Unlimited amount of MapServers - Create a Server per MapID or even per MapInstance. Servers do not have to be on the same Server nor on the same Network.
* Decentralized MessageServer for sending packets across every server (for chatting mainly)
* Decentralized Login server. Takes login requests, assigns players to the right Map Server and stuff.
* C# Scripting engine, not the bullshit Bauss implemented into Noitu a year ago :P Will use VOTC's one probably. Items, Mob AI, Npcs, .... I'll make a fair amount scriptable

#Points of Interest
This source will be utter shit. I will try to keep the code clean but in terms of efficiency I don't really expect much. It's mainly a playground for me to learn the new things in the upcoming Framework / Language Version. 
My casing is very strict. It's almost always PascalCasing. Private's have a underscore in front of _Them. This is my preference when I'm working alone. 

#Working Things:

* 1004 - Server -> Client yes, Client -> Server no
* 1005 - Walking, no dmap checks yet
* 1009 - Ping
* 1010 - MapShow, Jump
* 1017 - Done
* 1052 - Everything
