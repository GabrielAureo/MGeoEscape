using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Socket : NetworkBehaviour, IARInteractable
{
    /// <summary>
    /// Sockets in exclusive mode only allow one kind of object to be placed;
    /// </summary>
    public bool exclusiveMode;
    public GameObject exclusiveObject;
    private MeshRenderer previewRenderer = null;
    private MeshFilter previewFilter = null;
    [HideInInspector] public Movable currentObject;
    private Movable lastObject;
    private Mesh lastMesh;

    public MovablePlacementPose placementPose;
    [SyncVar]
    public bool busy;

    void Awake()
    {
        //previewRenderer = GetComponentInChildren<MeshRenderer>();
        //previewFilter = GetComponentInChildren<MeshFilter>();
        //if(previewRenderer != null) previewRenderer.enabled = false;
        var movable = GetComponentInChildren<Movable>();
        if(movable) SetObject(movable);

        

    }

    void Start(){
        var preview = GameObject.Instantiate(GameResources.Instance.previewSocketPrefab,transform);
        previewRenderer = preview.GetComponent<MeshRenderer>();
        previewFilter = preview.GetComponent<MeshFilter>();
    }

    public bool TryTarget(Movable obj){
        if(currentObject != null) return false;

        /*if(previewRenderer == null){
            var preview = new GameObject("Preview");

            preview.transform.parent = transform;
            preview.transform.localPosition = Vector3.zero;
            preview.transform.localRotation = Quaternion.identity;
            preview.transform.localScale = Vector3.one;

            previewRenderer = preview.AddComponent<MeshRenderer>();
            previewFilter = preview.AddComponent<MeshFilter>();
           
        }*/

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
        if(busy && obj != lastObject) return false;
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

    private void UnsetObject(IARInteractable oldInteractable, IARInteractable newInteractable){
        print(newInteractable);
        print(oldInteractable);
        if(newInteractable != null && newInteractable != oldInteractable){
            this.currentObject = null;
        }
    }

    public void onTap(){}

    public void onHold()
    {
        if(currentObject == null) return;
        currentObject.onHold();
        currentObject.rb.isKinematic = false;
        //controller.movableController.HoldMovable(currentObject);
        lastObject = currentObject;
        currentObject = null;
        busy = true;
    }

    public void onRelease()
    {
        if(currentObject == null) return;
        currentObject.onRelease();
    }

    public void FreeSocket(){
        busy = false;
    }

    public void onTarget(Movable movable)
    {
        TryTarget(movable);
    }
    public void onUntarget(Movable movable){
        Untarget();
    }
}
