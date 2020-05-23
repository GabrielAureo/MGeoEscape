using UnityEngine;
using UnityEngine.Events;
using Mirror;

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
                CmdTouch(socket.netIdentity);
            }
            
        }
    }
    [Command]
    void CmdTouch(NetworkIdentity socketNetID){
        var socket = socketNetID.GetComponent<Socket>();
        socket.busy = true;
        lastSocketNetIdentity = socketNetID;
        currentMovable = socket.currentObject;
    }

    void CheckTarget(ARTouchData touchData){
        if(currentMovable == null) return;

        HoldMovable(currentMovable);

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