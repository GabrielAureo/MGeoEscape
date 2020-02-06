using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : ARInteractable
{
    [SerializeField] private MeshRenderer previewRenderer;
    [SerializeField] private MeshFilter previewFilter;
    public Movable currentObject;
    private Mesh lastMesh;
    public Transform bottomAnchor;

    

    void Awake()
    {
        previewRenderer = GetComponentInChildren<MeshRenderer>();
        previewFilter = GetComponentInChildren<MeshFilter>();
        previewRenderer.enabled = false;
        var movable = GetComponentInChildren<Movable>();
        if(movable) SetObject(movable);
    }

    public bool TryTarget(Movable obj){
        if(currentObject != null) return false;

        if(obj.mesh != lastMesh){
            lastMesh = obj.mesh;
            previewFilter.mesh = obj.mesh;
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
        //print(obj.name + " set to socket " + this.name);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero; // change this to offset
        obj.transform.localRotation = Quaternion.identity;
        currentObject = obj;
        obj.releaseAction = UnsetObject;
        //print(obj.releaseAction.Method.Name);
    }

    private void UnsetObject(ARInteractable oldInteractable, ARInteractable newInteractable){
        print(newInteractable);
        print(oldInteractable);
        if(newInteractable != null && newInteractable != oldInteractable){
            this.currentObject = null;
        }
    }

    public override void onTap(ARTouchData touchData)
    {
        
    }

    public override void onHold(ARTouchData touchData)
    {
        if(currentObject == null) return;
        currentObject.onHold();
        currentObject.rb.isKinematic = false;
        //controller.movableController.HoldMovable(currentObject);
        currentObject = null;
    }

    public override void onRelease(ARTouchData touchData)
    {
        
    }

    public override void onTarget(Movable movable)
    {
        TryTarget(movable);
    }
    public override void onUntarget(Movable movable){
        Untarget();
    }
}
