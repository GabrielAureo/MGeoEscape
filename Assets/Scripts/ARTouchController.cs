using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ARTouchController : MonoBehaviour{

    float timer;
    public float holdThreshold = 0.2f;
    bool holding;

    ARInteractable currentInteractable;
    void Update()
    {
        if(Input.touchCount > 0){
            var input = Input.touches[0];
            if(input.phase == TouchPhase.Began){
                Ray ray = Camera.main.ScreenPointToRay(input.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                    var interactable = hit.transform.gameObject.GetComponent<ARInteractable>();
                    print(interactable);
                    // if(interactable)
                    //     StartCoroutine(HoldCheck(input, interactable));
                    timer = 0.0f;
                    holding = false;
                }                
            }            
            timer += Time.deltaTime;
            if(!holding && timer >= holdThreshold){
                print("vai");
                currentInteractable?.onHold(input);
                holding = true;
            }else if(input.phase == TouchPhase.Ended){
                currentInteractable?.onTap(input);
            }

            if(input.phase == TouchPhase.Ended){
                currentInteractable?.onRelease(input);
                currentInteractable = null;
            }

        }
    }

    // IEnumerator HoldCheck(Touch input, ARInteractable interactable){
    //     float timer = 0.0f;
    //     while(timer <= holdThreshold){
    //         print("mds");
    //         if(input.phase == TouchPhase.Ended){
    //             print("tap");
    //             interactable.onTap();
    //             yield break;
    //         }
            
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     print("hold");
    //     interactable.onHold();        
    // }

}