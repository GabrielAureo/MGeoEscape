using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class ARInteractable : MonoBehaviour
{
    public virtual void onTap(ARTouchData touchData){}
    public virtual void onHold(ARTouchData touchData){}
    public virtual void onRelease(ARTouchData touchData){}
    public virtual void onTarget(Movable movable){}
    public virtual void onUntarget(Movable movable){}
}
