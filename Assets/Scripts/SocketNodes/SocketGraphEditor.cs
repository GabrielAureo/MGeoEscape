using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class SocketGraphEditor: EditorWindow{
    private SocketViewGraph _graphView;

    [MenuItem("Graph/Socket Graph")]
    public static void OpenSocketGraphWindow(){
        var window = GetWindow<SocketGraphEditor>();
        window.titleContent = new GUIContent("Socket Graph");
    }

    private void OnEnable(){
        _graphView = new SocketViewGraph{
            name = "Socket Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void OnDisable(){
        rootVisualElement.Remove(_graphView);
    }
}