using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

public class SocketViewGraph: GraphView{
    public SocketViewGraph(){
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        AddElement(GenerateEntryPointNode());
    }


    private Port GeneratePort(SocketNodeView node, Direction portDirection, Port.Capacity capacity){
        return null;
    }
    private SocketNodeView GenerateEntryPointNode()
    {
        var node = new SocketNodeView{
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
        };
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }
}