using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200052C RID: 1324
[Preserve]
public class ItemActionOpenLootBundle : ItemAction
{
	// Token: 0x06002AD5 RID: 10965 RVA: 0x0011A64B File Offset: 0x0011884B
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionOpenLootBundle.MyInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x0011A654 File Offset: 0x00118854
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("Consume"))
		{
			this.Consume = StringParsers.ParseBool(_props.Values["Consume"], 0, -1, true);
		}
		else
		{
			this.Consume = true;
		}
		_props.ParseString("LootList", ref this.lootListName);
		this.UseAnimation = false;
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x0011A6B9 File Offset: 0x001188B9
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		((ItemActionOpenLootBundle.MyInventoryData)_data).bEatingStarted = false;
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0011A6D0 File Offset: 0x001188D0
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		ItemActionOpenLootBundle.MyInventoryData myInventoryData = (ItemActionOpenLootBundle.MyInventoryData)_actionData;
		if (!_bReleased)
		{
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		if (this.IsActionRunning(_actionData))
		{
			return;
		}
		EntityAlive holdingEntity = myInventoryData.invData.holdingEntity;
		if (EffectManager.GetValue(PassiveEffects.DisableItem, holdingEntity.inventory.holdingItemItemValue, 0f, holdingEntity, null, _actionData.invData.item.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
		{
			_actionData.lastUseTime = Time.time + 1f;
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			return;
		}
		BlockValue blockValue = BlockValue.Air;
		if (this.ConditionBlockTypes != null)
		{
			Ray lookRay = holdingEntity.GetLookRay();
			int modelLayer = holdingEntity.GetModelLayer();
			holdingEntity.SetModelLayer(2, false, null);
			Voxel.Raycast(myInventoryData.invData.world, lookRay, 2.5f, 131, (holdingEntity is EntityPlayer) ? 0.2f : 0.4f);
			holdingEntity.SetModelLayer(modelLayer, false, null);
			WorldRayHitInfo voxelRayHitInfo = Voxel.voxelRayHitInfo;
			if (!GameUtils.IsBlockOrTerrain(voxelRayHitInfo.tag))
			{
				return;
			}
			HitInfoDetails hit = voxelRayHitInfo.hit;
			blockValue = voxelRayHitInfo.hit.blockValue;
			if (blockValue.isair || !this.ConditionBlockTypes.Contains(blockValue.type))
			{
				lookRay = myInventoryData.invData.holdingEntity.GetLookRay();
				lookRay.origin += lookRay.direction.normalized * 0.5f;
				if (!Voxel.Raycast(myInventoryData.invData.world, lookRay, 2.5f, -538480645, 4095, 0f))
				{
					return;
				}
				HitInfoDetails hit2 = voxelRayHitInfo.hit;
				blockValue = voxelRayHitInfo.hit.blockValue;
				if (blockValue.isair || !this.ConditionBlockTypes.Contains(blockValue.type))
				{
					return;
				}
			}
		}
		_actionData.lastUseTime = Time.time;
		this.ExecuteInstantAction(myInventoryData.invData.holdingEntity, myInventoryData.invData.itemStack, true, null);
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x0011A8E4 File Offset: 0x00118AE4
	public override bool ExecuteInstantAction(EntityAlive ent, ItemStack stack, bool isHeldItem, XUiC_ItemStack stackController)
	{
		ent.MinEventContext.ItemValue = stack.itemValue;
		ent.MinEventContext.ItemValue.FireEvent(MinEventTypes.onSelfPrimaryActionStart, ent.MinEventContext);
		ent.FireEvent(MinEventTypes.onSelfPrimaryActionStart, false);
		if (this.soundStart != null)
		{
			ent.PlayOneShot(this.soundStart, false, false, false, null);
		}
		LootContainer lootContainer = LootContainer.GetLootContainer(this.lootListName, true);
		if (lootContainer == null)
		{
			return false;
		}
		if (this.Consume)
		{
			if (stack.itemValue.MaxUseTimes > 0 && stack.itemValue.UseTimes + 1f < (float)stack.itemValue.MaxUseTimes)
			{
				stack.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, stack.itemValue, 1f, ent, null, stack.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
				return true;
			}
			if (isHeldItem)
			{
				ent.inventory.DecHoldingItem(1);
			}
			else
			{
				stack.count--;
			}
		}
		ent.MinEventContext.ItemValue = stack.itemValue;
		ent.MinEventContext.ItemValue.FireEvent(MinEventTypes.onSelfPrimaryActionEnd, ent.MinEventContext);
		ent.FireEvent(MinEventTypes.onSelfPrimaryActionEnd, false);
		new List<ItemStack>();
		EntityPlayer entityPlayer = ent as EntityPlayer;
		if (entityPlayer != null)
		{
			IList<ItemStack> list = lootContainer.Spawn(ent.rand, 100, (float)entityPlayer.GetHighestPartyLootStage(0f, 0f), 0f, entityPlayer, FastTags<TagGroup.Global>.none, true, false);
			for (int i = 0; i < list.Count; i++)
			{
				ItemStack itemStack = list[i].Clone();
				if (!LocalPlayerUI.GetUIForPlayer(ent as EntityPlayerLocal).xui.PlayerInventory.AddItem(itemStack))
				{
					ent.world.gameManager.ItemDropServer(itemStack, ent.GetPosition(), Vector3.zero, -1, 60f, false);
				}
			}
		}
		return true;
	}

	// Token: 0x04002153 RID: 8531
	public new bool Consume;

	// Token: 0x04002154 RID: 8532
	public HashSet<int> ConditionBlockTypes;

	// Token: 0x04002155 RID: 8533
	public string lootListName = "";

	// Token: 0x0200052D RID: 1325
	[PublicizedFrom(EAccessModifier.Private)]
	public class MyInventoryData : ItemActionAttackData
	{
		// Token: 0x06002ADB RID: 10971 RVA: 0x00112618 File Offset: 0x00110818
		public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002156 RID: 8534
		public bool bEatingStarted;
	}
}
