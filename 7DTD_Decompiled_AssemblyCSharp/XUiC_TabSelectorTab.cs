using System;
using UnityEngine.Scripting;

// Token: 0x02000E5D RID: 3677
[Preserve]
public class XUiC_TabSelectorTab : XUiController
{
	// Token: 0x17000BB3 RID: 2995
	// (get) Token: 0x06007382 RID: 29570 RVA: 0x002F12AE File Offset: 0x002EF4AE
	// (set) Token: 0x06007383 RID: 29571 RVA: 0x002F12B6 File Offset: 0x002EF4B6
	public string TabKey
	{
		get
		{
			return this.tabKey;
		}
		set
		{
			this.tabKey = value;
			this.TabHeaderText = Localization.Get(this.tabKey, false);
		}
	}

	// Token: 0x17000BB4 RID: 2996
	// (get) Token: 0x06007384 RID: 29572 RVA: 0x002F12D1 File Offset: 0x002EF4D1
	// (set) Token: 0x06007385 RID: 29573 RVA: 0x002F12D9 File Offset: 0x002EF4D9
	public string TabHeaderText
	{
		get
		{
			return this.tabHeaderText;
		}
		set
		{
			this.tabHeaderText = value;
			if (this.TabButton != null)
			{
				this.TabButton.IsDirty = true;
			}
		}
	}

	// Token: 0x17000BB5 RID: 2997
	// (get) Token: 0x06007386 RID: 29574 RVA: 0x002F12F6 File Offset: 0x002EF4F6
	// (set) Token: 0x06007387 RID: 29575 RVA: 0x002F12FE File Offset: 0x002EF4FE
	public bool TabVisible
	{
		get
		{
			return this.tabVisible;
		}
		set
		{
			if (value == this.tabVisible)
			{
				return;
			}
			this.tabVisible = value;
			this.TabButton.UpdateVisibilityState();
			this.parentSelector.TabVisibilityChanged(this, this.tabVisible);
		}
	}

	// Token: 0x17000BB6 RID: 2998
	// (get) Token: 0x06007388 RID: 29576 RVA: 0x002F132E File Offset: 0x002EF52E
	// (set) Token: 0x06007389 RID: 29577 RVA: 0x002F1336 File Offset: 0x002EF536
	public bool TabSelected
	{
		get
		{
			return this.tabSelected;
		}
		set
		{
			if (value == this.tabSelected)
			{
				return;
			}
			this.tabSelected = value;
			this.TabButton.UpdateSelectionState();
			base.ViewComponent.IsVisible = this.tabSelected;
			this.parentSelector.SelectedTab = this;
		}
	}

	// Token: 0x0600738A RID: 29578 RVA: 0x002F1371 File Offset: 0x002EF571
	public override void Init()
	{
		base.Init();
		this.parentSelector = base.GetParentByType<XUiC_TabSelector>();
	}

	// Token: 0x0600738B RID: 29579 RVA: 0x002F1388 File Offset: 0x002EF588
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "tab_key")
		{
			this.TabKey = _value;
			return true;
		}
		if (!(_name == "tab_visible"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.TabVisible = StringParsers.ParseBool(_value, 0, -1, true);
		return true;
	}

	// Token: 0x040057EB RID: 22507
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector parentSelector;

	// Token: 0x040057EC RID: 22508
	public XUiC_TabSelectorButton TabButton;

	// Token: 0x040057ED RID: 22509
	[PublicizedFrom(EAccessModifier.Private)]
	public string tabKey;

	// Token: 0x040057EE RID: 22510
	[PublicizedFrom(EAccessModifier.Private)]
	public string tabHeaderText;

	// Token: 0x040057EF RID: 22511
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tabVisible = true;

	// Token: 0x040057F0 RID: 22512
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tabSelected;
}
