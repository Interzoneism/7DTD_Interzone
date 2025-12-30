using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000520 RID: 1312
[Preserve]
public class ItemActionExchangeItem : ItemAction
{
	// Token: 0x06002A96 RID: 10902 RVA: 0x00118028 File Offset: 0x00116228
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (!_props.Values.ContainsKey("Change_item_to"))
		{
			throw new Exception("Missing attribute 'change_item_to' in use_action 'ExchangeItem'");
		}
		this.changeItemToItem = _props.Values["Change_item_to"];
		if (_props.Values.ContainsKey("Change_block_to"))
		{
			this.changeBlockTo = _props.Values["Change_block_to"];
		}
		if (_props.Values.ContainsKey("Do_block_action"))
		{
			this.doBlockAction = _props.Values["Do_block_action"];
		}
		int num = 1;
		while (_props.Values.ContainsKey("Focused_blockname_" + num.ToString()))
		{
			string text = _props.Values["Focused_blockname_" + num.ToString()];
			BlockValue item = ItemClass.GetItem(text, false).ToBlockValue();
			if (item.Equals(BlockValue.Air))
			{
				throw new Exception("Unknown block name '" + text + "' in use_action!");
			}
			this.focusedBlocks.Add(item);
			num++;
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x00118140 File Offset: 0x00116340
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isFocusingBlock(WorldRayHitInfo _hitInfo)
	{
		for (int i = 0; i < this.focusedBlocks.Count; i++)
		{
			BlockValue other = this.focusedBlocks[i];
			if (_hitInfo.hit.blockValue.Equals(other))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x0011818C File Offset: 0x0011638C
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
		Ray lookRay = invData.holdingEntity.GetLookRay();
		lookRay.origin += lookRay.direction.normalized * 0.5f;
		if (!Voxel.Raycast(invData.world, lookRay, Constants.cDigAndBuildDistance, -538480645, 4095, 0f))
		{
			return;
		}
		if (Voxel.voxelRayHitInfo.bHitValid && this.isFocusingBlock(Voxel.voxelRayHitInfo))
		{
			this.hitLiquidBlock = Voxel.voxelRayHitInfo.hit.blockValue;
			this.hitLiquidPos = Voxel.voxelRayHitInfo.hit.blockPos;
			_actionData.lastUseTime = Time.time;
			invData.holdingEntity.RightArmAnimationUse = true;
			if (this.soundStart != null)
			{
				invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
			}
		}
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x00112599 File Offset: 0x00110799
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return _actionData.lastUseTime != 0f && Time.time - _actionData.lastUseTime < this.Delay;
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x00118283 File Offset: 0x00116483
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		_data.lastUseTime = 0f;
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x00118298 File Offset: 0x00116498
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		if (_actionData.lastUseTime == 0f || this.IsActionRunning(_actionData))
		{
			return;
		}
		QuestEventManager.Current.ExchangedFromItem(_actionData.invData.itemStack);
		ItemValue item = ItemClass.GetItem(this.changeItemToItem, false);
		_actionData.invData.holdingEntity.inventory.SetItem(_actionData.invData.slotIdx, new ItemStack(item, _actionData.invData.holdingEntity.inventory.holdingCount));
		if (this.doBlockAction != null && GameManager.Instance.World.IsWater(this.hitLiquidPos))
		{
			this.hitLiquidBlock.Block.DoExchangeAction(_actionData.invData.world, 0, this.hitLiquidPos, this.hitLiquidBlock, this.doBlockAction, _actionData.invData.holdingEntity.inventory.holdingCount);
		}
		if (this.changeBlockTo != null)
		{
			Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
			_actionData.invData.world.GetBlock(blockPos);
			BlockValue blockValue = ItemClass.GetItem(this.changeBlockTo, false).ToBlockValue();
			_actionData.invData.world.SetBlockRPC(blockPos, blockValue);
		}
	}

	// Token: 0x0400212F RID: 8495
	[PublicizedFrom(EAccessModifier.Protected)]
	public string changeItemToItem;

	// Token: 0x04002130 RID: 8496
	[PublicizedFrom(EAccessModifier.Protected)]
	public string changeBlockTo;

	// Token: 0x04002131 RID: 8497
	[PublicizedFrom(EAccessModifier.Protected)]
	public string doBlockAction;

	// Token: 0x04002132 RID: 8498
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue hitLiquidBlock;

	// Token: 0x04002133 RID: 8499
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i hitLiquidPos;

	// Token: 0x04002134 RID: 8500
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<BlockValue> focusedBlocks = new List<BlockValue>();
}
