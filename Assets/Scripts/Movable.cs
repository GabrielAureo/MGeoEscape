using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Vuforia;
[RequireComponent(typeof(Rigidbody))]
public class Movable : ARInteractable
{
    private Transform originalParent;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private Rigidbody rb;
    Coroutine followRoutine;
    void Start(){
        originalParent = transform.parent;
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }
    public override void onHold(ARTouchController controller)
    {
        transform.parent = null;
        //transform.rotation = Quaternion.identity;
        rb.isKinematic = false;
        controller.FollowTouch(rb);
        
    }

    public override void onRelease(ARTouchController controller)
    {
        transform.parent = originalParent;
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
        rb.isKinematic = true;
        controller.ReleaseFollow();
    }

    public override void onTap(ARTouchController controller)
    {
        
    }
 
}