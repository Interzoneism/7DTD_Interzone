using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000536 RID: 1334
[Preserve]
public class ItemActionRepair : ItemActionAttack
{
	// Token: 0x06002B30 RID: 11056 RVA: 0x0011E342 File Offset: 0x0011C542
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionRepair.InventoryDataRepair(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x0011E34C File Offset: 0x0011C54C
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		this.repairAmount = 0f;
		_props.ParseFloat("Repair_amount", ref this.repairAmount);
		this.hitCountOffset = 0f;
		_props.ParseFloat("Upgrade_hit_offset", ref this.hitCountOffset);
		this.repairActionSound = _props.GetString("Repair_action_sound");
		this.upgradeActionSound = _props.GetString("Upgrade_action_sound");
		this.allowedUpgradeItems = _props.GetString("Allowed_upgrade_items");
		this.restrictedUpgradeItems = _props.GetString("Restricted_upgrade_items");
		this.soundAnimActionSyncTimer = 0.3f;
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x0011E3E8 File Offset: 0x0011C5E8
	public override void StopHolding(ItemActionData _data)
	{
		((ItemActionRepair.InventoryDataRepair)_data).bUseStarted = false;
		this.bUpgradeCountChanged = false;
		this.blockUpgradeCount = 0;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_data.invData.holdingEntity as EntityPlayerLocal);
		if (uiforPlayer != null)
		{
			XUiC_FocusedBlockHealth.SetData(uiforPlayer, null, 0f);
		}
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x0011E43A File Offset: 0x0011C63A
	public override void StartHolding(ItemActionData _data)
	{
		((ItemActionRepair.InventoryDataRepair)_data).bUseStarted = false;
		this.bUpgradeCountChanged = false;
		this.blockUpgradeCount = 0;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x0011E458 File Offset: 0x0011C658
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		if (_actionData.invData.hitInfo.bHitValid && _actionData.invData.hitInfo.hit.distanceSq > Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = _actionData.invData.holdingEntity as EntityPlayerLocal;
		if (!entityPlayerLocal)
		{
			return;
		}
		GUIWindowManager windowManager = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal).windowManager;
		ItemActionRepair.InventoryDataRepair inventoryDataRepair = (ItemActionRepair.InventoryDataRepair)_actionData;
		if (windowManager.IsModalWindowOpen())
		{
			inventoryDataRepair.bUseStarted = false;
			inventoryDataRepair.repairType = ItemActionRepair.EnumRepairType.None;
			return;
		}
		if (_actionData.invData.holdingEntity != _actionData.invData.world.GetPrimaryPlayer())
		{
			return;
		}
		if (!inventoryDataRepair.bUseStarted)
		{
			return;
		}
		if (this.bUpgradeCountChanged)
		{
			BlockValue block = _actionData.invData.world.GetBlock(this.blockTargetPos);
			Block block2 = block.Block;
			int num;
			if (int.TryParse(block2.Properties.Values["UpgradeBlock.UpgradeHitCount"], out num))
			{
				num = (int)(((float)num + this.hitCountOffset < 1f) ? 1f : ((float)num + this.hitCountOffset));
				inventoryDataRepair.upgradePerc = (float)this.blockUpgradeCount / (float)num;
				if (this.blockUpgradeCount >= num)
				{
					if (this.RemoveRequiredResource(_actionData.invData, block))
					{
						BlockValue blockValue = Block.GetBlockValue(block2.Properties.Values[Block.PropUpgradeBlockClassToBlock], false);
						blockValue.rotation = block.rotation;
						blockValue.meta = block.meta;
						QuestEventManager.Current.BlockUpgraded(block2.GetBlockName(), this.blockTargetPos);
						_actionData.invData.holdingEntity.MinEventContext.ItemActionData = _actionData;
						_actionData.invData.holdingEntity.MinEventContext.BlockValue = blockValue;
						_actionData.invData.holdingEntity.MinEventContext.Position = this.blockTargetPos.ToVector3();
						_actionData.invData.holdingEntity.FireEvent(MinEventTypes.onSelfUpgradedBlock, true);
						Block block3 = block.Block;
						block3.DamageBlock(_actionData.invData.world, this.blockTargetClrIdx, this.blockTargetPos, block, -1, _actionData.invData.holdingEntity.entityId, null, false, false);
						int num2;
						if (int.TryParse(block2.Properties.Values[Block.PropUpgradeBlockClassItemCount], out num2))
						{
							_actionData.invData.holdingEntity.Progression.AddLevelExp((int)(blockValue.Block.blockMaterial.Experience * (float)num2), "_xpFromUpgradeBlock", Progression.XPTypes.Upgrading, true, true);
						}
						if (block3.UpgradeSound != null)
						{
							_actionData.invData.holdingEntity.PlayOneShot(block3.UpgradeSound, false, false, false, null);
						}
					}
					this.blockUpgradeCount = 0;
				}
			}
			string text = this.upgradeActionSound;
			string upgradeItemName = this.GetUpgradeItemName(block2);
			if (text.Length == 0 && this.item != null && upgradeItemName != null && upgradeItemName.Length > 0)
			{
				text = string.Format("ImpactSurface/{0}hit{1}", _actionData.invData.holdingEntity.inventory.holdingItem.MadeOfMaterial.SurfaceCategory, ItemClass.GetForId(ItemClass.GetItem(upgradeItemName, false).type).MadeOfMaterial.SurfaceCategory);
			}
			if (text.Length > 0)
			{
				_actionData.invData.holdingEntity.PlayOneShot(text, false, false, false, null);
			}
			this.bUpgradeCountChanged = false;
			return;
		}
		this.ExecuteAction(_actionData, false);
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x0011E7B8 File Offset: 0x0011C9B8
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		EntityPlayerLocal entityPlayerLocal = _actionData.invData.holdingEntity as EntityPlayerLocal;
		LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		if (_bReleased)
		{
			((ItemActionRepair.InventoryDataRepair)_actionData).bUseStarted = false;
			((ItemActionRepair.InventoryDataRepair)_actionData).repairType = ItemActionRepair.EnumRepairType.None;
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		ItemInventoryData invData = _actionData.invData;
		if (invData.hitInfo.bHitValid && invData.hitInfo.hit.distanceSq > Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance)
		{
			return;
		}
		if (EffectManager.GetValue(PassiveEffects.DisableItem, entityPlayerLocal.inventory.holdingItemItemValue, 0f, entityPlayerLocal, null, _actionData.invData.item.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
		{
			_actionData.lastUseTime = Time.time + 1f;
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			return;
		}
		_actionData.lastUseTime = Time.time;
		if (invData.hitInfo.bHitValid && _actionData.invData.world.IsWithinTraderArea(invData.hitInfo.hit.blockPos))
		{
			return;
		}
		if (invData.hitInfo.bHitValid && GameUtils.IsBlockOrTerrain(invData.hitInfo.tag))
		{
			this.blockTargetPos = invData.hitInfo.hit.blockPos;
			this.blockTargetClrIdx = invData.hitInfo.hit.clrIdx;
			BlockValue block = invData.world.GetBlock(this.blockTargetPos);
			if (block.ischild)
			{
				this.blockTargetPos = block.Block.multiBlockPos.GetParentPos(this.blockTargetPos, block);
				block = _actionData.invData.world.GetBlock(this.blockTargetPos);
			}
			if ((invData.itemValue.MaxUseTimes > 0 && invData.itemValue.UseTimes >= (float)invData.itemValue.MaxUseTimes) || (invData.itemValue.UseTimes == 0f && invData.itemValue.MaxUseTimes == 0))
			{
				if (this.item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
				{
					Manager.PlayInsidePlayerHead(this.item.Properties.Values[ItemClass.PropSoundJammed], -1, 0f, false, false);
				}
				GameManager.ShowTooltip(entityPlayerLocal, "ttItemNeedsRepair", false, false, 0f);
				return;
			}
			ItemActionRepair.InventoryDataRepair inventoryDataRepair = (ItemActionRepair.InventoryDataRepair)_actionData;
			Block block2 = block.Block;
			if (block2.CanRepair(block))
			{
				int num = Utils.FastMin((int)this.repairAmount, block.damage);
				float num2 = (float)num / (float)block2.MaxDamage;
				List<Block.SItemNameCount> list = block2.RepairItems;
				if (block2.RepairItemsMeshDamage != null && block2.shape.UseRepairDamageState(block))
				{
					num = 1;
					num2 = 1f;
					list = block2.RepairItemsMeshDamage;
				}
				if (list == null)
				{
					return;
				}
				if (inventoryDataRepair.lastHitPosition != this.blockTargetPos || inventoryDataRepair.lastHitBlockValue.type != block.type || inventoryDataRepair.lastRepairItems != list)
				{
					inventoryDataRepair.lastHitPosition = this.blockTargetPos;
					inventoryDataRepair.lastHitBlockValue = block;
					inventoryDataRepair.lastRepairItems = list;
					inventoryDataRepair.lastRepairItemsPercents = new float[list.Count];
				}
				inventoryDataRepair.blockDamagePerc = (float)block.damage / (float)block2.MaxDamage;
				EntityPlayerLocal entityPlayerLocal2 = inventoryDataRepair.invData.holdingEntity as EntityPlayerLocal;
				if (entityPlayerLocal2 == null)
				{
					return;
				}
				inventoryDataRepair.repairType = ItemActionRepair.EnumRepairType.Repair;
				float resourceScale = block2.ResourceScale;
				bool flag = false;
				for (int i = 0; i < list.Count; i++)
				{
					string itemName = list[i].ItemName;
					float num3 = (float)list[i].Count * num2 * resourceScale;
					if (inventoryDataRepair.lastRepairItemsPercents[i] <= 0f)
					{
						int count = Utils.FastMax((int)num3, 1);
						ItemStack itemStack = new ItemStack(ItemClass.GetItem(itemName, false), count);
						if (!this.canRemoveRequiredItem(inventoryDataRepair.invData, itemStack))
						{
							itemStack.count = 0;
							entityPlayerLocal2.AddUIHarvestingItem(itemStack, true);
							if (!flag)
							{
								flag = true;
							}
						}
					}
				}
				if (flag)
				{
					return;
				}
				inventoryDataRepair.invData.holdingEntity.RightArmAnimationUse = true;
				float num4 = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					float num5 = (float)list[j].Count * num2 * resourceScale;
					if (inventoryDataRepair.lastRepairItemsPercents[j] <= 0f)
					{
						string itemName2 = list[j].ItemName;
						int num6 = Utils.FastMax((int)num5, 1);
						inventoryDataRepair.lastRepairItemsPercents[j] += (float)num6;
						inventoryDataRepair.lastRepairItemsPercents[j] -= num5;
						ItemStack itemStack2 = new ItemStack(ItemClass.GetItem(itemName2, false), num6);
						num4 += itemStack2.itemValue.ItemClass.MadeOfMaterial.Experience * (float)num6;
						this.removeRequiredItem(inventoryDataRepair.invData, itemStack2);
						itemStack2.count *= -1;
						entityPlayerLocal2.AddUIHarvestingItem(itemStack2, false);
					}
					else
					{
						inventoryDataRepair.lastRepairItemsPercents[j] -= num5;
					}
				}
				if (this.repairActionSound != null && this.repairActionSound.Length > 0)
				{
					invData.holdingEntity.PlayOneShot(this.repairActionSound, false, false, false, null);
				}
				else if (this.soundStart != null && this.soundStart.Length > 0)
				{
					invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
				}
				if (invData.itemValue.MaxUseTimes > 0)
				{
					invData.itemValue.UseTimes += 1f;
				}
				int num7 = block.Block.DamageBlock(invData.world, invData.hitInfo.hit.clrIdx, this.blockTargetPos, block, -num, invData.holdingEntity.entityId, null, false, false);
				inventoryDataRepair.bUseStarted = true;
				inventoryDataRepair.blockDamagePerc = (float)num7 / (float)block.Block.MaxDamage;
				inventoryDataRepair.invData.holdingEntity.MinEventContext.ItemActionData = inventoryDataRepair;
				inventoryDataRepair.invData.holdingEntity.MinEventContext.BlockValue = block;
				inventoryDataRepair.invData.holdingEntity.MinEventContext.Position = this.blockTargetPos.ToVector3();
				inventoryDataRepair.invData.holdingEntity.FireEvent(MinEventTypes.onSelfRepairBlock, true);
				entityPlayerLocal2.Progression.AddLevelExp((int)num4, "_xpFromRepairBlock", Progression.XPTypes.Repairing, true, true);
				return;
			}
			else if (this.isUpgradeItem)
			{
				if (!this.CanRemoveRequiredResource(_actionData.invData, block))
				{
					string upgradeItemName = this.GetUpgradeItemName(block.Block);
					if (upgradeItemName != null)
					{
						ItemStack @is = new ItemStack(ItemClass.GetItem(upgradeItemName, false), 0);
						(_actionData.invData.holdingEntity as EntityPlayerLocal).AddUIHarvestingItem(@is, true);
					}
					inventoryDataRepair.upgradePerc = 0f;
					return;
				}
				_actionData.invData.holdingEntity.RightArmAnimationUse = true;
				inventoryDataRepair.repairType = ItemActionRepair.EnumRepairType.Upgrade;
				if (this.blockTargetPos == this.lastBlockTargetPos)
				{
					this.blockUpgradeCount++;
				}
				else
				{
					this.blockUpgradeCount = 1;
				}
				this.lastBlockTargetPos = this.blockTargetPos;
				this.bUpgradeCountChanged = true;
				inventoryDataRepair.bUseStarted = true;
				return;
			}
			else
			{
				inventoryDataRepair.bUseStarted = false;
				inventoryDataRepair.repairType = ItemActionRepair.EnumRepairType.None;
			}
		}
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x0011EEF5 File Offset: 0x0011D0F5
	public float GetRepairAmount()
	{
		return this.repairAmount;
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x0011EF00 File Offset: 0x0011D100
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetUpgradeItemName(Block block)
	{
		string text = block.Properties.Values["UpgradeBlock.Item"];
		if (text != null && text.Length == 1 && text[0] == 'r')
		{
			text = block.RepairItems[0].ItemName;
		}
		return text;
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x0011EF50 File Offset: 0x0011D150
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CanRemoveRequiredResource(ItemInventoryData data, BlockValue blockValue)
	{
		Block block = blockValue.Block;
		string upgradeItemName = this.GetUpgradeItemName(block);
		bool flag = upgradeItemName != null && upgradeItemName.Length > 0;
		if (flag)
		{
			if (this.allowedUpgradeItems.Length > 0 && !this.allowedUpgradeItems.ContainsCaseInsensitive(upgradeItemName))
			{
				return false;
			}
			if (this.restrictedUpgradeItems.Length > 0 && this.restrictedUpgradeItems.ContainsCaseInsensitive(upgradeItemName))
			{
				return false;
			}
		}
		int num;
		if (!int.TryParse(block.Properties.Values["UpgradeBlock.UpgradeHitCount"], out num))
		{
			return false;
		}
		int num2;
		if (!int.TryParse(block.Properties.Values[Block.PropUpgradeBlockClassItemCount], out num2) && flag)
		{
			return false;
		}
		if (block.GetBlockName() != null && flag)
		{
			ItemValue item = ItemClass.GetItem(upgradeItemName, false);
			if (data.holdingEntity.inventory.GetItemCount(item, false, -1, -1, true) >= num2)
			{
				return true;
			}
			if (data.holdingEntity.bag.GetItemCount(item, -1, -1, true) >= num2)
			{
				return true;
			}
		}
		else if (!flag)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x0011F050 File Offset: 0x0011D250
	[PublicizedFrom(EAccessModifier.Private)]
	public bool RemoveRequiredResource(ItemInventoryData data, BlockValue blockValue)
	{
		if (!this.CanRemoveRequiredResource(data, blockValue))
		{
			return false;
		}
		Block block = blockValue.Block;
		ItemValue item = ItemClass.GetItem(this.GetUpgradeItemName(block), false);
		int num;
		if (!int.TryParse(block.Properties.Values[Block.PropUpgradeBlockClassItemCount], out num))
		{
			return false;
		}
		if (data.holdingEntity.inventory.DecItem(item, num, false, null) == num)
		{
			EntityPlayerLocal entityPlayerLocal = data.holdingEntity as EntityPlayerLocal;
			if (entityPlayerLocal != null && num != 0)
			{
				entityPlayerLocal.AddUIHarvestingItem(new ItemStack(item, -num), false);
			}
			return true;
		}
		if (data.holdingEntity.bag.DecItem(item, num, false, null) == num)
		{
			EntityPlayerLocal entityPlayerLocal2 = data.holdingEntity as EntityPlayerLocal;
			if (entityPlayerLocal2 != null)
			{
				entityPlayerLocal2.AddUIHarvestingItem(new ItemStack(item, -num), false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x0011F120 File Offset: 0x0011D320
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canRemoveRequiredItem(ItemInventoryData _data, ItemStack _itemStack)
	{
		return _data.holdingEntity.inventory.GetItemCount(_itemStack.itemValue, false, -1, -1, true) >= _itemStack.count || _data.holdingEntity.bag.GetItemCount(_itemStack.itemValue, -1, -1, true) >= _itemStack.count;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x0011F178 File Offset: 0x0011D378
	[PublicizedFrom(EAccessModifier.Private)]
	public bool removeRequiredItem(ItemInventoryData _data, ItemStack _itemStack)
	{
		return _data.holdingEntity.inventory.DecItem(_itemStack.itemValue, _itemStack.count, false, null) == _itemStack.count || _data.holdingEntity.bag.DecItem(_itemStack.itemValue, _itemStack.count, false, null) == _itemStack.count;
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x0011F1D8 File Offset: 0x0011D3D8
	public override ItemClass.EnumCrosshairType GetCrosshairType(ItemActionData _actionData)
	{
		ItemActionRepair.EnumRepairType repairType = ((ItemActionRepair.InventoryDataRepair)_actionData).repairType;
		if (repairType == ItemActionRepair.EnumRepairType.Repair)
		{
			return ItemClass.EnumCrosshairType.Repair;
		}
		if (repairType != ItemActionRepair.EnumRepairType.Upgrade)
		{
			return ItemClass.EnumCrosshairType.Plus;
		}
		return ItemClass.EnumCrosshairType.Upgrade;
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x0011F200 File Offset: 0x0011D400
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isShowOverlay(ItemActionData _actionData)
	{
		WorldRayHitInfo hitInfo = _actionData.invData.hitInfo;
		if (hitInfo.bHitValid && hitInfo.hit.distanceSq > Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance)
		{
			return false;
		}
		bool result = false;
		ItemActionRepair.InventoryDataRepair inventoryDataRepair = (ItemActionRepair.InventoryDataRepair)_actionData;
		if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.None)
		{
			if (hitInfo.bHitValid)
			{
				int damage;
				if (!hitInfo.hit.blockValue.ischild)
				{
					damage = hitInfo.hit.blockValue.damage;
				}
				else
				{
					Vector3i parentPos = hitInfo.hit.blockValue.Block.multiBlockPos.GetParentPos(hitInfo.hit.blockPos, hitInfo.hit.blockValue);
					damage = _actionData.invData.world.GetBlock(parentPos).damage;
				}
				result = (damage > 0);
			}
		}
		else if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.Repair)
		{
			EntityPlayerLocal entityPlayerLocal = _actionData.invData.holdingEntity as EntityPlayerLocal;
			result = (entityPlayerLocal != null && entityPlayerLocal.HitInfo.bHitValid && Time.time - _actionData.lastUseTime <= 1.5f);
		}
		else if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.Upgrade)
		{
			EntityPlayerLocal entityPlayerLocal2 = _actionData.invData.holdingEntity as EntityPlayerLocal;
			result = (entityPlayerLocal2 != null && entityPlayerLocal2.HitInfo.bHitValid && Time.time - _actionData.lastUseTime <= 1.5f && inventoryDataRepair.upgradePerc > 0f);
		}
		return result;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x0011F384 File Offset: 0x0011D584
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void getOverlayData(ItemActionData _actionData, out float _perc, out string _text)
	{
		ItemActionRepair.InventoryDataRepair inventoryDataRepair = (ItemActionRepair.InventoryDataRepair)_actionData;
		if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.None)
		{
			BlockValue blockValue = _actionData.invData.hitInfo.hit.blockValue;
			if (blockValue.ischild)
			{
				Vector3i parentPos = blockValue.Block.multiBlockPos.GetParentPos(_actionData.invData.hitInfo.hit.blockPos, blockValue);
				blockValue = _actionData.invData.world.GetBlock(parentPos);
			}
			int shownMaxDamage = blockValue.Block.GetShownMaxDamage();
			_perc = ((float)shownMaxDamage - (float)blockValue.damage) / (float)shownMaxDamage;
			_text = string.Format("{0}/{1}", Utils.FastMax(0, shownMaxDamage - blockValue.damage), shownMaxDamage);
			return;
		}
		if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.Repair)
		{
			_perc = 1f - inventoryDataRepair.blockDamagePerc;
			_text = string.Format("{0}%", (_perc * 100f).ToCultureInvariantString("0"));
			return;
		}
		if (inventoryDataRepair.repairType == ItemActionRepair.EnumRepairType.Upgrade)
		{
			_perc = inventoryDataRepair.upgradePerc;
			_text = string.Format("{0}%", (_perc * 100f).ToCultureInvariantString("0"));
			return;
		}
		_perc = 0f;
		_text = string.Empty;
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x0011F4B4 File Offset: 0x0011D6B4
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionRepair.InventoryDataRepair inventoryDataRepair = (ItemActionRepair.InventoryDataRepair)_actionData;
		return Time.time - inventoryDataRepair.lastUseTime < this.Delay + 0.1f;
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x0011F4E5 File Offset: 0x0011D6E5
	public override void GetItemValueActionInfo(ref List<string> _infoList, ItemValue _itemValue, XUi _xui, int _actionIndex = 0)
	{
		base.GetItemValueActionInfo(ref _infoList, _itemValue, _xui, _actionIndex);
		_infoList.Add(ItemAction.StringFormatHandler(Localization.Get("lblBlkRpr", false), this.GetRepairAmount().ToCultureInvariantString()));
	}

	// Token: 0x040021A8 RID: 8616
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue targetBlock;

	// Token: 0x040021A9 RID: 8617
	[PublicizedFrom(EAccessModifier.Protected)]
	public float repairAmount;

	// Token: 0x040021AA RID: 8618
	[PublicizedFrom(EAccessModifier.Protected)]
	public float hitCountOffset;

	// Token: 0x040021AB RID: 8619
	[PublicizedFrom(EAccessModifier.Protected)]
	public float soundAnimActionSyncTimer;

	// Token: 0x040021AC RID: 8620
	[PublicizedFrom(EAccessModifier.Protected)]
	public const float SOUND_LENGTH = 0.3f;

	// Token: 0x040021AD RID: 8621
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isUpgradeItem = true;

	// Token: 0x040021AE RID: 8622
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockTargetPos;

	// Token: 0x040021AF RID: 8623
	[PublicizedFrom(EAccessModifier.Private)]
	public int blockTargetClrIdx;

	// Token: 0x040021B0 RID: 8624
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i lastBlockTargetPos;

	// Token: 0x040021B1 RID: 8625
	[PublicizedFrom(EAccessModifier.Private)]
	public int blockUpgradeCount;

	// Token: 0x040021B2 RID: 8626
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUpgradeCountChanged;

	// Token: 0x040021B3 RID: 8627
	[PublicizedFrom(EAccessModifier.Private)]
	public string repairActionSound;

	// Token: 0x040021B4 RID: 8628
	[PublicizedFrom(EAccessModifier.Private)]
	public string upgradeActionSound;

	// Token: 0x040021B5 RID: 8629
	[PublicizedFrom(EAccessModifier.Private)]
	public string allowedUpgradeItems;

	// Token: 0x040021B6 RID: 8630
	[PublicizedFrom(EAccessModifier.Private)]
	public string restrictedUpgradeItems;

	// Token: 0x02000537 RID: 1335
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum EnumRepairType
	{
		// Token: 0x040021B8 RID: 8632
		None,
		// Token: 0x040021B9 RID: 8633
		Repair,
		// Token: 0x040021BA RID: 8634
		Upgrade
	}

	// Token: 0x02000538 RID: 1336
	[PublicizedFrom(EAccessModifier.Protected)]
	public class InventoryDataRepair : ItemActionAttackData
	{
		// Token: 0x06002B42 RID: 11074 RVA: 0x0011F523 File Offset: 0x0011D723
		public InventoryDataRepair(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040021BB RID: 8635
		public new bool uiOpenedByMe;

		// Token: 0x040021BC RID: 8636
		public ItemActionRepair.EnumRepairType repairType;

		// Token: 0x040021BD RID: 8637
		public float blockDamagePerc;

		// Token: 0x040021BE RID: 8638
		public bool bUseStarted;

		// Token: 0x040021BF RID: 8639
		public float upgradePerc;

		// Token: 0x040021C0 RID: 8640
		public BlockValue lastHitBlockValue;

		// Token: 0x040021C1 RID: 8641
		public Vector3i lastHitPosition = Vector3i.zero;

		// Token: 0x040021C2 RID: 8642
		public List<Block.SItemNameCount> lastRepairItems;

		// Token: 0x040021C3 RID: 8643
		public float[] lastRepairItemsPercents;
	}
}
