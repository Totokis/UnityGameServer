using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class Server
{
    public static int MaxPlayers { get; private set; }

        private static int Port { get; set; }

        public static readonly Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        private static TcpListener _tcpListener;
        private static UdpClient _udpListener;
        
        public delegate void PacketHandler(int fromClient, Packet packet);

        public static Dictionary<int, PacketHandler> PacketHandlers;
        
        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;
            Debug.Log("Starting server...");
            InitializeServerData();
            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            _udpListener = new UdpClient(Port);
            _udpListener.BeginReceive(UdpReceiveCallback, null);
            Debug.Log("Server stared on: "+port);
        }
        private static void UdpReceiveCallback(IAsyncResult result)
        {
            try
            {
                var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = _udpListener.EndReceive(result, ref clientEndPoint);
                _udpListener.BeginReceive(UdpReceiveCallback, null);
                if (data.Length < 4)
                    return;

                using (var packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();
                    if (clientId == 0)
                    {
                        Debug.Log("Client ID equal 0");
                        return;
                    }
                    if (Clients[clientId].Udp.EndPoint == null)
                    {
                        Clients[clientId].Udp.Connect(clientEndPoint);
                        Debug.Log("Client endpoint equal null");
                        return;
                    }
                    if (Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
                    {
                        Clients[clientId].Udp.HandleData(packet);
                    }
                       
                }
               
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving UDP data {e}");
                throw;
            }
        }

        public static void SendUdpData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    _udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to {clientEndPoint} via UDP: {e}");
                throw;
            }
        }
        private static void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].Tcp.Socket != null)
                    continue;

                Clients[i].Tcp.Connect(client);
                return;
            }
            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server Full");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i,new Client(i));
            }

            PacketHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                
            };
            Debug.Log("Initialize packets");
        }
        
    }
