using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SocketGraphWindow: EditorWindow{
    private SocketGraphView _graphView;
    private string _fileName = "New Socket Graph";
    private GraphSaveUtility _saveUtility;
    private TemplateContainer _sidebar;
    

    [MenuItem("Graph/Socket Graph")]
    public static void OpenSocketGraphWindow(){
        var window = GetWindow<SocketGraphWindow>();
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
        CreateGraphFromGameObjectControls();
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
    
    
    private void CreateGraphFromGameObjectControls() {
        var creatBtn = new Button() {text = "Create new"};
        _sidebar.Add(creatBtn);
        creatBtn.clickable.clicked += () =>
        {
            var index = _sidebar.IndexOf(creatBtn);
            _sidebar.Remove(creatBtn);
            var objField = new ObjectField()
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
                
            };
            objField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                if (evt.newValue == null)
                {
                    objField.value = evt.previousValue;
                    return;
                }

                var parentCollection = (GameObject) evt.newValue;
                
                _saveUtility.CreateGraph(parentCollection);
                
            });
            
            _sidebar.Insert(index, objField);
        };
    }

    private void SetGraphAsLoaded()
    {
        _sidebar.Add(new Button(){text = "Export as new"});
        //_sidebar.Add(new Button(() => _saveUtility.UpdateSceneObject((SocketGraph)objField.value)){text = "Update Scene Graph"});
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