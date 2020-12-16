using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[System.Serializable]
public class SocketNodeData
{
    public string Guid;
    public string Title;
    public bool HasMovable;
    public string MovableID;
    [FormerlySerializedAs("Position")] public Vector2 GraphPosition;
    public Vector3 LocalScenePosition;
}
