using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Todo: Add Destructive and Supplier modes. Destructive socket destroys the object that it receives. Supplier provides infinite objects. They are mutually exclusive.
[RequireComponent(typeof(Collider))]
public class Socket : ARNetInteractable
{
    /// <summary>
    /// Sockets in exclusive mode only allow one kind of object to be placed;
    /// </summary>
    public bool exclusiveMode;
    public GameObject exclusiveObject;
    /// <summary>
    /// Anchor used to place objects in non-exclusive mode;
    /// </summary>
    public Vector3 placementAnchor;
    private MeshRenderer previewRenderer = null;
    private MeshFilter previewFilter = null;
    public Movable currentObject;
    private Mesh lastMesh;
    private Transfer currentTransfer;
    [SerializeField] public MovablePlacementPose placementPose = null;
    /// <summary>
    /// Only valid on the server
    /// </summary>
    private bool busy;

    public interface ITransfer{
        Movable GetMovable();
    }
    private class Transfer: ITransfer{
        private Movable movable;
        public Transfer(Movable movable){
            this.movable = movable;
        }

        public Movable GetMovable(){
            return movable;
        }
    }

    public bool ReturnMovable(ITransfer transfer){
        if(ReferenceEquals(transfer, currentTransfer)){
            SetObject(transfer.GetMovable());
            return true;
        }
        return false;
    }

    void Start(){
        var preview = GameObject.Instantiate(GameResources.Instance.previewSocketPrefab,transform);
        previewRenderer = preview.GetComponent<MeshRenderer>();
        previewFilter = preview.GetComponent<MeshFilter>();

    }
    public override void OnStartServer(){
        base.OnStartServer();
        busy = false;
    }

    [Server]
    public bool IsBusy(){
        return busy;
    }
    public virtual bool TryTarget(Movable obj){
        if(currentObject != null) return false;
        if(obj.mesh != lastMesh){
            lastMesh = obj.mesh;
            previewFilter.mesh = obj.mesh;
            previewRenderer.transform.localRotation = Quaternion.identity;
        }
        previewRenderer.enabled = true;
        return true;
    }
    [Server]
    public ITransfer TryTake(){
        if(busy || currentObject == null) return null;
        var obj = currentObject;

        var vis = currentObject.GetComponent<PlayerVisibility>();
        vis.SetObserverFlag(0);
        currentTransfer = new Transfer(obj);
        return currentTransfer;
    }

    public virtual void Untarget(){
        previewRenderer.enabled = false;
    }
    [Server]
    public bool TryPlaceObject(Socket otherSocket){
        if(currentObject != null) return false;
        var movable = otherSocket.currentObject;
        if(exclusiveMode && movable != exclusiveObject) return false;
        if(busy) return false;

        var flag = GetComponent<PlayerVisibility>().GetObserverFlag();
        movable.GetComponent<PlayerVisibility>().SetObserverFlag(flag);
        currentObject = movable;
        Debug.LogError(movable.netIdentity);
        RpcSetObject(currentObject.netIdentity);
        return true;
    }
    [ClientRpc]
    private void RpcSetObject(NetworkIdentity movableNetworkIdentity){
        var movable = movableNetworkIdentity.GetComponent<Movable>();
        Debug.LogError(movableNetworkIdentity);
        currentObject = movable;
        SetObject(movable);
    }

    private void SetObject(Movable obj){
        obj.gameObject.SetActive(true);
        obj.rb.isKinematic = true;
        if(!exclusiveMode){
            obj.transform.position = transform.position - (obj.bottomAnchor - placementAnchor);
        }else{
            obj.transform.localPosition = placementPose.position;
            obj.transform.localScale = placementPose.scale;
            obj.transform.localRotation = placementPose.rotation;
        }  
    }

    public override void onTap(){}

    public override void onHold()
    {
        if(currentObject == null) return;
        currentObject.onHold();
    }


    public override void onRelease()
    {
        if(currentObject == null) return;
        currentObject.onRelease();
    }
    public override void onTarget(Movable movable)
    {
        TryTarget(movable);
    }
    public override void onUntarget(Movable movable){
        Untarget();
    }
}
