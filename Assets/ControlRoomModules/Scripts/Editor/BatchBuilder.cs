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
        //static string TARGET_DIR = "target";
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

        static void Build_AOS()
        {
            Debug.Log($"AUTOBUILDER: Build {APP_NAME} AOS");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                throw new Exception("Invalid Platform");
            }

            string buildPath = CommandLineReader.GetCustomArgument("BuildPath");
            string version = CommandLineReader.GetCustomArgument("Version");
            string bundleVersionCode = CommandLineReader.GetCustomArgument("VersionCode");
            string useAppBundle = CommandLineReader.GetCustomArgument("UseAppBundle");
            string isDebugMode = CommandLineReader.GetCustomArgument("IsDebugMode");
            string isTableOnlineMode = CommandLineReader.GetCustomArgument("isTableOnlineMode");


            // Config.ReadXml();
            // Config.Version = version;
            // Config.VersionCode = bundleVersionCode;
            // Config.MarketURL = "https://naver.com";
            // if (isTableOnlineMode == "True")
            //     Config.IsOnlineTableMode = true;
            // else
            //     Config.IsOnlineTableMode = false;
            // Config.WriteXml();

            PlayerSettings.bundleVersion = version;
            PlayerSettings.productName = APP_NAME;
            PlayerSettings.applicationIdentifier = BUNDLE_ID;
            PlayerSettings.Android.bundleVersionCode = System.Convert.ToInt32(bundleVersionCode);
            PlayerSettings.Android.keystoreName = UnityEngine.Application.dataPath + "/cot.keystore";
            PlayerSettings.Android.keyaliasName = "catWater";
            PlayerSettings.Android.keystorePass = "carpinus0819";
            PlayerSettings.Android.keyaliasPass = "carpinus0819";

            if (isDebugMode == "True")
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;
            }
            else
            {
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.allowDebugging = false;
            }

            if (!System.IO.Directory.Exists(buildPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(buildPath);
                    UnityEngine.Debug.Log("Create Build Path: " + buildPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }
            }

            buildPath = buildPath + APP_NAME + "_" + version;

            if (useAppBundle == "True")
            {
                buildPath += ".aab";
                EditorUserBuildSettings.buildAppBundle = true;
            }
            else
            {
                buildPath += ".apk";
                EditorUserBuildSettings.buildAppBundle = false;
            }


            AssetDatabase.Refresh();

            GenericBuild(SCENES, buildPath, BuildTarget.Android, BuildOptions.None);
        }

        static void Build_IOS()
        {
            Debug.Log($"AUTOBUILDER: Build {APP_NAME} iOS");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                throw new Exception("Invalid Platform");
            }

            string buildPath = CommandLineReader.GetCustomArgument("BuildPath");
            string version = CommandLineReader.GetCustomArgument("Version");
            string bundleVersionCode = CommandLineReader.GetCustomArgument("VersionCode");
            string isDebugMode = CommandLineReader.GetCustomArgument("IsDebugMode");
            string isTableOnlineMode = CommandLineReader.GetCustomArgument("isTableOnlineMode");


            // Config.ReadXml();
            // Config.Version = version;
            // Config.VersionCode = bundleVersionCode;
            // Config.MarketURL = "https://naver.com";
            // if (isTableOnlineMode == "True")
            //     Config.IsOnlineTableMode = true;
            // else
            //     Config.IsOnlineTableMode = false;

            // Config.WriteXml();

            PlayerSettings.bundleVersion = version;
            PlayerSettings.productName = APP_NAME;
            PlayerSettings.applicationIdentifier = BUNDLE_ID;
            PlayerSettings.iOS.buildNumber = bundleVersionCode;
            PlayerSettings.iOS.targetOSVersionString = "10.0";


            if (isDebugMode == "True")
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;
            }
            else
            {
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.allowDebugging = false;
            }

            if (!System.IO.Directory.Exists(buildPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(buildPath);
                    UnityEngine.Debug.Log("Create Build Path: " + buildPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }
            }

            buildPath = buildPath + APP_NAME + "_" + version;

            AssetDatabase.Refresh();

            GenericBuild(SCENES, buildPath, BuildTarget.iOS, BuildOptions.None);
        }


        static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
        {
            UnityEngine.Debug.Log("Generic Build");
            var res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

            //if (res.summary.totalErrors > 0)
            //throw new Exception(res.summary.result.ToString());

        }

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
                Dictionary<string, string> dicCustomArgs = GetCustomArguments();
                if (dicCustomArgs.ContainsKey(argumentName))
                    return dicCustomArgs[argumentName];
                else
                    return string.Empty;

            }
        }
    }
}

