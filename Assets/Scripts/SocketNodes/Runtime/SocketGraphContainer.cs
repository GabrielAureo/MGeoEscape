using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SocketGraphContainer
{
    // [FormerlySerializedAs("GraphObjectInstanceId")] public GlobalObjectId GraphObjectId;
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<SocketNodeData> SocketNodes = new List<SocketNodeData>();
}
