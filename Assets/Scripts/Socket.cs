using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[RequireComponent(typeof(Collider))]
public class Socket : ARNetInteractable
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

    [SerializeField] public MovablePlacementPose placementPose = null;
    private Transform scaler;
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


    public override void OnStartClient(){
        var movable = GetComponentInChildren<Movable>();
        if(movable) SetObject(movable);
    }

    private void CreateScaler(){
        scaler = Instantiate(new GameObject("Scaler"), transform).transform;
        scaler.localPosition = Vector3.zero;
        
    }

    void Start(){
        var preview = GameObject.Instantiate(GameResources.Instance.previewSocketPrefab,transform);
        previewRenderer = preview.GetComponent<MeshRenderer>();
        previewFilter = preview.GetComponent<MeshFilter>();
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

    public bool TryPlaceObject(GameObject otherSocketObj){
        if(currentObject != null) return false;
        var movable = otherSocketObj.GetComponent<Socket>().currentObject;
        if(exclusiveMode && movable != exclusiveObject) return false;
        if(busy && movable != lastObject) return false;
        
        RpcSetObject(otherSocketObj);
        return true;
    }
    [ClientRpc]
    private void RpcSetObject(GameObject otherSocketObj){
        var movable = otherSocketObj.GetComponent<Socket>().currentObject;
        Debug.LogError("Socket: " + otherSocketObj + ", Movable: " + movable);
        SetObject(movable);
    }

    private void SetObject(Movable obj){
        //print(obj.name + " set to socket " + this.name);
        obj.gameObject.SetActive(true);
        obj.GetComponent<Rigidbody>().isKinematic = true;
        if(scaler == null) CreateScaler();
        if(!exclusiveMode){
            obj.transform.parent = scaler;
            FitObjectToSocket(obj);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }else{
            obj.transform.parent = transform;
            obj.transform.localPosition = placementPose.position;
            obj.transform.localScale = placementPose.scale;
            obj.transform.localRotation = placementPose.rotation;
        }

        currentObject = obj;
        obj.releaseAction = UnsetObject;
    }


    private void FitObjectToSocket(Movable obj){
        var model = obj.GetComponent<MeshRenderer>();
        var collider =  GetComponent<Collider>();
     
        var boundsScale = collider.bounds.size;

        var modelScale = model.bounds.size;
        var ratio = boundsScale.magnitude/modelScale.magnitude;
        print(gameObject.name + ", " + boundsScale + ", " + modelScale);
        scaler.transform.localScale *= ratio;
        print(collider.bounds.extents);
        var socketOffset = collider.bounds.center - transform.localPosition;
        //var movableOffset = model.bounds.center - model.transform.localPosition;
        scaler.transform.localPosition = Vector3.up * (socketOffset.y - collider.bounds.extents.y + (model.bounds.extents.y));
    }

    private void UnsetObject(IARInteractable oldInteractable, IARInteractable newInteractable){
        print(newInteractable);
        print(oldInteractable);
        if(newInteractable != null && newInteractable != oldInteractable){
            this.currentObject = null;
        }
    }

    public override void onTap(){}

    public override void onHold()
    {
        if(currentObject == null) return;
        currentObject.onHold();
        currentObject.rb.isKinematic = false;
        //controller.movableController.HoldMovable(currentObject);
        lastObject = currentObject;
        //currentObject = null;
        busy = true;
    }

    public override void onRelease()
    {
        if(currentObject == null) return;
        currentObject.onRelease();
    }

    public void FreeSocket(){
        busy = false;
    }

    public override void onTarget(Movable movable)
    {
        TryTarget(movable);
    }
    public override void onUntarget(Movable movable){
        Untarget();
    }
}
