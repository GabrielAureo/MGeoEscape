using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SupplierSocket : BaseSocket
{
    public List<Movable> products;
    public bool randomPick;
    [SyncVar]
    private int currentPick;
    private SyncList<int> picks;

    public override void OnStartServer()
    {
        base.OnStartServer();
        foreach(var product in products){
            product.GetComponent<PlayerVisibility>().SetObserverFlag(0);
        }
    }
    public override Movable ClientGetMovable()
    {
        return products[currentPick];
    }
    public override Movable GetCurrentObject()
    {
        return products[currentPick];
    }
    public override bool TryPlaceObject(Movable movable)
    {
        return false;
    }
    private void DiscardTransfer(SocketTransfer.Status status){
        switch(status){
            case SocketTransfer.Status.Success:
                products.RemoveAt(currentPick);
                break;
            case SocketTransfer.Status.Failure:
                products[currentPick].GetComponent<PlayerVisibility>().SetObserverFlag(0);
                break;
        }
    }
    
    public override SocketTransfer TryTake()
    {
        var movable_idx = (randomPick)?Random.Range(0, products.Count - 1):0;
        currentPick = movable_idx;
        //RpcShowMovable(movable_idx);
        //need to use spawn
        return new SocketTransfer(products[movable_idx], DiscardTransfer);
    }
}