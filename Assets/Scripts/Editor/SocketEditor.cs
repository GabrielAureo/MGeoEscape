using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(Socket))]
public class SocketEditor: Editor{
    SerializedProperty currentObject;
    Socket targetSocket;

    void OnEnable(){
        currentObject = serializedObject.FindProperty("currentObject");
        targetSocket = (Socket)target;
        EditorApplication.hierarchyChanged += ()=> CheckChildren();
    }
    public override void OnInspectorGUI(){
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.ObjectField(currentObject);
        if(EditorGUI.EndChangeCheck()){
            Movable movable = (Movable)currentObject.objectReferenceValue;
            var sockets = GameObject.FindObjectsOfType(typeof(Socket));

            foreach(Socket socket in sockets){
                if(socket != targetSocket && socket.currentObject == movable){
                    UnsetObject(socket);
                    // Undo.RecordObject(socket, "Unset Movable from old object");
                    // MethodInfo unset = socket.GetType().GetMethod("UnsetObject", BindingFlags.NonPublic | BindingFlags.Instance);
                    // unset.Invoke(socket, new object[]{});
                    // EditorUtility.SetDirty(socket);
                    
                }
            }
            SetObject(movable);
            // Undo.RecordObject(target,"Assign Movable");
            // MethodInfo method = targetSocket.GetType().GetMethod("SetObject", BindingFlags.NonPublic | BindingFlags.Instance);
            // method.Invoke(targetSocket, new object[]{movable});
        }
        
        DrawDefaultInspector();
    }

    private void CheckChildren(){
        var childMovable = targetSocket.transform.GetComponentInChildren<Movable>();
        if(childMovable == null && targetSocket.currentObject != null){
            UnsetObject(targetSocket);
        }
    }

    private void SetObject(Movable movable){
        Undo.SetCurrentGroupName("Set Movable to Socket");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObject(targetSocket, "Set Movable on currentObject");
        Undo.SetTransformParent(movable.transform, targetSocket.transform, "Set Movable parent");
        Undo.RecordObject(movable.transform, "Anchor Movable to surface");

        targetSocket.currentObject = movable;
        EditorUtility.SetDirty(targetSocket);

        movable.transform.localPosition = Vector3.zero; // change this to offset

        Undo.CollapseUndoOperations(group);
    }

    private void UnsetObject(Socket socket){
        Undo.SetCurrentGroupName("Unset Movable from Socket");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObject(socket, "Remove Movable reference from socket object");
        socket.currentObject = null;
        EditorUtility.SetDirty(socket);

        Undo.CollapseUndoOperations(group);
    }
    

     
    
}