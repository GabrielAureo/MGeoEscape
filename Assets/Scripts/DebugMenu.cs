using UnityEngine;
using System;

public class DebugMenu: MonoBehaviour{

    private static readonly Lazy<DebugMenu> LazyInstance = new Lazy<DebugMenu>(CreateSingleton);

    public static DebugMenu Instance => LazyInstance.Value;

    private static DebugMenu CreateSingleton()
    {
        var ownerObject = new GameObject($"{typeof(DebugMenu).Name} (singleton)");
        var instance = ownerObject.AddComponent<DebugMenu>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
    private Action func;
    
     void OnGUI(){
        GUILayout.Window(1, new Rect(Screen.width - 215, 40,215, 50), DebugWindow , "Debug Menu", GUILayout.ExpandHeight(true));
        
    }
    void DebugWindow( int id){
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Network")){
            func = NetworkDebug;
        }
        if(GUILayout.Button("Touch")){
            func = TouchControllerDebug;
        }

        GUILayout.EndHorizontal();
        if(func != null) func();

    }

    void NetworkDebug(){

    }

    void TouchControllerDebug(){
        GUILayout.Label(ARTouchController.touchData.currentStatus.ToString());
    }
}