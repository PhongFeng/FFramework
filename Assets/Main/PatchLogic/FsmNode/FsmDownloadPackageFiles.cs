using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;
using YooAsset;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework;

/// <summary>
/// 下载更新文件
/// </summary>
public class FsmDownloadPackageFiles : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;

    protected async override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        owner = procedureOwner.Owner;

        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "开始下载补丁文件！");
        await BeginDownload(procedureOwner);
    }

    private async UniTask BeginDownload(ProcedureOwner procedureOwner)
    {
        var downloader = owner.resourceDownloaderOperation;
        downloader.DownloadErrorCallback = WebFileDownloadFailed;
        downloader.DownloadUpdateCallback = DownloadProgressUpdate;
        downloader.DownloadFileBeginCallback = DownloadFileBeginCallback;
        downloader.BeginDownload();

        await UniTask.WaitUntil(() => downloader.IsDone);

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            return;

        ChangeState<FsmDownloadPackageOver>(procedureOwner);
    }

    public void DownloadProgressUpdate(DownloadUpdateData data)
    {
        PatchEventDefine.DownloadProgressUpdate.SendEventMessage(this, data.TotalDownloadCount, data.CurrentDownloadCount, data.TotalDownloadBytes, data.CurrentDownloadBytes);
    }

    public void WebFileDownloadFailed(DownloadErrorData data)
    {
        PatchEventDefine.WebFileDownloadFailed.SendEventMessage(this, data.FileName, data.ErrorInfo);
    }

    public void DownloadFileBeginCallback(DownloadFileData data)
    {
        UnityEngine.Debug.Log($"download file begin name: {data.FileName}, size: {data.FileSize}");
    }

    public static FsmDownloadPackageFiles Create()
    {
        FsmDownloadPackageFiles state = ReferencePool.Acquire<FsmDownloadPackageFiles>();
        return state;
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}