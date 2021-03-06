using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using UnityEditor.SceneManagement;


public class BuildScript{
    
    public static string buildPath{
        get{
            var rootPath = System.IO.Directory.GetCurrentDirectory();
            var path = System.IO.Path.Combine(rootPath, "Builds\\MGeoEscape.exe");
            return path;
        }
    }
    [MenuItem("File/Build Standalone",false,1000)]
    public static void PerformWindowsBuild(){
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        var scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            scenes.Add(scene.path);
        }
        
        buildPlayerOptions.scenes = scenes.ToArray();
        buildPlayerOptions.locationPathName = "./Builds/MGeoEscape.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        while(BuildPipeline.isBuildingPlayer){} 
        if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded){
            Run();
        }
        
    }
    [MenuItem("File/Run Standalone/All",false,1001)]
    public static void Run(){
        RunPlayerOne();
        RunPlayerTwo();
        RunPlayerThree();

        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

        EditorApplication.EnterPlaymode();
    }

    [MenuItem("File/Run Standalone/Single/Player 1",false,1002)]
    public static void RunPlayerOne(){
        StartWindow(1, "f342e61e-9946-41a6-b2ca-d3a6e54f7af8");
    }

    [MenuItem("File/Run Standalone/Single/Player 2",false,1003)]
    public static void RunPlayerTwo(){
        // Process.Start(buildPath,"a54e0a4a-df2c-4ec3-9c77-f8a8b1261a73");
        StartWindow(2, "a54e0a4a-df2c-4ec3-9c77-f8a8b1261a73");
    }

    [MenuItem("File/Run Standalone/Single/Player 3",false,1004)]
    public static void RunPlayerThree(){
        // Process.Start(buildPath,"0dc42813-73cb-4e3e-9736-0ece4049ad4c");
        StartWindow(3, "0dc42813-73cb-4e3e-9736-0ece4049ad4c");
    }

    private static void StartWindow(int player, string guid){
        var p = Process.Start(buildPath, string.Format("{0} {1}", player, guid));       

    }

}