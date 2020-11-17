using UnityEngine;
using Mirror;

public class BaseSocket : ARNetInteractable
{
    public Movable currentObject {get{return _currentObject;} private set{_currentObject = value;}}
    [SerializeField] private Movable _currentObject;
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

    [Server]
    public ITransfer TryTake(){
        if(busy || currentObject == null) return null;
        var obj = currentObject;

        currentTransfer = new Transfer(obj);
        RpcPlayBusyAnimation();
        return currentTransfer;
    }

    protected override 
    public override void onHold(){}

    public override void onRelease(){}

    public override void onTap(){}

    public override void onTarget(Movable movable){}

    public override void onUntarget(Movable movable){}
}