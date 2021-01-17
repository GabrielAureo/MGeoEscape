using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerVisibility : NetworkVisibility
{
    [EnumFlag] [SerializeField]
    Character characterObservers = (Character)VisibleFlag;

    public const int VisibleFlag = 0b111;

    public override void OnStartServer()
    {
        var manager = (MainNetworkManager) NetworkManager.singleton;
        manager.OnServerPlayersSpawn += () => netIdentity.RebuildObservers(false);
    }
    [Server]
    public void SetObserverFlag(int flag){
        characterObservers = (Character) flag;
        GetComponent<NetworkIdentity>().RebuildObservers(false);
    }
    [Server]
    public int GetObserverFlag(){
        return (int) characterObservers;
    }

    public override bool OnCheckObserver(NetworkConnection conn)
    {
        if (conn == null) return false;
        var player = conn.identity.GetComponent<GamePlayer>();
        if (player == null) return false;
        var character = player.character;
        return (characterObservers & character) != 0;
    }

    public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {

            if (conn == null) continue;
            var player = conn.identity.GetComponent<GamePlayer>();
            
            var character = player.character;

            if((characterObservers & character) != 0){
                observers.Add(conn);
            }
        }
    }

    
}