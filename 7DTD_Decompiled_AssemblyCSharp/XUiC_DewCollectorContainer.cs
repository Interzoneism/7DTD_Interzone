using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C88 RID: 3208
[Preserve]
public class XUiC_DewCollectorContainer : XUiC_ItemStackGrid, ITileEntityChangedListener
{
	// Token: 0x17000A17 RID: 2583
	// (get) Token: 0x060062EB RID: 25323 RVA: 0x00283007 File Offset: 0x00281207
	// (set) Token: 0x060062EC RID: 25324 RVA: 0x0028300F File Offset: 0x0028120F
	public Vector2i GridCellSize { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060062ED RID: 25325 RVA: 0x00283018 File Offset: 0x00281218
	public override ItemStack[] GetSlots()
	{
		return this.localTileEntity.items;
	}

	// Token: 0x060062EE RID: 25326 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x060062EF RID: 25327 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetStacks(ItemStack[] stackList)
	{
	}

	// Token: 0x060062F0 RID: 25328 RVA: 0x00283028 File Offset: 0x00281228
	public void SetSlots(TileEntityDewCollector lootContainer, ItemStack[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		this.localTileEntity = lootContainer;
		this.items = this.localTileEntity.items;
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		XUiV_Grid xuiV_Grid = (XUiV_Grid)this.viewComponent;
		xuiV_Grid.Columns = lootContainer.GetContainerSize().x;
		xuiV_Grid.Rows = lootContainer.GetContainerSize().y;
		int num = stackList.Length;
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			xuiC_ItemStack.InfoWindow = childByType;
			xuiC_ItemStack.SlotNumber = i;
			xuiC_ItemStack.SlotChangedEvent -= this.HandleLootSlotChangedEvent;
			xuiC_ItemStack.InfoWindow = childByType;
			xuiC_ItemStack.OverrideStackCount = 1;
			xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.LootContainer;
			if (i < num)
			{
				this.SetItemInSlot(i, this.localTileEntity.items[i], false);
				this.itemControllers[i].ViewComponent.IsVisible = true;
				xuiC_ItemStack.SlotChangedEvent += this.HandleLootSlotChangedEvent;
			}
			else
			{
				xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
				this.itemControllers[i].ViewComponent.IsVisible = false;
			}
		}
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x060062F1 RID: 25329 RVA: 0x00283158 File Offset: 0x00281358
	public override void Init()
	{
		base.Init();
		XUiV_Grid xuiV_Grid = (XUiV_Grid)this.viewComponent;
		this.GridCellSize = new Vector2i(xuiV_Grid.CellWidth, xuiV_Grid.CellHeight);
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_DewCollectorStack xuiC_DewCollectorStack = (XUiC_DewCollectorStack)this.itemControllers[i];
			xuiC_DewCollectorStack.RequiredItemClass = ItemClass.GetItemClass(this.requiredItem, false);
			xuiC_DewCollectorStack.RequiredItemOnly = true;
			xuiC_DewCollectorStack.TakeOnly = true;
		}
	}

	// Token: 0x060062F2 RID: 25330 RVA: 0x002831CD File Offset: 0x002813CD
	public void HandleLootSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		this.localTileEntity.UpdateSlot(slotNumber, stack);
		this.localTileEntity.SetModified();
	}

	// Token: 0x060062F3 RID: 25331 RVA: 0x002831E8 File Offset: 0x002813E8
	public void OnTileEntityChanged(ITileEntity _te)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			xuiC_ItemStack.SlotChangedEvent -= this.HandleLootSlotChangedEvent;
			this.SetItemInSlot(i, slots[i], true);
			xuiC_ItemStack.SlotChangedEvent += this.HandleLootSlotChangedEvent;
		}
	}

	// Token: 0x060062F4 RID: 25332 RVA: 0x00283240 File Offset: 0x00281440
	public void SetItemInSlot(int i, ItemStack stack, bool onTEChanged)
	{
		if (onTEChanged && this.itemControllers[i].ItemStack.IsEmpty() && !stack.IsEmpty())
		{
			string convertSound = ((BlockDewCollector)this.localTileEntity.blockValue.Block).ConvertSound;
			Manager.BroadcastPlayByLocalPlayer(this.localTileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, convertSound);
		}
		this.itemControllers[i].ItemStack = stack.Clone();
		XUiC_DewCollectorStack xuiC_DewCollectorStack = (XUiC_DewCollectorStack)this.itemControllers[i];
		xuiC_DewCollectorStack.FillAmount = this.localTileEntity.fillValues[i];
		xuiC_DewCollectorStack.MaxFill = this.localTileEntity.CurrentConvertTime;
		xuiC_DewCollectorStack.IsCurrentStack = (this.localTileEntity.CurrentIndex == i);
		xuiC_DewCollectorStack.IsBlocked = this.localTileEntity.IsBlocked;
		xuiC_DewCollectorStack.IsModded = this.localTileEntity.IsModdedConvertItem;
		xuiC_DewCollectorStack.RefreshBindings(false);
	}

	// Token: 0x060062F5 RID: 25333 RVA: 0x00283338 File Offset: 0x00281538
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.localTileEntity.listeners.Contains(this))
		{
			this.localTileEntity.listeners.Add(this);
		}
		this.localTileEntity.Destroyed += this.LocalTileEntity_Destroyed;
		this.blockValue = GameManager.Instance.World.GetBlock(this.localTileEntity.ToWorldPos());
	}

	// Token: 0x060062F6 RID: 25334 RVA: 0x002833A6 File Offset: 0x002815A6
	[PublicizedFrom(EAccessModifier.Private)]
	public void LocalTileEntity_Destroyed(ITileEntity te)
	{
		if (GameManager.Instance != null)
		{
			if (te == this.localTileEntity)
			{
				XUiC_DewCollectorWindowGroup.CloseIfOpenAtPos(te.ToWorldPos(), null);
				return;
			}
			te.Destroyed -= this.LocalTileEntity_Destroyed;
		}
	}

	// Token: 0x060062F7 RID: 25335 RVA: 0x002833E0 File Offset: 0x002815E0
	public override void OnClose()
	{
		base.OnClose();
		base.xui.lootContainer = null;
		if (this.localTileEntity != null)
		{
			this.localTileEntity.Destroyed -= this.LocalTileEntity_Destroyed;
			if (this.localTileEntity.listeners.Contains(this))
			{
				this.localTileEntity.listeners.Remove(this);
			}
			this.localTileEntity = null;
		}
	}

	// Token: 0x060062F8 RID: 25336 RVA: 0x0028344A File Offset: 0x0028164A
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "required_item")
		{
			this.requiredItem = _value;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x04004A91 RID: 19089
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityDewCollector localTileEntity;

	// Token: 0x04004A92 RID: 19090
	[PublicizedFrom(EAccessModifier.Private)]
	public string requiredItem = "";

	// Token: 0x04004A94 RID: 19092
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValue;
}
