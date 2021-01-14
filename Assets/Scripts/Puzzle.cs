using UnityEngine;
using Mirror;

public abstract class Puzzle: NetworkBehaviour{
    public abstract void OnServerInitialize();
    public abstract void OnLocalPlayerReady(NetworkIdentity player);
}