using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class LocalPlayerAnnouncer : NetworkBehaviour{
    public static event Action<NetworkIdentity> OnLocalPlayerUpdated;
    private static NetworkIdentity localStaticPlayer;

    public static void RunOnLocalPlayer(Action<NetworkIdentity> func){
        if(ClientScene.localPlayer != null){
            func(ClientScene.localPlayer);
        }else{
            OnLocalPlayerUpdated += func;
        }
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