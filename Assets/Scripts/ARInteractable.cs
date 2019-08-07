using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class ARInteractable : MonoBehaviour
{
    public abstract void onTap(ARTouchController controller);
    public abstract void onHold(ARTouchController controller);
    public abstract void onRelease(ARTouchController controller);
    public abstract void onTarget(ARTouchController controller, Movable movable);
    public abstract void onUntarget(ARTouchController controller, Movable movable);
}
