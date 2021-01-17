using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class LocalPlayerAnnouncer : NetworkBehaviour{
    public static event Action<NetworkIdentity> OnLocalPlayerUpdated;
    
    public static void RunOnLocalPlayer(Action<NetworkIdentity> func){
        func(ClientScene.localPlayer);
        OnLocalPlayerUpdated += func;   
    }
    
    public override void OnStartLocalPlayer(){
        base.OnStartLocalPlayer();
        OnLocalPlayerUpdated?.Invoke(base.netIdentity);
    }

    private void OnDestroy(){
        if(base.isLocalPlayer){
            OnLocalPlayerUpdated?.Invoke(null);
        }
    }

}