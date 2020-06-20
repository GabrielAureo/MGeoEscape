using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class MovableController : NetworkBehaviour{
    
    public Movable currentMovable; //Current movable object being held by the controller. Is assigned by the socket on the holding phase of the touch controller
    HingeJoint hinge;
    [SyncVar]
    NetworkIdentity lastSocketNetIdentity;
    IARInteractable targetInteractable;

    void Start(){
        SetupController(MainManager.Instance.ARTouchController);
    }

    public void SetupController(ARTouchController touchController){
        this.hinge = touchController.GetComponent<HingeJoint>();
        touchController.onTouch.AddListener(TouchSocket);
        touchController.onHold.AddListener(CheckTarget);
        touchController.onRelease.AddListener(Release);

    }

    void TouchSocket(ARTouchData touchData){
        if(touchData.selectedInteractable is Socket){
            var socket = touchData.selectedInteractable as Socket;
            if(!socket.busy){
                CmdTouch(socket.gameObject);
            }
            
        }
    }
    [Command]
    void CmdTouch(GameObject socketObj){
        Debug.LogError("Called Command");
        var socket = socketObj.GetComponent<Socket>();
        socket.busy = true;
        //lastSocketNetIdentity = socketObj;
        currentMovable = socket.currentObject;
        RpcEmptySocket(socketObj);
        TargetGrab(connectionToClient, socketObj);
    }
    
    [TargetRpc]
    void TargetGrab(NetworkConnection target, GameObject socketObj)
	{
        var socket = socketObj.GetComponent<Socket>();
        var movable = socket.currentObject;
        movable.gameObject.SetActive(true);
        HoldMovable(movable);

    }

    [ClientRpc]
    void RpcEmptySocket(GameObject socketObj)
	{
        var socket = socketObj.GetComponent<Socket>();
        socket.currentObject.gameObject.SetActive(false);
        socket.gameObject.SetActive(false);
        Debug.LogError("Disabled movable " + socket.currentObject.name + "from " + socket.name);
	}

    void CheckTarget(ARTouchData touchData){
        if(currentMovable == null) return;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(touchData.ray);
        if(hits.Length>0){
            foreach(var hit in hits){
                targetInteractable = hit.transform.GetComponent<IARInteractable>();
                if(targetInteractable!= null) break;
            }
            
            targetInteractable?.onTarget(currentMovable);
        }else{
            targetInteractable?.onUntarget(currentMovable);
            targetInteractable = null;
        }
    }

    void Release(ARTouchData touchData){
        if(touchData.lastStatus == ARTouchData.Status.HOLDING){
            targetInteractable?.onUntarget(currentMovable);
        }
        

        if(currentMovable){
            Socket target = (Socket)touchData.selectedInteractable;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(touchData.ray);

            if(hits.Length > 0){
                foreach(var hit in hits){
                    target = hit.transform.GetComponent<Socket>();
                    if(target != null) break;
                }
            }

            ReleaseMovable(target);            
            
        }
    }

    public void HoldMovable(Movable movable){
        //currentMovable = movable;
        hinge.connectedBody = movable.rb;
        movable.transform.parent = null;
    }

    public void ReleaseMovable(Socket target){
        currentMovable.rb.isKinematic = true;
        bool placed = false;
        var lastSocket = lastSocketNetIdentity.GetComponent<Socket>();

        if(target != null) placed = target.TryPlaceObject(currentMovable);

        if(!placed) lastSocket.TryPlaceObject(currentMovable);

        lastSocket.FreeSocket();
        

        lastSocket = null;
        currentMovable = null;
        hinge.connectedBody = null;
    }




}