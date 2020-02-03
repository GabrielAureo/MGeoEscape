using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class ARInteractable : MonoBehaviour
{
    public virtual void onTap(ARTouchController controller){}
    public virtual void onHold(ARTouchController controller){}
    public virtual void onRelease(ARTouchController controller){}
    public virtual void onTarget(Movable movable){}
    public virtual void onUntarget(Movable movable){}
}
