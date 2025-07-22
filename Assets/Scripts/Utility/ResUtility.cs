using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
public static class ResUtility
{
    public static void GetAsset(string assetName)
    {
        YooAssets.LoadAssetAsync(assetName);
    } 
    public static void GetPackageVersion(string packageName)
    {
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
    }
}
