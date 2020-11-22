using UnityEngine.Events;

public class SocketTransfer{
    public enum Status{Success, Failure}
    public Movable movable;
    public UnityAction<Status> finishAction;

    public SocketTransfer(Movable movable, UnityAction<Status> finishAction){
        this.movable = movable;
        this.finishAction = finishAction;
    }
}