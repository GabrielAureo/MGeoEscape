using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.EditorTools;

[CustomEditor(typeof(Movable))]
public class MovableEditor: Editor{
    bool isEditing = false;
    bool lastEditState = false;
    string editString;

    GUIContent[] tools;
    Action[] toolsActions;

    SerializedProperty m_anchorProperty;
    SerializedProperty m_rotationProperty;
    int m_selectedTool;
    MeshRenderer m_renderer;
    Mesh m_mesh;

    void OnEnable(){
        m_anchorProperty   = serializedObject.FindProperty("bottomAnchor");
        m_rotationProperty = serializedObject.FindProperty("placementRotation");

        var movable = (Movable)target;
        m_renderer = movable.GetComponent<MeshRenderer>();
        m_mesh = movable.GetComponent<MeshFilter>().sharedMesh;

        tools = new GUIContent[2];
        toolsActions = new Action[2];
        var index = 0;

        tools[index++] = EditorGUIUtility.TrIconContent("MoveTool", "Move Tool");
        tools[index++] = EditorGUIUtility.TrIconContent("RotateTool", "Rotate Tool");
        index = 0;
        toolsActions[index++] = MoveTool;
        toolsActions[index++] = RotateTool;

        m_selectedTool = -1;
    }

    public override void OnInspectorGUI(){
        var editStyle = new GUIStyle(GUI.skin.button);
        
        DrawToolbar();
        
        
        DrawPropertiesExcluding(serializedObject, "bottomAnchor");

        EditorGUILayout.EditorToolbar();
        EditorGUILayout.BeginHorizontal();
        
        isEditing = GUILayout.Toggle(isEditing, editString, editStyle);
        if(!isEditing){
            editString = "Show Anchor Gizmo";
            GUILayout.Button("Find Mesh Bottom");
        }else{
            editString = "Hide Anchor Gizmo";
        }
        
        //TODO: dISCOVER WHY I can't change values on inspector
        

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(m_anchorProperty);
        //serializedObject.ApplyModifiedProperties();  
        lastEditState = isEditing;
        
    }

    void RepaintView(){
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();
    }
    private void DrawToolbar(){
        EditorGUI.BeginChangeCheck();
        var _selectedTool = GUILayout.Toolbar(m_selectedTool, tools);  
        var changed = EditorGUI.EndChangeCheck();

        if (_selectedTool != m_selectedTool)
        {
            m_selectedTool = _selectedTool;
            //Do something since the selected button changed
        }
        else if (changed)
        {
            // A click occurred on a button that was already selected. Reset the tool selection.
            m_selectedTool = -1;
        }
        
        if(changed){
            RepaintView();
        }
    }

    private void DrawMesh(Vector3 position, Quaternion rotation){
        Graphics.DrawMesh(m_mesh, position, rotation, m_renderer.sharedMaterial, LayerMask.NameToLayer("Default"));
    }

    void OnSceneGUI(){
        if(m_selectedTool >0 && m_selectedTool < toolsActions.Length) toolsActions[m_selectedTool]();
    }

    private void MoveTool(){
        DrawPositionGizmo();
    }

    private void RotateTool(){
        var movable = (Movable)target;
        
        Handles.Label(movable.transform.position, m_rotationProperty.displayName);
        //SceneVisibilityManager.instance.ToggleVisibility(movable.gameObject, true);
        EditorGUI.BeginChangeCheck();
        var rotation = Handles.RotationHandle(m_rotationProperty.quaternionValue,movable.transform.position);
        if(EditorGUI.EndChangeCheck()){
            m_rotationProperty.quaternionValue = rotation;
            serializedObject.ApplyModifiedProperties();
        }
        DrawMesh(movable.transform.position, m_rotationProperty.quaternionValue);
        
    }

    private void CalculateMeshBottom(){
        
    }

    private void DrawPositionGizmo()
    {
        EditorGUI.BeginChangeCheck();
        Tools.current = Tool.None;
        var transform = ((Movable)target).transform;
        var worldAnchor = transform.TransformPoint(m_anchorProperty.vector3Value);
        Handles.Label(worldAnchor, "Bottom Anchor");
        worldAnchor = Handles.PositionHandle(worldAnchor, Quaternion.identity);

        if(EditorGUI.EndChangeCheck()){
            m_anchorProperty.vector3Value = transform.InverseTransformPoint(worldAnchor);
            serializedObject.ApplyModifiedProperties();  
        }
        
    }
}