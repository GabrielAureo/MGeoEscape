using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SupplierSocket : BaseSocket
{
    public List<Movable> products;
    public bool randomPick;
    [SyncVar]
    private int currentPick;
    readonly SyncList<int> picks = new SyncList<int>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        for(int i =0; i < products.Count; i++){
            var product = products[i];
            picks.Add(i);
            //product.GetComponent<PlayerVisibility>().SetObserverFlag(0);
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
                picks.Remove(currentPick);
                break;
            case SocketTransfer.Status.Failure:
                products[currentPick].GetComponent<PlayerVisibility>().SetObserverFlag(0);
                break;
        }
    }
    
    public override SocketTransfer TryTake()
    {
        if(picks.Count == 0) return null;
        var pick = (randomPick)?Random.Range(0, picks.Count):0;
        var movable_idx = picks[pick];
        currentPick = movable_idx;

        return new SocketTransfer(products[movable_idx], DiscardTransfer);
    }
}