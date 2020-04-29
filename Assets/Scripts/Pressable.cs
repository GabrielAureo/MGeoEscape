using UnityEngine;
using UnityEngine.Events;
public class Pressable: MonoBehaviour, IARInteractable
{
    public UnityEvent pressEvent;

    public void onHold(){}

    public void onRelease(){}

    public void onTap()
    {
        pressEvent.Invoke();
    }

    public void onTarget(Movable movable){}

    public void onUntarget(Movable movable){}
}