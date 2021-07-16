using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using PlayFab;
// using PlayFab.MultiplayerAgent.Model;
using UnityEngine;

public class Configurator : MonoBehaviour
{
   [SerializeField] public bool isRemote = false;
   [SerializeField] bool isDebugging = false;
   //List<ConnectedPlayer> _connectedPlayers;

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
      // _connectedPlayers = new List<ConnectedPlayer>();
      // PlayFabMultiplayerAgentAPI.Start();
      // PlayFabMultiplayerAgentAPI.IsDebugging = isDebugging;
      // PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
      // PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
      // PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
      // PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

      //Server.OnPlayerAdded.AddListener(OnPlayerAdded);
      //Server.OnPlayerRemoved.AddListener(OnPlayerRemoved);

      StartCoroutine(ReadyForPlayers());
      StartCoroutine(ShutdownServerInXTime());
   }
   IEnumerator ShutdownServerInXTime()
   {
      yield return new WaitForSeconds(300f);
      StartShutdownProcess();
   }

   IEnumerator ReadyForPlayers()
   {
      yield return new WaitForSeconds(.5f);
      //PlayFabMultiplayerAgentAPI.ReadyForPlayers();
   }

   private void OnServerActive()
   {
      Server.Start(5,26950);
      Debug.Log("Server Started From Agent Activation");
   }

   private void OnPlayerRemoved(string playfabId)
   {
      // ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
      // _connectedPlayers.Remove(player);
      // PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
      CheckPlayerCountToShutdown();
   }

   private void OnPlayerAdded(string playfabId)
   {
      // _connectedPlayers.Add(new ConnectedPlayer(playfabId));
      // PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
   }
   private void CheckPlayerCountToShutdown()
   {
      // if (_connectedPlayers.Count <= 0)
      // {
      //    StartShutdownProcess();
      // }
   }


   private void OnAgentError(string error)
   {
      Debug.Log(error);
   }

   private void OnShutdown()
   {
      StartShutdownProcess();
   }

   private void StartShutdownProcess()
   {
      Debug.Log("Server is shutting down");
      ServerSend.SendMessageToAll("Server is shutting down");
      StartCoroutine(ShutdownServer());
   }

   IEnumerator ShutdownServer()
   {
      yield return new WaitForSeconds(5f);
      Application.Quit();
   }

   private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
   {
      Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
      ServerSend.SendMessageToAll("This is message with i dont know what to do: \n " +
                                  $"Maintenance scheduled for: {NextScheduledMaintenanceUtc.Value.ToLongDateString()}");
   }

}
