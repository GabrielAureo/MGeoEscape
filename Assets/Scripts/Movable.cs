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
    TrackableBehaviour trackableBehaviour;
    void Start(){
        originalParent = transform.parent;
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
        trackableBehaviour = GetComponentInParent<TrackableBehaviour>();
    }
    public override void onHold(ARTouchController controller)
    {
        transform.parent = null;
        //transform.rotation = Quaternion.identity;
        transform.rotation = originalRotation;
        rb.isKinematic = false;
        controller.FollowTouch(rb);        
    }

    public override void onRelease(ARTouchController controller)
    {
        transform.parent = originalParent;
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
        rb.isKinematic = true;
        if(trackableBehaviour.CurrentStatus == TrackableBehaviour.Status.NO_POSE){
            DisableObject();
        }
        controller.ReleaseFollow();
    }

    private void DisableObject(){
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;
    }

    public override void onTap(ARTouchController controller)
    {
        
    }
 
}