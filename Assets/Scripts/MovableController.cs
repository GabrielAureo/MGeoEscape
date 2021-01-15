 using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class MovableController : NetworkBehaviour{
    /// <summary>
    /// Cached reference to the current movable object being held by the controller. Only available on the client to reduce traffic.
    /// </summary>
    public Movable currentMovable;
    public bool isHolding;
    HingeJoint _hinge;
    //GameObject lastSocketObj; //Sender 
    private Targetable _currentTargetable;
    private SocketTransfer _currentTransfer;
    public Transform previewer;


    
    public void SetupController(ARTouchController touchController){
        this._hinge = touchController.hinge;
        isHolding = false;
        isTargeting = false;
        //touchController.onTouch.AddListener(Touch);
        touchController.onHold.AddListener(Grab);
        touchController.onRelease.AddListener(Release);
        previewer = new GameObject("Previewer", typeof(MeshFilter), typeof(MeshRenderer)).transform;
    }   

    private void Grab(ARTouchData touchData){
        if(!(touchData.selectedInteractable is BaseSocket)) return;
        var socket = touchData.selectedInteractable as BaseSocket;
        isHolding = true;
        CmdGrab(socket.netIdentity);
        
    }
    [Command]
    private void CmdGrab(NetworkIdentity socketNetIdentity){
        var playerCharacter = connectionToClient.identity.GetComponent<GamePlayer>().character;

        var placed = socketNetIdentity.GetComponent<BaseSocket>().TryTake(out _currentTransfer);
        if(!placed) return;

        var movable = _currentTransfer.movable;
        
        var vis = movable.GetComponent<PlayerVisibility>();
        vis.SetObserverFlag((int)playerCharacter);
        Debug.LogError(movable);

        if(movable){
            Debug.LogError(socketNetIdentity.name);
            //lastSocketObj = socketObj;                        
            //RpcEmptySocket(socketObj);
            TargetGrab((int)playerCharacter, socketNetIdentity);
        } 
        
    }
    [TargetRpc]
    private void TargetGrab(int character, NetworkIdentity socketNetIdentity)
	{
        var socket = socketNetIdentity.GetComponent<BaseSocket>();
        var movable = socket.ClientGetMovable();
        currentMovable = movable;
        movable.GrabAnimation();
        ConnectToHinge(movable);
    }

    [ClientCallback]
    void Update()
    {
        CheckTarget(ARTouchController.touchData);
    }

    private bool isTargeting;    
    void CheckTarget(ARTouchData touchData)
    {
        if (!isHolding) return;
        RaycastHit[] hits = new RaycastHit[10];
        var hitSize = Physics.RaycastNonAlloc(touchData.ray, hits);
        if (hitSize <= 0)
        {
            if (!isTargeting) return;
            if (_currentTargetable.useDefaultTargetAnimation)
            {
                previewer.gameObject.SetActive(false);
            }
            else
            {
                _currentTargetable.OnUntarget?.Invoke(currentMovable);
            }

            isTargeting = false;
            _currentTargetable = null;
            return;

        }
        Targetable newTargetable = null;
        foreach (var hit in hits)
        {
            newTargetable = hit.transform.GetComponent<Targetable>();
            if (newTargetable != null)
            {
                isTargeting = true;
                break;
            }
        }

        if (!newTargetable.ShouldTarget(currentMovable)) return;

        if (newTargetable != _currentTargetable)
        {
            if (newTargetable.useDefaultTargetAnimation)
            {
                var pose = newTargetable.TargetPose(currentMovable);
                SetPreviewer(pose);
            }
            else
            {
                newTargetable.OnTarget?.Invoke(currentMovable);
            }

            _currentTargetable = newTargetable;
        }
    }

    private void SetPreviewer(MovablePlacementPose pose)
    {
        previewer.position = pose.position;
        previewer.rotation = pose.rotation;
        previewer.localScale = pose.scale;

        var filter = previewer.GetComponent<MeshFilter>();
        filter.sharedMesh = currentMovable.mesh;

        previewer.gameObject.SetActive(true);
        
    }

    void Release(ARTouchData touchData)
    {
        if (touchData.lastStatus != ARTouchData.Status.HOLDING) return;
        
        if (_currentTargetable != null)
        {
            if (_currentTargetable.useDefaultTargetAnimation)
            {
                previewer.gameObject.SetActive(false);
            }
            else
            {
                _currentTargetable.OnUntarget.Invoke(currentMovable);
            }
        }


        if(!(touchData.selectedInteractable is BaseSocket)) return;
        BaseSocket lastSocket = (BaseSocket)touchData.selectedInteractable;

        BaseSocket target = null;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(touchData.ray);

        if(hits.Length > 0){
            foreach(var hit in hits){
                target = hit.transform.GetComponent<BaseSocket>();
                if(target != null) break;
            }
        }
        CmdPlace(target?.netIdentity, lastSocket.netIdentity);
        isHolding = false;

        //}
    }

    public void ConnectToHinge(Movable movable){
        //currentMovable = movable;
        movable.transform.parent = null;
        movable.rb.isKinematic = false;
        _hinge.connectedBody = movable.rb;
    }

    [Command]
    void CmdPlace(NetworkIdentity targetNetIdentity, NetworkIdentity lastSocketNetworkIdentity){
        if(_currentTransfer == null) return;
        var lastSocket = lastSocketNetworkIdentity.GetComponent<BaseSocket>();
        var canPlace = false;
        
        
        
        var transferCallback = _currentTransfer.finishAction;
        
        if(targetNetIdentity != null && targetNetIdentity != lastSocketNetworkIdentity){
            var target = targetNetIdentity.GetComponent<BaseSocket>();
            if(target != null) canPlace = target.TryPlaceObject(lastSocket.GetCurrentObject());
        }

        var status = (canPlace)?SocketTransfer.Status.Success:SocketTransfer.Status.Failure;
        Debug.Log(status);
        transferCallback(status);

        lastSocket = null;        
        TargetPlace();
    }
    [TargetRpc]
    void TargetPlace(){
        currentMovable.ReleaseAnimation();
        currentMovable = null;
        _hinge.connectedBody = null;
    }




}