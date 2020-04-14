using UnityEngine;
using UnityEngine.Events;
public class Pressable: MonoBehaviour, IARInteractable
{
    public UnityEvent pressEvent;

    public void onHold(ARTouchData touchData){}

    public void onRelease(ARTouchData touchData){}

    public void onTap(ARTouchData touchData)
    {
        pressEvent.Invoke();
    }

    public void onTarget(Movable movable){}

    public void onUntarget(Movable movable){}
}