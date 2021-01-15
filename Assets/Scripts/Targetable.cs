
using System;
using UnityEngine;
using UnityEngine.Events;

public class Targetable: MonoBehaviour
{
    public bool useDefaultTargetAnimation = true;

    public TargetEvent OnTarget;
    public TargetEvent OnUntarget;

    public Func<Movable, MovablePlacementPose> TargetPose;
    public Func<Movable, bool> TargetCondition;
    
    public MovablePlacementPose GetTargetPose(Movable obj)
    {
        return TargetPose(obj);
    }

    public bool ShouldTarget(Movable obj)
    {
        return TargetCondition(obj);
    }
    
    public class TargetEvent:UnityEvent<Movable>{}
}
