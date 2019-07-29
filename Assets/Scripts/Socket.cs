﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Socket : MonoBehaviour
{
    [SerializeField] private MeshRenderer previewRenderer;
    [SerializeField] private MeshFilter previewFilter;
    [HideInInspector] public Movable currentObject;
    private Mesh lastMesh;
    public Transform bottomAnchor;

    

    void Awake()
    {
        previewRenderer = GetComponentInChildren<MeshRenderer>();
        previewFilter = GetComponentInChildren<MeshFilter>();
        previewRenderer.enabled = false;
    }

    public bool TryTarget(Movable obj){
        if(currentObject != null) return false;

        if(obj.mesh != lastMesh){
            lastMesh = obj.mesh;
            previewFilter.mesh = obj.mesh;
            //previewRenderer.transform.localPosition = localPosition;
            //previewRenderer.transform.localPosition = Vector3.forward * GetMeshOffset(obj.mesh);
            previewRenderer.transform.localRotation = Quaternion.identity;
        }
        previewRenderer.enabled = true;
        return true;
    }

    public bool TryTake(){
        if(currentObject == null) return false;
        currentObject = null;
        return true;
    }

    float GetMeshOffset(Mesh mesh){
        return mesh.bounds.extents.y/2;
    }

    public void Untarget(){
        previewRenderer.enabled = false;
    }

    public bool TryPlaceObject(Movable obj){
        if(currentObject != null) return false;
        SetObject(obj);
        return true;
    }

    private void SetObject(Movable obj){
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero; // change this to offset
        currentObject = obj;
    }

    private void UnsetObject(){
        currentObject = null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
