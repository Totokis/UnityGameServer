using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    [SerializeField] GameObject playerPrefab;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = Constants.TICKS_PER_SEC;
        QualitySettings.vSyncCount = 0;
    #if UNITY_EDITOR
        Debug.Log("Build the project to start the server");
    #else
        Server.Start(5,26950);
    #endif
    }
    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
}
