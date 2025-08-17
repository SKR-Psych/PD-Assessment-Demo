using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
#endif

namespace SortingBoardGame
{
    public class BuildConfiguration : MonoBehaviour
    {
        [Header("Build Settings")]
        public string buildName = "SortingBoardGame";
        public string version = "1.0.0";
        public bool developmentBuild = false;
        
        #if UNITY_EDITOR
        [ContextMenu("Build Windows Executable")]
        public void BuildWindowsExecutable()
        {
            BuildGame();
        }
        
        public static void BuildGame()
        {
            // Build settings
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Game/Scenes/MainGame.unity" };
            buildPlayerOptions.locationPathName = "Builds/SortingBoardGame.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.None;
            
            // Set player settings
            PlayerSettings.companyName = "SortingBoardGame";
            PlayerSettings.productName = "Sorting Board Game";
            PlayerSettings.bundleVersion = "1.0.0";
            
            // Set resolution and graphics settings
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.runInBackground = false;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.defaultIsFullScreen = false;
            PlayerSettings.resizableWindow = true;
            
            // Graphics settings
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.gpuSkinning = true;
            
            // Build the game
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.outputPath} ({summary.totalSize} bytes)");
                
                // Open build folder
                EditorUtility.RevealInFinder(summary.outputPath);
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed");
            }
        }
        
        [MenuItem("Sorting Board Game/Build Windows Executable")]
        public static void BuildFromMenu()
        {
            BuildGame();
        }
        
        [MenuItem("Sorting Board Game/Open Build Folder")]
        public static void OpenBuildFolder()
        {
            string buildPath = System.IO.Path.Combine(Application.dataPath, "../Builds");
            if (System.IO.Directory.Exists(buildPath))
            {
                EditorUtility.RevealInFinder(buildPath);
            }
            else
            {
                Debug.LogWarning("Build folder does not exist. Build the game first.");
            }
        }
        
        [MenuItem("Sorting Board Game/Setup Build Settings")]
        public static void SetupBuildSettings()
        {
            // Add scenes to build settings
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Game/Scenes/MainGame.unity", true)
            };
            
            Debug.Log("Build settings configured");
        }
        #endif
    }
}