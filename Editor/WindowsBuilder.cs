using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectBuilder
{
    public static class WindowsBuilder
    {
        [MenuItem("Build/All")]
        private static void BuildAll()
        {
            BuildPlayer(eBackendType.MONO, eBuildType.HEADLESS, eShippingType.RELEASE);
            BuildPlayer(eBackendType.MONO, eBuildType.HEADLESS, eShippingType.DEVELOPMENT);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.HEADLESS, eShippingType.RELEASE);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.HEADLESS, eShippingType.DEVELOPMENT);
            
            BuildPlayer(eBackendType.MONO, eBuildType.CLIENT, eShippingType.RELEASE);
            BuildPlayer(eBackendType.MONO, eBuildType.CLIENT, eShippingType.DEVELOPMENT);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.CLIENT, eShippingType.RELEASE);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.CLIENT, eShippingType.DEVELOPMENT);
        }
        
        [MenuItem("Build/Headless")]
        private static void BuildHeadless()
        {
            BuildPlayer(eBackendType.MONO, eBuildType.HEADLESS, eShippingType.RELEASE);
            BuildPlayer(eBackendType.MONO, eBuildType.HEADLESS, eShippingType.DEVELOPMENT);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.HEADLESS, eShippingType.RELEASE);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.HEADLESS, eShippingType.DEVELOPMENT);
        }
        
        [MenuItem("Build/Client")]
        private static void BuildClient()
        {
            BuildPlayer(eBackendType.MONO, eBuildType.CLIENT, eShippingType.RELEASE);
            BuildPlayer(eBackendType.MONO, eBuildType.CLIENT, eShippingType.DEVELOPMENT);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.CLIENT, eShippingType.RELEASE);
            BuildPlayer(eBackendType.IL2CPP, eBuildType.CLIENT, eShippingType.DEVELOPMENT);
        }

        public static void BuildPlayer(eBackendType backend, eBuildType buildType, eShippingType shippingType)
        {
            bool isRelease = shippingType == eShippingType.RELEASE;
            bool isHeadless = buildType == eBuildType.HEADLESS;
            bool isIL2CPP = backend != eBackendType.MONO;
            bool isExportSolution = backend == eBackendType.IL2CPP_SOLUTION;
            
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, 
                isIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
            
            BuildOptions option = (isRelease ? BuildOptions.None : BuildOptions.Development);
            if (isHeadless)
            {
                option |= BuildOptions.EnableHeadlessMode;
            }

            if (isExportSolution)
            {
                
            }
            
            
            string[] scenePaths = UnityEditor.EditorBuildSettings.scenes
                .Select(x => x.path).ToArray();

            string subPath = (isIL2CPP ? "IL2CPP" : "MONO") + "_" +
                             (isRelease ? "RELEASE" : "DEVELOPMENT") + "_" +
                             (isHeadless ? "HEADLESS" : "CLIENT");
            
            string targetPath = Application.dataPath.Replace("/Assets", $"/Build/Windows64/{subPath}/{PlayerSettings.productName}.exe");

            string dir = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(dir);
            
            Debug.Log(dir);

            BuildPipeline.BuildPlayer(scenePaths, targetPath, EditorUserBuildSettings.activeBuildTarget, option);
        }
    }
}
