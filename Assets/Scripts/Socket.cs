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
        obj.releaseAction = x => {
            if(x != null){
                currentObject = null;
            }
        };
    }

    private void UnsetObject(){
        currentObject = null;
    }

    public override void onTap(ARTouchController controller)
    {
        
    }

    public override void onHold(ARTouchController controller)
    {
        currentObject.onHold();
        currentObject.rb.isKinematic = false;
        controller.HoldMovable(currentObject); 
    }

    public override void onRelease(ARTouchController controller)
    {
        // var currentInteractable = controller.GetCurrentObject();
        // if(currentInteractable is Socket && currentInteractable != this){
        //     ((Socket)currentInteractable).TryPlaceObject(currentObject);
        // }
        //currentObject.onRelease();
        currentObject.rb.isKinematic = true;
        
    }

    public override void onTarget(ARTouchController controller)
    {
        throw new System.NotImplementedException();
    }
}
