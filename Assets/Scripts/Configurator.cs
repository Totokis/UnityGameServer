using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class Configurator : MonoBehaviour
{
   [SerializeField] bool isRemote = false;
   List<ConnectedPlayer> _connectedPlayers;

   private void Awake()
   {
      if (isRemote)
      {
         StartRemoteServer();
      }
   }
   private void StartRemoteServer()
   {
      Debug.Log("[ServerStartUp].StartRemoteServer");
      _connectedPlayers = new List<ConnectedPlayer>();
      PlayFabMultiplayerAgentAPI.Start();
      PlayFabMultiplayerAgentAPI.IsDebugging = configuration.playFabDebugging;
      PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
      PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
      PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
      PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;
   }

}
