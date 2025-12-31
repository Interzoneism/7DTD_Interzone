using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000519 RID: 1305
[Preserve]
public class ItemActionDisconnectPower : ItemAction
{
	// Token: 0x06002A74 RID: 10868 RVA: 0x00116D05 File Offset: 0x00114F05
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionDisconnectPower.MyInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x00116D0E File Offset: 0x00114F0E
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x00116D17 File Offset: 0x00114F17
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		((ItemActionDisconnectPower.MyInventoryData)_data).StartDisconnect = false;
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x00116D2C File Offset: 0x00114F2C
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		_actionData.lastUseTime = Time.time;
		((ItemActionDisconnectPower.MyInventoryData)_actionData).StartDisconnect = true;
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x00116D60 File Offset: 0x00114F60
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionDisconnectPower.MyInventoryData myInventoryData = (ItemActionDisconnectPower.MyInventoryData)_actionData;
		return myInventoryData.StartDisconnect && Time.time - myInventoryData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast;
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x00116DB8 File Offset: 0x00114FB8
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionDisconnectPower.MyInventoryData myInventoryData = (ItemActionDisconnectPower.MyInventoryData)_actionData;
		if (!myInventoryData.StartDisconnect || Time.time - myInventoryData.lastUseTime < AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast)
		{
			return;
		}
		myInventoryData.StartDisconnect = false;
		ItemActionDisconnectPower.MyInventoryData myInventoryData2 = (ItemActionDisconnectPower.MyInventoryData)_actionData;
		ItemInventoryData invData = _actionData.invData;
		Vector3i lastBlockPos = invData.hitInfo.lastBlockPos;
		Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
		if (!invData.hitInfo.bHitValid || invData.hitInfo.tag.StartsWith("E_"))
		{
			return;
		}
		if (((ItemActionConnectPower)_actionData.invData.holdingEntity.inventory.holdingItem.Actions[1]).DisconnectWire((ItemActionConnectPower.ConnectPowerData)_actionData.invData.holdingEntity.inventory.holdingItemData.actionData[1]))
		{
			return;
		}
		if (!myInventoryData.invData.world.CanPlaceBlockAt(blockPos, myInventoryData.invData.world.gameManager.GetPersistentLocalPlayer(), false))
		{
			return;
		}
		IPowered poweredBlock = this.GetPoweredBlock(invData);
		if (poweredBlock == null)
		{
			((ItemActionConnectPower)_actionData.invData.holdingEntity.inventory.holdingItem.Actions[1]).DisconnectWire((ItemActionConnectPower.ConnectPowerData)_actionData.invData.holdingEntity.inventory.holdingItemData.actionData[1]);
			return;
		}
		if (myInventoryData.invData.itemValue.MaxUseTimes > 0 && myInventoryData.invData.itemValue.UseTimes >= (float)myInventoryData.invData.itemValue.MaxUseTimes)
		{
			EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
			if (this.item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
			{
				Manager.PlayInsidePlayerHead(this.item.Properties.Values[ItemClass.PropSoundJammed], -1, 0f, false, false);
			}
			GameManager.ShowTooltip(player, "ttItemNeedsRepair", false, false, 0f);
			return;
		}
		if (myInventoryData.invData.itemValue.MaxUseTimes > 0)
		{
			_actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, _actionData.invData.itemValue, 1f, invData.holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
			base.HandleItemBreak(_actionData);
		}
		_actionData.invData.holdingEntity.RightArmAnimationAttack = true;
		poweredBlock.RemoveParentWithWiringTool(_actionData.invData.holdingEntity.entityId);
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x00117068 File Offset: 0x00115268
	[PublicizedFrom(EAccessModifier.Private)]
	public IPowered GetPoweredBlock(ItemInventoryData data)
	{
		Block block = data.world.GetBlock(data.hitInfo.hit.blockPos).Block;
		if (!(block is BlockPowered) && !(block is BlockPowerSource))
		{
			return null;
		}
		Vector3i blockPos = data.hitInfo.hit.blockPos;
		ChunkCluster chunkCluster = data.world.ChunkClusters[data.hitInfo.hit.clrIdx];
		if (chunkCluster == null)
		{
			return null;
		}
		Chunk chunk = (Chunk)chunkCluster.GetChunkSync(World.toChunkXZ(blockPos.x), blockPos.y, World.toChunkXZ(blockPos.z));
		if (chunk == null)
		{
			return null;
		}
		TileEntity tileEntity = chunk.GetTileEntity(World.toBlock(blockPos));
		if (tileEntity == null)
		{
			if (block is BlockPowered)
			{
				tileEntity = (block as BlockPowered).CreateTileEntity(chunk);
			}
			else if (block is BlockPowerSource)
			{
				tileEntity = (block as BlockPowerSource).CreateTileEntity(chunk);
			}
			tileEntity.localChunkPos = World.toBlock(blockPos);
			BlockEntityData blockEntity = chunk.GetBlockEntity(blockPos);
			if (blockEntity != null)
			{
				((TileEntityPowered)tileEntity).BlockTransform = blockEntity.transform;
			}
			((TileEntityPowered)tileEntity).InitializePowerData();
			chunk.AddTileEntity(tileEntity);
		}
		return tileEntity as IPowered;
	}

	// Token: 0x0200051A RID: 1306
	[PublicizedFrom(EAccessModifier.Private)]
	public class MyInventoryData : ItemActionAttackData
	{
		// Token: 0x06002A7C RID: 10876 RVA: 0x00112618 File Offset: 0x00110818
		public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002120 RID: 8480
		public bool StartDisconnect;
	}
}
