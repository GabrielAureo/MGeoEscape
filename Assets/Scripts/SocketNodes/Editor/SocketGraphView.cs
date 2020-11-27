using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;

public class SocketGraphView: GraphView{

    public readonly Vector2 defaultNodeSize = new Vector2(150,200);
    public SocketGraphView(){
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        GenerateEntryPointNode();
    }


    private Port GeneratePort(SocketNodeView node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single){
        return node.InstantiatePort(Orientation.Horizontal, portDirection,capacity,typeof(float));
    }
    private void GenerateEntryPointNode()
    {
        CreateNode("Socket Node");
    }

    public void CreateNode(string nodeName)
    {
        var node = CreateSocketNode(nodeName);
        node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        node.GUID = Guid.NewGuid().ToString();
        
        AddElement(node);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var blockedNodes = new List<Node>();
        edges.ForEach((edge) =>
        {
            if(edge.input == null || edge.output == null) return;
            if(edge.input.node == startPort.node) blockedNodes.Add(edge.output.node);
            if(edge.output.node == startPort.node) blockedNodes.Add(edge.input.node);
        });
        ports.ForEach((port)=>{
            if(startPort != port && startPort.node != port.node && !blockedNodes.Contains(port.node) && startPort.direction != port.direction){
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    public SocketNodeView CreateSocketNode(string nodeName)
    {
        var node = new SocketNodeView();
        node.title = nodeName;
        
        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        var outputPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);
        
        var movableField = new ObjectField
        {
            allowSceneObjects = true,
            label = "Movable",
            objectType = typeof(Movable)
        };
        node.contentContainer.Add(movableField);
        
        return node;

        
    }
    
}