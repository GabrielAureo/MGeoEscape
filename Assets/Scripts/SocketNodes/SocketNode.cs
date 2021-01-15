using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class SocketNode: BaseSocket{
    [SyncVar]
    private bool _locked;
    [SyncVar]
    private bool _busy;
    [SyncVar]
    private bool _empty;
    [HideInInspector]
    public string GUID = System.Guid.NewGuid().ToString();

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

    private void Start()
    {
        SetTargetable();
    }

    void SetTargetable()
    {
        var targetable = GetComponent<Targetable>();
        if (targetable == null) return;
        targetable.TargetCondition = PlacementCondition;
        targetable.TargetPose = movable =>
        {
            var transform1 = movable.transform;
            var pose = new MovablePlacementPose()
            {
                position = transform.position,
                rotation = transform1.rotation,
                scale = transform1.localScale
            };
            return pose;
        };
    }

    public void Initialize(){
        _busy = false;
        _empty = true;
        _locked = false;
    }
 
    protected override bool TakeOperation(out SocketTransfer transfer)
    {
        if (_locked || _busy || _empty || _locked)
        {
            transfer = null;
            return false;
        }
        _busy = true;
        transfer = new SocketTransfer(currentMovable, OnFinish);
        return true;
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

    protected override bool ShouldPlace(Movable movable)
    {
        if (!PlacementCondition(movable)) return false;

        var visibility = GetComponent<PlayerVisibility>();

        var flag = (visibility == null) ? PlayerVisibility.visibleFlag : visibility.GetObserverFlag();

        movable.GetComponent<PlayerVisibility>().SetObserverFlag(flag);
        _currentMovable = movable;
        //movable.transform.parent = transform.parent;
        Debug.LogError(movable.netIdentity);
        
        SetObject(currentMovable);
        return true;
        
        
    }

    private bool PlacementCondition(Movable movable)
    {
        var isCompatible = MovableAuth(movable);

        return isCompatible && !_busy && _empty && !_locked;
    }

    protected override void OnClientPlace(NetworkIdentity movableIdentity)
    {
        SetObject(movableIdentity.GetComponent<Movable>());
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
        // RpcSetObject(movable.netIdentity, shouldLock);
        if(shouldLock) LockSocket();
        //var movable = movableNetID.GetComponent<Movable>();
        _currentMovable = movable; 
        movable.transform.position = transform.position;
        movable.rb.isKinematic = true;
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