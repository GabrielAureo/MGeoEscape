using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ARTouchController : MonoBehaviour{
    public enum Status {HOLDING, WAITING, NO_TOUCH}
    public Status currentStatus;
    Status lastStatus;
    float timer;
    public float holdThreshold = 0.2f;
    bool holding;
    HingeJoint hinge;
    ARInteractable currentInteractable;
    public Touch currentTouch;
    Socket lastSocket;

    void Awake(){
        hinge = GetComponent<HingeJoint>();
        currentStatus = Status.NO_TOUCH;
    }
    void Update()
    {
        #if UNITY_ANDROID
            TouchControl();
        #endif

        #if UNITY_EDITOR
        var input = Input.GetMouseButton(0);
        if(input){
            var wrldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.35f));
            transform.position = new Vector3(wrldPos.x, wrldPos.y,transform.position.z);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(currentStatus == Status.NO_TOUCH){
                timer = 0.0f;
                ChangeStatus(Status.WAITING);
                if(Physics.Raycast(ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
                    currentInteractable = hit.transform.GetComponent<Socket>().currentObject;
                    
                }
            }
            if(currentStatus == Status.HOLDING){
                Debug.DrawRay(ray.origin, ray.direction, Color.green);
                if(currentInteractable is Movable && Physics.Raycast(ray,out hit, Mathf.Infinity,1<<LayerMask.NameToLayer("Sockets"))){
                    var socket = hit.transform.GetComponent<Socket>();
                    if(socket){
                        socket.TryTarget((Movable)currentInteractable);
                        lastSocket = socket;
                    }
                }else if(lastSocket){
                        lastSocket.Untarget();
                        lastSocket = null;
                }
            }

            if(currentStatus == Status.WAITING){
                timer+=Time.deltaTime;
                if(timer >= holdThreshold){
                    ChangeStatus(Status.HOLDING);
                    currentInteractable?.onHold(this);
                }
            }
        }

        if(Input.GetMouseButtonUp(0)){
            Release();
        }
        #endif
    }

    private void Release(){
        if(currentStatus == Status.WAITING){
                currentInteractable?.onTap(this);
            }
        currentInteractable?.onRelease(this);
        if(currentInteractable is Movable && lastSocket){
            var placed = lastSocket.TryPlaceObject((Movable)currentInteractable);
            if(!placed){
                //TODO
            }
            lastSocket.Untarget();
            lastSocket = null;
        }
        currentInteractable = null;
        ChangeStatus(Status.NO_TOUCH);
    }

    private void ChangeStatus(Status newStatus){
        lastStatus = currentStatus;
        currentStatus = newStatus;
    }

    private void TouchControl(){
        if(Input.touchCount > 0){
            print("s");
            var input = Input.touches[0];
            currentTouch = input;
            var wrldPos = Camera.main.ScreenToWorldPoint(input.position);
            transform.position = new Vector3(wrldPos.x, wrldPos.y,transform.position.z);
            if(input.phase == TouchPhase.Began){
                Ray ray = Camera.main.ScreenPointToRay(input.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                    currentInteractable = hit.transform.gameObject.GetComponent<ARInteractable>();
                    // if(interactable)
                    //     StartCoroutine(HoldCheck(input, interactable));
                    timer = 0.0f;
                    holding = false;
                }                
            }            
            timer += Time.deltaTime;
            if(!holding && timer >= holdThreshold){
                print("vai");
                currentInteractable?.onHold(this);
                holding = true;
            }else if(input.phase == TouchPhase.Ended){
                currentInteractable?.onTap(this);
            }

            if(input.phase == TouchPhase.Ended){
                currentInteractable?.onRelease(this);
                currentInteractable = null;
            }

        }
    }
    public void FollowTouch(Rigidbody objBody){
        hinge.connectedBody = objBody;
    }

    public void ReleaseFollow(){
        hinge.connectedBody = null;
    }

}