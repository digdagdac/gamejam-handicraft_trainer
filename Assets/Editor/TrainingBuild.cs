using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class TrainingBuild
{
    private const string ScenePath = "Assets/Scenes/2.Game1.unity";
    private const string OutputPath = "build/handicraft.exe";

    public static void BuildWindows64()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Training player build failed: {report.summary.result}");
        }
    }
}
