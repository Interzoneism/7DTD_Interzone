using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200051B RID: 1307
[Preserve]
public class ItemActionDumpWater : ItemAction
{
	// Token: 0x06002A7D RID: 10877 RVA: 0x001171A1 File Offset: 0x001153A1
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("Change_item_to"))
		{
			this.changeItemToItem = _props.Values["Change_item_to"];
		}
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x001171D2 File Offset: 0x001153D2
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionDumpWater.DumpWaterActionData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x001171DC File Offset: 0x001153DC
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (!(_actionData.invData.item is ItemClassWaterContainer))
		{
			Debug.LogError("Cannot dump water as item is not a WaterContainer");
			return;
		}
		if (_actionData.invData.itemValue.Meta < 195)
		{
			return;
		}
		if (_actionData.lastUseTime > 0f)
		{
			return;
		}
		ItemInventoryData invData = _actionData.invData;
		if (!Voxel.Raycast(invData.world, invData.hitInfo.ray, Constants.cDigAndBuildDistance, -555266053, 4095, 0f))
		{
			return;
		}
		WorldRayHitInfo voxelRayHitInfo = Voxel.voxelRayHitInfo;
		if (voxelRayHitInfo.bHitValid)
		{
			ItemActionDumpWater.DumpWaterActionData dumpWaterActionData = (ItemActionDumpWater.DumpWaterActionData)_actionData;
			if (!this.TryFindDumpPosition(voxelRayHitInfo, out dumpWaterActionData.targetPosition))
			{
				return;
			}
			if (GameManager.Instance.World.IsWithinTraderArea(dumpWaterActionData.targetPosition))
			{
				GameManager.ShowTooltip(_actionData.invData.holdingEntity as EntityPlayerLocal, "ttCannotUseAtThisTime", false, false, 0f);
				return;
			}
			if (GameManager.Instance.World.GetLandClaimOwner(dumpWaterActionData.targetPosition, GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_actionData.invData.holdingEntity.entityId)) == EnumLandClaimOwner.Other)
			{
				GameManager.ShowTooltip(_actionData.invData.holdingEntity as EntityPlayerLocal, "ttCannotUseAtThisTime", false, false, 0f);
				return;
			}
			_actionData.lastUseTime = Time.time;
			invData.holdingEntity.RightArmAnimationUse = true;
			if (this.soundStart != null)
			{
				invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
			}
		}
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x00117354 File Offset: 0x00115554
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryFindDumpPosition(WorldRayHitInfo hitInfo, out Vector3i blockPos)
	{
		blockPos = World.worldToBlockPos(hitInfo.hit.pos);
		if (WaterUtils.CanWaterFlowThrough(GameManager.Instance.World.GetBlock(blockPos)))
		{
			return true;
		}
		if (!hitInfo.hit.blockValue.isair)
		{
			BlockFace blockFace = hitInfo.hit.blockFace;
			switch (blockFace)
			{
			case BlockFace.Top:
				blockPos += Vector3i.up;
				goto IL_11C;
			case BlockFace.Bottom:
				blockPos += Vector3i.down;
				goto IL_11C;
			case BlockFace.North:
				blockPos += Vector3i.forward;
				goto IL_11C;
			case BlockFace.West:
				blockPos += Vector3i.right;
				goto IL_11C;
			case BlockFace.South:
				blockPos += Vector3i.back;
				goto IL_11C;
			case BlockFace.East:
				blockPos += Vector3i.left;
				goto IL_11C;
			case BlockFace.Middle:
				break;
			default:
				if (blockFace != BlockFace.None)
				{
					goto IL_11C;
				}
				break;
			}
			return false;
			IL_11C:
			if (WaterUtils.CanWaterFlowThrough(GameManager.Instance.World.GetBlock(blockPos)))
			{
				return true;
			}
		}
		blockPos = hitInfo.lastBlockPos;
		return WaterUtils.CanWaterFlowThrough(GameManager.Instance.World.GetBlock(blockPos));
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x00112599 File Offset: 0x00110799
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return _actionData.lastUseTime != 0f && Time.time - _actionData.lastUseTime < this.Delay;
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x001174C8 File Offset: 0x001156C8
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		if (_actionData.lastUseTime == 0f || this.IsActionRunning(_actionData))
		{
			return;
		}
		_actionData.lastUseTime = 0f;
		ItemActionDumpWater.DumpWaterActionData dumpWaterActionData = (ItemActionDumpWater.DumpWaterActionData)_actionData;
		int meta = _actionData.invData.itemValue.Meta;
		WaterValue water = GameManager.Instance.World.GetWater(dumpWaterActionData.targetPosition);
		int num = water.GetMass() + meta;
		if (num > 65535)
		{
			Debug.LogError(string.Format("Trying to dump {0} into {1} which more than the maximum mass. Mass will be clamped to {2}", meta, water.GetMass(), 65535));
			num = 65535;
		}
		NetPackageWaterSet package = NetPackageManager.GetPackage<NetPackageWaterSet>();
		package.AddChange(dumpWaterActionData.targetPosition, new WaterValue(num));
		GameManager.Instance.SetWaterRPC(package);
		if (!string.IsNullOrEmpty(this.changeItemToItem))
		{
			ItemValue item = ItemClass.GetItem(this.changeItemToItem, false);
			_actionData.invData.holdingEntity.inventory.SetItem(_actionData.invData.slotIdx, new ItemStack(item, _actionData.invData.holdingEntity.inventory.holdingCount));
		}
	}

	// Token: 0x04002121 RID: 8481
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropChangeItemTo = "Change_item_to";

	// Token: 0x04002122 RID: 8482
	[PublicizedFrom(EAccessModifier.Private)]
	public string changeItemToItem;

	// Token: 0x0200051C RID: 1308
	[PublicizedFrom(EAccessModifier.Private)]
	public class DumpWaterActionData : ItemActionAttackData
	{
		// Token: 0x06002A84 RID: 10884 RVA: 0x00112618 File Offset: 0x00110818
		public DumpWaterActionData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002123 RID: 8483
		public Vector3i targetPosition;
	}
}
