using UnityEngine;
using Mirror;
public class PlayerController: NetworkBehaviour{
    [SyncVar] public Character character;
}