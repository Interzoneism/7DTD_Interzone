using System;
using System.Collections.Generic;
using System.Globalization;
using UniLinq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DAE RID: 3502
[Preserve]
public class XUiC_QuestListWindow : XUiController
{
	// Token: 0x06006D90 RID: 28048 RVA: 0x002CADEC File Offset: 0x002C8FEC
	public override void Init()
	{
		base.Init();
		this.questList = base.GetChildByType<XUiC_QuestList>();
		this.questList.QuestListWindow = this;
		this.trackBtn = (XUiV_Button)base.GetChildById("trackBtn").ViewComponent;
		this.trackBtn.Controller.OnPress += this.trackBtn_OnPress;
		this.showOnMapBtn = (XUiV_Button)base.GetChildById("showOnMapBtn").ViewComponent;
		this.showOnMapBtn.Controller.OnPress += this.showOnMapBtn_OnPress;
		this.questRemoveBtn = (XUiV_Button)base.GetChildById("questRemoveBtn").ViewComponent;
		this.questRemoveBtn.Controller.OnPress += this.questRemoveBtn_OnPress;
		this.questShareBtn = (XUiV_Button)base.GetChildById("questShareBtn").ViewComponent;
		this.questShareBtn.Controller.OnPress += this.questShareBtn_OnPress;
		this.buttonSpacing = this.showOnMapBtn.Position.x - this.trackBtn.Position.x;
		this.txtInput = (XUiC_TextInput)base.GetChildById("searchInput");
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.HandleOnChangedHandler;
			this.txtInput.Text = "";
		}
	}

	// Token: 0x06006D91 RID: 28049 RVA: 0x002CAF60 File Offset: 0x002C9160
	[PublicizedFrom(EAccessModifier.Private)]
	public void questShareBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_QuestEntry selectedEntry = this.questList.SelectedEntry;
		Quest selectedQuest = (selectedEntry != null) ? selectedEntry.Quest : null;
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		PartyQuests.ShareQuestWithParty(selectedQuest, entityPlayer, true);
	}

	// Token: 0x06006D92 RID: 28050 RVA: 0x002CAF9C File Offset: 0x002C919C
	[PublicizedFrom(EAccessModifier.Private)]
	public void showOnMapBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.questList.SelectedEntry != null)
		{
			Quest quest = this.questList.SelectedEntry.Quest;
			if (quest.HasPosition)
			{
				XUiC_WindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "map");
				((XUiC_MapArea)base.xui.GetWindow("mapArea").Controller).PositionMapAt(quest.Position);
				return;
			}
			GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, Localization.Get("ttQuestNoLocation", false), false, false, 0f);
		}
	}

	// Token: 0x06006D93 RID: 28051 RVA: 0x002CB03C File Offset: 0x002C923C
	[PublicizedFrom(EAccessModifier.Private)]
	public void trackBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiM_Quest questTracker = base.xui.QuestTracker;
		if (this.questList.SelectedEntry != null)
		{
			Quest quest = this.questList.SelectedEntry.Quest;
			if (quest.Active)
			{
				quest.Tracked = !quest.Tracked;
				base.xui.playerUI.entityPlayer.QuestJournal.TrackedQuest = (quest.Tracked ? quest : null);
				base.xui.playerUI.entityPlayer.QuestJournal.RefreshTracked();
			}
		}
	}

	// Token: 0x06006D94 RID: 28052 RVA: 0x002CB0CA File Offset: 0x002C92CA
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnChangedHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.filterText = _text;
		this.IsDirty = true;
	}

	// Token: 0x06006D95 RID: 28053 RVA: 0x002CB0DC File Offset: 0x002C92DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void questRemoveBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.questList.SelectedEntry != null)
		{
			base.xui.playerUI.entityPlayer.QuestJournal.RemoveQuest(this.questList.SelectedEntry.Quest);
			this.questList.SelectedEntry = null;
		}
	}

	// Token: 0x06006D96 RID: 28054 RVA: 0x002CB12C File Offset: 0x002C932C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.filterText = this.filterText.ToLower();
			this.currentItems = (from quest in this.player.QuestJournal.quests
			where this.filterText == "" || QuestClass.GetQuest(quest.ID).Name.ToLower().Contains(this.filterText)
			orderby !quest.Active, quest.FinishTime descending, QuestClass.GetQuest(quest.ID).Name
			select quest).ToList<Quest>();
			this.questList.SetQuestList(this.currentItems);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006D97 RID: 28055 RVA: 0x002CB20C File Offset: 0x002C940C
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.player.QuestAccepted += this.QuestJournal_QuestAccepted;
		this.player.QuestRemoved += this.QuestJournal_QuestRemoved;
		this.IsDirty = true;
	}

	// Token: 0x06006D98 RID: 28056 RVA: 0x002CB26A File Offset: 0x002C946A
	public override void OnClose()
	{
		base.OnClose();
		this.player.QuestAccepted -= this.QuestJournal_QuestAccepted;
		this.player.QuestRemoved -= this.QuestJournal_QuestRemoved;
	}

	// Token: 0x06006D99 RID: 28057 RVA: 0x002CB2A0 File Offset: 0x002C94A0
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "visible_quest_count")
		{
			this.questList.VisibleEntries = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06006D9A RID: 28058 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_QuestAccepted(Quest q)
	{
		this.IsDirty = true;
	}

	// Token: 0x06006D9B RID: 28059 RVA: 0x002CB2CE File Offset: 0x002C94CE
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_QuestRemoved(Quest q)
	{
		this.IsDirty = true;
		if (q == base.xui.QuestTracker.TrackedQuest)
		{
			base.xui.QuestTracker.TrackedQuest = null;
		}
	}

	// Token: 0x06006D9C RID: 28060 RVA: 0x002CB2FC File Offset: 0x002C94FC
	public void ShowTrackButton(bool _show)
	{
		this.trackBtn.IsVisible = _show;
		if (this.showingTrackButton != _show)
		{
			this.showingTrackButton = _show;
			this.trackBtn.Enabled = _show;
			Vector3 localPosition = this.showOnMapBtn.UiTransform.localPosition;
			Vector3 localPosition2 = this.questRemoveBtn.UiTransform.localPosition;
			if (_show)
			{
				this.showOnMapBtn.UiTransform.localPosition = new Vector3(localPosition.x + (float)this.buttonSpacing, localPosition.y, localPosition.z);
				this.questRemoveBtn.UiTransform.localPosition = new Vector3(localPosition2.x + (float)this.buttonSpacing, localPosition2.y, localPosition2.z);
				return;
			}
			this.showOnMapBtn.UiTransform.localPosition = new Vector3(localPosition.x - (float)this.buttonSpacing, localPosition.y, localPosition.z);
			this.questRemoveBtn.UiTransform.localPosition = new Vector3(localPosition2.x - (float)this.buttonSpacing, localPosition2.y, localPosition2.z);
		}
	}

	// Token: 0x06006D9D RID: 28061 RVA: 0x002CB416 File Offset: 0x002C9616
	public void ShowShareQuest(bool _show)
	{
		this.questShareBtn.IsVisible = (_show && !PartyQuests.AutoShare);
	}

	// Token: 0x06006D9E RID: 28062 RVA: 0x002CB431 File Offset: 0x002C9631
	public void ShowRemoveQuest(bool _show)
	{
		this.questRemoveBtn.IsVisible = _show;
	}

	// Token: 0x04005327 RID: 21287
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04005328 RID: 21288
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestListWindow.SearchTypes searchType;

	// Token: 0x04005329 RID: 21289
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestList questList;

	// Token: 0x0400532A RID: 21290
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button trackBtn;

	// Token: 0x0400532B RID: 21291
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button questRemoveBtn;

	// Token: 0x0400532C RID: 21292
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button showOnMapBtn;

	// Token: 0x0400532D RID: 21293
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button questShareBtn;

	// Token: 0x0400532E RID: 21294
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x0400532F RID: 21295
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Quest> currentItems;

	// Token: 0x04005330 RID: 21296
	[PublicizedFrom(EAccessModifier.Private)]
	public string filterText = "";

	// Token: 0x04005331 RID: 21297
	[PublicizedFrom(EAccessModifier.Private)]
	public int buttonSpacing;

	// Token: 0x04005332 RID: 21298
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showingTrackButton = true;

	// Token: 0x02000DAF RID: 3503
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SearchTypes
	{
		// Token: 0x04005334 RID: 21300
		All,
		// Token: 0x04005335 RID: 21301
		Active,
		// Token: 0x04005336 RID: 21302
		Completed
	}
}
