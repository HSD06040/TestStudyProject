using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

public class EditorScript
{
    static string[] SCENES = FindEnabledEditorScenes();

    [MenuItem("Build/Build Windows Standalone")]
    static void PerformBuild()
    {
        AddressableAssetSettings.CleanPlayerContent();        
        AddressableAssetSettings.BuildPlayerContent();

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), "Builds/Windows/MyGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }
}
