using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;
using RotaryHeart.Lib.SerializableDictionary;

public class SocketGraph: MonoBehaviour
{
    public List<Movable> acceptedMovables = new List<Movable>();
    public EdgesDictionary connections;
    [HideInInspector]
    public string GUID = System.Guid.NewGuid().ToString();
    

    void Start()
    {
        InitializeNodes();
    }
    private void InitializeNodes()
    {
        var nodes = GetNodesInScene();
        foreach (var node in nodes)
        {
            node.MovableAuth = CompatibleMovable;
        }
    }

    public void StartGraph(int startNodeIndex)
    {
        //if (!initialized) InitializeNodes();
        var startNode = connections.ElementAt(startNodeIndex).Key;
        //startNode.TryPlaceObject(startNode.exclusiveMovable);
    }
    

    private bool CompatibleMovable(Movable movable)
    {
        return acceptedMovables.Contains(movable);
    }
    
    public void TriggerNeighbors(int nodeIndex, bool activate)
    {
        var node = connections.ElementAt(nodeIndex).Key;

        var neighbors = connections[node];
        foreach (var neighbor in neighbors.data)
        {
            neighbor.gameObject.SetActive(activate);
        }
    }

    private SocketNode[] GetNodesInScene()
    {
        return transform.GetComponentsInChildren<SocketNode>();
    }
}