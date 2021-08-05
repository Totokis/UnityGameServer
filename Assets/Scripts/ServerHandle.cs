using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and now player {fromClient}");
        Debug.Log($"Username: {username}");
        if (fromClient != clientIdCheck)    
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient} has assumed the wrong client ID ({clientIdCheck})");
        }
        Server.Clients[fromClient].SendIntoGame(username);
    }

    public static void PlayerMovement(int fromclient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();

        Server.Clients[fromclient].Player.SetInput(inputs, rotation);
    }

    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 shootDirection = packet.ReadVector3();
        Server.Clients[fromClient].Player.Shoot(shootDirection);
    } 
    
}
