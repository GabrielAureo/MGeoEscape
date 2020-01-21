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
    [HideInInspector]
    public Mesh mesh;
    public UnityAction<ARInteractable, ARInteractable> releaseAction;
    [HideInInspector]
    public bool released;

    void Start(){
        originalParent = transform.parent;
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        material = GetComponent<MeshRenderer>().material;
        released = false;
    }

    public void onHold(){
        print("holding");
        material.DOFade(0.5f,"_BaseColor", .2f);
        released = false;
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
        released = true;
        //releaseAction(dropInteractable);
        
    }

    

 
}
