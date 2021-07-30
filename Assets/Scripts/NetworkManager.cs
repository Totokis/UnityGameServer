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
        
        Server.Start(5,26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f,0.5f,0f), Quaternion.identity).GetComponent<Player>();
    }
}
