using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;

public class SocketGraphView: GraphView{

    public static readonly Vector2 defaultNodeSize = new Vector2(150,200);

    public SocketGraph sceneGraph;
    public SocketGraphView(){
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        GenerateEntryPointNode();

        RegisterCallback<ContextualMenuPopulateEvent>(evt =>
        {
            evt.menu.AppendAction("Fix LocalPositions", x =>
            {
                nodes.ToList().Cast<SocketNodeView>().ToList().ForEach(node =>
                {
                    var visualBounds = node.movable.GetComponent<MeshRenderer>().bounds;
                    var localCenter = node.movable.transform.parent.InverseTransformPoint(visualBounds.center);
                    node.localPosition = localCenter;
                });
            });
        });
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
        var node = CreateSocketNode(nodeName, Vector3.zero);
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

    public SocketNodeView CreateSocketNode(string nodeName, Vector3 position, Movable movable = null)
    {
        var node = new SocketNodeView()
        {
            title = nodeName,
            movable = movable,
            localPosition = position
        };

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
            objectType = typeof(Movable),
        };

        movableField.RegisterCallback < ChangeEvent < UnityEngine.Object>>(evt => node.movable = (Movable)evt.newValue);

        movableField.value = movable;

        node.contentContainer.Add(movableField);
        
        /*var socketField = new ObjectField
        {
            allowSceneObjects = true,
            label = "Socket",
            objectType = typeof(SocketNode),
            value = socket
        };
        
        socketField.RegisterCallback < ChangeEvent < UnityEngine.Object>>(evt => node.socket = (SocketNode)evt.newValue);
        socketField.value = socket;
        
        node.contentContainer.Add(socketField);*/
        

        Editor gameObjectEditor = null;
        
        if (node.movable != null)
        {
            gameObjectEditor = Editor.CreateEditor(node.movable.gameObject);
        }
        
        var imguiContainer = new IMGUIContainer(() =>
        {
            if (node.movable != null)
            {
                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect (200,200), null);
                //node.GUIRenderPreview();
                
                
            }
            
        });
        node.Add(imguiContainer);
        node.RegisterCallback<DetachFromPanelEvent>(evt=>
        {
            Editor.DestroyImmediate(gameObjectEditor);
        });
        return node;
    }

}