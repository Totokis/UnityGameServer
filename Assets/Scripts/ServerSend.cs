using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend 
{
    public static void Welcome(int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(message);
                packet.Write(toClient);
                SendTcpData(toClient, packet);
                Debug.Log("Welcome packet send");
            }
        }
        
        public static void SpawnPlayer(int toClient, Player clientPlayer)
        {
            using Packet packet = new Packet((int)ServerPackets.SpawnPlayer);
            packet.Write(clientPlayer.Id);
            packet.Write(clientPlayer.Username);
            packet.Write(clientPlayer.transform.position);
            packet.Write(clientPlayer.transform.rotation);
                
            SendTcpData(toClient,packet);
        }

        public static void SendMessageToAll(string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.Message))
            {
                packet.Write(message); 
                SendTcpDataToAll(packet);
            }
        }
        
        private static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClient].Tcp.SendData(packet);
        }

        private static void SendUdpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClient].Udp.SendData(packet);
        }


        private static void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].Tcp.SendData(packet); 
            }
        }
        
        private static void SendTcpDataToAll(int exceptPlayer, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i == exceptPlayer)
                    continue;

                Server.Clients[i].Tcp.SendData(packet);
            }
        }
        
        private static void SendUdpDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].Udp.SendData(packet); 
            }
        }

        private static void SendUdpDataToAll(int exceptPlayer, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i == exceptPlayer)
                    continue;

                Server.Clients[i].Udp.SendData(packet);
            }
        }
        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
            {
                packet.Write(player.Id);
                packet.Write(player.transform.position);
                
                SendUdpDataToAll(packet);
            }
            
        }
        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
            {
                packet.Write(player.Id);
                packet.Write(player.transform.rotation);
                
                SendUdpDataToAll(player.Id,packet);
            }
        }

        public static void PlayerDisconnected(int playerId)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerDisconnect))
            {
                packet.Write(playerId);
                SendTcpDataToAll(packet);
            }
        }

        public static void PlayerHealth(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerHealth))
            {
                packet.Write(player.Id);
                packet.Write(player.Health);
                SendTcpDataToAll(packet);
            } 
        }

        public static void PlayerRespawned(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerRespawned))
            {
                packet.Write(player.Id);
                SendTcpDataToAll(packet);
            }
        }
        
        

}
