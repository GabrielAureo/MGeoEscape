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
    Socket.ITransfer currentTransfer;


    
    public void SetupController(ARTouchController touchController){
        this.hinge = touchController.hinge;
        //touchController.onTouch.AddListener(Touch);
        touchController.onHold.AddListener(Grab);
        touchController.onRelease.AddListener(Release);

    }

    /*void Touch(ARTouchData touchData){
        if(!(touchData.selectedInteractable is Socket)) return;

        var socket = touchData.selectedInteractable as Socket;
        if(socket.busy) return;
        CmdTouch(socket.gameObject);          
    }
    [Command]
    void CmdTouch(GameObject socketObj){
        var socket = socketObj.GetComponent<Socket>();  
        socket.busy = true;
        //lastSocketNetIdentity = socketObj;
        currentMovable = socket.currentObject;
        
    }*/
    void Grab(ARTouchData touchData){
        if(!(touchData.selectedInteractable is Socket)) return;
        var socket = touchData.selectedInteractable as Socket;
        CmdGrab(socket.netIdentity);
        
    }
    [Command]
    void CmdGrab(NetworkIdentity socketNetIdentity){
        var playerCharacter = connectionToClient.identity.GetComponent<GamePlayer>().character;

        currentTransfer = socketNetIdentity.GetComponent<Socket>().TryTake();
        if(currentTransfer == null) return;

        var movable = currentTransfer.GetMovable();
        
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
        var socket = socketNetIdentity.GetComponent<Socket>();
        var movable = socket.currentObject;
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
        

        if(!(touchData.selectedInteractable is Socket)) return;
        Socket lastSocket = (Socket)touchData.selectedInteractable;

        Socket target = null;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(touchData.ray);

        if(hits.Length > 0){
            foreach(var hit in hits){
                target = hit.transform.GetComponent<Socket>();
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

    [Server]
    void ReturnToSourceSocket(Socket lastSocket){
        if(lastSocket.ReturnMovable(currentTransfer)){
            currentTransfer = null;
            lastSocket = null;
        }else{
            Debug.LogError("Could not return object to source");
        }
        
        
    }

    [Command]
    void CmdPlace(NetworkIdentity targetNetIdentity, NetworkIdentity lastSocketNetworkIdentity){
        TargetPlace();
        if(targetNetIdentity == null){
            var _lastSocket = lastSocketNetworkIdentity.GetComponent<Socket>();
            ReturnToSourceSocket(_lastSocket);
            return;
        }
        var target = targetNetIdentity.GetComponent<Socket>();
        
        bool placed = false;
        var lastSocket = lastSocketNetworkIdentity.GetComponent<Socket>();
        
       

        if(target != null) placed = target.TryPlaceObject(lastSocket);

        if(!placed){
            ReturnToSourceSocket(lastSocket);
        }else{
            lastSocket.EndTransfer(currentTransfer);
        }
        
        lastSocket = null;
        hinge.connectedBody = null;
    }
    [TargetRpc]
    void TargetPlace(){
        currentMovable.ReleaseAnimation();
        currentMovable = null;
    }




}