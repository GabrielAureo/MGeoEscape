using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Vuforia;
using UnityEngine.Events;
using Mirror;

//Instead of making abstract classes, use events. So that the custom editor can still be used;

[RequireComponent(typeof(Rigidbody))]
public class Movable : NetworkBehaviour{
    public Vector3 bottomAnchor;
    public Quaternion placementRotation = Quaternion.identity;
    [HideInInspector] public Rigidbody rb;
    
    [HideInInspector] public Mesh mesh;
    public UnityAction<IARInteractable, IARInteractable> releaseAction;
    [HideInInspector] public bool released;
    [SerializeField] Material opaqueMaterial = null;
    [SerializeField] Material transparentMaterial = null;
    private MeshRenderer meshRenderer;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshRenderer = GetComponent<MeshRenderer>();
        opaqueMaterial = meshRenderer.materials[0];
    }


    void Start(){        
        released = false;
    }

    public void onHold(){
        print("holding");

        var mats = meshRenderer.materials;
        mats[0] = transparentMaterial;
        meshRenderer.materials = mats;
        
        transparentMaterial.DOFade(0.5f,"_BaseColor", .2f);
        
        released = false;
        // transform.parent = null;
        // //transform.rotation = Quaternion.identity;
        // transform.rotation = originalRotation;
        
    }

    public void onRelease()
    {
        transparentMaterial.DOFade(1f,"_BaseColor", .2f);

        var mats = meshRenderer.materials;
        mats[0] = opaqueMaterial;
        meshRenderer.materials = mats;
        // transform.parent = originalParent;
        // transform.localRotation = originalRotation;
        // transform.localPosition = originalPosition;
        rb.isKinematic = true;
        released = true;
        //releaseAction(dropInteractable);
        
    }

    

 
}
