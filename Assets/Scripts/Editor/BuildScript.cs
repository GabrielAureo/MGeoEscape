using UnityEditor;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;


public class BuildScript{
    [MenuItem("File/Build Standalone")]
    public static void PerformWindowsBuild(){
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "./Builds/MGeoEscape.exe", BuildTarget.StandaloneWindows64,  BuildOptions.Development);
        while(BuildPipeline.isBuildingPlayer){} 
        if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded){
            Run();
        }
        
    }
    [MenuItem("File/Run Standalone Build")]
    public static void Run(){
        var rootPath = System.IO.Directory.GetCurrentDirectory();
        var path = System.IO.Path.Combine(rootPath, "Builds\\MGeoEscape.exe");
     
        ProcessStartInfo psi = new ProcessStartInfo(path);
        
        Process p1 = Process.Start(psi);
        Process p2 = Process.Start(psi);
        Process p3 = Process.Start(psi);

        EditorApplication.EnterPlaymode();
        
    }

    [MenuItem("File/Build Android")]
    public static void PerformAndroidBuild(){
        UnityEngine.Debug.Log(EditorApplication.applicationContentsPath);
        var adbPath = System.IO.Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools\\adb.exe")
    }

}