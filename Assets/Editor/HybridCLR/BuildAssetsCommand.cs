using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HybridCLR.Editor
{
    public static class BuildAssetsCommand
    {
        [MenuItem("HybridCLR/Build/BuildAssetsAndCopyToRes")]
        public static void BuildAndCopyABAOTHotUpdateDlls()
        {
            // 设置AOT
            HybridCLRSettings.Instance.patchAOTAssemblies = AOTGenericReferences.PatchedAOTAssemblyList.ToArray();
            HybridCLRSettings.Save();

            // Copy Dll bytes
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            CopyABAOTHotUpdateDlls(target);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void CopyABAOTHotUpdateDlls(BuildTarget target)
        {
            CopyAOTAssembliesToStreamingAssets();
            CopyHotUpdateAssembliesToStreamingAssets();
        }

        public static void CopyAOTAssembliesToStreamingAssets()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = Application.dataPath + "/GameRes/Dlls";

            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToStreamingAssets()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = Application.dataPath + "/GameRes/Dlls";

            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }
    }
}