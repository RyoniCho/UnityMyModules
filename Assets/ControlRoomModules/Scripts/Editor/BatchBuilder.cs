using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine.Networking;
namespace ControlRoom
{
    static class BatchBuilder
    {

        static string[] SCENES = 
        {
            "Assets/Scenes/SampleScene.unity",
        };


        static string APP_NAME = "==APP NAME HERE==";
        static string BUNDLE_ID = "==APP BUNDLE ID HERE==";
        

        static string BuildinfoPath => Application.dataPath + "/Resources/buildinfo.txt";

        static void UpdateBuildInfo()
        {
            string info = DateTime.Now.ToString();
            string path = BuildinfoPath;
            System.IO.File.WriteAllText(path, info);

            AssetDatabase.Refresh();
        }

        static void ToggleDefine(BuildTargetGroup target, string defineName, bool on)
        {

            string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            bool defined = define.Contains(defineName);
            if (defined == false && on)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, define + ";" + defineName);
            else if (defined == true && on == false)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, define.Replace(defineName, ""));
        }

/*

<Batch Command>
{UnityPath} -quit -batchmode -logFile {BuildLogFilePath} -projectPath {UnityProjectPath} -buildTarget {BuildPlatform} -executeMethod ControlRoom.BatchBuilder.Build_AOS
-CustomArg:BuildPath={BuildPath}?Version={Version}?VersionCode={VersionCode}?UseAppBundle={UseAppBundle}

<Example>
/Applications/Unity/Hub/Editor/2019.4.16f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -logFile /Users/cho-eul-yeon/PrivateProject/Test/1_build.log 
-projectPath /Users/cho-eul-yeon/PrivateProject/Test -buildTarget AOS -executeMethod ControlRoom.BatchBuilder.Build_AOS
-CustomArg:BuildPath=/Users/cho-eul-yeon/PrivateProject/Test/Build/?Version=1.0.0?VersionCode=1?UseAppBundle=True
*/

        public static void Build_AOS()
        {
            Debug.Log($"AUTOBUILDER: Build {APP_NAME} AOS");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                throw new Exception("Invalid Platform");
            }

            
            string version = CommandLineReader.GetCustomArgument("Version");
            string bundleVersionCode = CommandLineReader.GetCustomArgument("VersionCode");
            string buildPath = CommandLineReader.GetCustomArgument("BuildPath");
            string useAppBundle = CommandLineReader.GetCustomArgument("UseAppBundle");
            string isDebugMode = CommandLineReader.GetCustomArgument("IsDebugMode");

            string androidKeyStoreName = CommandLineReader.GetCustomArgument("androidKeyStoreName");
            string androidKeyAliasName = CommandLineReader.GetCustomArgument("androidKeyAliasName");
            string androidKeyStorePassword = CommandLineReader.GetCustomArgument("androidKeyStorePassword");
            string androidKeyAliasPassword = CommandLineReader.GetCustomArgument("androidKeyAliasPassword");
            string MarketURL_AOS = CommandLineReader.GetCustomArgument("MarketURL_AOS");


          

            Config.ReadXml();

            if (!string.IsNullOrEmpty(version))
                Config.Version = version;
            if (!string.IsNullOrEmpty(bundleVersionCode))
                Config.VersionCode = bundleVersionCode;
            if (!string.IsNullOrEmpty(buildPath))
                Config.BuildPath = buildPath;
            if(!string.IsNullOrEmpty(useAppBundle))
            {
                if (useAppBundle == "True")
                    Config.useAppBundle = true;
                else
                    Config.useAppBundle = false;
            }
            if (!string.IsNullOrEmpty(isDebugMode))
            {
                if (isDebugMode == "True")
                    Config.isDebugMode = true;
                else
                    Config.isDebugMode = false;
            }
            if (!string.IsNullOrEmpty(androidKeyStoreName))
                Config.androidKeyStoreName = androidKeyStoreName;
            if (!string.IsNullOrEmpty(androidKeyAliasName))
                Config.androidKeyAliasName = androidKeyAliasName;
            if (!string.IsNullOrEmpty(androidKeyStorePassword))
                Config.androidKeyStorePassword = androidKeyStorePassword;
            if (!string.IsNullOrEmpty(androidKeyAliasPassword))
                Config.androidKeyAliasPassword = androidKeyAliasPassword;
            if (!string.IsNullOrEmpty(MarketURL_AOS))
                Config.MarketURL_AOS = MarketURL_AOS;
            

            PlayerSettings.bundleVersion = Config.Version;
            PlayerSettings.productName = APP_NAME;
            PlayerSettings.applicationIdentifier = BUNDLE_ID;
            PlayerSettings.Android.bundleVersionCode = System.Convert.ToInt32(Config.VersionCode);
            PlayerSettings.Android.keystoreName = Config.androidKeyStoreName;
            PlayerSettings.Android.keyaliasName = Config.androidKeyAliasName;
            PlayerSettings.Android.keystorePass = Config.androidKeyStorePassword;
            PlayerSettings.Android.keyaliasPass = Config.androidKeyAliasPassword;

            if (Config.isDebugMode == true)
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;
            }
            else
            {
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.allowDebugging = false;
            }

            if (!System.IO.Directory.Exists(Config.BuildPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Config.BuildPath);
                    UnityEngine.Debug.Log("Create Build Path: " + Config.BuildPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }
            }

            Config.BuildPath = buildPath + APP_NAME + "_" + version;

            if (Config.useAppBundle ==true)
            {
                Config.BuildPath += ".aab";
                EditorUserBuildSettings.buildAppBundle = true;
            }
            else
            {
                Config.BuildPath += ".apk";
                EditorUserBuildSettings.buildAppBundle = false;
            }


            AssetDatabase.Refresh();

            GenericBuild(SCENES, Config.BuildPath, BuildTarget.Android, BuildOptions.None);
        }

        public static void Build_IOS()
        {
            Debug.Log($"AUTOBUILDER: Build {APP_NAME} iOS");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                throw new Exception("Invalid Platform");
            }

            string version = CommandLineReader.GetCustomArgument("Version");
            string bundleVersionCode = CommandLineReader.GetCustomArgument("VersionCode");
            string buildPath = CommandLineReader.GetCustomArgument("BuildPath");
            string isDebugMode = CommandLineReader.GetCustomArgument("IsDebugMode");
            string MarketURL_iOS = CommandLineReader.GetCustomArgument("MarketURL_iOS");
            string isIOSSimulatorBuild= CommandLineReader.GetCustomArgument("isIOSSimulatorBuild");
            string iOSMinimalOSVersion = CommandLineReader.GetCustomArgument("iOSMinimalOSVersion");

            Config.ReadXml();

            if (!string.IsNullOrEmpty(version))
                Config.Version = version;
            if (!string.IsNullOrEmpty(bundleVersionCode))
                Config.VersionCode = bundleVersionCode;
            if (!string.IsNullOrEmpty(buildPath))
                Config.BuildPath = buildPath;
            
            if (!string.IsNullOrEmpty(isDebugMode))
            {
                if (isDebugMode == "True")
                    Config.isDebugMode = true;
                else
                    Config.isDebugMode = false;
            }
            if (!string.IsNullOrEmpty(isIOSSimulatorBuild))
            {
                if (isDebugMode == "True")
                    Config.isIOSSimulatorBuild = true;
                else
                    Config.isIOSSimulatorBuild = false;
            }

            if (!string.IsNullOrEmpty(MarketURL_iOS))
                Config.MarketURL_AOS = MarketURL_iOS;
            if (!string.IsNullOrEmpty(iOSMinimalOSVersion))
                Config.iOSMinimalOSVersionString = iOSMinimalOSVersion;



            PlayerSettings.bundleVersion = Config.Version;
            PlayerSettings.productName = APP_NAME;
            PlayerSettings.applicationIdentifier = BUNDLE_ID;
            PlayerSettings.iOS.buildNumber = Config.VersionCode;
            PlayerSettings.iOS.targetOSVersionString = Config.iOSMinimalOSVersionString;

            if (Config.isIOSSimulatorBuild==true)
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            }
            else
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            }


            if (Config.isDebugMode == true)
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;
            }
            else
            {
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.allowDebugging = false;
            }

            if (!System.IO.Directory.Exists(Config.BuildPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Config.BuildPath);
                    UnityEngine.Debug.Log("Create Build Path: " + Config.BuildPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }
            }

            Config.BuildPath = System.IO.Path.Join(buildPath,APP_NAME + "_" + version);
#if TABLE_DATA_BUILDER
            //BuildAndMoveTableData();
#endif
            AssetDatabase.Refresh();

            GenericBuild(SCENES, Config.BuildPath, BuildTarget.iOS, BuildOptions.None);
        }


        static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
        {
            UnityEngine.Debug.Log("Generic Build");
            var res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

            //if (res.summary.totalErrors > 0)
            //throw new Exception(res.summary.result.ToString());

        }
#if TABLE_DATA_BUILDER
        public static async void BuildAndMoveTableData()
        {
            UnityEngine.Debug.Log("Start Table Build -Binary ");

            await TableDataBuilder.BuildTableDataFromBinary();

            UnityEngine.Debug.Log("Table Build End");

            var sourPath = $"{UnityEngine.Application.dataPath }/Resources/Table/";
            var destPath = $"{UnityEngine.Application.streamingAssetsPath}/Table/";
            if (!System.IO.Directory.Exists(destPath))
            {
                System.IO.Directory.CreateDirectory(destPath);
            }


            foreach (var files in System.IO.Directory.GetFiles(sourPath))
            {
                var fileName = System.IO.Path.GetFileName(files);
                var fileExtension = System.IO.Path.GetExtension(files);
                var destFilePath = System.IO.Path.Combine(destPath, fileName);

                if (fileExtension != ".meta")
                    System.IO.File.Copy(files, destFilePath, true);

            }

        }
#endif


        public class CommandLineReader
        {
            private const string CUSTOM_ARGS_PREFIX = "-CustomArg:";
            private const char SEPARATOR = '?';

            public static string[] GetCommandLineArgs()
            {
                return Environment.GetCommandLineArgs();
            }

            public static Dictionary<string, string> GetCustomArguments()
            {
                Dictionary<string, string> dicCustomArgs = new Dictionary<string, string>();
                string[] commandLingArgs = GetCommandLineArgs();
                string[] customArgs;
                string[] customArgBuffer;
                string customArgsStr = "";

                try
                {
                    customArgsStr = commandLingArgs.Where(row => row.Contains(CUSTOM_ARGS_PREFIX)).Single();
                }
                catch (Exception e)
                {
                    Debug.LogError("GetCustomArgument Error: [" + commandLingArgs + "]. Exception: " + e);
                    EditorApplication.Exit(1);
                }

                customArgsStr = customArgsStr.Replace(CUSTOM_ARGS_PREFIX, "");
                customArgs = customArgsStr.Split(SEPARATOR);

                foreach (string customArg in customArgs)
                {
                    customArgBuffer = customArg.Split('=');
                    if (customArgBuffer.Length == 2)
                    {
                        dicCustomArgs.Add(customArgBuffer[0], customArgBuffer[1]);
                    }
                    else
                    {
                        Debug.LogWarning("GetCustomArgument Warrning: " + customArg);
                    }
                }

                return dicCustomArgs;
            }

            public static string GetCustomArgument(string argumentName)
            {
                if (!Application.isBatchMode)
                    return string.Empty;

                Dictionary<string, string> dicCustomArgs = GetCustomArguments();
                if (dicCustomArgs.ContainsKey(argumentName))
                    return dicCustomArgs[argumentName];
                else
                    return string.Empty;

            }
        }
    }
}

