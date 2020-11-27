using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SocketGraph: NetworkBehaviour
{
    public List<Movable> acceptedMovables;

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

    private bool CompatibleMovable(Movable movable)
    {
        return acceptedMovables.Contains(movable);
    }
    
    

    private SocketNode[] GetNodes()
    {
        return transform.GetComponentsInChildren<SocketNode>();
    }
}