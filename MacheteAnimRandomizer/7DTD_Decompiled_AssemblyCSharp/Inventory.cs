using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using UnityEngine;

// Token: 0x020004EE RID: 1262
public class Inventory
{
	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x060028BF RID: 10431 RVA: 0x000768A9 File Offset: 0x00074AA9
	public int PUBLIC_SLOTS_PLAYMODE
	{
		get
		{
			return 10;
		}
	}

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x060028C0 RID: 10432 RVA: 0x0010ABE7 File Offset: 0x00108DE7
	public int PUBLIC_SLOTS_PREFABEDITOR
	{
		get
		{
			return 2 * this.PUBLIC_SLOTS_PLAYMODE;
		}
	}

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x060028C1 RID: 10433 RVA: 0x000768A9 File Offset: 0x00074AA9
	public int SHIFT_KEY_SLOT_OFFSET
	{
		get
		{
			return 10;
		}
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x060028C2 RID: 10434 RVA: 0x0010ABF1 File Offset: 0x00108DF1
	public int PUBLIC_SLOTS
	{
		get
		{
			if (PrefabEditModeManager.Instance == null || !PrefabEditModeManager.Instance.IsActive())
			{
				return this.PUBLIC_SLOTS_PLAYMODE;
			}
			return this.PUBLIC_SLOTS_PREFABEDITOR;
		}
	}

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x060028C3 RID: 10435 RVA: 0x0010AC13 File Offset: 0x00108E13
	public int INVENTORY_SLOTS
	{
		get
		{
			return this.PUBLIC_SLOTS + 1;
		}
	}

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x060028C4 RID: 10436 RVA: 0x0010AC1D File Offset: 0x00108E1D
	public int DUMMY_SLOT_IDX
	{
		get
		{
			return this.INVENTORY_SLOTS - 1;
		}
	}

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x060028C5 RID: 10437 RVA: 0x0010AC28 File Offset: 0x00108E28
	// (remove) Token: 0x060028C6 RID: 10438 RVA: 0x0010AC60 File Offset: 0x00108E60
	public event XUiEvent_ToolbeltItemsChangedInternal OnToolbeltItemsChangedInternal;

	// Token: 0x060028C7 RID: 10439 RVA: 0x0010AC98 File Offset: 0x00108E98
	public Inventory(IGameManager _gameManager, EntityAlive _entity)
	{
		this.m_LastDrawnHoldingItemIndex = this.DUMMY_SLOT_IDX;
		this.m_HoldingItemIdx = this.DUMMY_SLOT_IDX;
		this.preferredItemSlots = new int[this.PUBLIC_SLOTS];
		this.models = new Transform[this.INVENTORY_SLOTS];
		this.slots = new ItemInventoryData[this.INVENTORY_SLOTS];
		this.entity = _entity;
		this.gameManager = _gameManager;
		this.emptyItem = new ItemInventoryData(null, ItemStack.Empty.Clone(), _gameManager, _entity, 0);
		this.Clear();
		this.m_HoldingItemIdx = 0;
		this.m_LastDrawnHoldingItemIndex = -1;
		this.previousHeldItemValue = null;
		this.previousHeldItemSlotIdx = -1;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x0010AD6C File Offset: 0x00108F6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitInactiveItemsObject()
	{
		if (!this.inactiveItems)
		{
			GameObject gameObject = new GameObject("InactiveItems");
			gameObject.SetActive(false);
			this.inactiveItems = gameObject.transform;
			this.inactiveItems.SetParent(this.entity.transform, false);
		}
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x0010ADBB File Offset: 0x00108FBB
	[PublicizedFrom(EAccessModifier.Private)]
	public void onInventoryChanged()
	{
		if (this.OnToolbeltItemsChangedInternal != null)
		{
			this.OnToolbeltItemsChangedInternal();
		}
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x0010ADD0 File Offset: 0x00108FD0
	public void CallOnToolbeltChangedInternal()
	{
		this.onInventoryChanged();
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x0010ADD8 File Offset: 0x00108FD8
	public virtual ItemClass GetBareHandItem()
	{
		return this.bareHandItem;
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x0010ADE0 File Offset: 0x00108FE0
	public virtual ItemValue GetBareHandItemValue()
	{
		return this.bareHandItemValue;
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x0010ADE8 File Offset: 0x00108FE8
	public virtual void SetBareHandItem(ItemValue _bareHandItemValue)
	{
		this.bareHandItemValue = _bareHandItemValue;
		this.bareHandItem = ItemClass.GetForId(this.bareHandItemValue.type);
		this.bareHandItemInventoryData = this.bareHandItem.CreateInventoryData(new ItemStack(_bareHandItemValue, 1), this.gameManager, this.entity, 0);
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x0010AE38 File Offset: 0x00109038
	public virtual void SetSlots(ItemStack[] _slots, bool _allowSettingDummySlot = true)
	{
		int num = 0;
		while (num < _slots.Length && num < (_allowSettingDummySlot ? this.INVENTORY_SLOTS : this.PUBLIC_SLOTS))
		{
			this.SetItem(num, _slots[num].itemValue, _slots[num].count, false);
			num++;
		}
		this.notifyListeners();
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x0010AE85 File Offset: 0x00109085
	public virtual ItemActionData GetItemActionDataInSlot(int _slotIdx, int _actionIdx)
	{
		if (_slotIdx == this.holdingItemIdx)
		{
			return this.holdingItemData.actionData[_actionIdx];
		}
		return this.slots[_slotIdx].actionData[_actionIdx];
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x0010AEB8 File Offset: 0x001090B8
	public virtual ItemAction GetItemActionInSlot(int _slotIdx, int _actionIdx)
	{
		if (_slotIdx == this.holdingItemIdx)
		{
			return this.holdingItem.Actions[_actionIdx];
		}
		if (this.slots[_slotIdx] == null || this.slots[_slotIdx].item == null)
		{
			return null;
		}
		return this.slots[_slotIdx].item.Actions[_actionIdx];
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x0010AF0B File Offset: 0x0010910B
	public virtual ItemClass GetItemInSlot(int _idx)
	{
		if (this.slots[_idx].item == null)
		{
			return this.bareHandItem;
		}
		return this.slots[_idx].item;
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x0010AF30 File Offset: 0x00109130
	public ItemInventoryData GetItemDataInSlot(int _idx)
	{
		if (this.slots[_idx].item == null)
		{
			return this.bareHandItemInventoryData;
		}
		return this.slots[_idx];
	}

	// Token: 0x17000437 RID: 1079
	public virtual ItemValue this[int _idx]
	{
		get
		{
			return this.slots[_idx].itemStack.itemValue;
		}
		set
		{
			this.slots[_idx].itemStack.itemValue = value;
			this.notifyListeners();
		}
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x0010AF80 File Offset: 0x00109180
	public void ModifyValue(ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags)
	{
		if (this.holdingItemItemValue != null && !this.holdingItemItemValue.Equals(_originalItemValue) && !this.holdingItemItemValue.ItemClass.ItemTags.Test_AnySet(this.ignoreWhenHeld))
		{
			this.holdingItemItemValue.ModifyValue(this.entity, _originalItemValue, _passiveEffect, ref _base_val, ref _perc_val, tags, true, false);
		}
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x0010AFDC File Offset: 0x001091DC
	public void OnUpdate()
	{
		if (!this.isSwitchingHeldItem && !this.entity.IsDead() && this.entity.IsSpawned())
		{
			this.holdingItem.OnHoldingUpdate(this.holdingItemData);
			ItemValue itemValue = this.holdingItemData.itemValue;
			this.entity.MinEventContext.ItemValue = itemValue;
			itemValue.FireEvent(MinEventTypes.onSelfEquipUpdate, this.entity.MinEventContext);
		}
		if (this.entity is EntityPlayer)
		{
			if (this.holdingCount <= 0)
			{
				this.clearSlotByIndex(this.m_HoldingItemIdx);
			}
			if (this.entity.emodel && this.entity.emodel.avatarController)
			{
				this.entity.emodel.avatarController.CancelEvent(AvatarController.itemHasChangedTriggerHash);
			}
		}
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x0010B0B4 File Offset: 0x001092B4
	public void ShowHeldItemOld(bool show, float waitTime = 0.015f)
	{
		if (show)
		{
			this.entity.MinEventContext.ItemValue = this.holdingItemItemValue;
			EntityPlayerLocal entityPlayerLocal = this.entity as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.HolsterWeapon(false);
			}
		}
		this.HoldingItemHasChanged();
		GameManager.Instance.StartCoroutine(this.delayedShowHideHeldItem(show, waitTime));
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x0010B10F File Offset: 0x0010930F
	public void ShowHeldItem(float waitTime = 0.015f, bool hideFirst = false)
	{
		GameManager.Instance.StartCoroutine(this.delayedShowHideHeldItem(hideFirst, waitTime));
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x0010B124 File Offset: 0x00109324
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator delayedShowHideHeldItem(bool hideFirst, float waitTime)
	{
		EntityPlayerLocal epl = this.entity as EntityPlayerLocal;
		if (epl != null)
		{
			while (epl.emodel.avatarController.IsAnimationPlayerFPRevivePlaying())
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (hideFirst)
			{
				epl.HolsterWeapon(true);
				this.entity.StopOneShot(this.holdingItem.SoundUnholster);
				this.entity.PlayOneShot(this.holdingItem.SoundHolster, false, false, false, null);
			}
			if (waitTime < 0.15f)
			{
				waitTime = 0.15f;
			}
			yield return new WaitForSeconds(waitTime);
			this.entity.MinEventContext.ItemValue = this.holdingItemItemValue;
			this.HoldingItemHasChanged();
			this.entity.StopOneShot(this.holdingItem.SoundHolster);
			this.entity.PlayOneShot(this.holdingItem.SoundUnholster, false, false, false, null);
			if (this.entity.emodel && this.entity.emodel.avatarController)
			{
				this.entity.emodel.avatarController.TriggerEvent(AvatarController.itemHasChangedTriggerHash);
			}
			this.updateHoldingItem();
			epl.HolsterWeapon(false);
			if (epl.bFirstPersonView)
			{
				epl.ShowHoldingItemLayer(true);
			}
			this.ShowRightHand(true);
		}
		this.SetIsFinishedSwitchingHeldItem();
		ItemValue currentItemValue = this.holdingItemItemValue;
		yield return new WaitForSeconds(Mathf.Max(waitTime, 0.3f));
		if (currentItemValue == this.holdingItemItemValue && this.previousHeldItemValue != this.holdingItemItemValue && !this.holdingItem.Equals(this.bareHandItem))
		{
			this.quickSwapItemValue = this.previousHeldItemValue;
			this.quickSwapSlotIdx = this.previousHeldItemSlotIdx;
			this.previousHeldItemValue = this.holdingItemItemValue;
			this.previousHeldItemSlotIdx = this.holdingItemIdx;
		}
		yield break;
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x0010B144 File Offset: 0x00109344
	public void ShowRightHand(bool _show)
	{
		if (this.entity.emodel && this.entity.emodel.avatarController)
		{
			Transform rightHandTransform = this.entity.emodel.avatarController.GetRightHandTransform();
			if (rightHandTransform)
			{
				rightHandTransform.gameObject.SetActive(_show);
			}
		}
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x0010B1A4 File Offset: 0x001093A4
	public void SetRightHandAsModel()
	{
		if (this.entity.emodel && this.entity.emodel.avatarController)
		{
			this.models[this.m_HoldingItemIdx] = this.entity.emodel.avatarController.GetRightHandTransform();
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x060028DC RID: 10460 RVA: 0x0010B1FC File Offset: 0x001093FC
	public virtual ItemValue holdingItemItemValue
	{
		get
		{
			ItemValue itemValue = this.slots[this.m_HoldingItemIdx].itemStack.itemValue;
			if (!itemValue.IsEmpty())
			{
				return itemValue;
			}
			return this.bareHandItemValue;
		}
	}

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x060028DD RID: 10461 RVA: 0x0010B234 File Offset: 0x00109434
	public virtual ItemStack holdingItemStack
	{
		get
		{
			ItemStack itemStack = this.slots[this.m_HoldingItemIdx].itemStack;
			if (!itemStack.IsEmpty())
			{
				return itemStack;
			}
			return new ItemStack(this.bareHandItemValue, 0);
		}
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x060028DE RID: 10462 RVA: 0x0010B26C File Offset: 0x0010946C
	public virtual ItemClass holdingItem
	{
		get
		{
			ItemValue itemValue = this.slots[this.m_HoldingItemIdx].itemStack.itemValue;
			if (!itemValue.IsEmpty() && ItemClass.list != null)
			{
				return ItemClass.GetForId(itemValue.type);
			}
			return this.bareHandItem;
		}
	}

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x060028DF RID: 10463 RVA: 0x0010B2B4 File Offset: 0x001094B4
	public virtual int holdingCount
	{
		get
		{
			ItemStack itemStack = this.slots[this.m_HoldingItemIdx].itemStack;
			if (!itemStack.itemValue.IsEmpty())
			{
				return itemStack.count;
			}
			return 0;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x060028E0 RID: 10464 RVA: 0x0010B2EC File Offset: 0x001094EC
	public virtual ItemInventoryData holdingItemData
	{
		get
		{
			if (!this.slots[this.m_HoldingItemIdx].itemStack.itemValue.IsEmpty())
			{
				return this.slots[this.m_HoldingItemIdx];
			}
			this.bareHandItemInventoryData.slotIdx = this.holdingItemIdx;
			return this.bareHandItemInventoryData;
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x060028E1 RID: 10465 RVA: 0x0010B33C File Offset: 0x0010953C
	public virtual int holdingItemIdx
	{
		get
		{
			return this.m_HoldingItemIdx;
		}
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x0010B344 File Offset: 0x00109544
	public virtual bool IsHolsterDelayActive()
	{
		return this.isSwitchingHeldItem;
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x0010B344 File Offset: 0x00109544
	public virtual bool IsUnholsterDelayActive()
	{
		return this.isSwitchingHeldItem;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x0010B34C File Offset: 0x0010954C
	public void SetIsFinishedSwitchingHeldItem()
	{
		this.isSwitchingHeldItem = false;
		this.entity.MinEventContext.Self = this.entity;
		this.entity.MinEventContext.ItemValue = this.holdingItemItemValue;
		this.HoldingItemHasChanged();
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x0010B388 File Offset: 0x00109588
	[PublicizedFrom(EAccessModifier.Private)]
	public void syncHeldItem()
	{
		this.entity.IsEquipping = true;
		if (this.holdingItemItemValue == null || this.holdingItemItemValue.IsEmpty())
		{
			return;
		}
		if (this.holdingItemItemValue.ItemClass == null)
		{
			if (ItemClass.list == null)
			{
				Log.Out("[Inventory:syncHeldItem] Cannot find item class for held item id '{0}'. Item list is null!", new object[]
				{
					this.holdingItemItemValue.type
				});
			}
			else
			{
				Log.Out("[Inventory:syncHeldItem] Cannot find item class for held item id '{0}'. Item id is out of range! ItemClass list length '{1}'", new object[]
				{
					this.holdingItemItemValue.type,
					ItemClass.list.Length
				});
			}
		}
		else if (!this.holdingItemItemValue.ItemClass.ItemTags.Test_AnySet(this.ignoreWhenHeld))
		{
			this.entity.MinEventContext.ItemValue = this.holdingItemItemValue;
			if (this.holdingItemItemValue.ItemClass != null && this.holdingItemItemValue.ItemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
			{
				if (this.holdingItemItemValue.Activated == 1)
				{
					this.holdingItemItemValue.FireEvent(MinEventTypes.onSelfItemActivate, this.entity.MinEventContext);
				}
				else
				{
					this.holdingItemItemValue.FireEvent(MinEventTypes.onSelfItemDeactivate, this.entity.MinEventContext);
				}
			}
			if (this.holdingItemItemValue.Modifications.Length != 0)
			{
				ItemValue itemValue = this.entity.MinEventContext.ItemValue;
				for (int i = 0; i < this.holdingItemItemValue.Modifications.Length; i++)
				{
					ItemValue itemValue2 = this.holdingItemItemValue.Modifications[i];
					if (itemValue2 != null && itemValue2.ItemClass != null)
					{
						ItemClass itemClass = itemValue2.ItemClass;
						this.entity.MinEventContext.ItemValue = itemValue2;
						if (itemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
						{
							if (itemValue2.Activated == 1)
							{
								itemValue2.FireEvent(MinEventTypes.onSelfItemActivate, this.entity.MinEventContext);
							}
							else
							{
								itemValue2.FireEvent(MinEventTypes.onSelfItemDeactivate, this.entity.MinEventContext);
							}
						}
					}
				}
				this.entity.MinEventContext.ItemValue = itemValue;
			}
		}
		this.CallOnToolbeltChangedInternal();
		this.entity.IsEquipping = false;
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x0010B588 File Offset: 0x00109788
	public bool GetIsFinishedSwitchingHeldItem()
	{
		return !this.isSwitchingHeldItem;
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x0010B593 File Offset: 0x00109793
	public virtual int GetFocusedItemIdx()
	{
		return this.m_FocusedItemIdx;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x0010B59B File Offset: 0x0010979B
	public virtual int SetFocusedItemIdx(int _idx)
	{
		while (_idx < 0)
		{
			_idx += this.PUBLIC_SLOTS;
		}
		while (_idx >= this.PUBLIC_SLOTS)
		{
			_idx -= this.PUBLIC_SLOTS;
		}
		this.m_FocusedItemIdx = _idx;
		return _idx;
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x0010B5CA File Offset: 0x001097CA
	public virtual bool IsHoldingItemActionRunning()
	{
		return this.holdingItem.IsActionRunning(this.holdingItemData);
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x0010B5DD File Offset: 0x001097DD
	public virtual void SetHoldingItemIdxNoHolsterTime(int _inventoryIdx)
	{
		this.setHeldItemByIndex(_inventoryIdx, false);
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x0010B5E7 File Offset: 0x001097E7
	public virtual void SetHoldingItemIdx(int _inventoryIdx)
	{
		this.setHeldItemByIndex(_inventoryIdx, true);
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x0010B5F1 File Offset: 0x001097F1
	public void BeginSwapHoldingItem()
	{
		this.isSwitchingHeldItem = true;
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x0010B5FC File Offset: 0x001097FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void setHeldItemByIndex(int _inventoryIdx, bool _applyHolsterTime)
	{
		this.BeginSwapHoldingItem();
		while (_inventoryIdx < 0)
		{
			_inventoryIdx += this.slots.Length;
		}
		while (_inventoryIdx >= this.slots.Length)
		{
			_inventoryIdx -= this.slots.Length;
		}
		bool flag = this.flashlightOn && this.IsHoldingFlashlight;
		this.HoldingItemHasChanged();
		if (this.entity != null && this.entity.emodel != null && this.entity.emodel.avatarController != null)
		{
			this.entity.emodel.avatarController.TriggerEvent(AvatarController.itemHasChangedTriggerHash);
		}
		for (int i = 0; i < this.holdingItem.Actions.Length; i++)
		{
			if (this.holdingItem.Actions[i] is ItemActionAttack)
			{
				Manager.BroadcastStop(this.entity.entityId, this.holdingItem.Actions[i].GetSoundStart());
			}
		}
		this.m_HoldingItemIdx = _inventoryIdx;
		this.m_FocusedItemIdx = _inventoryIdx;
		if (this.entity.isEntityRemote)
		{
			this.updateHoldingItem();
			return;
		}
		this.ShowHeldItem((!_applyHolsterTime) ? 0f : 0.2f, true);
		if (flag)
		{
			bool flag2 = this.SetFlashlight(false);
			this.currActiveItemIndex = -1;
			if (flag2)
			{
				this.entity.PlayOneShot("flashlight_toggle", false, false, false, null);
			}
		}
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x0010B754 File Offset: 0x00109954
	public void HoldingItemHasChanged()
	{
		if (this.entity != null && this.entity.emodel != null && this.entity.emodel.avatarController != null)
		{
			this.entity.emodel.avatarController.CancelEvent("WeaponFire");
			this.entity.emodel.avatarController.CancelEvent("PowerAttack");
			this.entity.emodel.avatarController.CancelEvent("UseItem");
			this.entity.emodel.avatarController.CancelEvent("ItemUse");
			this.entity.emodel.avatarController.UpdateBool("Reload", false, true);
		}
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x0010B827 File Offset: 0x00109A27
	public virtual bool IsHoldingGun()
	{
		return this.holdingItem != null && this.holdingItem.IsGun();
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x0010B83E File Offset: 0x00109A3E
	public virtual bool IsHoldingDynamicMelee()
	{
		return this.holdingItem != null && this.holdingItem.IsDynamicMelee();
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x0010B855 File Offset: 0x00109A55
	public virtual bool IsHoldingBlock()
	{
		return this.holdingItem != null && this.holdingItem.IsBlock();
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x0010B86C File Offset: 0x00109A6C
	public virtual ItemAction GetHoldingPrimary()
	{
		return this.holdingItem.Actions[0];
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x0010B87B File Offset: 0x00109A7B
	public virtual ItemAction GetHoldingSecondary()
	{
		return this.holdingItem.Actions[1];
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x0010B88A File Offset: 0x00109A8A
	public virtual ItemActionAttack GetHoldingGun()
	{
		return this.holdingItem.Actions[0] as ItemActionAttack;
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x0010B89E File Offset: 0x00109A9E
	public virtual ItemActionDynamic GetHoldingDynamicMelee()
	{
		return this.holdingItem.Actions[0] as ItemActionDynamic;
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x0010B8B2 File Offset: 0x00109AB2
	public virtual ItemClassBlock GetHoldingBlock()
	{
		return this.holdingItem as ItemClassBlock;
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x0010B8BF File Offset: 0x00109ABF
	public Transform GetHoldingItemTransform()
	{
		return this.models[this.m_HoldingItemIdx];
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x0010B8D0 File Offset: 0x00109AD0
	public virtual int GetItemCount(ItemValue _itemValue, bool _bConsiderTexture = false, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		int num = 0;
		for (int i = 0; i < this.slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !this.slots[i].itemValue.HasModSlots || !this.slots[i].itemValue.HasMods()) && this.slots[i].itemStack.itemValue.type == _itemValue.type && (!_bConsiderTexture || this.slots[i].itemStack.itemValue.TextureFullArray == _itemValue.TextureFullArray) && (_seed == -1 || _seed == (int)this.slots[i].itemValue.Seed) && (_meta == -1 || _meta == this.slots[i].itemValue.Meta))
			{
				num += this.slots[i].itemStack.count;
			}
		}
		return num;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x0010B9B8 File Offset: 0x00109BB8
	public virtual int GetItemCount(FastTags<TagGroup.Global> itemTags, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		int num = 0;
		for (int i = 0; i < this.slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !this.slots[i].itemValue.HasModSlots || !this.slots[i].itemValue.HasMods()) && !this.slots[i].itemValue.IsEmpty() && this.slots[i].itemValue.ItemClass.ItemTags.Test_AnySet(itemTags) && (_seed == -1 || _seed == (int)this.slots[i].itemValue.Seed) && (_meta == -1 || _meta == this.slots[i].itemValue.Meta))
			{
				num += this.slots[i].itemStack.count;
			}
		}
		return num;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x0010BA88 File Offset: 0x00109C88
	public virtual bool AddItem(ItemStack _itemStack)
	{
		int num;
		return this.AddItem(_itemStack, out num);
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x0010BAA0 File Offset: 0x00109CA0
	public bool AddItem(ItemStack _itemStack, out int _slot)
	{
		for (int i = 0; i < this.slots.Length - 1; i++)
		{
			if (this.slots[i].itemStack.itemValue.type == _itemStack.itemValue.type && this.slots[i].itemStack.CanStackWith(_itemStack, false))
			{
				this.slots[i].itemStack.count += _itemStack.count;
				this.notifyListeners();
				this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
				_slot = i;
				return true;
			}
		}
		for (int j = 0; j < this.slots.Length - 1; j++)
		{
			if (this.slots[j].itemStack.IsEmpty())
			{
				this.SetItem(j, _itemStack.itemValue, _itemStack.count, true);
				this.notifyListeners();
				this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
				_slot = j;
				return true;
			}
		}
		_slot = -1;
		return false;
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x0010BBAC File Offset: 0x00109DAC
	public virtual bool ReturnItem(ItemStack _itemStack)
	{
		for (int i = 0; i < this.PUBLIC_SLOTS; i++)
		{
			i = this.PreferredItemSlot(_itemStack.itemValue.type, i);
			if (i < 0 || i >= this.PUBLIC_SLOTS)
			{
				return false;
			}
			if (this.AddItemAtSlot(_itemStack, i))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x0010BBFC File Offset: 0x00109DFC
	public bool AddItemAtSlot(ItemStack _itemStack, int _slot)
	{
		bool flag = false;
		if (_slot >= 0 && _slot < this.PUBLIC_SLOTS)
		{
			if (this.slots[_slot].itemStack.itemValue.type == _itemStack.itemValue.type && this.slots[_slot].itemStack.CanStackWith(_itemStack, false))
			{
				this.slots[_slot].itemStack.count += _itemStack.count;
				flag = true;
			}
			if (this.slots[_slot].itemStack.IsEmpty())
			{
				this.SetItem(_slot, _itemStack.itemValue, _itemStack.count, true);
				flag = true;
			}
		}
		if (flag)
		{
			this.notifyListeners();
			this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
			if (_slot == this.m_HoldingItemIdx)
			{
				this.HoldingItemHasChanged();
			}
		}
		return flag;
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x0010BCD8 File Offset: 0x00109ED8
	public virtual int DecItem(ItemValue _itemValue, int _count, bool _ignoreModdedItems = false, IList<ItemStack> _removedItems = null)
	{
		int num = _count;
		int num2 = 0;
		while (_count > 0 && num2 < this.slots.Length - 1)
		{
			if (this.slots[num2].itemStack.itemValue.type == _itemValue.type && (!_ignoreModdedItems || !this.slots[num2].itemValue.HasModSlots || !this.slots[num2].itemValue.HasMods()))
			{
				if (ItemClass.GetForId(this.slots[num2].itemStack.itemValue.type).CanStack())
				{
					int count = this.slots[num2].itemStack.count;
					int num3 = (count >= _count) ? _count : count;
					if (_removedItems != null)
					{
						_removedItems.Add(new ItemStack(this.slots[num2].itemStack.itemValue.Clone(), num3));
					}
					this.slots[num2].itemStack.count -= num3;
					_count -= num3;
					if (this.slots[num2].itemStack.count <= 0)
					{
						this.clearSlotByIndex(num2);
					}
				}
				else
				{
					if (_removedItems != null)
					{
						_removedItems.Add(this.slots[num2].itemStack.Clone());
					}
					this.clearSlotByIndex(num2);
					_count--;
				}
			}
			num2++;
		}
		this.notifyListeners();
		return num - _count;
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x0010BE30 File Offset: 0x0010A030
	[PublicizedFrom(EAccessModifier.Private)]
	public void clearSlotByIndex(int _idx)
	{
		if (!this.slots[_idx].itemStack.itemValue.IsEmpty())
		{
			this.slots[_idx].itemStack = ItemStack.Empty.Clone();
		}
		Transform transform = this.models[_idx];
		if (transform)
		{
			this.HoldingItemHasChanged();
			transform.SetParent(null, false);
			transform.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(transform.gameObject);
			this.models[_idx] = null;
		}
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x0010BEAC File Offset: 0x0010A0AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual Transform createHeldItem(int _idx, ItemValue _itemValue)
	{
		this.InitInactiveItemsObject();
		Transform transform = _itemValue.ItemClass.CloneModel(this.entity.world, _itemValue, this.entity.GetPosition(), this.inactiveItems, BlockShape.MeshPurpose.Hold, default(TextureFullArray));
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
		return transform;
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x0010BF08 File Offset: 0x0010A108
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual ItemInventoryData createInventoryData(int _idx, ItemValue _itemValue)
	{
		return ItemClass.GetForId(_itemValue.type).CreateInventoryData(ItemStack.Empty.Clone(), this.gameManager, this.entity, _idx);
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x0010BF31 File Offset: 0x0010A131
	public virtual void SetItem(int _idx, ItemStack _itemStack)
	{
		this.SetItem(_idx, _itemStack.itemValue, _itemStack.count, true);
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x0010BF48 File Offset: 0x0010A148
	public virtual void SetItem(int _idx, ItemValue _itemValue, int _count, bool _notifyListeners = true)
	{
		if (_idx == this.holdingItemIdx && !_itemValue.EqualsExceptUseTimesAndAmmo(this.slots[_idx].itemStack.itemValue))
		{
			this.ShowHeldItem(0.2f, true);
		}
		if ((ulong)_idx >= (ulong)((long)this.slots.Length))
		{
			return;
		}
		if (_itemValue.type != 0 && _itemValue.ItemClass == null)
		{
			Log.Warning("Inventory slot {0} {1} missing item class", new object[]
			{
				_idx,
				_itemValue.type
			});
			_itemValue.Clear();
		}
		bool flag = false;
		if (_idx < this.preferredItemSlots.Length)
		{
			if (_itemValue.type != 0 && _count != 0)
			{
				this.preferredItemSlots[_idx] = _itemValue.type;
			}
			else if (this.slots[_idx].itemStack.itemValue.type != 0)
			{
				this.preferredItemSlots[_idx] = this.slots[_idx].itemStack.itemValue.type;
			}
		}
		if (_count == 0)
		{
			_itemValue.Clear();
		}
		ItemClass itemClass = this.slots[_idx].itemStack.itemValue.ItemClass;
		ItemClass itemClass2 = _itemValue.ItemClass;
		if (itemClass == null || itemClass != itemClass2)
		{
			this.clearSlotByIndex(_idx);
			if (_itemValue.ItemClass != null)
			{
				this.models[_idx] = (_itemValue.ItemClass.CanHold() ? this.createHeldItem(_idx, _itemValue) : null);
				this.slots[_idx] = this.createInventoryData(_idx, _itemValue);
			}
			flag = true;
		}
		this.slots[_idx].itemStack.itemValue = _itemValue.Clone();
		this.slots[_idx].itemStack.count = _count;
		if (flag && _idx == this.holdingItemIdx)
		{
			this.updateHoldingItem();
		}
		if (_notifyListeners)
		{
			this.notifyListeners();
		}
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x0010C0E8 File Offset: 0x0010A2E8
	public void FireEvent(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		ItemValue itemValue = _eventParms.ItemValue;
		if (itemValue == null)
		{
			itemValue = this.holdingItemItemValue;
		}
		itemValue.FireEvent(_eventType, _eventParms);
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x0010C10E File Offset: 0x0010A30E
	public virtual ItemStack GetItem(int _idx)
	{
		return this.slots[_idx].itemStack;
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x0010C11D File Offset: 0x0010A31D
	public virtual int GetItemCount()
	{
		return this.slots.Length;
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x0010C128 File Offset: 0x0010A328
	public virtual bool DecHoldingItem(int _count)
	{
		bool flag = true;
		if (ItemClass.GetForId(this.holdingItemItemValue.type).CanStack())
		{
			this.slots[this.m_HoldingItemIdx].itemStack.count -= _count;
			flag = (this.slots[this.m_HoldingItemIdx].itemStack.count <= 0);
		}
		if (flag)
		{
			this.HandleTurningOffHoldingFlashlight();
			this.clearSlotByIndex(this.m_HoldingItemIdx);
		}
		this.updateHoldingItem();
		this.notifyListeners();
		return true;
	}

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06002908 RID: 10504 RVA: 0x0010C1AD File Offset: 0x0010A3AD
	public bool IsFlashlightOn
	{
		get
		{
			return this.flashlightOn;
		}
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x0010C1B5 File Offset: 0x0010A3B5
	public bool IsAnItemActive()
	{
		return this.currActiveItemIndex != -1;
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x0010C1C3 File Offset: 0x0010A3C3
	public void SetActiveItemIndexOff()
	{
		this.currActiveItemIndex = -1;
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x0010C1CC File Offset: 0x0010A3CC
	public void ResetActiveIndex()
	{
		this.currActiveItemIndex = -2;
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool CycleActivatableItems()
	{
		return true;
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x00002914 File Offset: 0x00000B14
	public void HandleTurningOffHoldingFlashlight()
	{
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x00002914 File Offset: 0x00000B14
	public void TurnOffLightFlares()
	{
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool SetFlashlight(bool on)
	{
		return false;
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06002910 RID: 10512 RVA: 0x0010C1D8 File Offset: 0x0010A3D8
	public virtual bool IsHoldingFlashlight
	{
		get
		{
			if (this.holdingItem.IsLightSource())
			{
				ItemClass forId = ItemClass.GetForId(this.holdingItemItemValue.type);
				Transform transform = this.models[this.m_HoldingItemIdx].gameObject.transform.Find(forId.ActivateObject.Value);
				if (transform == null && this.models[this.m_HoldingItemIdx].gameObject.name.Equals(forId.ActivateObject.Value))
				{
					transform = this.models[this.m_HoldingItemIdx].gameObject.transform;
				}
				return transform != null && transform.parent.gameObject.activeInHierarchy;
			}
			if (this.holdingItemItemValue.HasQuality)
			{
				for (int i = 0; i < this.holdingItemItemValue.Modifications.Length; i++)
				{
					if (this.holdingItemItemValue.Modifications[i] != null)
					{
						ItemClass itemClass = this.holdingItemItemValue.Modifications[i].ItemClass;
						if (itemClass != null && itemClass.ActivateObject != null)
						{
							Transform transform2 = this.models[this.m_HoldingItemIdx].gameObject.transform.Find(itemClass.ActivateObject.Value);
							if (transform2 == null && this.models[this.m_HoldingItemIdx].gameObject.name.Equals(itemClass.ActivateObject.Value))
							{
								transform2 = this.models[this.m_HoldingItemIdx].gameObject.transform;
							}
							if (transform2 != null && transform2.parent.gameObject.activeInHierarchy)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x0010C398 File Offset: 0x0010A598
	public float GetLightLevel()
	{
		float num = 0f;
		this.activatables.Clear();
		this.entity.CollectActivatableItems(this.activatables);
		for (int i = 0; i < this.activatables.Count; i++)
		{
			ItemValue itemValue = this.activatables[i];
			if (itemValue != null && itemValue.Activated > 0)
			{
				ItemClass itemClass = itemValue.ItemClass;
				if (itemClass != null)
				{
					num += itemClass.lightValue;
				}
			}
		}
		ItemClass holdingItem = this.holdingItem;
		if (holdingItem.AlwaysActive != null && holdingItem.AlwaysActive.Value)
		{
			num += holdingItem.lightValue;
		}
		string propertyOverride = this.holdingItemItemValue.GetPropertyOverride("LightValue", string.Empty);
		if (propertyOverride.Length > 0)
		{
			num += float.Parse(propertyOverride);
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x0010C463 File Offset: 0x0010A663
	public IEnumerator SimulateActionExecution(int _actionIdx, ItemStack _itemStack, Action<ItemStack> onComplete)
	{
		Inventory.<>c__DisplayClass135_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1._itemStack = _itemStack;
		CS$<>8__locals1.onComplete = onComplete;
		while (!this.GetItem(this.DUMMY_SLOT_IDX).IsEmpty())
		{
			yield return null;
		}
		this.SetItem(this.DUMMY_SLOT_IDX, CS$<>8__locals1._itemStack.Clone());
		yield return new WaitForSeconds(0.1f);
		CS$<>8__locals1.previousHoldingIdx = this.m_HoldingItemIdx;
		CS$<>8__locals1.previousFocusedIdx = this.m_FocusedItemIdx;
		this.SetHoldingItemIdx(this.DUMMY_SLOT_IDX);
		yield return new WaitForSeconds(0.1f);
		this.CallOnToolbeltChangedInternal();
		yield return new WaitForSeconds(0.1f);
		while (this.IsHolsterDelayActive())
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.<SimulateActionExecution>g__IsDummySlotActive|135_0(ref CS$<>8__locals1))
		{
			this.<SimulateActionExecution>g__HandleComplete|135_1(ref CS$<>8__locals1);
			yield break;
		}
		this.Execute(_actionIdx, false, null);
		yield return new WaitForSeconds(0.1f);
		if (!this.<SimulateActionExecution>g__IsDummySlotActive|135_0(ref CS$<>8__locals1))
		{
			this.<SimulateActionExecution>g__HandleComplete|135_1(ref CS$<>8__locals1);
			yield break;
		}
		this.Execute(_actionIdx, true, null);
		if (!this.<SimulateActionExecution>g__IsDummySlotActive|135_0(ref CS$<>8__locals1))
		{
			this.<SimulateActionExecution>g__HandleComplete|135_1(ref CS$<>8__locals1);
			yield break;
		}
		CS$<>8__locals1.dummyItem = this.GetItem(this.DUMMY_SLOT_IDX);
		if (CS$<>8__locals1.dummyItem.itemValue.ItemClass != null && CS$<>8__locals1.dummyItem.itemValue.ItemClass.Actions.Length > _actionIdx && CS$<>8__locals1.dummyItem.itemValue.ItemClass.Actions[_actionIdx] != null)
		{
			CS$<>8__locals1.dummyItem.itemValue.ItemClass.Actions[_actionIdx].OnHoldingUpdate(this.GetItemActionDataInSlot(this.DUMMY_SLOT_IDX, _actionIdx));
			while (this.IsHoldingItemActionRunning())
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (!this.<SimulateActionExecution>g__IsDummySlotActive|135_0(ref CS$<>8__locals1))
			{
				this.<SimulateActionExecution>g__HandleComplete|135_1(ref CS$<>8__locals1);
				yield break;
			}
		}
		while (this.IsHolsterDelayActive())
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.<SimulateActionExecution>g__HandleComplete|135_1(ref CS$<>8__locals1);
		yield break;
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x0010C488 File Offset: 0x0010A688
	public void ForceHoldingItemUpdate()
	{
		if (this.models[this.holdingItemIdx] != null)
		{
			UnityEngine.Object.Destroy(this.models[this.holdingItemIdx].gameObject);
		}
		ItemStack holdingItemStack = this.holdingItemStack;
		ItemValue itemValue = holdingItemStack.itemValue.Clone();
		int count = holdingItemStack.count;
		if (itemValue.ItemClass != null)
		{
			this.models[this.holdingItemIdx] = (itemValue.ItemClass.CanHold() ? this.createHeldItem(this.holdingItemIdx, itemValue) : null);
			if (this.slots[this.holdingItemIdx] == null || !(this.slots[this.holdingItemIdx] is ItemClassBlock.ItemBlockInventoryData) || !(itemValue.ItemClass is ItemClassBlock))
			{
				this.slots[this.holdingItemIdx] = this.createInventoryData(this.holdingItemIdx, itemValue);
			}
		}
		this.slots[this.holdingItemIdx].itemStack.itemValue = itemValue;
		this.slots[this.holdingItemIdx].itemStack.count = count;
		this.m_LastDrawnHoldingItemIndex = -1;
		this.updateHoldingItem();
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x0010C594 File Offset: 0x0010A794
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateHoldingItem()
	{
		ItemValue holdingItemItemValue = this.holdingItemItemValue;
		if (this.lastDrawnHoldingItemValue == holdingItemItemValue && this.m_LastDrawnHoldingItemIndex == this.holdingItemIdx)
		{
			this.holdingItem.OnHoldingReset(this.holdingItemData);
			return;
		}
		this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
		if (this.lastdrawnHoldingItem != null)
		{
			this.entity.MinEventContext.ItemValue = this.lastDrawnHoldingItemValue;
			this.entity.MinEventContext.Transform = this.lastdrawnHoldingItemTransform;
			this.lastdrawnHoldingItem.StopHolding(this.lastdrawnHoldingItemData, this.lastdrawnHoldingItemTransform);
			if (!this.lastDrawnHoldingItemValue.ItemClass.ItemTags.Test_AnySet(this.ignoreWhenHeld))
			{
				this.lastDrawnHoldingItemValue.FireEvent(MinEventTypes.onSelfEquipStop, this.entity.MinEventContext);
			}
			if (this.lastdrawnHoldingItemTransform != null)
			{
				this.InitInactiveItemsObject();
				this.lastdrawnHoldingItemTransform.SetParent(this.inactiveItems, false);
				this.lastdrawnHoldingItemTransform.gameObject.SetActive(false);
			}
		}
		QuestEventManager.Current.HeldItem(this.holdingItemData.itemValue);
		this.holdingItem.StartHolding(this.holdingItemData, this.models[this.holdingItemIdx]);
		this.entity.MinEventContext.ItemValue = holdingItemItemValue;
		this.entity.MinEventContext.ItemValue.Seed = holdingItemItemValue.Seed;
		this.entity.MinEventContext.Transform = this.models[this.holdingItemIdx];
		this.setHoldingItemTransform(this.models[this.holdingItemIdx]);
		this.ShowRightHand(true);
		holdingItemItemValue.FireEvent(MinEventTypes.onSelfHoldingItemCreated, this.entity.MinEventContext);
		if (!holdingItemItemValue.ItemClass.ItemTags.Test_AnySet(this.ignoreWhenHeld))
		{
			holdingItemItemValue.FireEvent(MinEventTypes.onSelfEquipStart, this.entity.MinEventContext);
		}
		this.entity.OnHoldingItemChanged();
		this.m_LastDrawnHoldingItemIndex = this.m_HoldingItemIdx;
		this.lastdrawnHoldingItem = this.holdingItem;
		this.lastDrawnHoldingItemValue = holdingItemItemValue;
		this.lastdrawnHoldingItemTransform = this.models[this.holdingItemIdx];
		this.lastdrawnHoldingItemData = this.holdingItemData;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x0010C7C4 File Offset: 0x0010A9C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void setHoldingItemTransform(Transform _t)
	{
		this.entity.SetHoldingItemTransform(_t);
		if (_t != null)
		{
			_t.position = Vector3.zero;
			_t.localPosition = (this.entity.emodel.IsFPV ? Vector3.zero : AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].position);
			_t.eulerAngles = Vector3.zero;
			_t.localEulerAngles = (this.entity.emodel.IsFPV ? Vector3.zero : AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].rotation);
			_t.localEulerAngles = _t.localRotation.eulerAngles;
			if (!this.holdingItem.GetCorrectionScale().Equals(Vector3.zero))
			{
				_t.localScale = this.holdingItem.GetCorrectionScale();
			}
			_t.gameObject.SetActive(!this.holdingItem.HoldingItemHidden);
		}
		this.syncHeldItem();
		this.lastdrawnHoldingItemTransform = _t;
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x0010C8F4 File Offset: 0x0010AAF4
	public int PreferredItemSlot(int _itemType, int _startSlotIdx)
	{
		for (int i = _startSlotIdx; i < this.preferredItemSlots.Length; i++)
		{
			if (this.preferredItemSlots[i] == _itemType)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x0010C922 File Offset: 0x0010AB22
	public void ClearPreferredItemInSlot(int _slotIdx)
	{
		if (_slotIdx < this.preferredItemSlots.Length)
		{
			this.preferredItemSlots[_slotIdx] = 0;
		}
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x0010C938 File Offset: 0x0010AB38
	public bool CanStack(ItemStack _itemStack)
	{
		for (int i = 0; i < this.PUBLIC_SLOTS; i++)
		{
			if (this.slots[i].itemStack.IsEmpty() || this.slots[i].itemStack.CanStackWith(_itemStack, false))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x0010C984 File Offset: 0x0010AB84
	public bool CanStackNoEmpty(ItemStack _itemStack)
	{
		for (int i = 0; i < this.PUBLIC_SLOTS; i++)
		{
			if (this.slots[i].itemStack.CanStackPartlyWith(_itemStack))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x0010C9BC File Offset: 0x0010ABBC
	public bool TryStackItem(int startIndex, ItemStack _itemStack)
	{
		int num = 0;
		for (int i = startIndex; i < this.PUBLIC_SLOTS; i++)
		{
			num = _itemStack.count;
			ItemStack itemStack = this.slots[i].itemStack;
			if (_itemStack.itemValue.type == itemStack.itemValue.type && !itemStack.IsEmpty() && itemStack.CanStackPartly(ref num))
			{
				itemStack.count += num;
				_itemStack.count -= num;
				this.notifyListeners();
				this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
				if (_itemStack.count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x0010CA68 File Offset: 0x0010AC68
	public bool TryTakeItem(ItemStack _itemStack)
	{
		int num = 0;
		for (int i = 0; i < this.PUBLIC_SLOTS; i++)
		{
			num = _itemStack.count;
			ItemStack itemStack = this.slots[i].itemStack;
			if (itemStack.IsEmpty())
			{
				itemStack = _itemStack.Clone();
				this.notifyListeners();
				this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
				return true;
			}
			if (_itemStack.itemValue.type == itemStack.itemValue.type && !itemStack.IsEmpty() && itemStack.CanStackPartly(ref num))
			{
				itemStack.count += num;
				_itemStack.count -= num;
				this.notifyListeners();
				this.entity.bPlayerStatsChanged = !this.entity.isEntityRemote;
				if (_itemStack.count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x0010CB44 File Offset: 0x0010AD44
	public virtual bool CanTakeItem(ItemStack _itemStack)
	{
		for (int i = 0; i < this.slots.Length - 1; i++)
		{
			if (this.slots[i].itemStack.CanStackPartlyWith(_itemStack))
			{
				return true;
			}
			if (this.slots[i].itemStack.IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x0010CB94 File Offset: 0x0010AD94
	public virtual void AddChangeListener(IInventoryChangedListener _listener)
	{
		if (this.listeners == null)
		{
			this.listeners = new HashSet<IInventoryChangedListener>();
		}
		this.listeners.Add(_listener);
		_listener.OnInventoryChanged(this);
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x0010CBBD File Offset: 0x0010ADBD
	public virtual void RemoveChangeListener(IInventoryChangedListener _listener)
	{
		if (this.listeners != null)
		{
			this.listeners.Remove(_listener);
		}
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x0010CBD4 File Offset: 0x0010ADD4
	public void Changed()
	{
		this.notifyListeners();
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x0010CBDC File Offset: 0x0010ADDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void notifyListeners()
	{
		this.onInventoryChanged();
		if (this.listeners == null)
		{
			return;
		}
		foreach (IInventoryChangedListener inventoryChangedListener in this.listeners)
		{
			inventoryChangedListener.OnInventoryChanged(this);
		}
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x0010CC3C File Offset: 0x0010AE3C
	public virtual bool IsHUDDisabled()
	{
		return this.holdingItem.IsHUDDisabled(this.holdingItemData);
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x0010CC50 File Offset: 0x0010AE50
	public virtual ItemStack[] CloneItemStack()
	{
		ItemStack[] array = new ItemStack[this.slots.Length - 1];
		for (int i = 0; i < this.slots.Length - 1; i++)
		{
			array[i] = this.slots[i].itemStack.Clone();
		}
		return array;
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x0010CC98 File Offset: 0x0010AE98
	public string CanInteract()
	{
		return this.holdingItem.CanInteract(this.holdingItemData);
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x0010CCAB File Offset: 0x0010AEAB
	public void Interact()
	{
		this.holdingItem.Interact(this.holdingItemData);
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x0010CCBE File Offset: 0x0010AEBE
	public virtual void Execute(int _actionIdx, bool _bReleased, PlayerActionsLocal _playerActions = null)
	{
		if (this.IsHolsterDelayActive() || this.IsUnholsterDelayActive())
		{
			return;
		}
		if (this.WaitForSecondaryRelease && _actionIdx == 1)
		{
			if (!_bReleased)
			{
				return;
			}
			this.WaitForSecondaryRelease = false;
		}
		this.holdingItem.ExecuteAction(_actionIdx, this.holdingItemData, _bReleased, _playerActions);
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x0010CD00 File Offset: 0x0010AF00
	public void ReleaseAll(PlayerActionsLocal _playerActions = null)
	{
		ItemClass holdingItem = this.holdingItem;
		for (int i = 0; i < holdingItem.Actions.Length; i++)
		{
			this.Execute(i, true, _playerActions);
		}
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x0010CD30 File Offset: 0x0010AF30
	public void Clear()
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i] = this.emptyItem;
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x0010CD5E File Offset: 0x0010AF5E
	public void Cleanup()
	{
		this.entity = null;
		this.listeners = null;
		this.slots = null;
		this.models = null;
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x0010CD7C File Offset: 0x0010AF7C
	public void CleanupHoldingActions()
	{
		if (this.holdingItem != null && this.holdingItemData != null)
		{
			this.holdingItem.CleanupHoldingActions(this.holdingItemData);
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x0010CDA0 File Offset: 0x0010AFA0
	public ItemStack[] GetSlots()
	{
		ItemStack[] array = new ItemStack[this.slots.Length];
		for (int i = 0; i < this.slots.Length; i++)
		{
			array[i] = this.slots[i].itemStack;
		}
		return array;
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x0010C11D File Offset: 0x0010A31D
	public int GetSlotCount()
	{
		return this.slots.Length;
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x0010CDE0 File Offset: 0x0010AFE0
	public void PerformActionOnSlots(Action<ItemStack> _action)
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			_action(this.slots[i].itemStack);
		}
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x0010CE14 File Offset: 0x0010B014
	public int GetSlotWithItemValue(ItemValue _itemValue)
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			if (this.slots[i].itemValue.Equals(_itemValue))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x0010CE4C File Offset: 0x0010B04C
	public List<int> GetSlotsWithBlock(Block _block)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.slots.Length; i++)
		{
			if (this.slots[i].item != null && this.slots[i].item.IsBlock())
			{
				if (_block.CanPickup)
				{
					if (this.slots[i].item.Name.Equals(_block.PickedUpItemValue))
					{
						list.Add(i);
					}
				}
				else if ((this.slots[i].item as ItemClassBlock).GetBlock().Equals(_block))
				{
					list.Add(i);
				}
			}
		}
		return list;
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x0010CEF0 File Offset: 0x0010B0F0
	public int GetBestQuickSwapSlot()
	{
		if (this.quickSwapSlotIdx == -1 || this.quickSwapItemValue == null)
		{
			return -1;
		}
		if (this.slots[this.quickSwapSlotIdx].itemValue.Equals(this.quickSwapItemValue))
		{
			return this.quickSwapSlotIdx;
		}
		for (int i = 0; i < this.slots.Length; i++)
		{
			if (this.slots[i].itemValue.Equals(this.quickSwapItemValue))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x0010CF66 File Offset: 0x0010B166
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <SimulateActionExecution>g__IsDummySlotActive|135_0(ref Inventory.<>c__DisplayClass135_0 A_1)
	{
		return this.m_HoldingItemIdx == this.DUMMY_SLOT_IDX && !this.GetItem(this.DUMMY_SLOT_IDX).IsEmpty();
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x0010CF8C File Offset: 0x0010B18C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <SimulateActionExecution>g__HandleComplete|135_1(ref Inventory.<>c__DisplayClass135_0 A_1)
	{
		A_1.dummyItem = this.GetItem(this.DUMMY_SLOT_IDX);
		ItemStack obj;
		if (object.Equals(A_1._itemStack, A_1.dummyItem))
		{
			obj = A_1._itemStack;
		}
		else
		{
			obj = A_1.dummyItem;
		}
		if (this.m_FocusedItemIdx == this.DUMMY_SLOT_IDX)
		{
			this.SetHoldingItemIdx(A_1.previousHoldingIdx);
			this.SetFocusedItemIdx(A_1.previousFocusedIdx);
		}
		this.SetItem(this.DUMMY_SLOT_IDX, ItemStack.Empty.Clone());
		A_1.onComplete(obj);
	}

	// Token: 0x04001FFB RID: 8187
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly FastTags<TagGroup.Global> ignoreWhenHeld = FastTags<TagGroup.Global>.Parse("clothing,armor");

	// Token: 0x04001FFC RID: 8188
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemInventoryData[] slots;

	// Token: 0x04001FFD RID: 8189
	public Transform[] models;

	// Token: 0x04001FFE RID: 8190
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] preferredItemSlots;

	// Token: 0x04001FFF RID: 8191
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass wearingActiveItem;

	// Token: 0x04002000 RID: 8192
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue wearingActiveItemValue;

	// Token: 0x04002001 RID: 8193
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass lastdrawnHoldingItem;

	// Token: 0x04002002 RID: 8194
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue lastDrawnHoldingItemValue;

	// Token: 0x04002003 RID: 8195
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform lastdrawnHoldingItemTransform;

	// Token: 0x04002004 RID: 8196
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData lastdrawnHoldingItemData;

	// Token: 0x04002005 RID: 8197
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool flashlightOn;

	// Token: 0x04002006 RID: 8198
	public ItemStack itemArmor;

	// Token: 0x04002007 RID: 8199
	public ItemStack itemOnBack;

	// Token: 0x04002008 RID: 8200
	[PublicizedFrom(EAccessModifier.Protected)]
	public int m_HoldingItemIdx;

	// Token: 0x04002009 RID: 8201
	[PublicizedFrom(EAccessModifier.Protected)]
	public int m_LastDrawnHoldingItemIndex;

	// Token: 0x0400200A RID: 8202
	public Transform inactiveItems;

	// Token: 0x0400200B RID: 8203
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive entity;

	// Token: 0x0400200C RID: 8204
	[PublicizedFrom(EAccessModifier.Protected)]
	public IGameManager gameManager;

	// Token: 0x0400200D RID: 8205
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue bareHandItemValue;

	// Token: 0x0400200E RID: 8206
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass bareHandItem;

	// Token: 0x0400200F RID: 8207
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData bareHandItemInventoryData;

	// Token: 0x04002010 RID: 8208
	[PublicizedFrom(EAccessModifier.Protected)]
	public HashSet<IInventoryChangedListener> listeners;

	// Token: 0x04002011 RID: 8209
	[PublicizedFrom(EAccessModifier.Protected)]
	public int m_FocusedItemIdx;

	// Token: 0x04002012 RID: 8210
	public Inventory.HeldItemState HoldState;

	// Token: 0x04002013 RID: 8211
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData emptyItem;

	// Token: 0x04002015 RID: 8213
	public bool WaitForSecondaryRelease;

	// Token: 0x04002016 RID: 8214
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue previousHeldItemValue;

	// Token: 0x04002017 RID: 8215
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousHeldItemSlotIdx;

	// Token: 0x04002018 RID: 8216
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue quickSwapItemValue;

	// Token: 0x04002019 RID: 8217
	[PublicizedFrom(EAccessModifier.Private)]
	public int quickSwapSlotIdx;

	// Token: 0x0400201A RID: 8218
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSwitchingHeldItem;

	// Token: 0x0400201B RID: 8219
	[PublicizedFrom(EAccessModifier.Private)]
	public int currActiveItemIndex = -2;

	// Token: 0x0400201C RID: 8220
	public bool bResetLightLevelWhenChanged = true;

	// Token: 0x0400201D RID: 8221
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemValue> activatables = new List<ItemValue>();

	// Token: 0x020004EF RID: 1263
	public enum HeldItemState
	{
		// Token: 0x0400201F RID: 8223
		None,
		// Token: 0x04002020 RID: 8224
		Unholstering,
		// Token: 0x04002021 RID: 8225
		Holding,
		// Token: 0x04002022 RID: 8226
		Holstering
	}

	// Token: 0x020004F0 RID: 1264
	public enum ActiveIndex
	{
		// Token: 0x04002024 RID: 8228
		NOT_INITIALIZED = -2,
		// Token: 0x04002025 RID: 8229
		ALL_OFF
	}
}
