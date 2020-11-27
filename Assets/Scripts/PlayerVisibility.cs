using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerVisibility : NetworkVisibility
{
    [EnumFlag] [SerializeField]
    Character characterObservers = (Character)visibleFlag;

    public readonly static int visibleFlag = 0b111;
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
        var character = conn.identity.GetComponent<GamePlayer>().character;
        return (characterObservers & character) != 0;
    }

    public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values){
            var character = conn.identity.GetComponent<GamePlayer>().character;
            if((characterObservers & character) != 0){
                observers.Add(conn);
            }
        }
    }
}