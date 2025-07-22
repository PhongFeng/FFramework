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
    public static void BuildYooAsset(BuildTarget buildTarget, string buildVersion, EBuildinFileCopyOption copyOp = EBuildinFileCopyOption.ClearAndCopyAll)
    {
        Debug.Log($"开始构建 : {buildTarget}");

        var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        // 构建参数
        BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
        buildParameters.BuildOutputRoot = buildoutputRoot;
        buildParameters.BuildinFileRoot = streamingAssetsRoot;
        buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString();
        buildParameters.BuildBundleType = (int)EBuildBundleType.AssetBundle; //必须指定资源包类型
        buildParameters.BuildTarget = buildTarget;
        buildParameters.PackageName = "DefaultPackage";
        buildParameters.PackageVersion = buildVersion;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.EnableSharePackRule = true; //启用共享资源构建模式，兼容1.5x版本
        buildParameters.FileNameStyle = EFileNameStyle.HashName;
        buildParameters.BuildinFileCopyOption = copyOp;
        buildParameters.BuildinFileCopyParams = string.Empty;
        buildParameters.EncryptionServices = null;
        buildParameters.CompressOption = ECompressOption.LZ4;
        buildParameters.ClearBuildCacheFiles = false; //不清理构建缓存，启用增量构建，可以提高打包速度！
        buildParameters.UseAssetDependencyDB = true; //使用资源依赖关系数据库，可以提高打包速度！

        // 执行构建
        BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
        var buildResult = pipeline.Run(buildParameters, true);
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
    }

    // 从构建命令里获取参数示例
    private static string GetBuildPackageName()
    {
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith("buildPackage"))
                return arg.Split("="[0])[1];
        }
        return string.Empty;
    }
}
