using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SocketGraph: NetworkBehaviour
{
    public List<Movable> acceptedMovables = new List<Movable>();
    public readonly Dictionary<SocketNode, List<SocketNode>> connections = new Dictionary<SocketNode, List<SocketNode>>();
  
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeNodes();
    }

    private void InitializeNodes()
    {
        var nodes = GetNodes();
        foreach (var node in nodes)
        {
            node.MovableAuth = CompatibleMovable;
            node.Initialize();
        }
    }
    [Server]
    public void StartGraph(List<SocketNode> startingNodes)
    {
        foreach (var node in startingNodes)
        {
            RpcTriggerNeighbors(node.netIdentity, true);
        }
    }
    

    private bool CompatibleMovable(Movable movable)
    {
        return acceptedMovables.Contains(movable);
    }
    
    [ClientRpc]
    private void RpcTriggerNeighbors(NetworkIdentity nodeNetId, bool activate)
    {
        var node = nodeNetId.GetComponent<SocketNode>();
        var neighbors = connections[node];
        foreach (var neighbor in neighbors)
        {
            neighbor.gameObject.SetActive(activate);
        }
    }

    private SocketNode[] GetNodes()
    {
        return transform.GetComponentsInChildren<SocketNode>();
    }
}