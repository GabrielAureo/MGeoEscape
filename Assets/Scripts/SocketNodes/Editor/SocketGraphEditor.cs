using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SocketGraphEditor: EditorWindow{
    private SocketGraphView _graphView;
    private string _fileName = "New Socket Graph";

    [MenuItem("Graph/Socket Graph")]
    public static void OpenSocketGraphWindow(){
        var window = GetWindow<SocketGraphEditor>();
        window.titleContent = new GUIContent("Socket Graph");
    }

    private void OnEnable(){
        ConstructGraphView();
        ConstructToolbar();
    }

    private void ConstructGraphView(){
        _graphView = new SocketGraphView{
            name = "Socket Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void ConstructToolbar(){
        var toolbar = new Toolbar();
        
        var fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt =>_fileName = evt.newValue);
        toolbar.Add(fileNameTextField);
        
        toolbar.Add(new Button(() => RequestDataOperation(true)){text="Save Data"});
        toolbar.Add(new Button(() => RequestDataOperation(false)){text="Load Data"});
        
        var nodeCreateBtn = new Button(clickEvent: () =>{
            _graphView.CreateNode("Socket Node");
        });

        nodeCreateBtn.text = "Create Node";
        toolbar.Add(nodeCreateBtn);
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "Ok");
            return;
        }
        Debug.Log(_graphView.nodes.ToList().Count);
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save) saveUtility.SaveGraph(_fileName);
        else saveUtility.LoadGraph(_fileName);
    }

    private void OnDisable(){
        rootVisualElement.Remove(_graphView);
    }
}