using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SupplierSocket : BaseSocket
{
    public List<Movable> products;
    public bool randomPick;
    [FormerlySerializedAs("_currentPickIndex")] [SyncVar]
    public int currentPickIndex;
    private int movableIndex => _picks[currentPickIndex];
    public SyncList<int> _picks = new SyncList<int>();


    

    public override void OnStartServer()
    {
        base.OnStartServer();
        for(int i =0; i < products.Count; i++){
            var product = products[i];
            _picks.Add(i);
            Debug.Log(_picks.Count);
            //product.GetComponent<PlayerVisibility>().SetObserverFlag(0);
        }
    }
    
    public override Movable ClientGetMovable()
    {
        return products[movableIndex];
    }
    public override Movable GetCurrentObject()
    {
        return products[movableIndex];
    }

    protected override bool ShouldPlace(Movable movable)
    {
        return false;
    }

    protected override void OnClientPlace(NetworkIdentity movableIdentity)
    {
        return;
    }

    private void DiscardTransfer(SocketTransfer.Status status){
        switch(status){
            case SocketTransfer.Status.Success:
                _picks.RemoveAt(currentPickIndex);
                break;
            case SocketTransfer.Status.Failure:
                RpcReturnToSupplier();
                // products[_currentPick].GetComponent<PlayerVisibility>().SetObserverFlag(0);
                break;
        }
    }

    [ClientRpc]
    private void RpcReturnToSupplier()
    {
        
        products[movableIndex].transform.position = transform.position;
    }

    protected override bool TakeOperation(out SocketTransfer transfer)
    {
        if (_picks.Count == 0)
        {
            transfer = null;
            return false;
        }
        currentPickIndex = (randomPick)?Random.Range(0, _picks.Count):0;
        Debug.Log(products[movableIndex]);
        products[movableIndex].gameObject.SetActive(true);
        transfer = new SocketTransfer(products[movableIndex], DiscardTransfer);
        return true;
    }
}