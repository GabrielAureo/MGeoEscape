using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;

public class GraphSaveUtility
{
    private SocketGraphView _targetGraphView;
    private SocketGraphContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<SocketNodeView> Nodes => _targetGraphView.nodes.ToList().Cast<SocketNodeView>().ToList();
    public static GraphSaveUtility GetInstance(SocketGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        //if (!Edges.Any()) return;

        var socketGraphContainer = ScriptableObject.CreateInstance<SocketGraphContainer>();
        var connectedPorts = Edges.Where(edge => edge.input != null && edge.output != null).ToArray();

        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as SocketNodeView;
            var inputNode = connectedPorts[i].input.node as SocketNodeView;
            
            socketGraphContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var node in Nodes)
        {
            var socketData = new SocketNodeData()
            {
                Guid = node.GUID,
                Title = node.title,
                HasMovable = node.movable != null,
                GraphPosition = node.GetPosition().position,
                LocalScenePosition = node.localPosition
            };
            if (socketData.HasMovable) socketData.MovableID = GlobalObjectId.GetGlobalObjectIdSlow(node.movable).ToString();
            
            socketGraphContainer.SocketNodes.Add(socketData);
        
        }
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets","Resources");
        }
        AssetDatabase.CreateAsset(socketGraphContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }
    
    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<SocketGraphContainer>(fileName);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target Socket Graph file doesn't exist.", "Ok");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    public void LoadGraph(MovableCollection collection)
    {
        ClearGraph();
        CreateNodes(collection);
        //ConnectNodes(graphObj);
        
    }

    private void CreateNodes(MovableCollection collection)
    {
        var movables = collection.GetComponentsInChildren<Movable>();
        movables.ToList().ForEach(movable =>
        {
            var visualBounds = movable.GetComponent<MeshRenderer>().bounds;
            var localCenter = movable.transform.parent.InverseTransformPoint(visualBounds.center);
                
            var nodeView = _targetGraphView.CreateSocketNode(movable.name, localCenter, movable);
            
            nodeView.GUID = Guid.NewGuid().ToString();
            _targetGraphView.AddElement(nodeView);
        });
    }
    
    private void ConnectNodes()
    {
        _containerCache.NodeLinks.ForEach(linkData =>
        {
            //Debug.Log((Nodes.Count));
            var baseNodePort = Nodes.First(node => node.GUID == linkData.BaseNodeGuid).outputContainer.Children().ToArray()[0] as Port;
            //Debug.Log(baseNodePort);
            var targetNodePort = Nodes.First(node => node.GUID == linkData.TargetNodeGuid).inputContainer.Children().ToArray()[0] as Port;

            var edge = baseNodePort?.ConnectTo(targetNodePort);
            _targetGraphView.AddElement(edge);
        } );
    }

    private void CreateNodes()
    {
        _containerCache.SocketNodes.ForEach(nodeData =>
        {
            Movable movable = null;
            // SocketNode socket = null;
            var success = GlobalObjectId.TryParse(nodeData.MovableID, out var goID);
            if (!success)
            {
                EditorUtility.DisplayDialog("Failed to retrieve object",
                    "Global Object ID failed parse. Please check the input string.", "Ok");
                return;
            }
            if (nodeData.HasMovable) movable = (Movable)GlobalObjectId.GlobalObjectIdentifierToObjectSlow(goID);
            

            // if (nodeData.HasSocket) socket = (SocketNode) GlobalObjectId.GlobalObjectIdentifierToObjectSlow(nodeData.SocketID);

            var node = _targetGraphView.CreateSocketNode(nodeData.Title, nodeData.LocalScenePosition, movable);
            node.GUID = nodeData.Guid;

            node.SetPosition(new Rect(nodeData.GraphPosition, SocketGraphView.defaultNodeSize));
            _targetGraphView.AddElement(node);
        });
    }

    private void ClearGraph()
    {
        Nodes.ForEach(node =>
        {
            _targetGraphView.RemoveElement(node);
            node.Clear();
        });
        Edges.ForEach(edge =>
        {
            _targetGraphView.RemoveElement(edge);
            edge.Clear();
        });
    }

    public void ConvertToSceneObject()
    {
        // if (_targetGraphView.graphObject == null)
        // {
        //     EditorUtility.DisplayDialog("Graph object is not set", "You need to set a graph object present in the scene.", "Ok");
        //     return;
        // }
        var data = new Dictionary<SocketNode, List<SocketNode>>();
        
        var instanceDictionary = new Dictionary<SocketNodeView, SocketNode>();

        var go = new GameObject("New Socket Graph");
        var socketGraph = go.AddComponent<SocketGraph>();
        
        
        Nodes.ForEach(nodeView =>
        {
            if (nodeView.movable == null) return;
            var movableCopy = GameObject.Instantiate(nodeView.movable);
            socketGraph.acceptedMovables.Add(movableCopy);
            var child = new GameObject($"{movableCopy.name} Socket",typeof(BoxCollider))
            {
                transform =
                {
                    parent = go.transform,
                    localPosition = nodeView.localPosition,
                    rotation = Quaternion.identity
                }
                
            };
            //GameObject.Instantiate(child, nodeView.localPosition, Quaternion.identity, go.transform);

            var node = child.AddComponent<SocketNode>();
            node.exclusiveMovable = movableCopy;
            data.Add(node, new List<SocketNode>());
            instanceDictionary.Add(nodeView,node);
        });
        
        Edges.ForEach(edge =>
        {
            var outputNodeView = (SocketNodeView) edge.output.node;
            var inputNodeView = (SocketNodeView) edge.input.node;

            var outputNode = instanceDictionary[outputNodeView];
            var inputNode = instanceDictionary[inputNodeView];
            data[outputNode].Add(inputNode);
            data[inputNode].Add(outputNode);
        });
        DebugDictionary(data);
        var field = typeof(SocketGraph).GetField("connections", BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(socketGraph, data);
        DebugDictionary(socketGraph.connections);
    }

    void DebugDictionary(Dictionary<SocketNode, List<SocketNode>> data)
    {
        var s = "";
        data.ToList().ForEach(pair =>
        {
            var sub = "";
            pair.Value.ForEach(node =>
            {
                sub += $"{node.name},";
            });
            s += $"{pair.Key.name}, ({sub})\n";
        } );
        
        Debug.Log(s);
    }
    
}
