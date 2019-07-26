using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ARInteractable : MonoBehaviour
{
    public abstract void onTap(ARTouchController touch);
    public abstract void onHold(ARTouchController touch);
    public abstract void onRelease(ARTouchController touch);
}
