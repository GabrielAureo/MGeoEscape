using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ARTouchController : MonoBehaviour{
    public enum Status {HOLDING, WAITING, NO_TOUCH}
    public Status currentStatus;
    Status lastStatus;
    float timer;
    public float holdThreshold = 0.2f;
    HingeJoint hinge;
    ARInteractable selectedInteractable;
    Socket lastSocket;
    Movable currentMovable;
    ARInteractable targetInteractable;

    void Awake(){
        hinge = GetComponent<HingeJoint>();
        currentStatus = Status.NO_TOUCH;
    }
    public ARInteractable GetCurrentObject(){
        if(currentStatus == Status.HOLDING){
            return selectedInteractable;
        }
        return null;
    }
    void Update()
    {
        Ray ray = CameraRay();
        #if UNITY_ANDROID
            if(Input.touchCount > 0){
                InputStateMachine(ray);

                if(Input.touches[0].phase == TouchPhase.Ended){
                    Release(ray);
                }
            }
        #endif

        #if UNITY_EDITOR
        var input = Input.GetMouseButton(0);
        if(input){
          
           InputStateMachine(ray);           
        }
        if(Input.GetMouseButtonUp(0)){
            Release(ray);
        }
        #endif
    }

    private void InputStateMachine(Ray ray){
        RaycastHit hit;
        if(currentStatus == Status.NO_TOUCH){
            timer = 0.0f;
            if(Physics.Raycast(ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
                selectedInteractable = hit.transform.GetComponent<ARInteractable>();
                if(selectedInteractable is Socket){
                    lastSocket = (Socket)selectedInteractable;
                }
            }
            ChangeStatus(Status.WAITING);
        }
        if(currentStatus == Status.HOLDING){
            if(lastStatus == Status.WAITING){
                selectedInteractable?.onHold(this);
                ChangeStatus(Status.HOLDING);                
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            
            if(currentMovable){
                if(Physics.Raycast(ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
                    targetInteractable = hit.transform.GetComponent<ARInteractable>();
                    targetInteractable?.onTarget(this, currentMovable);
                }else{
                    targetInteractable?.onUntarget(this,currentMovable);
                }
            }
        }

        if(currentStatus == Status.WAITING){
            timer+=Time.deltaTime;
            if(timer >= holdThreshold){
                ChangeStatus(Status.HOLDING);
            }
        }
    }

    private Ray CameraRay(){
        Vector2 inputPosition;
        #if UNITY_EDITOR
        inputPosition = Input.mousePosition;
        #elif UNITY_ANDROID
        inputPosition = Input.touches[0].position;
        #endif

        var wrldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 1.35f));
        transform.position = new Vector3(wrldPos.x, wrldPos.y,transform.position.z);
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        return ray;
        
    }

    private void Release(Ray ray){
        RaycastHit hit;
        if(lastStatus == Status.HOLDING){
            targetInteractable?.onUntarget(this,currentMovable);
        }

        if(currentStatus == Status.WAITING){
            selectedInteractable?.onTap(this);
        }
        selectedInteractable?.onRelease(this);

        if(currentMovable){
            Socket target = (Socket) selectedInteractable;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sockets"))){
                target = hit.transform.GetComponent<Socket>();
            }
           
            ReleaseMovable(target);            
            
        }
        selectedInteractable = null;
        ChangeStatus(Status.NO_TOUCH);
    }

    private void ChangeStatus(Status newStatus){
        lastStatus = currentStatus;
        currentStatus = newStatus;
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