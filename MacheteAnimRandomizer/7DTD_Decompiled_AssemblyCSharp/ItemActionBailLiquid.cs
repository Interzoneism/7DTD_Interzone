using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000507 RID: 1287
[Preserve]
public class ItemActionBailLiquid : ItemAction
{
	// Token: 0x06002A11 RID: 10769 RVA: 0x001124C9 File Offset: 0x001106C9
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionBailLiquid.MyInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x001124D4 File Offset: 0x001106D4
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (_actionData.lastUseTime > 0f)
		{
			return;
		}
		ItemInventoryData invData = _actionData.invData;
		if (!Voxel.Raycast(invData.world, invData.hitInfo.ray, Constants.cDigAndBuildDistance, 16, 4095, 0f))
		{
			return;
		}
		if (Voxel.voxelRayHitInfo.bHitValid && Voxel.voxelRayHitInfo.hit.voxelData.WaterValue.HasMass())
		{
			_actionData.lastUseTime = Time.time;
			((ItemActionBailLiquid.MyInventoryData)_actionData).targetPosition = Voxel.voxelRayHitInfo.hit.blockPos;
			invData.holdingEntity.RightArmAnimationUse = true;
			if (this.soundStart != null)
			{
				invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
			}
		}
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x00112599 File Offset: 0x00110799
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return _actionData.lastUseTime != 0f && Time.time - _actionData.lastUseTime < this.Delay;
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x001125C0 File Offset: 0x001107C0
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		if (_actionData.lastUseTime == 0f || this.IsActionRunning(_actionData))
		{
			return;
		}
		_actionData.lastUseTime = 0f;
		Vector3i targetPosition = ((ItemActionBailLiquid.MyInventoryData)_actionData).targetPosition;
		NetPackageWaterSet package = NetPackageManager.GetPackage<NetPackageWaterSet>();
		package.AddChange(targetPosition, WaterValue.Empty);
		GameManager.Instance.SetWaterRPC(package);
	}

	// Token: 0x02000508 RID: 1288
	[PublicizedFrom(EAccessModifier.Private)]
	public class MyInventoryData : ItemActionAttackData
	{
		// Token: 0x06002A16 RID: 10774 RVA: 0x00112618 File Offset: 0x00110818
		public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040020CF RID: 8399
		public Vector3i targetPosition;
	}
}
