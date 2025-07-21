using System.Collections;
using UnityEngine;
using YooAsset;

public class Main : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    void Awake()
    {
#if !UNITY_EDITOR
        PlayMode = EPlayMode.OfflinePlayMode;
#endif
        Debug.Log($"资源系统运行模式：{PlayMode}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }
    IEnumerator Start()
    {
        // 初始化资源系统
        YooAssets.Initialize();

        // 加载更新页面
        var go = Resources.Load<GameObject>("PatchWindow");
        GameObject.Instantiate(go);

        // 开始补丁更新流程
        var operation = new PatchOperation("DefaultPackage", PlayMode);
        YooAssets.StartOperation(operation);
        yield return operation;

        // 设置默认的资源包
        var gamePackage = YooAssets.GetPackage("DefaultPackage");
        YooAssets.SetDefaultPackage(gamePackage);

        // 切换到主页面场景
        gameObject.AddComponent<LoadDll>();
    }

}