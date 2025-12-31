using System;
using System.Collections.Generic;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E81 RID: 3713
[Preserve]
public class XUiC_TwitchActionHistoryEntryList : XUiController
{
	// Token: 0x17000BE9 RID: 3049
	// (get) Token: 0x060074EA RID: 29930 RVA: 0x002F8A72 File Offset: 0x002F6C72
	// (set) Token: 0x060074EB RID: 29931 RVA: 0x002F8A7A File Offset: 0x002F6C7A
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			if (this.page != value)
			{
				this.page = value;
				this.isDirty = true;
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging == null)
				{
					return;
				}
				xuiC_Paging.SetPage(this.page);
			}
		}
	}

	// Token: 0x17000BEA RID: 3050
	// (get) Token: 0x060074EC RID: 29932 RVA: 0x002F8AA9 File Offset: 0x002F6CA9
	// (set) Token: 0x060074ED RID: 29933 RVA: 0x002F8AB1 File Offset: 0x002F6CB1
	public XUiC_TwitchActionHistoryEntry SelectedEntry
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
		}
	}

	// Token: 0x060074EE RID: 29934 RVA: 0x002F8AE4 File Offset: 0x002F6CE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetFirstEntry()
	{
		if (this.entryList[0].HistoryItem != null)
		{
			this.SelectedEntry = this.entryList[0];
			this.entryList[0].SelectCursorElement(true, false);
		}
		else
		{
			this.SelectedEntry = null;
			base.WindowGroup.Controller.GetChildById("searchControls").SelectCursorElement(true, false);
		}
		((XUiC_TwitchInfoWindowGroup)base.WindowGroup.Controller).SetEntry(this.selectedEntry);
	}

	// Token: 0x060074EF RID: 29935 RVA: 0x002F8B6C File Offset: 0x002F6D6C
	public override void Init()
	{
		base.Init();
		XUiC_TwitchInfoWindowGroup xuiC_TwitchInfoWindowGroup = (XUiC_TwitchInfoWindowGroup)base.WindowGroup.Controller;
		XUiController childById = xuiC_TwitchInfoWindowGroup.GetChildByType<XUiC_TwitchEntryDescriptionWindow>().GetChildById("btnRefund");
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] is XUiC_TwitchActionHistoryEntry)
			{
				XUiC_TwitchActionHistoryEntry xuiC_TwitchActionHistoryEntry = (XUiC_TwitchActionHistoryEntry)this.children[i];
				xuiC_TwitchActionHistoryEntry.Owner = this;
				xuiC_TwitchActionHistoryEntry.TwitchInfoUIHandler = xuiC_TwitchInfoWindowGroup;
				xuiC_TwitchActionHistoryEntry.OnScroll += this.OnScrollEntry;
				xuiC_TwitchActionHistoryEntry.ViewComponent.NavRightTarget = childById.ViewComponent;
				this.entryList.Add(xuiC_TwitchActionHistoryEntry);
			}
		}
		this.pager = base.Parent.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += delegate()
			{
				if (this.viewComponent.IsVisible)
				{
					this.Page = this.pager.CurrentPageNumber;
					this.SelectedEntry = null;
					this.setFirstEntry = true;
				}
			};
		}
	}

	// Token: 0x060074F0 RID: 29936 RVA: 0x002F8C4C File Offset: 0x002F6E4C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			if (this.entryList != null)
			{
				TwitchActionHistoryEntry twitchActionHistoryEntry = (this.selectedEntry != null) ? this.selectedEntry.HistoryItem : null;
				bool flag = false;
				int num = this.GetPage(twitchActionHistoryEntry);
				int num2 = this.page;
				if (num != -1 && num != this.page)
				{
					flag = true;
					num2 = num;
				}
				for (int i = 0; i < this.entryList.Count; i++)
				{
					int num3 = i + this.entryList.Count * num2;
					XUiC_TwitchActionHistoryEntry xuiC_TwitchActionHistoryEntry = this.entryList[i];
					if (xuiC_TwitchActionHistoryEntry != null)
					{
						xuiC_TwitchActionHistoryEntry.OnPress -= this.OnPressEntry;
						if (num3 < this.redemptionList.Count)
						{
							xuiC_TwitchActionHistoryEntry.HistoryItem = this.redemptionList[num3];
							xuiC_TwitchActionHistoryEntry.OnPress += this.OnPressEntry;
							xuiC_TwitchActionHistoryEntry.ViewComponent.SoundPlayOnClick = true;
							xuiC_TwitchActionHistoryEntry.Selected = (xuiC_TwitchActionHistoryEntry.HistoryItem == twitchActionHistoryEntry);
							if (xuiC_TwitchActionHistoryEntry.Selected)
							{
								this.SelectedEntry = xuiC_TwitchActionHistoryEntry;
								((XUiC_TwitchInfoWindowGroup)base.WindowGroup.Controller).SetEntry(this.selectedEntry);
							}
							xuiC_TwitchActionHistoryEntry.ViewComponent.IsNavigatable = true;
						}
						else
						{
							xuiC_TwitchActionHistoryEntry.HistoryItem = null;
							xuiC_TwitchActionHistoryEntry.ViewComponent.SoundPlayOnClick = false;
							xuiC_TwitchActionHistoryEntry.Selected = false;
							xuiC_TwitchActionHistoryEntry.ViewComponent.IsNavigatable = false;
						}
					}
				}
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging != null)
				{
					xuiC_Paging.SetLastPageByElementsAndPageLength(this.redemptionList.Count, this.entryList.Count);
				}
				if (flag)
				{
					this.Page = num2;
					if (this.pager != null)
					{
						this.pager.RefreshBindings(false);
					}
				}
			}
			if (this.setFirstEntry)
			{
				this.SetFirstEntry();
				this.setFirstEntry = false;
			}
			this.isDirty = false;
		}
	}

	// Token: 0x060074F1 RID: 29937 RVA: 0x002F8E2C File Offset: 0x002F702C
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetPage(TwitchActionHistoryEntry historyItem)
	{
		for (int i = 0; i < this.redemptionList.Count; i++)
		{
			if (this.redemptionList[i] == historyItem)
			{
				return i / this.entryList.Count;
			}
		}
		return -1;
	}

	// Token: 0x060074F2 RID: 29938 RVA: 0x002F8E70 File Offset: 0x002F7070
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressEntry(XUiController _sender, int _mouseButton)
	{
		XUiC_TwitchActionHistoryEntry xuiC_TwitchActionHistoryEntry = _sender as XUiC_TwitchActionHistoryEntry;
		if (xuiC_TwitchActionHistoryEntry != null)
		{
			this.SelectedEntry = xuiC_TwitchActionHistoryEntry;
			this.SelectedEntry.TwitchInfoUIHandler.SetEntry(this.SelectedEntry);
		}
	}

	// Token: 0x060074F3 RID: 29939 RVA: 0x002F8EA4 File Offset: 0x002F70A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnScrollEntry(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging == null)
			{
				return;
			}
			xuiC_Paging.PageDown();
			return;
		}
		else
		{
			XUiC_Paging xuiC_Paging2 = this.pager;
			if (xuiC_Paging2 == null)
			{
				return;
			}
			xuiC_Paging2.PageUp();
			return;
		}
	}

	// Token: 0x060074F4 RID: 29940 RVA: 0x002F8ED1 File Offset: 0x002F70D1
	public void SetTwitchActionHistoryList(List<TwitchActionHistoryEntry> newRedemptionList)
	{
		this.redemptionList = newRedemptionList;
		if (this.SelectedEntry == null)
		{
			this.Page = 0;
			this.setFirstEntry = true;
		}
		this.isDirty = true;
		((XUiC_TwitchInfoWindowGroup)base.WindowGroup.Controller).ClearEntries();
	}

	// Token: 0x060074F5 RID: 29941 RVA: 0x002F8F0C File Offset: 0x002F710C
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
	}

	// Token: 0x060074F6 RID: 29942 RVA: 0x002F8F2A File Offset: 0x002F712A
	public override void OnClose()
	{
		base.OnClose();
		this.SelectedEntry = null;
	}

	// Token: 0x040058EC RID: 22764
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x040058ED RID: 22765
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_TwitchActionHistoryEntry> entryList = new List<XUiC_TwitchActionHistoryEntry>();

	// Token: 0x040058EE RID: 22766
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x040058EF RID: 22767
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040058F0 RID: 22768
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchActionHistoryEntry selectedEntry;

	// Token: 0x040058F1 RID: 22769
	public bool setFirstEntry = true;

	// Token: 0x040058F2 RID: 22770
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchActionHistoryEntry> redemptionList = new List<TwitchActionHistoryEntry>();

	// Token: 0x040058F3 RID: 22771
	public XUiC_TwitchEntryListWindow TwitchEntryListWindow;

	// Token: 0x040058F4 RID: 22772
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;
}
