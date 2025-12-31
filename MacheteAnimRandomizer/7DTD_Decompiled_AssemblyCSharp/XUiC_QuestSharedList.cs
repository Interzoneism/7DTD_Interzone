using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000DBE RID: 3518
[Preserve]
public class XUiC_QuestSharedList : XUiController
{
	// Token: 0x17000B11 RID: 2833
	// (get) Token: 0x06006E0C RID: 28172 RVA: 0x002CDD4F File Offset: 0x002CBF4F
	// (set) Token: 0x06006E0D RID: 28173 RVA: 0x002CDD57 File Offset: 0x002CBF57
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
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging != null)
				{
					xuiC_Paging.SetPage(this.page);
				}
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000B12 RID: 2834
	// (get) Token: 0x06006E0E RID: 28174 RVA: 0x002CDD87 File Offset: 0x002CBF87
	// (set) Token: 0x06006E0F RID: 28175 RVA: 0x002CDD8F File Offset: 0x002CBF8F
	public XUiC_QuestEntry SelectedEntry
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
				this.QuestList.SelectedEntry = null;
			}
		}
	}

	// Token: 0x06006E10 RID: 28176 RVA: 0x002CDDCC File Offset: 0x002CBFCC
	public override void Init()
	{
		base.Init();
		XUiC_QuestWindowGroup questUIHandler = (XUiC_QuestWindowGroup)base.WindowGroup.Controller;
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] is XUiC_QuestEntry)
			{
				XUiC_QuestEntry xuiC_QuestEntry = (XUiC_QuestEntry)this.children[i];
				xuiC_QuestEntry.QuestUIHandler = questUIHandler;
				xuiC_QuestEntry.OnScroll += this.OnScrollQuest;
				this.entryList.Add(xuiC_QuestEntry);
			}
		}
		this.pager = base.Parent.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += delegate()
			{
				this.Page = this.pager.CurrentPageNumber;
			};
		}
	}

	// Token: 0x06006E11 RID: 28177 RVA: 0x002CDE80 File Offset: 0x002CC080
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			XUiC_QuestEntry xuiC_QuestEntry = this.selectedEntry;
			Quest quest = (xuiC_QuestEntry != null) ? xuiC_QuestEntry.Quest : null;
			for (int i = 0; i < this.entryList.Count; i++)
			{
				int num = i + this.entryList.Count * this.page;
				XUiC_QuestEntry xuiC_QuestEntry2 = this.entryList[i];
				if (xuiC_QuestEntry2 != null)
				{
					xuiC_QuestEntry2.OnPress -= this.OnPressQuest;
					if (num < this.questList.Count)
					{
						xuiC_QuestEntry2.Quest = this.questList[num].Quest;
						xuiC_QuestEntry2.SharedQuestEntry = this.questList[num];
						xuiC_QuestEntry2.OnPress += this.OnPressQuest;
						xuiC_QuestEntry2.ViewComponent.SoundPlayOnClick = true;
						xuiC_QuestEntry2.Selected = (this.questList[num].Quest == quest);
						if (xuiC_QuestEntry2.Selected)
						{
							this.SelectedEntry = xuiC_QuestEntry2;
						}
					}
					else
					{
						xuiC_QuestEntry2.Quest = null;
						xuiC_QuestEntry2.ViewComponent.SoundPlayOnClick = false;
						xuiC_QuestEntry2.Selected = false;
					}
				}
			}
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging != null)
			{
				xuiC_Paging.SetLastPageByElementsAndPageLength(this.questList.Count, this.entryList.Count);
			}
			if (this.selectedEntry != null && this.selectedEntry.Quest == null)
			{
				this.selectedEntry = null;
				if (this.questList.Count == 0 && this.selectedEntry == null)
				{
					this.SelectedEntry = null;
					((XUiC_QuestWindowGroup)base.WindowGroup.Controller).SetQuest(this.selectedEntry);
				}
			}
			this.isDirty = false;
		}
	}

	// Token: 0x06006E12 RID: 28178 RVA: 0x002CE028 File Offset: 0x002CC228
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressQuest(XUiController _sender, int _mouseButton)
	{
		XUiC_QuestEntry xuiC_QuestEntry = _sender as XUiC_QuestEntry;
		if (xuiC_QuestEntry != null)
		{
			this.SelectedEntry = xuiC_QuestEntry;
			this.SelectedEntry.QuestUIHandler.SetQuest(this.SelectedEntry);
		}
	}

	// Token: 0x06006E13 RID: 28179 RVA: 0x002CE05C File Offset: 0x002CC25C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnScrollQuest(XUiController _sender, float _delta)
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

	// Token: 0x06006E14 RID: 28180 RVA: 0x002CE089 File Offset: 0x002CC289
	public void SetSharedQuestList(List<SharedQuestEntry> newQuestList)
	{
		this.Page = 0;
		this.questList = newQuestList;
		this.isDirty = true;
	}

	// Token: 0x06006E15 RID: 28181 RVA: 0x002CE0A0 File Offset: 0x002CC2A0
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.player.QuestChanged += this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedQuestChanged += this.QuestTracker_OnTrackedQuestChanged;
	}

	// Token: 0x06006E16 RID: 28182 RVA: 0x002CE0FC File Offset: 0x002CC2FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestTracker_OnTrackedQuestChanged()
	{
		for (int i = 0; i < this.entryList.Count; i++)
		{
			this.entryList[i].IsDirty = true;
		}
	}

	// Token: 0x06006E17 RID: 28183 RVA: 0x002CE131 File Offset: 0x002CC331
	public override void OnClose()
	{
		base.OnClose();
		this.player.QuestChanged -= this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedQuestChanged -= this.QuestTracker_OnTrackedQuestChanged;
	}

	// Token: 0x06006E18 RID: 28184 RVA: 0x002CE16C File Offset: 0x002CC36C
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_QuestChanged(Quest q)
	{
		if (this.selectedEntry != null && this.selectedEntry.Quest == q)
		{
			this.selectedEntry.IsDirty = true;
		}
	}

	// Token: 0x06006E19 RID: 28185 RVA: 0x002CE190 File Offset: 0x002CC390
	public bool HasQuests()
	{
		return this.questList != null && this.questList.Count > 0;
	}

	// Token: 0x0400539E RID: 21406
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x0400539F RID: 21407
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_QuestEntry> entryList = new List<XUiC_QuestEntry>();

	// Token: 0x040053A0 RID: 21408
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x040053A1 RID: 21409
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040053A2 RID: 21410
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestEntry selectedEntry;

	// Token: 0x040053A3 RID: 21411
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SharedQuestEntry> questList = new List<SharedQuestEntry>();

	// Token: 0x040053A4 RID: 21412
	public XUiC_QuestList QuestList;

	// Token: 0x040053A5 RID: 21413
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;
}
