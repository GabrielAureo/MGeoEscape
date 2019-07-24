using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ARInteractable : MonoBehaviour
{
    public abstract void onTap(Touch touch);
    public abstract void onHold(Touch touch);
    public abstract void onRelease(Touch touch);
}
