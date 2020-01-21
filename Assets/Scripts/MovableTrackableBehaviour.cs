using UnityEngine;
using Vuforia;


public class MovableTrackableBehaviour : MonoBehaviour, ITrackableEventHandler
{
    Movable movable;

    public void Start(){
        movable = GetComponent<Movable>();
    }
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if(newStatus == TrackableBehaviour.Status.NO_POSE && movable.released){
            DisableObject();
        }
    }

    private void DisableObject(){
        var rendererComponents = movable.GetComponentsInChildren<Renderer>();
        var colliderComponents = movable.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;
    }
}