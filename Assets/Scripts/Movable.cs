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
    private Material materialInstance;
    private Tweener fade;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        materialInstance = GetComponent<MeshRenderer>().material;

    }
    [Client]
    public void GrabAnimation(){
        if(fade!= null && fade.IsActive()) fade.Kill();
        fade = materialInstance.DOFloat(.5f, "Opacity", .1f);
    }
    [Client]
    public void ReleaseAnimation()
    {
        if(fade!= null && fade.IsActive()) fade.Kill();
        fade = materialInstance.DOFloat(1f, "Opacity", .1f);
    }

    

 
}
