using UnityEditor;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;


public class BuildScript{
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    #endif

    [MenuItem("File/Build Standalone")]
    public static void PerformBuild(){
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "./Builds/MGeoEscape.exe", BuildTarget.StandaloneWindows64,  BuildOptions.Development);
        while(BuildPipeline.isBuildingPlayer){} 
        if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded){
            Run();
        }
        
    }
    [MenuItem("File/Run Build")]
    public static void Run(){
        var rootPath = System.IO.Directory.GetCurrentDirectory();
        var path = System.IO.Path.Combine(rootPath, "Builds\\MGeoEscape.exe");
     
        ProcessStartInfo psi = new ProcessStartInfo(path);
        
        Process p1 = Process.Start(psi);
        Process p2 = Process.Start(psi);
        Process p3 = Process.Start(psi);

        EditorApplication.EnterPlaymode();
        
    }


}