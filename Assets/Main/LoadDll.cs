using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class LoadDll : MonoBehaviour
{
    void Start()
    {
        LoadMetadataForAOTAssemblies();
#if !UNITY_EDITOR
        LoadHotFix();
#endif
        HotUpdatePrefab();
    }

    async void LoadHotFix()
    {
        var op = YooAssets.LoadAssetAsync<TextAsset>("HotFix.dll");
        await op.ToUniTask();
        var handleHotFix = op.AssetObject as TextAsset;
        System.Reflection.Assembly.Load(handleHotFix.bytes);
    }

    async void HotUpdatePrefab()
    {
        await ShaderVariantsWarmUp();
        var sceneAssetName = "Assets/GameRes/Scenes/GameStart/GameStart.unity";
        SceneHandle asyncOperation = YooAssets.LoadSceneAsync(sceneAssetName, LoadSceneMode.Single);
    }

    /// <summary>
    /// 变体预热
    /// </summary>
    /// <returns></returns>
    private async UniTask ShaderVariantsWarmUp()
    {
        var op = YooAssets.LoadAssetAsync<ShaderVariantCollection>("GameShaderVariants");
        await op.ToUniTask();
        var shaderVariants = op.AssetObject as ShaderVariantCollection;
        if (!shaderVariants.isWarmedUp)
        {
            shaderVariants.WarmUp();
        }
        op.Release();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private async void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            var op = YooAssets.LoadAssetAsync<TextAsset>(aotDllName);
            await op.ToUniTask();
            var dllBytes = op.AssetObject as TextAsset;
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes.bytes, mode);
            if (err == LoadImageErrorCode.OK)
            {
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
            else
            {
                Debug.LogError($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
        }
    }
}
