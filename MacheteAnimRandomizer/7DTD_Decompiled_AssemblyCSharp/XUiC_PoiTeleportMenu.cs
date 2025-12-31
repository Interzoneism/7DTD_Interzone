using System;
using UnityEngine.Scripting;

// Token: 0x02000D79 RID: 3449
[Preserve]
public class XUiC_PoiTeleportMenu : XUiController
{
	// Token: 0x06006BD3 RID: 27603 RVA: 0x002C252C File Offset: 0x002C072C
	public override void Init()
	{
		base.Init();
		XUiC_PoiTeleportMenu.ID = base.WindowGroup.ID;
		this.list = base.GetChildByType<XUiC_PoiList>();
		this.list.SelectionChanged += this.ListSelectionChanged;
		XUiController childById = base.GetChildById("filterSmall");
		XUiC_ToggleButton xuiC_ToggleButton = (childById != null) ? childById.GetChildByType<XUiC_ToggleButton>() : null;
		xuiC_ToggleButton.Value = this.list.FilterSmallPois;
		xuiC_ToggleButton.OnValueChanged += this.FilterSmall_Changed;
		XUiController childById2 = base.GetChildById("cbxFilterTier");
		this.cbxFilterTier = ((childById2 != null) ? childById2.GetChildByType<XUiC_ComboBoxInt>() : null);
		this.cbxFilterTier.Value = (long)this.list.FilterTier;
		this.cbxFilterTier.OnValueChanged += this.CbxFilterTier_OnValueChanged;
	}

	// Token: 0x06006BD4 RID: 27604 RVA: 0x002C25F6 File Offset: 0x002C07F6
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxFilterTier_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		this.list.FilterTier = (int)_newValue;
	}

	// Token: 0x06006BD5 RID: 27605 RVA: 0x002C2605 File Offset: 0x002C0805
	[PublicizedFrom(EAccessModifier.Private)]
	public void FilterSmall_Changed(XUiC_ToggleButton _sender, bool _newvalue)
	{
		this.list.FilterSmallPois = _newvalue;
	}

	// Token: 0x06006BD6 RID: 27606 RVA: 0x002C2614 File Offset: 0x002C0814
	[PublicizedFrom(EAccessModifier.Private)]
	public void ListSelectionChanged(XUiC_ListEntry<XUiC_PoiList.PoiListEntry> _previousEntry, XUiC_ListEntry<XUiC_PoiList.PoiListEntry> _newEntry)
	{
		if (_newEntry != null && _newEntry.GetEntry() != null)
		{
			XUiC_PoiList.PoiListEntry entry = _newEntry.GetEntry();
			this.EntryPressed(entry);
		}
	}

	// Token: 0x06006BD7 RID: 27607 RVA: 0x002C263A File Offset: 0x002C083A
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntryPressed(XUiC_PoiList.PoiListEntry _key)
	{
		base.xui.playerUI.entityPlayer.Teleport(_key.prefabInstance.boundingBoxPosition.ToVector3(), 45f);
	}

	// Token: 0x06006BD8 RID: 27608 RVA: 0x002C2666 File Offset: 0x002C0866
	public override void OnOpen()
	{
		base.OnOpen();
		this.cbxFilterTier.Max = (long)this.list.MaxTier;
	}

	// Token: 0x04005216 RID: 21014
	public static string ID = "";

	// Token: 0x04005217 RID: 21015
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PoiList list;

	// Token: 0x04005218 RID: 21016
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt cbxFilterTier;
}
