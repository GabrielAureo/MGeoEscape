 using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class MovableController : NetworkBehaviour{
    
    public Movable currentMovable; //Current movable object being held by the controller. Is assigned by the socket on the holding phase of the touch controller
    HingeJoint hinge;
    GameObject lastSocketObj; //Sender 
    IARInteractable targetInteractable;


    
    public void SetupController(ARTouchController touchController){
        this.hinge = touchController.hinge;
        touchController.onTouch.AddListener(Touch);
        touchController.onHold.AddListener(Grab);
        touchController.onRelease.AddListener(Release);

    }

    void Touch(ARTouchData touchData){
        if(touchData.selectedInteractable is Socket){
            var socket = touchData.selectedInteractable as Socket;
            if(!socket.busy){
                CmdTouch(socket.gameObject);
            }
            
        }
    }
    [Command]
    void CmdTouch(GameObject socketObj){
        var socket = socketObj.GetComponent<Socket>();
        socket.busy = true;
        //lastSocketNetIdentity = socketObj;
        currentMovable = socket.currentObject;
        
    }
    void Grab(ARTouchData touchData){
        if(touchData.selectedInteractable is Socket){
            var socket = touchData.selectedInteractable as Socket;
            currentMovable = socket.currentObject;
            CmdGrab(socket.gameObject);
        }
    }
    [Command]
    void CmdGrab(GameObject socketObj){
        var containerObj = Instantiate(GameResources.Instance.movableContainer);
        Debug.LogError("Container: " + containerObj);
        var playerCharacter = connectionToClient.identity.GetComponent<GamePlayer>().character;

        var movable = socketObj.GetComponent<Socket>().TryTake();
        Debug.LogError(movable);

        //if(movable){
            Debug.LogError(socketObj.name);
            lastSocketObj = socketObj;

            NetworkServer.Spawn(containerObj);
                        
            RpcEmptySocket(socketObj);
            //TargetGrab(socketObj);
        //} 
        
    }
    
    [ClientRpc]
    void RpcGrab(){

    }
    [TargetRpc]
    void TargetGrab(GameObject socketObj)
	{
        var socket = socketObj.GetComponent<Socket>();
        var movable = socket.currentObject;
        movable.gameObject.SetActive(true);
        ConnectToHinge(movable);
    }

    [ClientRpc]
    void RpcEmptySocket(GameObject socketObj)
	{
        var socket = socketObj.GetComponent<Socket>();
        socket.currentObject.gameObject.SetActive(false);

        Debug.LogError("Disabled movable " + socket.currentObject.name + "from " + socket.name);
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
        

        //if(currentMovable){
            Socket target = (Socket)touchData.selectedInteractable;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(touchData.ray);

            if(hits.Length > 0){
                foreach(var hit in hits){
                    target = hit.transform.GetComponent<Socket>();
                    if(target != null) break;
                }
            }

            CmdPlace(target?.gameObject);            
            
        //}
    }

    public void ConnectToHinge(Movable movable){
        //currentMovable = movable;
        hinge.connectedBody = movable.rb;
        movable.transform.parent = null;
    }
    [Command]
    public void CmdPlace(GameObject targetObj){
        var target = targetObj.GetComponent<Socket>();
        
        bool placed = false;
        var lastSocket = lastSocketObj.GetComponent<Socket>();

        if(target != null) placed = target.TryPlaceObject(lastSocketObj);

        if(!placed){
            lastSocket.TryPlaceObject(lastSocketObj);
        }else{
            lastSocket.currentObject = null;
        }
        
        lastSocket.FreeSocket();
        

        lastSocket = null;
        currentMovable = null;
        hinge.connectedBody = null;
    }




}