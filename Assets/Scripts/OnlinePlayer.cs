using UnityEngine;
using Mirror;
public class OnlinePlayer: NetworkBehaviour{
    [SyncVar] public Character character;
}