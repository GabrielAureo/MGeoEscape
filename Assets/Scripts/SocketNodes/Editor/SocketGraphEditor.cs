using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SocketGraphEditor: EditorWindow{
    private SocketGraphView _graphView;
    private string _fileName = "New Socket Graph";
    private GraphSaveUtility _saveUtility;
    private TemplateContainer _sidebar;
    

    [MenuItem("Graph/Socket Graph")]
    public static void OpenSocketGraphWindow(){
        var window = GetWindow<SocketGraphEditor>();
        window.titleContent = new GUIContent("Socket Graph");
    }

    private void Init()
    {
        _graphView = new SocketGraphView();
        _graphView.AddToClassList("graphView");
        
        _saveUtility = GraphSaveUtility.GetInstance(_graphView);
        _sidebar = new TemplateContainer();
        _sidebar.AddToClassList("sidebar");

    }

    private void ConstructLayout()
    {
        rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/SocketNodes/SocketGraphEditorStylesheet.uss"));
        ConstructSidebar();
        var separator = new VisualElement();
        separator.AddToClassList("separator");
        rootVisualElement.Add(separator);
        ConstructGraphView();
    }
    private void OnEnable()
    {
        Init();
        ConstructLayout();
        
    }

    private void ConstructSidebar()
    {
        _sidebar.Add(new Button() {text = "Load by Graph Asset"});
        LoadSceneGraphControls();
        
        _sidebar.Add(new Button((() => _graphView.CreateNode("Socket Node"))){text = "Create Empty Node"});
        _sidebar.Add(new Button(() => RequestDataOperation(true)){text="Save Data"});
        _sidebar.Add(new Button(() => RequestDataOperation(false)){text="Load Data"});
        rootVisualElement.Add(_sidebar);
    }

    private void LoadSceneGraphControls()
    {
        var loadGraphBtn = new Button() {text = "Load by Scene Graph"};
        _sidebar.Add(loadGraphBtn);
        loadGraphBtn.clickable.clicked += () =>
        {
            var index = _sidebar.IndexOf(loadGraphBtn);
            _sidebar.Remove(loadGraphBtn);
            var objField = new ObjectField()
            {
                objectType = typeof(SocketGraph),
                allowSceneObjects = true,
            };
            objField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                if (evt.newValue == null)
                {
                    objField.value = evt.previousValue;
                    return;
                }

                var sceneGraph = (SocketGraph) evt.newValue;
                _graphView.sceneGraph = sceneGraph;
                _saveUtility.LoadGraph(sceneGraph);
                _sidebar.Add(new Button(){text = "Export as new"});
                _sidebar.Add(new Button(() => _saveUtility.UpdateSceneObject((SocketGraph)objField.value)){text = "Update Scene Graph"});
            });
            
            _sidebar.Insert(index, objField);
            
        };

    }



    private void ConstructGraphView()
    {
        var container = new VisualElement()
        {
            name = "graphContainer"
        };
        
        _graphView.StretchToParentSize();
        container.Add(_graphView);
        rootVisualElement.Add(container);
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

        
        _saveUtility.LoadGraph(sg);
        
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

        _saveUtility.ConvertToSceneObject();
        
    }

    private void RequestDataOperation(bool save)
    {
        // if (string.IsNullOrEmpty(_fileName))
        // {
        //     EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "Ok");
        //     return;
        // }
        // Debug.Log(_graphView.nodes.ToList().Count);
        var guid = (_graphView.sceneGraph != null)?_graphView.sceneGraph.GUID:System.Guid.NewGuid().ToString();
        if (save) _saveUtility.SaveGraph(guid);
        else _saveUtility.LoadGraph(guid);
        
        //_objectField.value = _graphView.graphObject;
    }

    private void OnDisable(){
        
    }
}