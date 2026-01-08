using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Fsm;
using System;
using UnityEngine;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;

/// <summary>
/// 更新资源版本号
/// </summary>
internal class FsmUpdatePackageVersion : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;

    protected async override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        owner = procedureOwner.Owner;

        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "获取最新的资源版本 !");
        await UpdatePackageVersion(procedureOwner);
    }

    private async UniTask UpdatePackageVersion(ProcedureOwner procedureOwner)
    {
        var packageName = owner.packageName;
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();

        await UniTask.WaitUntil(() => operation.IsDone);

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage(this);
        }
        else
        {
            Debug.Log($"Request package version : {operation.PackageVersion}");
            owner.packageVersion = operation.PackageVersion;
            ChangeState<FsmUpdatePackageManifest>(procedureOwner);
        }
    }
    public static FsmUpdatePackageVersion Create()
    {
        FsmUpdatePackageVersion state = ReferencePool.Acquire<FsmUpdatePackageVersion>();
        return state;
    }
    public void Clear()
    {
        throw new NotImplementedException();
    }
}