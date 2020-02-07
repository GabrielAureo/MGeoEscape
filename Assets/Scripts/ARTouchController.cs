using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class ARTouchController : MonoBehaviour{
    [System.Serializable]
    public sealed class TouchEvent : UnityEvent<ARTouchData>{}
    //public ARTouchData.Status currentStatus;
    //ARTouchData.Status lastStatus;
    float timer;
    public float holdThreshold = 0.2f;

    [HideInInspector] public Ray ray;
    [HideInInspector] public TouchEvent onTouch;
    [HideInInspector] public TouchEvent onHold;
    [HideInInspector] public TouchEvent onRelease;

    public MovableController movableController;

    public ARTouchData touchData;

    void Awake(){
        var hinge = GetComponent<HingeJoint>();
        movableController = new MovableController();
        movableController.SetupController(this, hinge);
        touchData = new ARTouchData();
    }
    /*public ARInteractable GetCurrentObject(){
        if(touchData.currentStatus == ARTouchData.Status.HOLDING){
            return selectedInteractable;
        }
        return null;
    }*/
    public void HandleInput()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            
            if(Input.touchCount > 0){
                ray = CameraRay();
                touchData.ray = ray;
                InputStateMachine();

                if(Input.touches[0].phase == TouchPhase.Ended){
                    Release();
                }
            }
            return;
        #endif

        ray = CameraRay();
        touchData.ray = ray;
        var input = Input.GetMouseButton(0);
        if(input){
          
           InputStateMachine();           
        }
        if(Input.GetMouseButtonUp(0)){
            Release();
        }
    }

    private void InputStateMachine(){
        RaycastHit hit;
        if(touchData.currentStatus == ARTouchData.Status.NO_TOUCH){
            timer = 0.0f;
            if(Physics.Raycast(ray,out hit, Mathf.Infinity, 1<<LayerMask.NameToLayer("Default"))){
                var selectedInteractable = hit.transform.GetComponent<ARInteractable>();
                touchData.selectedInteractable = selectedInteractable;

                onTouch.Invoke(touchData);
                
                /*if(selectedInteractable is Socket){
                    lastSocket = (Socket)selectedInteractable;
                }*/
            }
            ChangeStatus(touchData, ARTouchData.Status.WAITING);
        }
        if(touchData.currentStatus == ARTouchData.Status.HOLDING){
            if(touchData.lastStatus == ARTouchData.Status.WAITING){
                touchData.selectedInteractable?.onHold(touchData);
                ChangeStatus(touchData, ARTouchData.Status.HOLDING);                
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.green);

            onHold.Invoke(touchData);
            
            /*if(currentMovable){
                if(Physics.Raycast(ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
                    targetInteractable = hit.transform.GetComponent<ARInteractable>();
                    targetInteractable?.onTarget(this, currentMovable);
                }else{
                    targetInteractable?.onUntarget(this,currentMovable);
                }
            }*/
        }

        if(touchData.currentStatus == ARTouchData.Status.WAITING){
            timer+=Time.deltaTime;
            if(timer >= holdThreshold){
                ChangeStatus(touchData, ARTouchData.Status.HOLDING);
            }
        }
    }

    private Ray CameraRay(){
        Vector2 inputPosition = Input.mousePosition;
        #if UNITY_ANDROID && !UNITY_EDITOR
            inputPosition = Input.touches[0].position;
        #endif

        var wrldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 1.35f));
        transform.position = new Vector3(wrldPos.x, wrldPos.y,transform.position.z);
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        return ray;
        
    }

    private void Release(){
        //RaycastHit hit;
        onRelease.Invoke(touchData);

        if(touchData.currentStatus == ARTouchData.Status.WAITING){
            touchData.selectedInteractable?.onTap(touchData);
           
        }
       touchData.selectedInteractable?.onRelease(touchData);

        /*if(currentMovable){
            Socket target = (Socket) selectedInteractable;
            if(Physics.Raycast(touchData.ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sockets"))){
                target = hit.transform.GetComponent<Socket>();
            }
           
            ReleaseMovable(target);            
            
        }*/
         touchData.selectedInteractable = null;
        ChangeStatus(touchData, ARTouchData.Status.NO_TOUCH);
    }

    private void ChangeStatus(ARTouchData touchData, ARTouchData.Status newStatus){
        touchData.lastStatus = touchData.currentStatus;
        touchData.currentStatus = newStatus;
    }

   
    // public void HoldMovable(Movable movable){
    //     currentMovable = movable;
    //     hinge.connectedBody = movable.rb;
    //     movable.transform.parent = null;
    // }

    // public void ReleaseMovable(Socket target){
    //     currentMovable.rb.isKinematic = true;
    //     var placed = target.TryPlaceObject(currentMovable);
    //     if(!placed){
    //         lastSocket.TryPlaceObject(currentMovable);
    //     }
    //     lastSocket = null;
    //     currentMovable = null;
    //     hinge.connectedBody = null;
    // }

}