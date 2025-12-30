using System;
using System.Collections.Generic;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E94 RID: 3732
[Preserve]
public class XUiC_TwitchVoteInfoEntryList : XUiController
{
	// Token: 0x17000BFE RID: 3070
	// (get) Token: 0x060075B4 RID: 30132 RVA: 0x002FE9FA File Offset: 0x002FCBFA
	// (set) Token: 0x060075B5 RID: 30133 RVA: 0x002FEA02 File Offset: 0x002FCC02
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

	// Token: 0x17000BFF RID: 3071
	// (get) Token: 0x060075B6 RID: 30134 RVA: 0x002FEA31 File Offset: 0x002FCC31
	// (set) Token: 0x060075B7 RID: 30135 RVA: 0x002FEA39 File Offset: 0x002FCC39
	public XUiC_TwitchVoteInfoEntry SelectedEntry
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

	// Token: 0x060075B8 RID: 30136 RVA: 0x002FEA6C File Offset: 0x002FCC6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetFirstEntry()
	{
		if (this.entryList[0].Vote != null)
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

	// Token: 0x060075B9 RID: 30137 RVA: 0x002FEAF4 File Offset: 0x002FCCF4
	public override void Init()
	{
		base.Init();
		XUiC_TwitchInfoWindowGroup xuiC_TwitchInfoWindowGroup = (XUiC_TwitchInfoWindowGroup)base.WindowGroup.Controller;
		XUiController childById = xuiC_TwitchInfoWindowGroup.GetChildByType<XUiC_TwitchEntryDescriptionWindow>().GetChildById("btnEnable");
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] is XUiC_TwitchVoteInfoEntry)
			{
				XUiC_TwitchVoteInfoEntry xuiC_TwitchVoteInfoEntry = (XUiC_TwitchVoteInfoEntry)this.children[i];
				xuiC_TwitchVoteInfoEntry.Owner = this;
				xuiC_TwitchVoteInfoEntry.TwitchInfoUIHandler = xuiC_TwitchInfoWindowGroup;
				xuiC_TwitchVoteInfoEntry.OnScroll += this.OnScrollEntry;
				xuiC_TwitchVoteInfoEntry.ViewComponent.NavRightTarget = childById.ViewComponent;
				this.entryList.Add(xuiC_TwitchVoteInfoEntry);
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
				}
			};
		}
	}

	// Token: 0x060075BA RID: 30138 RVA: 0x002FEBD4 File Offset: 0x002FCDD4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			Log.Out("Vote list update");
			if (this.entryList != null)
			{
				for (int i = 0; i < this.entryList.Count; i++)
				{
					int num = i + this.entryList.Count * this.page;
					XUiC_TwitchVoteInfoEntry xuiC_TwitchVoteInfoEntry = this.entryList[i];
					if (xuiC_TwitchVoteInfoEntry != null)
					{
						xuiC_TwitchVoteInfoEntry.OnPress -= this.OnPressEntry;
						xuiC_TwitchVoteInfoEntry.Selected = false;
						if (num < this.voteList.Count)
						{
							xuiC_TwitchVoteInfoEntry.Vote = this.voteList[num];
							xuiC_TwitchVoteInfoEntry.OnPress += this.OnPressEntry;
							xuiC_TwitchVoteInfoEntry.ViewComponent.SoundPlayOnClick = true;
							xuiC_TwitchVoteInfoEntry.ViewComponent.IsNavigatable = true;
						}
						else
						{
							xuiC_TwitchVoteInfoEntry.Vote = null;
							xuiC_TwitchVoteInfoEntry.ViewComponent.SoundPlayOnClick = false;
							xuiC_TwitchVoteInfoEntry.ViewComponent.IsNavigatable = false;
						}
					}
				}
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging != null)
				{
					xuiC_Paging.SetLastPageByElementsAndPageLength(this.voteList.Count, this.entryList.Count);
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

	// Token: 0x060075BB RID: 30139 RVA: 0x002FED14 File Offset: 0x002FCF14
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressEntry(XUiController _sender, int _mouseButton)
	{
		XUiC_TwitchVoteInfoEntry xuiC_TwitchVoteInfoEntry = _sender as XUiC_TwitchVoteInfoEntry;
		if (xuiC_TwitchVoteInfoEntry != null)
		{
			this.SelectedEntry = xuiC_TwitchVoteInfoEntry;
			this.SelectedEntry.TwitchInfoUIHandler.SetEntry(this.SelectedEntry);
		}
	}

	// Token: 0x060075BC RID: 30140 RVA: 0x002FED48 File Offset: 0x002FCF48
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

	// Token: 0x060075BD RID: 30141 RVA: 0x002FED75 File Offset: 0x002FCF75
	public void SetTwitchVoteList(List<TwitchVote> newVoteEntryList)
	{
		this.Page = 0;
		this.voteList = newVoteEntryList;
		this.setFirstEntry = true;
		this.isDirty = true;
	}

	// Token: 0x060075BE RID: 30142 RVA: 0x002FED93 File Offset: 0x002FCF93
	public override void OnOpen()
	{
		base.OnOpen();
		this.setFirstEntry = true;
		this.player = base.xui.playerUI.entityPlayer;
	}

	// Token: 0x060075BF RID: 30143 RVA: 0x002FEDB8 File Offset: 0x002FCFB8
	public override void OnClose()
	{
		base.OnClose();
		this.SelectedEntry = null;
	}

	// Token: 0x040059CA RID: 22986
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x040059CB RID: 22987
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_TwitchVoteInfoEntry> entryList = new List<XUiC_TwitchVoteInfoEntry>();

	// Token: 0x040059CC RID: 22988
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x040059CD RID: 22989
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040059CE RID: 22990
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchVoteInfoEntry selectedEntry;

	// Token: 0x040059CF RID: 22991
	public bool setFirstEntry = true;

	// Token: 0x040059D0 RID: 22992
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchVote> voteList = new List<TwitchVote>();

	// Token: 0x040059D1 RID: 22993
	public XUiC_TwitchEntryListWindow TwitchEntryListWindow;

	// Token: 0x040059D2 RID: 22994
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;
}
