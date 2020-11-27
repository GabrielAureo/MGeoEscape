using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class SocketNode: BaseSocket{
    
    private bool _locked;
    private bool _busy;
    private bool _empty;

    public Movable currentMovable
    {
        get
        {
            return _currentMovable;
        }
        set
        {
            _empty = value == null;
            _currentMovable = value;
        }
        
    }

    private Movable _currentMovable;
    public Func<Movable, bool> MovableAuth;

    public Movable exclusiveMovable;

    public void Initialize(){
        _busy = false;
        _empty = true;
        _locked = false;
    }
 
    public override SocketTransfer TryTake()
    {
        if (_locked || _busy || _empty || _locked) return null;
        _busy = true;
        return new SocketTransfer(currentMovable, OnFinish);
    }

    private void OnFinish(SocketTransfer.Status status)
    {
        switch (status)
        {
            case SocketTransfer.Status.Success:
                currentMovable = null;
                break;
            case SocketTransfer.Status.Failure:
                SetObject(currentMovable);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        _busy = false;
    }





    public override Movable ClientGetMovable()
    {
        return currentMovable;
    }

    public override Movable GetCurrentObject()
    {
        return currentMovable;
    }

    public override bool TryPlaceObject(Movable movable)
    {
        var isCompatible = MovableAuth(movable);

        if (!isCompatible || _busy || !_empty || _locked) return false;

        var visibility = GetComponent<PlayerVisibility>();

        var flag = (visibility == null) ? PlayerVisibility.visibleFlag : visibility.GetObserverFlag();

        movable.GetComponent<PlayerVisibility>().SetObserverFlag(flag);
        _currentMovable = movable;
        //movable.transform.parent = transform.parent;
        Debug.LogError(movable.netIdentity);
        
        SetObject(currentMovable);
        return true;
    }

    [Client]
    private void LockSocket()
    {
        _locked = true;
        foreach (var component in GetComponents<Collider>())
        {
            component.enabled = false;
        }
    }
    
    private void SetObject(Movable movable)
    {
        currentMovable = movable;
        var shouldLock = movable == exclusiveMovable;
        RpcSetObject(movable.netIdentity, shouldLock);
    }
    
    private void RpcSetObject(NetworkIdentity movableNetID, bool shouldLock)
    {
        if(shouldLock) LockSocket();
        var movable = movableNetID.GetComponent<Movable>();
        _currentMovable = movable; 
        movable.transform.position = transform.position;
        movable.rb.isKinematic = true;
    }
}