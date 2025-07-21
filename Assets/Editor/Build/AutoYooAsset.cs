using HybridCLR.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public class AutoYooAsset : Editor
{

    public static void BuildYooAsset(BuildTarget buildTarget, string buildVersion, string packageName = "DefaultPackage", EBuildinFileCopyOption copyOp = EBuildinFileCopyOption.ClearAndCopyAll)
    {
        Debug.Log($"开始构建 : {buildTarget}");

        var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        // 构建参数
        ScriptableBuildParameters scriptableBuildParameters = new ScriptableBuildParameters();
        scriptableBuildParameters.BuildOutputRoot = buildoutputRoot;
        scriptableBuildParameters.BuildinFileRoot = streamingAssetsRoot;
        scriptableBuildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
        scriptableBuildParameters.BuildTarget = buildTarget;
        //scriptableBuildParameters.BuildMode = EBuildMode.IncrementalBuild;
        scriptableBuildParameters.PackageName = packageName;
        scriptableBuildParameters.PackageVersion = buildVersion;
        scriptableBuildParameters.VerifyBuildingResult = true;
        scriptableBuildParameters.EnableSharePackRule = true; //启用共享资源构建模式，兼容1.5x版本
        scriptableBuildParameters.BuildinFileCopyOption = copyOp;
        scriptableBuildParameters.BuildinFileCopyParams = string.Empty;
        scriptableBuildParameters.CompressOption = ECompressOption.LZ4;

        // Bundle格式
        scriptableBuildParameters.FileNameStyle = EFileNameStyle.BundleName;
        scriptableBuildParameters.EncryptionServices = null;



        // 执行构建
        ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
        var buildResult = pipeline.Run(scriptableBuildParameters, true);
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
    }
}
