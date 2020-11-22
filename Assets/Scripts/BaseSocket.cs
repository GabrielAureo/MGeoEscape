using UnityEngine;
using Mirror;
using UnityEngine.Events;
public abstract class BaseSocket : ARNetInteractable
{
    /// <summary>
    /// Only available on the Server
    /// </summary>
    /// <returns></returns>
    public abstract SocketTransfer TryTake();
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
    public abstract bool TryPlaceObject(Movable movable);
    public override void onHold(){}

    public override void onRelease(){}

    public override void onTap(){}

    public override void onTarget(Movable movable){}

    public override void onUntarget(Movable movable){}
}