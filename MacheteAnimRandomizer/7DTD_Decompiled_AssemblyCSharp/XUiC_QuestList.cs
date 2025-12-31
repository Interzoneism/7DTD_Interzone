using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000DAD RID: 3501
[Preserve]
public class XUiC_QuestList : XUiController
{
	// Token: 0x17000AF9 RID: 2809
	// (get) Token: 0x06006D7E RID: 28030 RVA: 0x002CA81B File Offset: 0x002C8A1B
	// (set) Token: 0x06006D7F RID: 28031 RVA: 0x002CA823 File Offset: 0x002C8A23
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

	// Token: 0x17000AFA RID: 2810
	// (get) Token: 0x06006D80 RID: 28032 RVA: 0x002CA853 File Offset: 0x002C8A53
	// (set) Token: 0x06006D81 RID: 28033 RVA: 0x002CA85C File Offset: 0x002C8A5C
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
			XUiC_QuestListWindow questListWindow = this.QuestListWindow;
			bool show;
			if (base.xui.playerUI.entityPlayer.IsInParty())
			{
				XUiC_QuestEntry xuiC_QuestEntry = this.selectedEntry;
				show = (xuiC_QuestEntry != null && xuiC_QuestEntry.Quest.IsShareable);
			}
			else
			{
				show = false;
			}
			questListWindow.ShowShareQuest(show);
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = true;
				if (this.SharedList != null)
				{
					this.SharedList.SelectedEntry = null;
				}
				this.QuestListWindow.ShowRemoveQuest(this.selectedEntry.Quest.QuestClass.AllowRemove);
				return;
			}
			this.QuestListWindow.ShowRemoveQuest(true);
		}
	}

	// Token: 0x17000AFB RID: 2811
	// (get) Token: 0x06006D82 RID: 28034 RVA: 0x002CA915 File Offset: 0x002C8B15
	// (set) Token: 0x06006D83 RID: 28035 RVA: 0x002CA91D File Offset: 0x002C8B1D
	public int VisibleEntries
	{
		get
		{
			return this.visibleEntries;
		}
		set
		{
			if (value != this.visibleEntries)
			{
				this.isDirty = true;
				this.visibleEntries = value;
			}
		}
	}

	// Token: 0x06006D84 RID: 28036 RVA: 0x002CA938 File Offset: 0x002C8B38
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

	// Token: 0x06006D85 RID: 28037 RVA: 0x002CA9EC File Offset: 0x002C8BEC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			XUiC_QuestEntry xuiC_QuestEntry = this.selectedEntry;
			Quest quest = (xuiC_QuestEntry != null) ? xuiC_QuestEntry.Quest : null;
			for (int i = 0; i < this.entryList.Count; i++)
			{
				int num = i + this.visibleEntries * this.page;
				XUiC_QuestEntry xuiC_QuestEntry2 = this.entryList[i];
				xuiC_QuestEntry2.OnPress -= this.OnPressQuest;
				xuiC_QuestEntry2.ViewComponent.IsVisible = (i < this.visibleEntries);
				if (num < this.questList.Count && i < this.visibleEntries)
				{
					xuiC_QuestEntry2.Quest = this.questList[num];
					xuiC_QuestEntry2.OnPress += this.OnPressQuest;
					xuiC_QuestEntry2.ViewComponent.SoundPlayOnClick = true;
					xuiC_QuestEntry2.Selected = (this.questList[num] == quest);
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
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging != null)
			{
				xuiC_Paging.SetLastPageByElementsAndPageLength(this.questList.Count, this.visibleEntries);
			}
			if (this.selectedEntry != null && this.selectedEntry.Quest == null)
			{
				this.selectedEntry = null;
			}
			if (this.selectedEntry == null && this.questList.Count > 0)
			{
				this.SelectedEntry = this.entryList[0];
				((XUiC_QuestWindowGroup)base.WindowGroup.Controller).SetQuest(this.selectedEntry);
			}
			else if (this.questList.Count == 0 && this.selectedEntry == null)
			{
				this.SelectedEntry = null;
				((XUiC_QuestWindowGroup)base.WindowGroup.Controller).SetQuest(this.selectedEntry);
			}
			this.isDirty = false;
		}
	}

	// Token: 0x06006D86 RID: 28038 RVA: 0x002CABC8 File Offset: 0x002C8DC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressQuest(XUiController _sender, int _mouseButton)
	{
		XUiC_QuestEntry xuiC_QuestEntry = _sender as XUiC_QuestEntry;
		if (xuiC_QuestEntry != null)
		{
			this.SelectedEntry = xuiC_QuestEntry;
			this.SelectedEntry.QuestUIHandler.SetQuest(this.SelectedEntry);
			if (InputUtils.ShiftKeyPressed)
			{
				Quest quest = xuiC_QuestEntry.Quest;
				if (quest.Active && !quest.Tracked)
				{
					quest.Tracked = !quest.Tracked;
					base.xui.playerUI.entityPlayer.QuestJournal.TrackedQuest = (quest.Tracked ? quest : null);
					base.xui.playerUI.entityPlayer.QuestJournal.RefreshTracked();
				}
			}
		}
	}

	// Token: 0x06006D87 RID: 28039 RVA: 0x002CAC6C File Offset: 0x002C8E6C
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

	// Token: 0x06006D88 RID: 28040 RVA: 0x002CAC99 File Offset: 0x002C8E99
	public void SetQuestList(List<Quest> newQuestList)
	{
		this.Page = 0;
		this.questList = newQuestList;
		this.isDirty = true;
	}

	// Token: 0x06006D89 RID: 28041 RVA: 0x002CACB0 File Offset: 0x002C8EB0
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.player.QuestChanged += this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedQuestChanged += this.QuestTracker_OnTrackedQuestChanged;
	}

	// Token: 0x06006D8A RID: 28042 RVA: 0x002CAD0C File Offset: 0x002C8F0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestTracker_OnTrackedQuestChanged()
	{
		for (int i = 0; i < this.entryList.Count; i++)
		{
			this.entryList[i].IsDirty = true;
		}
	}

	// Token: 0x06006D8B RID: 28043 RVA: 0x002CAD41 File Offset: 0x002C8F41
	public override void OnClose()
	{
		base.OnClose();
		this.player.QuestChanged -= this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedQuestChanged -= this.QuestTracker_OnTrackedQuestChanged;
	}

	// Token: 0x06006D8C RID: 28044 RVA: 0x002CAD7C File Offset: 0x002C8F7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_QuestChanged(Quest q)
	{
		if (this.selectedEntry != null && this.selectedEntry.Quest == q)
		{
			this.selectedEntry.IsDirty = true;
		}
	}

	// Token: 0x06006D8D RID: 28045 RVA: 0x002CADA0 File Offset: 0x002C8FA0
	public bool HasQuests()
	{
		return this.questList != null && this.questList.Count > 0;
	}

	// Token: 0x0400531D RID: 21277
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x0400531E RID: 21278
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_QuestEntry> entryList = new List<XUiC_QuestEntry>();

	// Token: 0x0400531F RID: 21279
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x04005320 RID: 21280
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005321 RID: 21281
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestEntry selectedEntry;

	// Token: 0x04005322 RID: 21282
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Quest> questList = new List<Quest>();

	// Token: 0x04005323 RID: 21283
	public XUiC_QuestListWindow QuestListWindow;

	// Token: 0x04005324 RID: 21284
	public XUiC_QuestSharedList SharedList;

	// Token: 0x04005325 RID: 21285
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x04005326 RID: 21286
	[PublicizedFrom(EAccessModifier.Private)]
	public int visibleEntries;
}
