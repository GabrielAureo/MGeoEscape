using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IARInteractable{
    void onTap(ARTouchData touchData);
    void onHold(ARTouchData touchData);
    void onRelease(ARTouchData touchData);
    void onTarget(Movable movable);
    void onUntarget(Movable movable);
}
