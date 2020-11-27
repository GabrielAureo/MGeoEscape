using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SocketGraphContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<SocketNodeData> SocketNodes = new List<SocketNodeData>();
}
