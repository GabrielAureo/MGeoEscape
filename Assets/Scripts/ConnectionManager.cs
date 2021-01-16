using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;
using Mirror.Discovery;
public class ConnectionManager : MonoBehaviour{
    [SerializeField] NetworkDiscovery networkDiscovery = null;

    void Start(){
        #if !UNITY_EDITOR
            networkDiscovery.StartDiscovery();
        #endif
        DontDestroyOnLoad(this);
    }
    
    public void Connect(ServerResponse info){
        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            NetworkManager.singleton.StartClient(info.uri);
    }

    void OnGUI()
    {
        if (NetworkManager.singleton == null)
            return;

        if (NetworkServer.active || NetworkClient.active)
            return;

        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            DrawGUI();
    }

    void DrawGUI()
        {
            GUILayout.BeginHorizontal();

            // LAN Host
            if (GUILayout.Button("Start Host"))
            {
                NetworkManager.singleton.StartHost();
                networkDiscovery.AdvertiseServer();
            }

            // Dedicated server
            if (GUILayout.Button("Start Server"))
            {
                NetworkManager.singleton.StartServer();

                networkDiscovery.AdvertiseServer();
            }

            if(GUILayout.Button("Find Server"))
            {
                networkDiscovery.StartDiscovery();
            }

            GUILayout.EndHorizontal();

        }
}