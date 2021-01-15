using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class SupplierSocket : BaseSocket
{
    public List<Movable> products;
    public bool randomPick;
    [SyncVar]
    private int _currentPick;
    readonly SyncList<int> _picks = new SyncList<int>();

    private void OnValidate()
    {
        // foreach (var product in products)
        // {
        //     product.transform.position = transform.position;
        // }
    }
    

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
        return products[_currentPick];
    }
    public override Movable GetCurrentObject()
    {
        return products[_currentPick];
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
                _picks.Remove(_currentPick);
                break;
            case SocketTransfer.Status.Failure:
                products[_currentPick].GetComponent<PlayerVisibility>().SetObserverFlag(0);
                break;
        }
    }

    protected override bool TakeOperation(out SocketTransfer transfer)
    {
        if (_picks.Count == 0)
        {
            transfer = null;
            return false;
        }
        var pick = (randomPick)?Random.Range(0, _picks.Count):0;
        var movableIdx = _picks[pick];
        _currentPick = movableIdx;
        Debug.Log(products[movableIdx]);
        transfer = new SocketTransfer(products[movableIdx], DiscardTransfer);
        return true;
    }
}