using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class ConnectionManager : MonoBehaviour{

    bool connected;

    void Start(){
        #if UNITY_EDITOR
            NetworkManager.singleton.StartHost();
        #endif
    }
    void Update(){
        #if UNITY_EDITOR
            return;
        #endif
        if(!NetworkClient.active){
            NetworkManager.singleton.StartClient();
        }    
        
    }
}