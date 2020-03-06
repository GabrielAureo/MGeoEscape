using UnityEditor;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Threading;


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
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "./Builds/MGeoEscape.exe", BuildTarget.StandaloneWindows64,  BuildOptions.Development);
        while(BuildPipeline.isBuildingPlayer){} 
        if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded){
            Run();
        }
        
    }
    [MenuItem("File/Run All Standalone Builds",false,1001)]
    public static void Run(){
        RunPlayerOne();
        RunPlayerTwo();
        RunPlayerThree();

        EditorApplication.EnterPlaymode();
    }

    [MenuItem("File/Run Single Standalone Build/Player 1",false,1002)]
    public static void RunPlayerOne(){
        StartWindow(1, "f342e61e-9946-41a6-b2ca-d3a6e54f7af8");
    }

    [MenuItem("File/Run Single Standalone Build/Player 2",false,1003)]
    public static void RunPlayerTwo(){
        // Process.Start(buildPath,"a54e0a4a-df2c-4ec3-9c77-f8a8b1261a73");
        StartWindow(2, "a54e0a4a-df2c-4ec3-9c77-f8a8b1261a73");
    }

    [MenuItem("File/Run Single Standalone Build/Player 3",false,1004)]
    public static void RunPlayerThree(){
        // Process.Start(buildPath,"0dc42813-73cb-4e3e-9736-0ece4049ad4c");
        StartWindow(3, "0dc42813-73cb-4e3e-9736-0ece4049ad4c");
    }

    private static void StartWindow(int player, string guid){
        var p = Process.Start(buildPath, string.Format("{0} {1}", player, guid));       

    }

    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern System.IntPtr FindWindow(System.String className, System.String windowName);

}