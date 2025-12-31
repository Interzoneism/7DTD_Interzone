using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200050C RID: 1292
[Preserve]
public class ItemActionCollectWater : ItemAction
{
	// Token: 0x06002A25 RID: 10789 RVA: 0x00112C27 File Offset: 0x00110E27
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("Change_item_to"))
		{
			this.changeItemToItem = _props.Values["Change_item_to"];
		}
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x00112C58 File Offset: 0x00110E58
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionCollectWater.CollectWaterActionData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x00112C64 File Offset: 0x00110E64
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		int num = 19500;
		ItemClassWaterContainer itemClassWaterContainer = _actionData.invData.item as ItemClassWaterContainer;
		if (itemClassWaterContainer != null)
		{
			int meta = _actionData.invData.itemValue.Meta;
			num = Mathf.Max(0, itemClassWaterContainer.MaxMass - meta);
		}
		if (num < 195)
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
			ItemActionCollectWater.CollectWaterActionData collectWaterActionData = (ItemActionCollectWater.CollectWaterActionData)_actionData;
			collectWaterActionData.targetPosition = Voxel.voxelRayHitInfo.hit.blockPos;
			collectWaterActionData.targetMass = num;
			invData.holdingEntity.RightArmAnimationUse = true;
			if (this.soundStart != null)
			{
				invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
			}
		}
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x00112599 File Offset: 0x00110799
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return _actionData.lastUseTime != 0f && Time.time - _actionData.lastUseTime < this.Delay;
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x00112D74 File Offset: 0x00110F74
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		if (_actionData.lastUseTime == 0f || this.IsActionRunning(_actionData))
		{
			return;
		}
		_actionData.lastUseTime = 0f;
		World world = GameManager.Instance.World;
		ChunkCluster chunkCluster = (world != null) ? world.ChunkCache : null;
		if (chunkCluster == null)
		{
			return;
		}
		ItemActionCollectWater.CollectWaterActionData collectWaterActionData = (ItemActionCollectWater.CollectWaterActionData)_actionData;
		int num = CollectWaterUtils.CollectInCube(chunkCluster, collectWaterActionData.targetMass, collectWaterActionData.targetPosition, 1, this.waterPoints);
		if (num > 195)
		{
			NetPackageWaterSet package = NetPackageManager.GetPackage<NetPackageWaterSet>();
			foreach (CollectWaterUtils.WaterPoint waterPoint in this.waterPoints)
			{
				if (waterPoint.massToTake > 0)
				{
					package.AddChange(waterPoint.worldPos, new WaterValue(waterPoint.finalMass));
				}
			}
			GameManager.Instance.SetWaterRPC(package);
			if (!string.IsNullOrEmpty(this.changeItemToItem))
			{
				ItemStack itemStack = new ItemStack(ItemClass.GetItem(this.changeItemToItem, false), _actionData.invData.holdingEntity.inventory.holdingCount);
				itemStack.itemValue.Meta = _actionData.invData.itemValue.Meta + num;
				_actionData.invData.holdingEntity.inventory.SetItem(_actionData.invData.slotIdx, itemStack);
			}
		}
		this.waterPoints.Clear();
	}

	// Token: 0x040020D7 RID: 8407
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropChangeItemTo = "Change_item_to";

	// Token: 0x040020D8 RID: 8408
	[PublicizedFrom(EAccessModifier.Private)]
	public string changeItemToItem;

	// Token: 0x040020D9 RID: 8409
	[PublicizedFrom(EAccessModifier.Private)]
	public List<CollectWaterUtils.WaterPoint> waterPoints = new List<CollectWaterUtils.WaterPoint>();

	// Token: 0x0200050D RID: 1293
	[PublicizedFrom(EAccessModifier.Private)]
	public class CollectWaterActionData : ItemActionAttackData
	{
		// Token: 0x06002A2B RID: 10795 RVA: 0x00112618 File Offset: 0x00110818
		public CollectWaterActionData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040020DA RID: 8410
		public Vector3i targetPosition;

		// Token: 0x040020DB RID: 8411
		public int targetMass;
	}
}
