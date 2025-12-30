using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000CAF RID: 3247
[Preserve]
public class XUiC_EquipmentStackGrid : XUiController
{
	// Token: 0x17000A42 RID: 2626
	// (get) Token: 0x06006462 RID: 25698 RVA: 0x00075C39 File Offset: 0x00073E39
	public virtual XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Equipment;
		}
	}

	// Token: 0x06006463 RID: 25699 RVA: 0x0028AFD8 File Offset: 0x002891D8
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_EquipmentStack>(null);
		this.itemControllers = childrenByType;
		this.bAwakeCalled = true;
		this.IsDirty = false;
		XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment;
		base.xui.OnShutdown += this.HandleShutdown;
	}

	// Token: 0x06006464 RID: 25700 RVA: 0x0028B030 File Offset: 0x00289230
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment(XUiM_PlayerEquipment _playerEquipment)
	{
		if (base.xui.PlayerEquipment == _playerEquipment)
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x06006465 RID: 25701 RVA: 0x0028B047 File Offset: 0x00289247
	public void HandleShutdown()
	{
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment;
		base.xui.OnShutdown -= this.HandleShutdown;
	}

	// Token: 0x06006466 RID: 25702 RVA: 0x0028B074 File Offset: 0x00289274
	public void SetEquipmentSlotForStack(EquipmentSlots equipSlot)
	{
		XUiC_EquipmentStack xuiC_EquipmentStack = (XUiC_EquipmentStack)this.itemControllers[(int)equipSlot];
		if (xuiC_EquipmentStack != null)
		{
			xuiC_EquipmentStack.EquipSlot = equipSlot;
			this.equipmentList.Add(xuiC_EquipmentStack);
		}
	}

	// Token: 0x06006467 RID: 25703 RVA: 0x0028B0A8 File Offset: 0x002892A8
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.IsDirty)
		{
			if (!this.slotsSetup)
			{
				this.slotIndexList.Clear();
				this.equipmentList.Clear();
				int num = 0;
				while (num < 8 && num < this.itemControllers.Length)
				{
					XUiC_EquipmentStack xuiC_EquipmentStack = this.itemControllers[num] as XUiC_EquipmentStack;
					if (xuiC_EquipmentStack != null)
					{
						xuiC_EquipmentStack.EquipSlot = (EquipmentSlots)num;
						this.equipmentList.Add(xuiC_EquipmentStack);
					}
					num++;
				}
				if (this.ExtraSlot != null)
				{
					this.equipmentList.Add(this.ExtraSlot);
				}
				if (this.ExtraSlot2 != null)
				{
					this.equipmentList.Add(this.ExtraSlot2);
				}
				if (this.ExtraSlot3 != null)
				{
					this.equipmentList.Add(this.ExtraSlot3);
				}
				if (this.ExtraSlot4 != null)
				{
					this.equipmentList.Add(this.ExtraSlot4);
				}
				this.slotsSetup = true;
			}
			this.items = this.GetSlots();
			this.SetStacks(this.items);
			this.IsDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06006468 RID: 25704 RVA: 0x0028B1C8 File Offset: 0x002893C8
	public virtual ItemValue[] GetSlots()
	{
		Equipment equipment = base.xui.PlayerEquipment.Equipment;
		ItemValue[] array = new ItemValue[this.equipmentList.Count];
		for (int i = 0; i < this.equipmentList.Count; i++)
		{
			ItemValue itemValue = equipment.GetSlotItem(i);
			if (itemValue == null)
			{
				itemValue = ItemValue.None.Clone();
			}
			array[i] = itemValue;
		}
		return array;
	}

	// Token: 0x06006469 RID: 25705 RVA: 0x0028B228 File Offset: 0x00289428
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleBagContentsChanged(ItemValue[] _items)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (base.xui.playerUI.entityPlayer != null)
		{
			this.SetStacks(_items);
		}
	}

	// Token: 0x0600646A RID: 25706 RVA: 0x0028B264 File Offset: 0x00289464
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetStacks(ItemValue[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		XUiC_CharacterFrameWindow childByType2 = base.xui.GetChildByType<XUiC_CharacterFrameWindow>();
		int num = 0;
		while (num < stackList.Length && this.equipmentList.Count > num && stackList.Length > num)
		{
			XUiC_EquipmentStack xuiC_EquipmentStack = this.equipmentList[num];
			xuiC_EquipmentStack.SlotChangedEvent -= this.HandleSlotChangedEvent;
			xuiC_EquipmentStack.ItemValue = stackList[num];
			xuiC_EquipmentStack.SlotChangedEvent += this.HandleSlotChangedEvent;
			xuiC_EquipmentStack.SlotNumber = num;
			xuiC_EquipmentStack.InfoWindow = childByType;
			xuiC_EquipmentStack.FrameWindow = childByType2;
			num++;
		}
	}

	// Token: 0x0600646B RID: 25707 RVA: 0x0028B2FC File Offset: 0x002894FC
	public void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		if (stack.IsEmpty())
		{
			base.xui.PlayerEquipment.Equipment.SetSlotItem(slotNumber, null, true);
			base.xui.PlayerEquipment.RefreshEquipment();
			return;
		}
		this.items[slotNumber] = stack.itemValue.Clone();
		base.xui.PlayerEquipment.Equipment.SetSlotItem(slotNumber, stack.itemValue, true);
		base.xui.PlayerEquipment.RefreshEquipment();
		QuestEventManager.Current.WoreItem(stack.itemValue);
	}

	// Token: 0x0600646C RID: 25708 RVA: 0x0028B38A File Offset: 0x0028958A
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.IsDirty = true;
		this.IsDormant = false;
	}

	// Token: 0x0600646D RID: 25709 RVA: 0x0028B3BC File Offset: 0x002895BC
	public override void OnClose()
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			this.itemControllers[i].Hovered(false);
		}
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
		this.IsDormant = true;
	}

	// Token: 0x04004BAE RID: 19374
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController[] itemControllers;

	// Token: 0x04004BAF RID: 19375
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemValue[] items;

	// Token: 0x04004BB0 RID: 19376
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> slotIndexList = new List<int>();

	// Token: 0x04004BB1 RID: 19377
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_EquipmentStack> equipmentList = new List<XUiC_EquipmentStack>();

	// Token: 0x04004BB2 RID: 19378
	[PublicizedFrom(EAccessModifier.Private)]
	public bool slotsSetup;

	// Token: 0x04004BB3 RID: 19379
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAwakeCalled;

	// Token: 0x04004BB4 RID: 19380
	public XUiC_EquipmentStack ExtraSlot;

	// Token: 0x04004BB5 RID: 19381
	public XUiC_EquipmentStack ExtraSlot2;

	// Token: 0x04004BB6 RID: 19382
	public XUiC_EquipmentStack ExtraSlot3;

	// Token: 0x04004BB7 RID: 19383
	public XUiC_EquipmentStack ExtraSlot4;

	// Token: 0x02000CB0 RID: 3248
	public enum UIEquipmentSlots
	{
		// Token: 0x04004BB9 RID: 19385
		Headgear,
		// Token: 0x04004BBA RID: 19386
		Eyewear,
		// Token: 0x04004BBB RID: 19387
		Face,
		// Token: 0x04004BBC RID: 19388
		Shirt,
		// Token: 0x04004BBD RID: 19389
		Jacket,
		// Token: 0x04004BBE RID: 19390
		ChestArmor,
		// Token: 0x04004BBF RID: 19391
		Gloves,
		// Token: 0x04004BC0 RID: 19392
		Backpack,
		// Token: 0x04004BC1 RID: 19393
		Pants,
		// Token: 0x04004BC2 RID: 19394
		Footwear,
		// Token: 0x04004BC3 RID: 19395
		LegArmor
	}
}
