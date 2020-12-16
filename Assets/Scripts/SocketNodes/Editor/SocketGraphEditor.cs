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
        
        GraphObjectControls(toolbar);
        nodeCreateBtn.text = "Create Node";
        toolbar.Add(nodeCreateBtn);
        rootVisualElement.Add(toolbar);
    }

    private ObjectField _objectField;
    private void GraphObjectControls(Toolbar toolbar)
    {
        _objectField = new ObjectField
        {
            objectType = typeof(MovableCollection),
            allowSceneObjects = true,
        };
        
        var loadBtn = new Button(() =>
        {
            LoadGraphObject(_objectField.value);
        }){text = "Add Graph Object"};
        
        var exportBtn = new Button(ExportGraphObject){text = "Export Graph Object"};

        _objectField.RegisterCallback<ChangeEvent<Object>>(evt =>
        {
            loadBtn.SetEnabled(evt.newValue != null);
        });
        
        loadBtn.SetEnabled(_objectField.value != null);
        toolbar.Add(_objectField);
        toolbar.Add(loadBtn);
        toolbar.Add(exportBtn);
    }

    private void LoadGraphObject(Object obj)
    {
        if (_graphView.collection != null || _graphView.nodes.ToList().Count > 0)
        {
            var proceed = EditorUtility.DisplayDialog("Graph View not empty",
                "The Graph View already has a serialized graph object.\n" +
                "Would you like to wipe the current graph?", "Yes", "No");
            if (!proceed) return;
        }


        var sg = _graphView.collection = (MovableCollection) obj;

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.LoadGraph(sg);
        
    }

    private void ExportGraphObject()
    {
        // var collection = _graphView.collection;
        // if (collection == null)
        // {
        //     EditorUtility.DisplayDialog("Graph Object not set",
        //         "You need to set a graph object before exporting the current graph view.", "Ok");
        //     return;
        // }

        GraphSaveUtility.GetInstance(_graphView).ConvertToSceneObject();
        
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
        
        //_objectField.value = _graphView.graphObject;
    }

    private void OnDisable(){
        rootVisualElement.Remove(_graphView);
    }
}