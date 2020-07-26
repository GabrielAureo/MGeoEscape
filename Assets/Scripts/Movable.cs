using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Vuforia;
using UnityEngine.Events;
using Mirror;

//Instead of making abstract classes, use events. So that the custom editor can still be used;

[RequireComponent(typeof(Rigidbody),typeof(PlayerVisibility))]
public class Movable : NetworkBehaviour{
    public Vector3 bottomAnchor;
    public Quaternion placementRotation = Quaternion.identity;
    [HideInInspector] public Rigidbody rb;
    
    [HideInInspector] public Mesh mesh;
    public UnityAction<IARInteractable, IARInteractable> releaseAction;
    [SerializeField] Material opaqueMaterial = null;
    [SerializeField] Material transparentMaterial = null;
    private MeshRenderer meshRenderer;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshRenderer = GetComponent<MeshRenderer>();
        opaqueMaterial = meshRenderer.materials[0];
    }

    public void onHold(){
        var mats = meshRenderer.materials;
        mats[0] = transparentMaterial;
        meshRenderer.materials = mats;
        transparentMaterial.DOFade(0.5f,"_BaseColor", .2f);
    }

    public void onRelease()
    {
        transparentMaterial.DOFade(1f,"_BaseColor", .2f);
        var mats = meshRenderer.materials;
        mats[0] = opaqueMaterial;
        meshRenderer.materials = mats;   
    }

    

 
}
