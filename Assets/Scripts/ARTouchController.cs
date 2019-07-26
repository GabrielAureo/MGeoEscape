using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ARTouchController : MonoBehaviour{

    float timer;
    public float holdThreshold = 0.2f;
    bool holding;
    HingeJoint hinge;
    ARInteractable currentInteractable;
    public Touch currentTouch;

    void Awake(){
        hinge = GetComponent<HingeJoint>();
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
            print(Input.mousePosition);
            transform.position = new Vector3(wrldPos.x, wrldPos.y,transform.position.z);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
             if(!currentInteractable && Physics.Raycast(ray, out hit, Mathf.Infinity)){
                currentInteractable = hit.transform.gameObject.GetComponent<ARInteractable>();
                timer = 0.0f;
                holding = false;
            }
            timer += Time.deltaTime;
             if(!holding && timer >= holdThreshold){
                print("vai");
                currentInteractable?.onHold(this);
                holding = true;
            }
        }
        if(Input.GetMouseButtonUp(0)){
            holding = false;
            if(timer < holdThreshold){
                currentInteractable?.onTap(this);
            }
            currentInteractable?.onRelease(this);
            currentInteractable = null;
        }
        #endif
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