using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class ConnectionManager : MonoBehaviour{
    void Start(){
        #if UNITY_EDITOR
            //NetworkManager.singleton.StartHost();
        #endif
        #if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(RefreshLAN());
        #endif
    }

    void OnEnable()
    {
        NetworkDiscovery.onReceivedServerResponse += Connect;
    }

    void OnDisable()
    {
        NetworkDiscovery.onReceivedServerResponse -= Connect;
    }

    void Connect(NetworkDiscovery.DiscoveryInfo info){
        if(NetworkClient.active) return;

        NetworkManager.singleton.networkAddress = info.EndPoint.Address.ToString();
        ((TelepathyTransport) Transport.activeTransport).port = ushort.Parse( info.KeyValuePairs[NetworkDiscovery.kPortKey] );
        NetworkManager.singleton.StartClient();
    }
    


    IEnumerator RefreshLAN(){
        while(!NetworkClient.active){
            NetworkDiscovery.SendBroadcast();
            yield return null;
        }
    }
}