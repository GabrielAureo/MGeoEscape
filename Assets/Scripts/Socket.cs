using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

//Todo: Add Destructive and Supplier modes. Destructive socket destroys the object that it receives. Supplier provides infinite objects. They are mutually exclusive.
[RequireComponent(typeof(Collider),typeof(Targetable))]
public class Socket : BaseSocket
{
    /// <summary>
    /// Sockets in exclusive mode only allow one kind of object to be placed;
    /// </summary>
    public bool exclusiveMode;

    public Movable currentObject
    {
        get
        {
            return _currentObject;
        }
        protected set
        {
            _currentObject = value;
        }
    }

    [SerializeField] protected Movable _currentObject;
    public GameObject exclusiveObject;
    /// <summary>
    /// Anchor used to place objects in non-exclusive mode;
    /// </summary>
    public Vector3 placementAnchor;
    private MeshRenderer previewRenderer = null;
    private MeshFilter previewFilter = null;
    private Mesh lastMesh;
    [SerializeField] private bool useDefaultTargetAnimation;
    [SerializeField] private bool useDefaultBusyAnimation;
    [SerializeField] private UnityEvent AvailableTargetAnimation;
    [SerializeField] private UnityEvent BusyTargetAnimation;
    [SerializeField] public MovablePlacementPose exclusivePose = null;

    private GameObject busyPreviewObject;
    /// <summary>
    /// Only valid on the server
    /// </summary>
    [SyncVar]
    public bool _busy;
    [SyncVar]
    public bool _empty;
    private SocketTransfer currentTransfer;
    

    [ClientRpc]
    private void RpcReturnMovable(){

        if(busyPreviewObject) busyPreviewObject.SetActive(false);
        SetupObject(_currentObject);
    }

    public override Movable ClientGetMovable()
    {
        return currentObject;
    }

    public override Movable GetCurrentObject()
    {
        return currentObject;
    }
    [ClientRpc]
    private void RpcEndTransfer(){
        if(busyPreviewObject) busyPreviewObject.SetActive(false);
    }
    void Start(){
        // var preview = GameObject.Instantiate(GameResources.Instance.previewSocketPrefab,transform);
        // previewRenderer = preview.GetComponent<MeshRenderer>();
        // previewFilter = preview.GetComponent<MeshFilter>();
        SetTargetable();

    }

    private void SetTargetable()
    {
        var targetable = GetComponent<Targetable>();
        if (targetable == null) return;
        targetable.TargetCondition = PlacementCondition;
        targetable.TargetPose = obj =>
        {
            var pose = new MovablePlacementPose();
            if (!exclusiveMode)
            {
                pose.position = transform.position - (obj.bottomAnchor - placementAnchor);
                pose.rotation = obj.placementRotation;
            }
            else
            {
                pose.position = exclusivePose.position;
                pose.scale = exclusivePose.scale;
                pose.rotation = exclusivePose.rotation;
            }

            return pose;
        };
    }
    public override void OnStartServer(){
        base.OnStartServer();
        _busy = false;
        _empty = currentObject == null;
    }

    //Change the currentObject comparison to a empty bool
    public virtual bool TryTarget(Movable obj){
        if(!_empty) return false;
        if(obj.mesh != lastMesh){
            lastMesh = obj.mesh;
            previewFilter.mesh = obj.mesh;
            previewRenderer.transform.localRotation = Quaternion.identity;
        }
        previewRenderer.enabled = true;
        return true;
    }
    [Server]
    protected override bool TakeOperation(out SocketTransfer transfer){
        if (_busy || _empty)
        {
            transfer = null;
            return false;
        }
        var obj = currentObject;

        var callback = new UnityAction<SocketTransfer.Status>(OnTransferFinish);
        _busy = true;
        currentTransfer = new SocketTransfer(currentObject, callback);
        RpcPlayBusyAnimation();
        transfer = currentTransfer;
        return true;
    }
    private void OnTransferFinish(SocketTransfer.Status status){
        if(status == SocketTransfer.Status.Success){
            currentObject = null;
            _empty = true;
            RpcEndTransfer();
        }else{
            RpcReturnMovable();
        }

        _busy = false;
        
    }

    [ClientRpc]
    void RpcPlayBusyAnimation(){
        if(useDefaultBusyAnimation){
            if(!busyPreviewObject){
                busyPreviewObject = Instantiate(GameResources.Instance.busyPreviewPrefab);
                SetPosition(busyPreviewObject, currentObject); 
            }else{
                busyPreviewObject.SetActive(true);
            }
            
        }
    }
    
    [Server]
    protected override bool ShouldPlace(Movable movable)
    {
        if (!PlacementCondition(movable)) return false;

        var flag = GetComponent<PlayerVisibility>().GetObserverFlag();
        movable.GetComponent<PlayerVisibility>().SetObserverFlag(flag);
        currentObject = movable;
        _empty = false;
        Debug.LogError(movable.netIdentity);
        return true;
    }

    private bool PlacementCondition(Movable movable)
    {
        if(!_empty) return false;
        if(exclusiveMode && movable != exclusiveObject) return false;
        if(_busy) return false;

        return true;
    }

    protected override void OnClientPlace(NetworkIdentity movableIdentity)
    {
        RpcSetObject(movableIdentity);
    }

    //[ClientRpc]
    private void RpcSetObject(NetworkIdentity movableNetworkIdentity){
        var movable = movableNetworkIdentity.GetComponent<Movable>();
        Debug.LogError(movableNetworkIdentity);
        currentObject = movable; 
        SetupObject(movable);
    }
    [Client]
    private void SetupObject(Movable obj){
        obj.gameObject.SetActive(true);
        obj.rb.isKinematic = true;
        SetPosition(obj);
    }

    private void SetPosition(Movable obj){
        var transform1 = obj.transform;
        if(!exclusiveMode){
            transform1.position = transform.position - (obj.bottomAnchor - placementAnchor);
            transform1.rotation = obj.placementRotation;
        }else{
            transform1.localPosition = exclusivePose.position;
            transform1.localScale = exclusivePose.scale;
            transform1.localRotation = exclusivePose.rotation;
        }  
    }

    private void SetPosition(GameObject obj, Movable guide){
        if(!exclusiveMode){
            obj.transform.position = transform.position - (guide.bottomAnchor - placementAnchor);
            obj.transform.rotation = guide.placementRotation;
        }else{
            obj.transform.localPosition = exclusivePose.position;
            obj.transform.localScale = exclusivePose.scale;
            obj.transform.localRotation = exclusivePose.rotation;
        }
    }
    

}
