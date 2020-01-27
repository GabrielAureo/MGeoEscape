using UnityEngine;
using Mirror;
public class GamePlayer: NetworkBehaviour{
    public string playerName;
    [SyncVar] public Character character;

    
}