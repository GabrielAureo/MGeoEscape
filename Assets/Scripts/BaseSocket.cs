using System;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseSocket : ARNetInteractable
{
    [SerializeField]
    public SocketCallbackController callbackController;

    /// <summary>
    /// Only available on the Server
    /// </summary>
    /// <returns></returns>
    public bool TryTake(out SocketTransfer transfer)
    {
        var taken = TakeOperation(out transfer);
        callbackController?.OnClientTake.Invoke(taken);
        return taken;
    }

    protected abstract bool TakeOperation(out SocketTransfer transfer);
    /// <summary>
    /// Only available on the Client. Use it for visuals.
    /// </summary>
    /// <returns></returns>
    public abstract Movable ClientGetMovable();
    /// <summary>
    /// Only available on the Server.
    /// </summary>
    /// <returns></returns>
    public abstract Movable GetCurrentObject();

    /// <summary>
    /// Only available on the server.
    /// </summary>
    /// <param name="movable"></param>
    /// <returns></returns>
    public bool TryPlaceObject(Movable movable)
    {
        var placed = ShouldPlace(movable);
        callbackController?.OnServerReceive.Invoke(placed);
        
        RpcPlace(movable.netIdentity, placed);
        return placed;
    }
    [ClientRpc]
    private void RpcPlace(NetworkIdentity movableIdentity, bool placed)
    {
        OnClientPlace(movableIdentity);
        callbackController?.OnClientReceive.Invoke(placed);
    }
    protected abstract bool ShouldPlace(Movable movable);

    protected abstract void OnClientPlace(NetworkIdentity movableIdentity);
    public override void onHold(){}
    
    public override void onRelease(){}
    
    public override void onTap(){}

    public override void onTarget(Movable movable){}

    public override void onUntarget(Movable movable){}
}