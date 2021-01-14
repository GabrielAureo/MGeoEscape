 using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class MovableController : NetworkBehaviour{
    /// <summary>
    /// Cached reference to the current movable object being held by the controller. Only available on the client to reduce traffic.
    /// </summary>
    public Movable currentMovable; 
    HingeJoint _hinge;
    //GameObject lastSocketObj; //Sender 
    private IARInteractable _targetInteractable;
    private SocketTransfer _currentTransfer;


    
    public void SetupController(ARTouchController touchController){
        this._hinge = touchController.hinge;
        //touchController.onTouch.AddListener(Touch);
        touchController.onHold.AddListener(Grab);
        touchController.onRelease.AddListener(Release);

    }

    private void Grab(ARTouchData touchData){
        if(!(touchData.selectedInteractable is BaseSocket)) return;
        var socket = touchData.selectedInteractable as BaseSocket;
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
    void Update(){
        if(currentMovable && ARTouchController.touchData.currentStatus == ARTouchData.Status.HOLDING){
            CheckTarget(ARTouchController.touchData);
        }
    }

    void CheckTarget(ARTouchData touchData){
        if(currentMovable == null) return;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(touchData.ray);
        IARInteractable newTargetInteractable = null;
        if(hits.Length>0){
            foreach(var hit in hits){
                newTargetInteractable = hit.transform.GetComponent<IARInteractable>();
                if(newTargetInteractable!= null) break;
            }
            if(newTargetInteractable != _targetInteractable){
                _targetInteractable?.onTarget(currentMovable);
                _targetInteractable?.onUntarget(currentMovable);
                _targetInteractable = newTargetInteractable;
            }
            
        }else{
            _targetInteractable?.onUntarget(currentMovable);
            _targetInteractable = null;
        }
    }

    void Release(ARTouchData touchData){
        if(touchData.lastStatus == ARTouchData.Status.HOLDING){
            _targetInteractable?.onUntarget(currentMovable);
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
        bool can_place = false;

        var transferCallback = _currentTransfer.finishAction;
        
        if(targetNetIdentity == null){
            can_place = false;
            //ReturnToSourceSocket(lastSocket);
        }else{
            var target = targetNetIdentity.GetComponent<BaseSocket>();
            if(target != null) can_place = target.TryPlaceObject(lastSocket.GetCurrentObject());
            // if(!can_place){
            //     //ReturnToSourceSocket(lastSocket);
            // }else{
            //     //lastSocket.EndTransfer(currentTransfer);
            // }
        }

        var status = (can_place)?SocketTransfer.Status.Success:SocketTransfer.Status.Failure;
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