using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using Mirror;

public class ARTouchController : NetworkBehaviour{
    [System.Serializable]
    public sealed class TouchEvent : UnityEvent<ARTouchData>{}
    //public ARTouchData.Status currentStatus;
    //ARTouchData.Status lastStatus;
    public float timer;
    public float holdThreshold = 0.2f;

    [HideInInspector] public Ray ray;
    //To be used by the movable controller
    [HideInInspector] public TouchEvent onTouch;
    [HideInInspector] public TouchEvent onHold;
    [HideInInspector] public TouchEvent onRelease;
    public MovableController movableController;
    public HingeJoint hinge = null;

    public static ARTouchData touchData;

    void Awake(){
        touchData = new ARTouchData();
        touchData.currentStatus = ARTouchData.Status.NO_TOUCH;

        movableController.SetupController(this);
    }


    void Update(){
        if(!isLocalPlayer) return;
        HandleInput();
    }

   
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
        RaycastHit[] hits;
        if(touchData.currentStatus == ARTouchData.Status.NO_TOUCH){
            
            hits = Physics.RaycastAll(ray, Mathf.Infinity, 1<<LayerMask.NameToLayer("Default"));
            if(hits.Length > 0){
                IARInteractable selectedInteractable = null;
                foreach(var hit in hits){
                    selectedInteractable = hit.transform.GetComponent<IARInteractable>();
                    if(selectedInteractable != null){
                        touchData.hit = hit;
                        break;
                    } 
                }
               
                touchData.selectedInteractable = selectedInteractable;
                onTouch.Invoke(touchData);
                

            }
            ChangeStatus(ARTouchData.Status.WAITING);
        }
        if(touchData.currentStatus == ARTouchData.Status.HOLDING){

            if(touchData.lastStatus == ARTouchData.Status.WAITING){
                try{
                    touchData.selectedInteractable?.onHold();
                }catch(System.Exception e){
                    Debug.LogError("ARTouchController: " + e.GetType().ToString() + " caught on " + touchData.selectedInteractable?.ToString() +" Hold Event");
                }
                onHold.Invoke(touchData);
                ChangeStatus(ARTouchData.Status.HOLDING);                
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.green);

        }

        if(touchData.currentStatus == ARTouchData.Status.WAITING){
            timer+=Time.deltaTime;
            if(timer >= holdThreshold){
                ChangeStatus(ARTouchData.Status.HOLDING);
            }
        }
    }

    private Ray CameraRay(){
        Vector2 inputPosition = Input.mousePosition;
        #if UNITY_ANDROID && !UNITY_EDITOR
            inputPosition = Input.touches[0].position;
        #endif

        

        var wrldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 1.35f));
        //Debug.Log(wrldPos);
        transform.position = new Vector3(wrldPos.x, wrldPos.y, wrldPos.z);
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        return ray;
        
    }

    private void Release(){
        timer = 0.0f;
        onRelease.Invoke(touchData);

        if(touchData.currentStatus == ARTouchData.Status.WAITING){
            try{
                touchData.selectedInteractable?.onTap();
            }catch(System.Exception e){
                Debug.LogError("ARTouchController: " + e.GetType().ToString() + " caught on " + touchData.selectedInteractable?.ToString() +" Tap Event");
            }
        }
            
        //Catch Exceptions so the controller doesn't get stuck in the Holding or Waiting status
        try{
            touchData.selectedInteractable?.onRelease();
        }catch(System.Exception e){
            Debug.LogError("ARTouchController: " + e.GetType().ToString() + " caught on " + touchData.selectedInteractable?.ToString() +" Release Event");
        }
        
        
        touchData.selectedInteractable = null;
        ChangeStatus(ARTouchData.Status.NO_TOUCH);
    }

    private void ChangeStatus(ARTouchData.Status newStatus){
        touchData.lastStatus = touchData.currentStatus;
        touchData.currentStatus = newStatus;
    }
}