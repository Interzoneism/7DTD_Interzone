using System;
using UnityEngine.Scripting;

// Token: 0x02000D09 RID: 3337
[Preserve]
public class XUiC_LootContainer : XUiC_ItemStackGrid, ITileEntityChangedListener
{
	// Token: 0x17000A9E RID: 2718
	// (get) Token: 0x060067C5 RID: 26565 RVA: 0x002A127D File Offset: 0x0029F47D
	// (set) Token: 0x060067C6 RID: 26566 RVA: 0x002A1285 File Offset: 0x0029F485
	public Vector2i GridCellSize { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060067C7 RID: 26567 RVA: 0x002A128E File Offset: 0x0029F48E
	public override ItemStack[] GetSlots()
	{
		return this.localTileEntity.items;
	}

	// Token: 0x060067C8 RID: 26568 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x060067C9 RID: 26569 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetStacks(ItemStack[] stackList)
	{
	}

	// Token: 0x060067CA RID: 26570 RVA: 0x002A129C File Offset: 0x0029F49C
	public void SetSlots(ITileEntityLootable lootContainer, ItemStack[] stackList)
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
			xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.LootContainer;
			if (i < num)
			{
				xuiC_ItemStack.ForceSetItemStack(this.localTileEntity.items[i].Clone());
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

	// Token: 0x060067CB RID: 26571 RVA: 0x002A13C8 File Offset: 0x0029F5C8
	public override void Init()
	{
		base.Init();
		XUiV_Grid xuiV_Grid = (XUiV_Grid)this.viewComponent;
		this.GridCellSize = new Vector2i(xuiV_Grid.CellWidth, xuiV_Grid.CellHeight);
	}

	// Token: 0x060067CC RID: 26572 RVA: 0x002A13FE File Offset: 0x0029F5FE
	public void HandleLootSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		this.localTileEntity.UpdateSlot(slotNumber, stack);
		this.localTileEntity.SetModified();
	}

	// Token: 0x060067CD RID: 26573 RVA: 0x002A1418 File Offset: 0x0029F618
	public void OnTileEntityChanged(ITileEntity _te)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			xuiC_ItemStack.SlotChangedEvent -= this.HandleLootSlotChangedEvent;
			xuiC_ItemStack.ItemStack = slots[i];
			xuiC_ItemStack.SlotChangedEvent += this.HandleLootSlotChangedEvent;
		}
	}

	// Token: 0x060067CE RID: 26574 RVA: 0x002A1470 File Offset: 0x0029F670
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.localTileEntity.listeners.Contains(this))
		{
			this.localTileEntity.listeners.Add(this);
		}
		base.xui.lootContainer = this.localTileEntity;
		this.localTileEntity.Destroyed += this.LocalTileEntity_Destroyed;
		QuestEventManager.Current.OpenedContainer(this.localTileEntity.EntityId, this.localTileEntity.ToWorldPos(), this.localTileEntity);
		this.blockValue = GameManager.Instance.World.GetBlock(this.localTileEntity.ToWorldPos());
	}

	// Token: 0x060067CF RID: 26575 RVA: 0x002A1518 File Offset: 0x0029F718
	[PublicizedFrom(EAccessModifier.Private)]
	public void LocalTileEntity_Destroyed(ITileEntity te)
	{
		if (GameManager.Instance != null)
		{
			if (te == this.localTileEntity)
			{
				base.xui.playerUI.windowManager.Close("looting");
				return;
			}
			te.Destroyed -= this.LocalTileEntity_Destroyed;
		}
	}

	// Token: 0x060067D0 RID: 26576 RVA: 0x002A1568 File Offset: 0x0029F768
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
			QuestEventManager.Current.ClosedContainer(this.localTileEntity.EntityId, this.localTileEntity.ToWorldPos(), this.localTileEntity);
			this.localTileEntity = null;
		}
	}

	// Token: 0x04004E4D RID: 20045
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntityLootable localTileEntity;

	// Token: 0x04004E4F RID: 20047
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValue;
}
