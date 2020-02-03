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
        if(touchData.selectedInteractable is Socket){
            lastSocket = (Socket)touchData.selectedInteractable;
        }
    }

    void CheckTarget(ARTouchData touchData){
        Debug.Log("Hold phase");
        if(currentMovable == null) return;

        RaycastHit hit;
        if(Physics.Raycast(touchData.ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
            targetInteractable = hit.transform.GetComponent<ARInteractable>();
            targetInteractable?.onTarget(currentMovable);
        }else{
            targetInteractable?.onUntarget(currentMovable);
        }
    }

    void Release(ARTouchData touchData){
        RaycastHit hit;
        Debug.Log("Release phase");
        if(touchData.lastStatus == ARTouchData.Status.HOLDING){
            targetInteractable?.onUntarget(currentMovable);
        }

        if(currentMovable){
            Socket target = (Socket)touchData.selectedInteractable;
            if(Physics.Raycast(touchData.ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sockets"))){
                target = hit.transform.GetComponent<Socket>();
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