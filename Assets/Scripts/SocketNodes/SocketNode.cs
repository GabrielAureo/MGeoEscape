using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.VFX;

public class SocketNode: BaseSocket{
    [SyncVar]
    private bool _locked;
    [SyncVar]
    private bool _busy;
    [SyncVar]
    private bool _empty;
    [HideInInspector]
    public string GUID = System.Guid.NewGuid().ToString();

    private Movable _currentMovable;
    [SyncVar(hook=nameof(SetObjectFromNetID))] private uint _currentMovableNetID;
    public SocketGraph graph;

    public Movable exclusiveMovable;
    [SerializeField] private VisualEffect vfx;

    private void Start()
    {
        SetTargetable();
    }
    

    public override void OnStartServer()
    {
        Initialize();
    }

    private void SetObjectFromNetID(uint oldValue, uint newValue)
    {
        _currentMovable = (newValue == 0) ? null : NetworkIdentity.spawned[newValue].GetComponent<Movable>();
    }
    void SetTargetable()
    {
        var targetable = GetComponent<Targetable>();
        if (targetable == null) return;
        targetable.TargetCondition = PlacementCondition;
        targetable.TargetPose = movable =>
        {
            var transform1 = transform;
            var pose = new MovablePlacementPose()
            {
                position = transform1.position,
                rotation = transform1.rotation,
                scale = transform1.localScale
            };
            return pose;
        };
    }

    private void Initialize(){
        _busy = false;
        _empty = true;
        _locked = false;
    }
 
    protected override bool TakeOperation(out SocketTransfer transfer)
    {
        if (_locked || _busy || _empty)
        {
            transfer = null;
            return false;
        }
        _busy = true;
        transfer = new SocketTransfer(_currentMovable, OnFinish);
        return true;
    }

    private void OnFinish(SocketTransfer.Status status)
    {
        switch (status)
        {
            case SocketTransfer.Status.Success:
                _currentMovableNetID = 0;
                _empty = true;
                break;
            case SocketTransfer.Status.Failure:
                RpcReturnToSocket();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        _busy = false;
    }

   

    public override Movable ClientGetMovable()
    {
        return _currentMovable;
    }

    public override Movable GetCurrentObject()
    {
        return _currentMovable;
    }

    protected override bool ShouldPlace(Movable movable)
    {
        if (!PlacementCondition(movable)) return false;

        var visibility = GetComponent<PlayerVisibility>();

        var flag = (visibility == null) ? PlayerVisibility.VisibleFlag : visibility.GetObserverFlag();
        _currentMovableNetID = movable.netId;
        movable.GetComponent<PlayerVisibility>().SetObserverFlag(flag);
        _empty = false;
        Debug.LogError(movable.netIdentity);
        return true;
        
        
    }

    private bool PlacementCondition(Movable movable)
    {

        var isCompatible = graph.CompatibleMovable(movable);

        return isCompatible && !_busy && _empty && !_locked;
    }

    public void PlayPlacementAnimation(bool placed)
    {
        Debug.Log($"placed {placed}, locked {_locked}");
        if (!placed || !_locked) return;
       
        vfx.SendEvent("OnExclusivePlace");

    }

    protected override void OnClientPlace(NetworkIdentity movableIdentity)
    {
        var movable = movableIdentity.GetComponent<Movable>();
        SetObject(movable);
    }

    [ClientRpc]
    private void RpcReturnToSocket()
    {
        SetObject(_currentMovable);
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
        var shouldLock = movable == exclusiveMovable;
        SetTransform(movable);
        if(shouldLock) LockSocket();
    }
    
    private void SetTransform(Movable movable)
    {
        _currentMovable = movable;
        var movableTransform = movable.transform;
        var socketTransform = transform;
        movable.rb.isKinematic = true;
        movableTransform.position = socketTransform.position;
        movableTransform.rotation = socketTransform.rotation;
        
    }
}