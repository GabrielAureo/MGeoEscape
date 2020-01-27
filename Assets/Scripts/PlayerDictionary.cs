using UnityEngine;
using Mirror;
[System.Serializable]
public class PlayerDictionary: SyncDictionary<int, NetworkIdentity>{

    public void Add(Character key, LobbyPlayer value){

        Add((int)key, value?.netIdentity);
    }
}