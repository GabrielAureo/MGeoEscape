using UnityEngine;
using System;
using Mirror;
using System.Collections.Generic;

public class DebugMenu: MonoBehaviour{

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
        if(GUILayout.Button("Puzzles")){
            func = PuzzlesDebug;
        }

        GUILayout.EndHorizontal();
        if(func != null) func();

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