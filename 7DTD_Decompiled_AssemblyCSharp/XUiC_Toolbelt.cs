using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E73 RID: 3699
[Preserve]
public class XUiC_Toolbelt : XUiC_ItemStackGrid
{
	// Token: 0x17000BD0 RID: 3024
	// (get) Token: 0x06007435 RID: 29749 RVA: 0x000197A5 File Offset: 0x000179A5
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.ToolBelt;
		}
	}

	// Token: 0x17000BD1 RID: 3025
	// (get) Token: 0x06007436 RID: 29750 RVA: 0x002F37E6 File Offset: 0x002F19E6
	public bool HasSecondRow
	{
		get
		{
			return this.itemControllers != null && this.backendSlotCount > this.itemControllers.Length / 2;
		}
	}

	// Token: 0x06007437 RID: 29751 RVA: 0x002F3804 File Offset: 0x002F1A04
	public override void Init()
	{
		base.Init();
		XUiC_Toolbelt.ID = base.WindowGroup.ID;
	}

	// Token: 0x06007438 RID: 29752 RVA: 0x002F381C File Offset: 0x002F1A1C
	public override ItemStack[] GetSlots()
	{
		return base.xui.PlayerInventory.GetToolbeltItemStacks();
	}

	// Token: 0x06007439 RID: 29753 RVA: 0x002F3830 File Offset: 0x002F1A30
	public override void Update(float _dt)
	{
		if (!XUi.IsGameRunning())
		{
			return;
		}
		if (this.openLater)
		{
			this.OnOpen();
			this.openLater = false;
		}
		Inventory toolbelt = base.xui.PlayerInventory.Toolbelt;
		if (this.currentHoldingIndex != toolbelt.GetFocusedItemIdx())
		{
			if (this.currentHoldingIndex != toolbelt.DUMMY_SLOT_IDX)
			{
				this.itemControllers[this.currentHoldingIndex].IsHolding = false;
			}
			this.currentHoldingIndex = toolbelt.GetFocusedItemIdx();
			if (this.currentHoldingIndex != toolbelt.DUMMY_SLOT_IDX)
			{
				this.itemControllers[this.currentHoldingIndex].IsHolding = true;
			}
		}
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 0.5f;
			bool flag = toolbelt.IsHoldingItemActionRunning();
			if (this.lastActionRunning != flag)
			{
				this.currentHoldingIndex = toolbelt.holdingItemIdx;
				if (this.currentHoldingIndex != toolbelt.DUMMY_SLOT_IDX)
				{
					if (flag)
					{
						this.lastActionSlot = toolbelt.holdingItemIdx;
						this.itemControllers[toolbelt.holdingItemIdx].HiddenLock = true;
					}
					else
					{
						this.itemControllers[this.lastActionSlot].HiddenLock = flag;
					}
				}
				this.lastActionRunning = flag;
			}
			if (!GameManager.Instance.bCursorVisible)
			{
				this.ClearHoveredItems();
			}
		}
		this.UpdateQuickSwap();
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x0600743A RID: 29754 RVA: 0x002F3985 File Offset: 0x002F1B85
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.xui.PlayerInventory.SetToolbeltItemStacks(stackList);
	}

	// Token: 0x0600743B RID: 29755 RVA: 0x002F3998 File Offset: 0x002F1B98
	public override bool AlwaysUpdate()
	{
		return this.openLater;
	}

	// Token: 0x0600743C RID: 29756 RVA: 0x002F39A0 File Offset: 0x002F1BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateQuickSwap()
	{
		int quickSwapSlot = base.xui.PlayerInventory.QuickSwapSlot;
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			this.itemControllers[i].isQuickSwap = (i == quickSwapSlot);
		}
	}

	// Token: 0x0600743D RID: 29757 RVA: 0x002F39E4 File Offset: 0x002F1BE4
	public override void OnOpen()
	{
		if (!XUi.IsGameRunning())
		{
			this.openLater = true;
			return;
		}
		base.OnOpen();
		base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
		this.PlayerInventory_OnToolbeltItemsChanged();
		this.currentHoldingIndex = base.xui.PlayerInventory.Toolbelt.holdingItemIdx;
		if (this.currentHoldingIndex != base.xui.PlayerInventory.Toolbelt.DUMMY_SLOT_IDX)
		{
			this.itemControllers[this.currentHoldingIndex].IsHolding = true;
		}
		base.xui.playerUI.windowManager.OpenIfNotOpen("dragAndDrop", false, false, true);
		if (this.backendSlotCount < 0)
		{
			this.backendSlotCount = base.xui.PlayerInventory.Toolbelt.PUBLIC_SLOTS;
			this.IsDirty = true;
		}
	}

	// Token: 0x0600743E RID: 29758 RVA: 0x0026533A File Offset: 0x0026353A
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.SetStacks(this.GetSlots());
		this.IsDirty = true;
	}

	// Token: 0x0600743F RID: 29759 RVA: 0x002F3ABC File Offset: 0x002F1CBC
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
		base.xui.playerUI.windowManager.CloseIfOpen("dragAndDrop");
		if (this.currentHoldingIndex != base.xui.PlayerInventory.Toolbelt.DUMMY_SLOT_IDX)
		{
			this.itemControllers[this.currentHoldingIndex].IsHolding = false;
		}
	}

	// Token: 0x06007440 RID: 29760 RVA: 0x002F3B35 File Offset: 0x002F1D35
	[PublicizedFrom(EAccessModifier.Internal)]
	public XUiC_ItemStack GetSlotControl(int slotIdx)
	{
		return this.itemControllers[slotIdx];
	}

	// Token: 0x06007441 RID: 29761 RVA: 0x002F3B40 File Offset: 0x002F1D40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "secondrow")
		{
			_value = this.HasSecondRow.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04005855 RID: 22613
	public static string ID = "";

	// Token: 0x04005856 RID: 22614
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentHoldingIndex;

	// Token: 0x04005857 RID: 22615
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastActionSlot;

	// Token: 0x04005858 RID: 22616
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastActionRunning;

	// Token: 0x04005859 RID: 22617
	[PublicizedFrom(EAccessModifier.Protected)]
	public float updateTime;

	// Token: 0x0400585A RID: 22618
	[PublicizedFrom(EAccessModifier.Private)]
	public int backendSlotCount = -1;

	// Token: 0x0400585B RID: 22619
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openLater;
}
