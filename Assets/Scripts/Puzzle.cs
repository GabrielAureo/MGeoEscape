using UnityEngine;
using Mirror;

public abstract class Puzzle: NetworkBehaviour{
    public abstract void Initialize();
    public abstract void OnLocalPlayerReady(NetworkIdentity player);
}