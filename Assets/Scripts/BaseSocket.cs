using System;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseSocket : ARNetInteractable
{
    [SerializeField]
    public SocketCallbackController callbackController;

    private bool hasCallbackController;

    private void OnValidate()
    {
        callbackController = GetComponent<SocketCallbackController>();
        hasCallbackController = callbackController != null;
        OnSocketValidate();
    }

    protected virtual void OnSocketValidate(){}

    /// <summary>
    /// Only available on the Server
    /// </summary>
    /// <returns></returns>
    public bool TryTake(out SocketTransfer transfer)
    {
        var taken = TakeOperation(out transfer);
        if (callbackController != null) callbackController.OnServerTake.Invoke(taken);

        if(transfer!=null && transfer.movable!=null) RpcTake(transfer.movable.netIdentity,taken);
        
        return taken;
    }

    [ClientRpc]
    private void RpcTake(NetworkIdentity movableIdentity, bool placed)
    {
        if (callbackController != null)
        {
            callbackController.OnClientTake.Invoke(placed);

        }
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
    protected void RpcPlace(NetworkIdentity movableIdentity, bool placed)
    {
        if(placed) OnClientPlace(movableIdentity);
        callbackController?.OnClientReceive.Invoke(placed);
    }
    protected abstract bool ShouldPlace(Movable movable);

    protected abstract void OnClientPlace(NetworkIdentity movableIdentity);
    public override void onHold(){}
    
    public override void onRelease(){}
    
    public override void onTap(){}
    
}