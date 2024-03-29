using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(Socket))]
public class SocketEditor: Editor{
    private SerializedProperty currentObject;
    private SerializedProperty exclusiveMode;
    private SerializedProperty exclusiveObject;
    //private SerializedProperty placementPose;
    private SerializedProperty placementAnchor;
    private SerializedProperty useDefaultBusyAnim;
    private SerializedProperty useDefaultTargetAnim;
    Socket targetSocket;
    GameObject placementDummy;
    System.Action selectionDelegate;
    bool isEditing;
    
    MovablePlacementPose movePose = new MovablePlacementPose();
    MovablePlacementPose handle = new MovablePlacementPose();

    void OnEnable(){
        currentObject = serializedObject.FindProperty("_currentObject");
        exclusiveMode = serializedObject.FindProperty("exclusiveMode");
        exclusiveObject = serializedObject.FindProperty("exclusiveObject");
        //placementPose = serializedObject.FindProperty("placementPose");
        placementAnchor = serializedObject.FindProperty("placementAnchor");
        useDefaultBusyAnim = serializedObject.FindProperty("useDefaultBusyAnimation");
        useDefaultTargetAnim = serializedObject.FindProperty("useDefaultTargetAnimation");
        targetSocket = (Socket)target;
        //EditorApplication.hierarchyChanged += ()=> CheckChildren();
        isEditing = false;
        if(movePose == null) movePose = new MovablePlacementPose();
        if(handle == null) handle = new MovablePlacementPose();
        //ovePose = GetPose();
        ConvertToGlobal(ref handle, movePose, targetSocket);

    }
    
    // private MovablePlacementPose GetPose(){
    //     var pos = placementPose.FindPropertyRelative("position").vector3Value;
    //     var rot = placementPose.FindPropertyRelative("rotation").quaternionValue;
    //     var scale = placementPose.FindPropertyRelative("scale").vector3Value;

    //     return new MovablePlacementPose(pos, rot, scale);
    // }

    private void ConvertToLocal(MovablePlacementPose global, ref MovablePlacementPose local, Socket target){
        local.position = target.transform.InverseTransformPoint(global.position);
        local.rotation = Quaternion.Inverse(target.transform.rotation) * global.rotation;
        local.scale = DivideVectors(global.scale, target.transform.localScale);
    }

    private void ConvertToGlobal(ref MovablePlacementPose global, MovablePlacementPose local, Socket target){
        global.position = target.transform.TransformPoint(local.position);
        global.rotation = target.transform.rotation * local.rotation;
        global.scale = MultiplyVectors(local.scale, target.transform.localScale);
    }

    private Vector3 MultiplyVectors(Vector3 v, Vector3 u){
        return new Vector3(v.x * u.x, v.y * u.y, v.z * u.z);
    }

    void OnDisable(){
        StopEditing();
    }

    private void StopEditing(){
        isEditing = false;
        if(placementDummy) DestroyImmediate(placementDummy);
        Tools.hidden = false;
    }


    private void CreateDummy(){
        placementDummy = (GameObject)GameObject.Instantiate(exclusiveObject.objectReferenceValue, targetSocket.transform);


        placementDummy.transform.localPosition = movePose.position; 
        placementDummy.transform.localRotation = movePose.rotation;
        placementDummy.transform.localScale = movePose.scale;

        //Selection.activeGameObject = placementDummy;

    }

    public override void OnInspectorGUI(){
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(exclusiveMode);
        if(EditorGUI.EndChangeCheck()){
            if(!exclusiveMode.boolValue) StopEditing();
        }
        if(exclusiveMode.boolValue){
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(exclusiveObject);
            if(EditorGUI.EndChangeCheck()){
                if(exclusiveObject.objectReferenceValue == null) StopEditing();
            }
            
            if(exclusiveObject.objectReferenceValue != null){
                if(!isEditing){
                    if(GUILayout.Button("Edit exclusive object pose")){
                        CreateDummy();
                        isEditing = true;
                        Tools.hidden = true;
                    }
                }else{
                    if(GUILayout.Button("Stop Editing")){
                        StopEditing();
                    }
                }
            }else{
                EditorGUILayout.HelpBox("Set an exclusive object to change the placement pose",MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }else{
            if(!isEditing){
                if(GUILayout.Button("Edit placement anchor")){
                    isEditing = true;
                    Tools.hidden = true;
                    RefreshSceneView();
                }
            }else{
                if(GUILayout.Button("Stop editing")){
                    isEditing = false;
                    Tools.hidden = false;
                    RefreshSceneView();
                    
                }
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(placementAnchor);
                if(EditorGUI.EndChangeCheck()){
                    RefreshSceneView();
                }
            }
        }

        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.ObjectField(currentObject);
        if(EditorGUI.EndChangeCheck()){
            Movable movable = (Movable)currentObject.objectReferenceValue;
            var sockets = GameObject.FindObjectsOfType(typeof(Socket));

            foreach(Socket socket in sockets){
                if(socket != targetSocket && socket.currentObject == movable){
                    UnsetObject(socket);
                    
                }
            }
            SetObject(movable);
        }

        // EditorGUI.BeginChangeCheck();
        // EditorGUILayout.PropertyField(placementPose);
        
        // if(EditorGUI.EndChangeCheck()){
        //     SetDummyPose();
        //     ConvertToGlobal(ref handle, movePose, targetSocket);
        // }
        EditorGUILayout.PropertyField(useDefaultTargetAnim);
        EditorGUILayout.PropertyField(useDefaultBusyAnim);
        serializedObject.ApplyModifiedProperties();
        
        
    }

    void RefreshSceneView(){
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();

    }
    
    void OnSceneGUI(){
        var socket = target as Socket;
        if(socket == null) return;
        if(isEditing){
            if(exclusiveMode.boolValue){
                ExclusiveTransformTool(socket);
            }else{
                MoveTool();
            }
            
             
        }
    }
    private void ExclusiveTransformTool(Socket socket){
        EditorGUI.BeginChangeCheck();
        Handles.TransformHandle(ref handle.position, ref handle.rotation, ref handle.scale);
        if(EditorGUI.EndChangeCheck()){
            Undo.RecordObject(targetSocket, "Changed Placement Pose");
            ConvertToLocal(handle, ref movePose, socket);
            movePose.SetToTransform(placementDummy.transform);
            socket.exclusivePose = movePose;
        } 
    }
    private void MoveTool(){
        EditorGUI.BeginChangeCheck();
        var transform = ((MonoBehaviour)target).transform;
        var worldAnchor = transform.TransformPoint(placementAnchor.vector3Value);
        Handles.Label(worldAnchor, "Placement Anchor");
        worldAnchor = Handles.PositionHandle(worldAnchor, Quaternion.identity);

        if(EditorGUI.EndChangeCheck()){
            placementAnchor.vector3Value = transform.InverseTransformPoint(worldAnchor);
            serializedObject.ApplyModifiedProperties();  
        }
        
    }

    

    private Vector3 DivideVectors(Vector3 v, Vector3 u){
        return new Vector3(v.x/u.x, v.y/u.y, v.z/u.z);
    }

    // public void SetDummyPose(){
    //     movePose.position = placementPose.FindPropertyRelative("position").vector3Value;
    //     movePose.rotation = placementPose.FindPropertyRelative("rotation").quaternionValue;
    //     movePose.scale = placementPose.FindPropertyRelative("scale").vector3Value;
    //     if(!placementDummy) return;
    //     placementDummy.transform.localPosition = movePose.position;
    //     placementDummy.transform.localRotation = movePose.rotation;
    //     placementDummy.transform.localScale = movePose.scale;
    // }

    // private void CheckChildren(){
    //     if(Application.isPlaying) return;
    //     var childMovable = targetSocket.transform.GetComponentInChildren<Movable>();
    //     if(childMovable == null && targetSocket.currentObject != null){
    //         UnsetObject(targetSocket);
    //     }
    // }

    private void SetObject(Movable movable){
        Undo.SetCurrentGroupName("Set Movable to Socket");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObject(targetSocket, "Set Movable on currentObject");
        

        //targetSocket.currentObject = movable;
        currentObject.objectReferenceValue = movable;
        if(movable != null){
            Undo.RecordObject(movable.transform, "Anchor Movable to surface");
            movable.transform.position = targetSocket.transform.position - (movable.bottomAnchor - targetSocket.placementAnchor);
        }

        Undo.CollapseUndoOperations(group);
    }

    private void UnsetObject(Socket socket){
        Undo.SetCurrentGroupName("Unset Movable from Socket");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObject(socket, "Remove Movable reference from socket object");
        //socket.currentObject = null;
        currentObject.objectReferenceValue = null;
        EditorUtility.SetDirty(socket);

        Undo.CollapseUndoOperations(group);
    }
    

     
    
}