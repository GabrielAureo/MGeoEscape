using UnityEngine;
using System;
using Mirror;
using System.Collections.Generic;

public class DebugMenu: MonoBehaviour{
    private Action func;
    private Socket[] sockets;
    
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
        if(GUILayout.Button("Puzzles")){
            func = PuzzlesDebug;
        }
        if(GUILayout.Button("Sockets")){
            func = SocketsDebug;
        }

        GUILayout.EndHorizontal();
        if(func != null) func();

    }

    void Update(){
        sockets = LookBehaviour<Socket>((x) =>
        {
            List<Socket> sockets = new List<Socket>();
            foreach(var y in x){
                var socket = y.collider.GetComponent<Socket>();
                if(socket) sockets.Add(socket);
            }
            return sockets.ToArray();
        });
    }


 
    T[] LookBehaviour<T>(Func<RaycastHit[], T[]> condition){
        Vector2 inputPosition = Input.mousePosition;
        #if UNITY_ANDROID && !UNITY_EDITOR
            inputPosition = Input.touches[0].position;
        #endif

        var wrldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 1.35f));
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        
        return condition(hits);
        
    }

    void SocketsDebug(){
        foreach(var socket in sockets){
            DrawLabeled("Socket", socket.name);
            var movableName = "Empty";
            if(socket.currentObject) movableName = socket.currentObject.name;
            DrawLabeled("Movable", movableName);
            DrawLabeled("Empty", socket._empty.ToString());
            DrawLabeled("Busy", socket._busy.ToString());
        }
    }

    void PuzzlesDebug(){
        int s = 0;
        string[] puzzles = {"Safe"};
        s = GUILayout.Toolbar(s,puzzles);

        switch(s){
            case 0: 
            SafePuzzleDebug();
            break;
        }
    }
    void SafePuzzleDebug(){
        SafePuzzle sp = MainManager.Instance.puzzlesManager.puzzles.Find((x) => x.GetType() == typeof(SafePuzzle)) as SafePuzzle;
        Debug.Log(sp.generatedPassword.Count);
        //sp.generatedPassword.Add(100);
        var password = "";

        foreach(var pass in sp.generatedPassword){
            password += pass.ToString() + " ";
        }
        password.TrimEnd(' ');
        DrawLabeled("Password", password);

        GUILayout.Button("Unlock Safe (TODO)");
    }
    void NetworkDebug(){
        if(ClientScene.localPlayer != null){
            var character = ClientScene.localPlayer.GetComponent<GamePlayer>().character;
            DrawLabeled("Character", System.Enum.GetName(typeof(Character), character));
        }

        if (NetworkServer.active)
        {
            foreach (var kvp in NetworkServer.connections)
            {
                DrawLabeled(kvp.Value.ToString(), kvp.Value.identity.ToString());
            }
        }
    }

    void DrawLabeled(string label, string content){
        var labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle);
        GUILayout.Label(content);
        GUILayout.EndHorizontal();
        
    }

    void TouchControllerDebug(){
        GUILayout.Label(ARTouchController.touchData.currentStatus.ToString());
    }
}