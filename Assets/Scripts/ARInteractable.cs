using UnityEngine;
using UnityEngine.Events;

public class ARInteractable :  MonoBehaviour, IARInteractable
{
    [System.Serializable]
    public class MovableEvent: UnityEvent<Movable>{}

    public UnityEvent HoldEvent;
    public UnityEvent ReleaseEvent;
    public UnityEvent TapEvent;
    public MovableEvent TargetEvent;
    public MovableEvent UntargetEvent;
    public void onHold()
    {
        HoldEvent.Invoke();
    }

    public void onRelease()
    {
        ReleaseEvent.Invoke();
    }

    public void onTap()
    {
        TapEvent.Invoke();
    }

    public void onTarget(Movable movable)
    {
        TargetEvent.Invoke(movable);
    }

    public void onUntarget(Movable movable)
    {
        UntargetEvent.Invoke(movable);
    }
}
