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
    private const string CONFIGURATION_ASSET_NAME = "GraphConfigurations";

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<SocketNodeView> Nodes => _targetGraphView.nodes.ToList().Cast<SocketNodeView>().ToList();
    public static GraphSaveUtility GetInstance(SocketGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    private Dictionary<SocketNode, string> sceneNodesTable;

    public void SaveGraph(string guid)
    {
        //if (!Edges.Any()) return;

        //var socketGraphContainer = ScriptableObject.CreateInstance<SocketGraphContainer>();
        var configurationsFile = Resources.Load<GraphViewConfigurations>(CONFIGURATION_ASSET_NAME);
        if (configurationsFile == null)
        {
            configurationsFile = CreateConfigurationFile();
        }

        var socketGraphContainer = new SocketGraphContainer();
        
        var connectedPorts = Edges.Where(edge => edge.input != null && edge.output != null).ToArray();

        foreach (var port in connectedPorts)
        {
            var outputNode = port.output.node as SocketNodeView;
            var inputNode = port.input.node as SocketNodeView;
            
            socketGraphContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = port.output.portName,
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

        if (_targetGraphView.sceneGraph != null)
        {
            if (configurationsFile.data.ContainsKey(_targetGraphView.sceneGraph.GUID))
            {
                configurationsFile.data[_targetGraphView.sceneGraph.GUID] = socketGraphContainer;
            }
            else
            {
                configurationsFile.data.Add(_targetGraphView.sceneGraph.GUID, socketGraphContainer);
            }
            
        }
        
        // if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        // {
        //     AssetDatabase.CreateFolder("Assets","Resources");
        // }
        // AssetDatabase.CreateAsset(socketGraphContainer, $"Assets/Resources/{fileName}.asset");
        // AssetDatabase.SaveAssets();
    }

    private GraphViewConfigurations CreateConfigurationFile()
    {
        var configurationsFile = ScriptableObject.CreateInstance<GraphViewConfigurations>();
        
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets","Resources");
        }
        AssetDatabase.CreateAsset(configurationsFile, $"Assets/Resources/{CONFIGURATION_ASSET_NAME}.asset");
        AssetDatabase.SaveAssets();
        return configurationsFile;
    }
    
    public void LoadGraph(string guid)
    {
        var configFile = Resources.Load<GraphViewConfigurations>(CONFIGURATION_ASSET_NAME);
        _containerCache = configFile.data[guid];
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target Socket Graph key doesn't exist.", "Ok");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    public void CreateGraph(GameObject collection)
    {
        var movables = collection.GetComponentsInChildren<Movable>();
        ClearGraph();
        CreateNodes(movables.ToList());
        //ConnectNodes(graphObj);
        
    }

    public void LoadGraph(SocketGraph sceneGraph)
    {
        var configFile = Resources.Load<GraphViewConfigurations>(CONFIGURATION_ASSET_NAME);
        if (configFile.data.ContainsKey(sceneGraph.GUID))
        {
            var loadFromConfigFile = EditorUtility.DisplayDialog("Graph Key Found",
                "An entry for this scene Socket Graph was found on the " +
                "configuration file. Would like to load it from there?", "Load from file", "Load from scene");
            if (loadFromConfigFile)
            {
                LoadGraph(sceneGraph.GUID);
                return;
            }
        }
        var childNodes = sceneGraph.GetComponentsInChildren<SocketNode>();
        ClearGraph();
        CreateNodes(childNodes.ToList());
        //ConnectNodes(sceneGraph.connections);
    }
    
    private void CreateNodes(List<SocketNode> nodeList)
    {
        sceneNodesTable = new Dictionary<SocketNode, string>();
        nodeList.ForEach(node =>
        {
            var movable = node.exclusiveMovable;
            var nodeView = _targetGraphView.CreateSocketNode(movable.name, node.transform.localPosition, movable);
            nodeView.GUID = node.GUID;
            sceneNodesTable.Add(node, nodeView.GUID);
            _targetGraphView.AddElement(nodeView);
        });
        
    }



    private void CreateNodes(List<Movable> movables)
    {
        movables.ToList().ForEach(movable =>
        {
            var visualBounds = movable.GetComponent<MeshRenderer>().bounds;
            var localCenter = movable.transform.parent.InverseTransformPoint(visualBounds.center);
                
            var nodeView = _targetGraphView.CreateSocketNode(movable.name, localCenter, movable);
            
            nodeView.GUID = Guid.NewGuid().ToString();
            _targetGraphView.AddElement(nodeView);
        });
    }
    private void ConnectNodes(EdgesDictionary connections)
    {
        var unpackedConnections = connections.Unpack();
        foreach (var kvp in unpackedConnections)
        {
            var searchGuid = sceneNodesTable[kvp.Key];
            var baseNodePort = Nodes.First(node => node.GUID == searchGuid).outputContainer.Children().ToArray()[0] as Port;
            var targetNodePort = Nodes.First(node => node.GUID == searchGuid).inputContainer.Children().ToArray()[0] as Port;
            var edge = baseNodePort?.ConnectTo(targetNodePort);
            _targetGraphView.AddElement(edge);
        }
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

    public void UpdateSceneObject(SocketGraph sceneGraph)
    {
        var acceptedMovables = new List<Movable>();
        var connections = new EdgesDictionary();

        var childrenNodes = sceneGraph.GetComponentsInChildren<SocketNode>();
        Nodes.ForEach(view =>
        {
            acceptedMovables.Add(view.movable);
        });
        Edges.ForEach(edge =>
        {
            var outputNodeView = (SocketNodeView) edge.output.node;
            var inputNodeView = (SocketNodeView) edge.input.node;
            var outputNode = childrenNodes.First(node => node.GUID == outputNodeView.GUID);
            var inputNode = childrenNodes.First(node => node.GUID == inputNodeView.GUID);
            // var outputNode = sceneNodesTable.First(kvp => kvp.Value == outputNodeView.GUID).Key;
            // var inputNode = sceneNodesTable.First(kvp => kvp.Value == inputNodeView.GUID).Key;
            if (inputNode == null || outputNode == null) return;
            if(!connections.ContainsKey(outputNode)) connections.Add(outputNode, new SocketNodeNeighbors());
            connections[outputNode].data.Add(inputNode);
            if(!connections.ContainsKey(inputNode)) connections.Add(inputNode, new SocketNodeNeighbors());
            connections[inputNode].data.Add(outputNode);
            
        });
        //sceneGraph.connections = connections;
        sceneGraph.acceptedMovables = acceptedMovables;
    }

    public void ExportAsSceneObject()
    {
        
    }
    public void ConvertToSceneObject()
    {
        // if (_targetGraphView.graphObject == null)
        // {
        //     EditorUtility.DisplayDialog("Graph object is not set", "You need to set a graph object present in the scene.", "Ok");
        //     return;
        // }
        var dict = new Dictionary<SocketNode, SocketNodeNeighbors>();
        
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
            dict.Add(node, new SocketNodeNeighbors());
            instanceDictionary.Add(nodeView,node);
        });
        
        Edges.ForEach(edge =>
        {
            var outputNodeView = (SocketNodeView) edge.output.node;
            var inputNodeView = (SocketNodeView) edge.input.node;

            var outputNode = instanceDictionary[outputNodeView];
            var inputNode = instanceDictionary[inputNodeView];
            dict[outputNode].data.Add(inputNode);
            dict[inputNode].data.Add(outputNode);
        });
        DebugDictionary(dict);
        var field = typeof(SocketGraph).GetField("connections", BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(socketGraph, dict);
        //DebugDictionary(socketGraph.connections.Clone());
    }

    void DebugDictionary(Dictionary<SocketNode, SocketNodeNeighbors> data)
    {
        var s = "";
        data.ToList().ForEach(pair =>
        {
            var sub = "";
            pair.Value.data.ForEach(node =>
            {
                sub += $"{node.name},";
            });
            s += $"{pair.Key.name}, ({sub})\n";
        } );
        
        Debug.Log(s);
    }
    
}
