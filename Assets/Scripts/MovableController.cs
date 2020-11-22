 using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class MovableController : NetworkBehaviour{
    /// <summary>
    /// Chached reference to the current movable object being held by the controller. Only available on the client to reduce traffic.
    /// </summary>
    public Movable currentMovable; 
    HingeJoint hinge;
    //GameObject lastSocketObj; //Sender 
    IARInteractable targetInteractable;
    SocketTransfer currentTransfer;


    
    public void SetupController(ARTouchController touchController){
        this.hinge = touchController.hinge;
        //touchController.onTouch.AddListener(Touch);
        touchController.onHold.AddListener(Grab);
        touchController.onRelease.AddListener(Release);

    }
    void Grab(ARTouchData touchData){
        if(!(touchData.selectedInteractable is BaseSocket)) return;
        var socket = touchData.selectedInteractable as BaseSocket;
        CmdGrab(socket.netIdentity);
        
    }
    [Command]
    void CmdGrab(NetworkIdentity socketNetIdentity){
        var playerCharacter = connectionToClient.identity.GetComponent<GamePlayer>().character;

        currentTransfer = socketNetIdentity.GetComponent<BaseSocket>().TryTake();
        if(currentTransfer == null) return;

        var movable = currentTransfer.movable;
        
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
    void TargetGrab(int character, NetworkIdentity socketNetIdentity)
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
            if(newTargetInteractable != targetInteractable){
                targetInteractable?.onTarget(currentMovable);
                targetInteractable?.onUntarget(currentMovable);
                targetInteractable = newTargetInteractable;
            }
            
        }else{
            targetInteractable?.onUntarget(currentMovable);
            targetInteractable = null;
        }
    }

    void Release(ARTouchData touchData){
        if(touchData.lastStatus == ARTouchData.Status.HOLDING){
            targetInteractable?.onUntarget(currentMovable);
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
        hinge.connectedBody = movable.rb;
    }

    // [Server]
    // void ReturnToSourceSocket(Socket lastSocket){
    //     if(lastSocket.ReturnMovable(currentTransfer)){
    //         currentTransfer = null;
    //         lastSocket = null;
    //     }else{
    //         Debug.LogError("Could not return object to source");
    //     }
        
        
    // }

    [Command]
    void CmdPlace(NetworkIdentity targetNetIdentity, NetworkIdentity lastSocketNetworkIdentity){
        print("Here");
        if(currentTransfer == null) return;
        var lastSocket = lastSocketNetworkIdentity.GetComponent<BaseSocket>();
        bool can_place = false;

        var transferCallback = currentTransfer.finishAction;
        
        if(targetNetIdentity == null){
            can_place = false;
            //ReturnToSourceSocket(lastSocket);
        }else{
            var target = targetNetIdentity.GetComponent<Socket>();
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
        hinge.connectedBody = null;
    }




}