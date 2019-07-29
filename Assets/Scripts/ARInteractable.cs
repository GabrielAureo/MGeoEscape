using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ARInteractable : MonoBehaviour
{
    public abstract void onTap(ARTouchController controller);
    public abstract void onHold(ARTouchController controller);
    public abstract void onRelease(ARTouchController controller);
}
