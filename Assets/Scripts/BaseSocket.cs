using UnityEngine;
using Mirror;
using UnityEngine.Events;
public abstract class BaseSocket : ARNetInteractable
{
    [Server]
    public abstract SocketTransfer TryTake();
    [Client]
    public abstract Movable ClientGetMovable();
    [Server]
    public abstract Movable GetCurrentObject();
    [Server]
    public abstract bool TryPlaceObject(Movable movable);
    public override void onHold(){}

    public override void onRelease(){}

    public override void onTap(){}

    public override void onTarget(Movable movable){}

    public override void onUntarget(Movable movable){}
}