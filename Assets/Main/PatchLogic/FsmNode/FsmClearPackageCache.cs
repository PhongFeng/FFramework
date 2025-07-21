using GameFramework;
using GameFramework.Fsm;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;

/// <summary>
/// 清理未使用的缓存文件
/// </summary>
internal class FsmClearPackageCache : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;
    private ProcedureOwner procedure;
    private void Operation_Completed(AsyncOperationBase obj)
    {
        ChangeState<FsmUpdaterDone>(procedure);
    }

    protected override void OnInit(ProcedureOwner procedureOwner)
    {
        base.OnInit(procedureOwner);
    }

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        procedure = procedureOwner;
        owner = procedureOwner.Owner;

        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "清理未使用的缓存文件！");
        var packageName = owner.packageName;
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operation.Completed += Operation_Completed;
    }
    public static FsmClearPackageCache Create()
    {
        FsmClearPackageCache state = ReferencePool.Acquire<FsmClearPackageCache>();
        return state;
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}
