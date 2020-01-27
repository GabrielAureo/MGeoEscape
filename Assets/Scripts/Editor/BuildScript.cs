using UnityEditor;
using System.Diagnostics;

public class BuildScript{
    [MenuItem("File/Build Standalone")]
    public static void PerformBuild(){
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "./Builds/MGeoEscape.exe", BuildTarget.StandaloneWindows64,  BuildOptions.Development);
        while(BuildPipeline.isBuildingPlayer){} 
        Run();
    }
    [MenuItem("File/Run Build")]
    public static void Run(){
        var rootPath = System.IO.Directory.GetCurrentDirectory();
        var path = System.IO.Path.Combine(rootPath, "Builds\\MGeoEscape.exe");

        UnityEngine.Debug.Log(path);      
        ProcessStartInfo psi = new ProcessStartInfo(path);
        Process p1 = Process.Start(psi);
        Process p2 = Process.Start(psi);
    }
}