using UnityEngine;
[System.Serializable]
public class MovablePlacementPose{
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 scale = Vector3.one;

    public MovablePlacementPose(){
        this.position = Vector3.zero;
        this.rotation = Quaternion.identity;
        this.scale = Vector3.one;
    }
    public MovablePlacementPose(Vector3 position, Quaternion rotation, Vector3 scale){
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
    public void SetFromTransform(Transform t){
        position = t.localPosition;
        rotation = t.localRotation;
        scale = t.localScale;
    }

    public void SetToTransform(Transform t){
        t.localPosition = position;
        t.localRotation = rotation;
        t.localScale = scale;
    }

}