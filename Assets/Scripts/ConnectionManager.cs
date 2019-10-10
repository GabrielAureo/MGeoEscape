using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class ConnectionManager : MonoBehaviour{

    void Start(){
        Connect();
    }
    void Connect(){
        #if UNITY_EDITOR
        NetworkManager.singleton.StartHost();
        #else
        NetworkDiscovery.onReceivedServerResponse += (info)=>{
            NetworkManager.singleton.networkAddress = info.EndPoint.Address.ToString();
            NetworkManager.singleton.StartClient();
            StopCoroutine(RefreshLAN());
        };
        StartCoroutine(RefreshLAN());
        #endif
        
    }
    IEnumerator RefreshLAN(){
        while(true){
            NetworkDiscovery.SendBroadcast();
            yield return new WaitForSeconds(1f);
        }
    }
}