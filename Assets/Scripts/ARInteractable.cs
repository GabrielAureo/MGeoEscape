using UnityEngine;
using UnityEngine.Events;

public class ARInteractable :  MonoBehaviour, IARInteractable
{
    [System.Serializable]
    public class InteractionEvent: UnityEvent<ARTouchData>{}
    [System.Serializable]
    public class MovableEvent: UnityEvent<Movable>{}

    public InteractionEvent HoldEvent;
    public InteractionEvent ReleaseEvent;
    public InteractionEvent TapEvent;
    public MovableEvent TargetEvent;
    public MovableEvent UntargetEvent;
    public void onHold(ARTouchData touchData)
    {
        HoldEvent.Invoke(touchData);
    }

    public void onRelease(ARTouchData touchData)
    {
        ReleaseEvent.Invoke(touchData);
    }

    public void onTap(ARTouchData touchData)
    {
        TapEvent.Invoke(touchData);
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
