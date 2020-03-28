using UnityEngine;
using UnityEngine.Events;

public class MovableController{
    
    public Movable currentMovable; //Current movable object being held by the controller. Is assigned by the socket on the holding phase of the touch controller
    HingeJoint hinge;
    Socket lastSocket;
    ARInteractable targetInteractable;

    public void SetupController(ARTouchController touchController, HingeJoint hinge){
        this.hinge = hinge;
        touchController.onTouch.AddListener(TouchSocket);
        touchController.onHold.AddListener(CheckTarget);
        touchController.onRelease.AddListener(Release);

    }

    void TouchSocket(ARTouchData touchData){
        Debug.Log("Touch phase");
        Debug.Log(touchData.selectedInteractable);
        if(touchData.selectedInteractable is Socket){
            var socket = touchData.selectedInteractable as Socket;
            lastSocket = socket;
            currentMovable = socket.currentObject;
        }
    }

    void CheckTarget(ARTouchData touchData){
        Debug.Log("Hold phase");
        if(currentMovable == null) return;
        Debug.Log(currentMovable);

        HoldMovable(currentMovable);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(touchData.ray);
        if(hits.Length>0){
            ARInteractable targetInteractable = null;
            foreach(var hit in hits){
                targetInteractable = hit.transform.GetComponent<ARInteractable>();
                if(targetInteractable!= null) break;
            }
            
            targetInteractable?.onTarget(currentMovable);
        }else{
            targetInteractable?.onUntarget(currentMovable);
        }
    }

    void Release(ARTouchData touchData){
        Debug.Log("Release phase");
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
        currentMovable = movable;
        hinge.connectedBody = movable.rb;
        movable.transform.parent = null;
    }

    public void ReleaseMovable(Socket target){
        currentMovable.rb.isKinematic = true;
        var placed = target.TryPlaceObject(currentMovable);
        if(!placed){
            lastSocket.TryPlaceObject(currentMovable);
        }
        lastSocket = null;
        currentMovable = null;
        hinge.connectedBody = null;
    }




}