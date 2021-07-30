using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client 
{ 
    public int Id;
    public Player Player;
    public TCP Tcp;
    public UDP Udp;
    private static int dataBufferSize = 4096;

    public Client(int clientId)
    {
        Id = clientId;
        Tcp = new TCP(Id);
        Udp = new UDP(Id);
    }

    public class TCP
    {
        readonly int _id;
        NetworkStream _stream;
        byte[] _receiveBuffer;
        Packet _receivedData;
        
        public TcpClient Socket;

        public TCP(int id)
        {
            _id = id;
        }

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.ReceiveBufferSize = dataBufferSize;
            Socket.SendBufferSize = dataBufferSize;

            _stream = socket.GetStream();
            _receivedData = new Packet();
            _receiveBuffer = new byte[dataBufferSize];

            _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            ServerSend.Welcome(_id,"Welcome to the server" );
            
        }
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = _stream.EndRead(result);
                if (byteLength <= 0)
                {
                    Server.Clients[_id].Disconnect();
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(_receiveBuffer,data,byteLength);
                _receivedData.Reset(HandleData(data));
                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback,null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving TCP data {e}");
                Server.Clients[_id].Disconnect();
                throw;
            }
        }
        
        private bool HandleData(byte[] data)
        {
            int packetLength = 0;
            _receivedData.SetBytes(data);
     
            if (_receivedData.UnreadLength() >= 4)
            {
                packetLength = _receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
            {
                byte[] packetBytes = _receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() => {
                    using(Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.PacketHandlers[packetId](_id,packet);
                    }
                });
                packetLength = 0;
        
                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();
                    if (packetLength <= 0)
                        return true;
                }

                if (packetLength <= 1)
                    return true;
            }
            return false;
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket != null)
                {
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    Debug.Log("Sending data..");

                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to player {_id} via TCP: {e}");
                throw;
            }
        }

        public void Disconnect()
        {
            Socket.Close();
            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            Socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint EndPoint;
        int _id;

        public UDP(int id)
        {
            _id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public void SendData(Packet packet)
        {
            Server.SendUdpData(EndPoint, packet);
        }

        public void HandleData(Packet packet)
        {
            int packetLength = packet.ReadInt();
            byte[] packetBytes = packet.ReadBytes(packetLength);
            
            ThreadManager.ExecuteOnMainThread(() => {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    Server.PacketHandlers[packetId](_id, packet);
                }
            });
        }

        public void Disconnect()
        {
            EndPoint = null;
        }
        
    }

    public void SendIntoGame(string playerName)
    {
        Player = NetworkManager.Instance.InstantiatePlayer();
        Player.Initialize(Id, playerName);

        foreach (var client in Server.Clients.Values)
        {
            if (client.Player != null)
            {
                if (client.Id != Id)
                {
                    ServerSend.SpawnPlayer(Id, client.Player);
                }
            }
        }

        foreach (var client in Server.Clients.Values)
        {
            if(client.Player != null)
            {
                ServerSend.SpawnPlayer(client.Id, Player);
            }
        }
    }

    void Disconnect()
    {
        Debug.Log($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected");
        ThreadManager.ExecuteOnMainThread(() => {
            UnityEngine.Object.Destroy(Player.gameObject);
            Player = null;
        });
        Tcp.Disconnect();
        Udp.Disconnect();
        
        ServerSend.PlayerDisconnected(Id);  

    }

}
