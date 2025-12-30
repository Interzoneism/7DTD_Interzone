using System;
using System.Collections.Generic;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000C2C RID: 3116
[Preserve]
public class XUiC_CharacterCosmeticList : XUiController
{
	// Token: 0x170009E7 RID: 2535
	// (get) Token: 0x06005FE0 RID: 24544 RVA: 0x0026E1E8 File Offset: 0x0026C3E8
	// (set) Token: 0x06005FE1 RID: 24545 RVA: 0x0026E1F0 File Offset: 0x0026C3F0
	public XUiC_CharacterCosmeticEntry SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = false;
			}
			this.selectedEntry = value;
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = true;
			}
			this.Owner.RefreshBindings(false);
		}
	}

	// Token: 0x06005FE2 RID: 24546 RVA: 0x0026E22D File Offset: 0x0026C42D
	public override void Init()
	{
		base.Init();
		this.entryList = base.GetChildrenByType<XUiC_CharacterCosmeticEntry>(null);
	}

	// Token: 0x06005FE3 RID: 24547 RVA: 0x0026E242 File Offset: 0x0026C442
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.IsDirty = true;
	}

	// Token: 0x06005FE4 RID: 24548 RVA: 0x0026E267 File Offset: 0x0026C467
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerEquipment.Equipment.ClearTempCosmeticSlot();
	}

	// Token: 0x06005FE5 RID: 24549 RVA: 0x0026E284 File Offset: 0x0026C484
	public override void Update(float _dt)
	{
		base.Update(_dt);
		base.xui.playerUI.entityPlayer.AimingGun = false;
		bool isVisible = base.ViewComponent.IsVisible;
	}

	// Token: 0x06005FE6 RID: 24550 RVA: 0x0026E2B0 File Offset: 0x0026C4B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateCallouts(bool _holdingItem)
	{
		string action = string.Format(Localization.Get("igcoItemActions", false), InControlExtensions.GetBlankDPadSourceString());
		XUiC_GamepadCalloutWindow calloutWindow = base.xui.calloutWindow;
		if (calloutWindow != null)
		{
			calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
			if (!_holdingItem)
			{
				calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoInspect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
				calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, action, XUiC_GamepadCalloutWindow.CalloutType.Menu);
				calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
			}
			calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		}
	}

	// Token: 0x06005FE7 RID: 24551 RVA: 0x0026E31C File Offset: 0x0026C51C
	public void SetCosmeticList(List<ItemClass> _currentItems, EquipmentSlots slot)
	{
		this.currentItems = _currentItems;
		if (this.currentItems == null)
		{
			return;
		}
		ItemClass cosmeticSlot = base.xui.PlayerEquipment.Equipment.GetCosmeticSlot((int)slot, false);
		this.SelectedEntry = null;
		int num = 0;
		XUiC_CharacterCosmeticEntry xuiC_CharacterCosmeticEntry = this.entryList[0];
		xuiC_CharacterCosmeticEntry.EntryType = XUiC_CharacterCosmeticEntry.EntryTypes.Empty;
		xuiC_CharacterCosmeticEntry.ItemClass = null;
		xuiC_CharacterCosmeticEntry.OnPress += this.OnPressCosmetic;
		xuiC_CharacterCosmeticEntry.Index = (int)slot;
		xuiC_CharacterCosmeticEntry.Owner = this;
		if (xuiC_CharacterCosmeticEntry.ItemClass == cosmeticSlot)
		{
			this.SelectedEntry = xuiC_CharacterCosmeticEntry;
		}
		num++;
		if (slot == EquipmentSlots.Head)
		{
			xuiC_CharacterCosmeticEntry = this.entryList[1];
			xuiC_CharacterCosmeticEntry.EntryType = XUiC_CharacterCosmeticEntry.EntryTypes.Hide;
			xuiC_CharacterCosmeticEntry.ItemClass = ItemClass.MissingItem;
			xuiC_CharacterCosmeticEntry.OnPress += this.OnPressCosmetic;
			xuiC_CharacterCosmeticEntry.Index = (int)slot;
			xuiC_CharacterCosmeticEntry.Owner = this;
			if (xuiC_CharacterCosmeticEntry.ItemClass == cosmeticSlot)
			{
				this.SelectedEntry = xuiC_CharacterCosmeticEntry;
			}
			num++;
		}
		int num2 = 0;
		int num3 = num;
		while (num3 < this.entryList.Length && num3 < this.entryList.Length && num2 < this.currentItems.Count)
		{
			xuiC_CharacterCosmeticEntry = this.entryList[num3];
			xuiC_CharacterCosmeticEntry.EntryType = XUiC_CharacterCosmeticEntry.EntryTypes.Item;
			xuiC_CharacterCosmeticEntry.ItemClass = this.currentItems[num2];
			xuiC_CharacterCosmeticEntry.OnPress += this.OnPressCosmetic;
			xuiC_CharacterCosmeticEntry.Index = (int)slot;
			xuiC_CharacterCosmeticEntry.Owner = this;
			if (xuiC_CharacterCosmeticEntry.ItemClass == cosmeticSlot)
			{
				this.SelectedEntry = xuiC_CharacterCosmeticEntry;
			}
			num++;
			num2++;
			num3++;
		}
		for (int i = num; i < this.entryList.Length; i++)
		{
			xuiC_CharacterCosmeticEntry = this.entryList[i];
			xuiC_CharacterCosmeticEntry.EntryType = XUiC_CharacterCosmeticEntry.EntryTypes.Item;
			xuiC_CharacterCosmeticEntry.ItemClass = null;
			xuiC_CharacterCosmeticEntry.Owner = this;
		}
		this.entryList[0].SelectCursorElement(true, false);
	}

	// Token: 0x06005FE8 RID: 24552 RVA: 0x0026E4D0 File Offset: 0x0026C6D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressCosmetic(XUiController _sender, int _mouseButton)
	{
		XUiC_CharacterCosmeticEntry xuiC_CharacterCosmeticEntry = _sender as XUiC_CharacterCosmeticEntry;
		if (xuiC_CharacterCosmeticEntry != null)
		{
			if (this.SelectedEntry == xuiC_CharacterCosmeticEntry)
			{
				return;
			}
			this.SelectedEntry = xuiC_CharacterCosmeticEntry;
			switch (xuiC_CharacterCosmeticEntry.EntryType)
			{
			case XUiC_CharacterCosmeticEntry.EntryTypes.Item:
				base.xui.PlayerEquipment.Equipment.SetTempCosmeticSlot(xuiC_CharacterCosmeticEntry.Index, xuiC_CharacterCosmeticEntry.ItemClass);
				break;
			case XUiC_CharacterCosmeticEntry.EntryTypes.Empty:
				base.xui.PlayerEquipment.Equipment.SetTempCosmeticSlot(xuiC_CharacterCosmeticEntry.Index, null);
				break;
			case XUiC_CharacterCosmeticEntry.EntryTypes.Hide:
				base.xui.PlayerEquipment.Equipment.SetTempCosmeticSlot(xuiC_CharacterCosmeticEntry.Index, ItemClass.MissingItem);
				break;
			}
			((XUiC_CharacterCosmeticWindowGroup)base.WindowGroup.Controller).ResetPreview();
		}
	}

	// Token: 0x0400483E RID: 18494
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x0400483F RID: 18495
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterCosmeticEntry[] entryList;

	// Token: 0x04004840 RID: 18496
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterCosmeticEntry selectedEntry;

	// Token: 0x04004841 RID: 18497
	public XUiC_CharacterCosmeticsListWindow Owner;

	// Token: 0x04004842 RID: 18498
	public List<ItemClass> currentItems;
}
