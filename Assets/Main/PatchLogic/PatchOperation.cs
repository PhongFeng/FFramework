using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using System.Collections.Generic;
using YooAsset;
public class PatchOperation : GameAsyncOperation
{
    private IEventManager eventManager = GameFrameworkEntry.GetModule<IEventManager>();

    protected IFsm<PatchOperation> fsm;

    public string packageName;
    public string packageVersion;
    public string buildPipeline;
    public EPlayMode playMode;
    public ResourceDownloaderOperation resourceDownloaderOperation;

    public PatchOperation(string packageName, EPlayMode playMode)
    {
        // 注册监听事件
        eventManager.Subscribe(UserEventDefine.UserTryInitialize.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserBeginDownloadWebFiles.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryUpdatePackageVersion.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryUpdatePatchManifest.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryDownloadWebFiles.EventId, EventMessage);

        // 创建状态机
        List<FsmState<PatchOperation>> stateList = new List<FsmState<PatchOperation>>();
        stateList.Add(FsmInitializePackage.Create());
        stateList.Add(FsmUpdatePackageVersion.Create());
        stateList.Add(FsmUpdatePackageManifest.Create());
        stateList.Add(FsmCreatePackageDownloader.Create());
        stateList.Add(FsmDownloadPackageFiles.Create());
        stateList.Add(FsmDownloadPackageOver.Create());
        stateList.Add(FsmClearPackageCache.Create());
        stateList.Add(FsmUpdaterDone.Create());
        var m_FsmManager = GameFrameworkEntry.GetModule<IFsmManager>();
        fsm = m_FsmManager.CreateFsm("Patch", this, stateList);

        this.packageName = packageName;
        this.playMode = playMode;
    }

    protected override void OnStart()
    {
        fsm.Start<FsmInitializePackage>();
    }

    protected override void OnUpdate()
    {

    }

    public void SetStatus()
    {
        Status = EOperationStatus.Succeed;
    }

    protected override void OnAbort()
    {

    }

    private void EventMessage(object sender, GameEventArgs args)
    {
        if (args is UserEventDefine.UserTryInitialize)
        {
            fsm.Start<FsmInitializePackage>();
        }
        else if (args is UserEventDefine.UserBeginDownloadWebFiles)
        {
            fsm.Start<FsmDownloadPackageFiles>();
        }
        else if (args is UserEventDefine.UserTryUpdatePackageVersion)
        {
            fsm.Start<FsmUpdatePackageVersion>();
        }
        else if (args is UserEventDefine.UserTryUpdatePatchManifest)
        {
            fsm.Start<FsmUpdatePackageManifest>();
        }
        else if (args is UserEventDefine.UserTryDownloadWebFiles)
        {
            fsm.Start<FsmCreatePackageDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{args.GetType()}");
        }
    }
}