#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AvatarTools.Editor
{
    /// <summary>
    /// Utility for exporting the Avatar Mesh Settings Validator as a UnityPackage
    /// </summary>
    public static class PackageExporter
    {
        private const string PACKAGE_NAME = "AvatarMeshSettingsValidator";
        private const string EXPORT_PATH = "Assets/AvatarMeshSettingsValidator";
        
        [MenuItem("Tools/Avatar Tools/Export Package", false, 100)]
        public static void ExportPackage()
        {
            var version = GetPackageVersion();
            var fileName = $"{PACKAGE_NAME}-v{version}.unitypackage";
            var exportPath = EditorUtility.SaveFilePanel(
                "Export Avatar Mesh Settings Validator Package",
                "",
                fileName,
                "unitypackage"
            );
            
            if (string.IsNullOrEmpty(exportPath))
                return;
            
            ExportPackageToPath(exportPath);
        }
        
        [MenuItem("Tools/Avatar Tools/Export Package (Quick)", false, 101)]
        public static void QuickExportPackage()
        {
            var version = GetPackageVersion();
            var fileName = $"{PACKAGE_NAME}-v{version}.unitypackage";
            var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            var exportPath = Path.Combine(desktopPath, fileName);
            
            ExportPackageToPath(exportPath);
        }
        
        private static void ExportPackageToPath(string path)
        {
            // Find all assets to export
            var assetPaths = new string[] { EXPORT_PATH };
            
            // Check if the export path exists
            if (!AssetDatabase.IsValidFolder(EXPORT_PATH))
            {
                Debug.LogError($"Export path not found: {EXPORT_PATH}");
                return;
            }
            
            try
            {
                AssetDatabase.ExportPackage(
                    assetPaths, 
                    path,
                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
                );
                
                Debug.Log($"Package exported successfully to: {path}");
                EditorUtility.DisplayDialog(
                    "Export Complete",
                    $"Package exported to:\n{path}",
                    "OK"
                );
                
                // Open the folder containing the exported package
                EditorUtility.RevealInFinder(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to export package: {e.Message}");
                EditorUtility.DisplayDialog(
                    "Export Failed",
                    $"Failed to export package:\n{e.Message}",
                    "OK"
                );
            }
        }
        
        private static string GetPackageVersion()
        {
            // Try to get version from package.json
            var packageJsonPath = Path.Combine(Application.dataPath, "AvatarMeshSettingsValidator/package.json");
            if (File.Exists(packageJsonPath))
            {
                try
                {
                    var json = File.ReadAllText(packageJsonPath);
                    var versionMatch = System.Text.RegularExpressions.Regex.Match(json, @"""version""\s*:\s*""([^""]+)""");
                    if (versionMatch.Success)
                    {
                        return versionMatch.Groups[1].Value;
                    }
                }
                catch { }
            }
            
            return "0.1.0"; // Default version
        }
        
        /// <summary>
        /// Export package from command line
        /// Usage: Unity -batchmode -projectPath [project] -executeMethod AvatarTools.Editor.PackageExporter.ExportFromCommandLine -outputPath [path]
        /// </summary>
        public static void ExportFromCommandLine()
        {
            var args = System.Environment.GetCommandLineArgs();
            var outputPath = "";
            
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-outputPath" && i + 1 < args.Length)
                {
                    outputPath = args[i + 1];
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(outputPath))
            {
                var version = GetPackageVersion();
                outputPath = $"{PACKAGE_NAME}-v{version}.unitypackage";
            }
            
            Debug.Log($"Exporting package to: {outputPath}");
            
            var assetPaths = new string[] { EXPORT_PATH };
            AssetDatabase.ExportPackage(
                assetPaths,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
            
            Debug.Log("Export complete!");
            EditorApplication.Exit(0);
        }
    }
}
#endif