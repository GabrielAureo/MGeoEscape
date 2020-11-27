using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        if (!Edges.Any()) return;

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
            socketGraphContainer.SocketNodes.Add(new SocketNodeData()
            {
                Guid = node.GUID,
                Title = node.title,
                Movable = node.movable,
                Position = node.GetPosition().position
            });
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

    private void ConnectNodes()
    {
        _containerCache.NodeLinks.ForEach(linkData =>
        {
            Debug.Log((Nodes.Count));
            var baseNodePort = Nodes.First(node =>
            {
                return node.GUID == linkData.BaseNodeGuid;
            }).outputContainer.Children().ToArray()[0] as Port;
            Debug.Log(baseNodePort);
            var targetNodePort = Nodes.First(node =>
            {
                
                return node.GUID == linkData.TargetNodeGuid;
            }).inputContainer.Children().ToArray()[0] as Port;

            var edge = baseNodePort.ConnectTo(targetNodePort);
            _targetGraphView.AddElement(edge);
        } );
    }

    private void CreateNodes()
    {
        _containerCache.SocketNodes.ForEach(nodeData =>
        {
            var node = _targetGraphView.CreateSocketNode(nodeData.Title);
            node.GUID = nodeData.Guid;
            node.movable = nodeData.Movable;
            node.SetPosition(new Rect(nodeData.Position, _targetGraphView.defaultNodeSize));
            _targetGraphView.AddElement(node);
            Debug.Log(_targetGraphView.nodes.ToList().Count);
        });
    }

    private void ClearGraph()
    {
        Nodes.ForEach(node => _targetGraphView.RemoveElement(node));
        Edges.ForEach(edge => _targetGraphView.RemoveElement(edge));
    }
    
}
