using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Vuforia;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Movable : MonoBehaviour
{
    private Transform originalParent;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    [HideInInspector] public Rigidbody rb;
    private Material material;
    TrackableBehaviour trackableBehaviour;
    [HideInInspector]
    public Mesh mesh;
    public UnityAction<ARInteractable, ARInteractable> releaseAction;

    void Start(){
        originalParent = transform.parent;
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        material = GetComponent<MeshRenderer>().material;
        trackableBehaviour = GetComponentInParent<TrackableBehaviour>();
    }

    public void onHold(){
        print("holding");
        material.DOFade(0.5f,"_BaseColor", .2f);
        // transform.parent = null;
        // //transform.rotation = Quaternion.identity;
        // transform.rotation = originalRotation;
        
    }

    public void onRelease(ARInteractable dropInteractable)
    {
        material.DOFade(1f,"_BaseColor", .2f);
        // transform.parent = originalParent;
        // transform.localRotation = originalRotation;
        // transform.localPosition = originalPosition;
        rb.isKinematic = true;
        if(trackableBehaviour?.CurrentStatus == TrackableBehaviour.Status.NO_POSE){
            DisableObject();
        }
        //releaseAction(dropInteractable);
        
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

 
}
