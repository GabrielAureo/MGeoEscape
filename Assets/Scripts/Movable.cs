using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Movable : ARInteractable
{
    private Transform originalParent;
    Coroutine followRoutine;
    void Start(){
        originalParent = transform.parent;
    }
    public override void onHold(Touch touch)
    {
        transform.parent = null;
        followRoutine = StartCoroutine(followTouch(touch));
    }

    public override void onRelease(Touch touch)
    {
        transform.parent = originalParent;
        if(followRoutine != null) StopCoroutine(followRoutine);
    }

    public override void onTap(Touch touch)
    {
        
    }

    private IEnumerator followTouch(Touch touch){
        while(true){        
            transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(touch.position), .2f);
            yield return null;
        }
        
    }
}