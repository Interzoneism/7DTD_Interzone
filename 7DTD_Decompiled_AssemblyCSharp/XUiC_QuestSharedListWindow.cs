using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DBF RID: 3519
[Preserve]
public class XUiC_QuestSharedListWindow : XUiController
{
	// Token: 0x06006E1C RID: 28188 RVA: 0x002CE1DC File Offset: 0x002CC3DC
	public override void Init()
	{
		base.Init();
		this.questList = base.GetChildByType<XUiC_QuestSharedList>();
		this.acceptBtn = (XUiV_Button)base.GetChildById("acceptBtn").ViewComponent;
		this.acceptBtn.Controller.OnPress += this.acceptBtn_OnPress;
		this.showOnMapBtn = (XUiV_Button)base.GetChildById("showOnMapBtn").ViewComponent;
		this.showOnMapBtn.Controller.OnPress += this.showOnMapBtn_OnPress;
		this.questRemoveBtn = (XUiV_Button)base.GetChildById("questRemoveBtn").ViewComponent;
		this.questRemoveBtn.Controller.OnPress += this.questRemoveBtn_OnPress;
		this.buttonSpacing = this.showOnMapBtn.Position.x - this.acceptBtn.Position.x;
		this.txtInput = (XUiC_TextInput)base.GetChildById("searchInput");
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.HandleOnChangedHandler;
			this.txtInput.Text = "";
		}
	}

	// Token: 0x06006E1D RID: 28189 RVA: 0x002CE30C File Offset: 0x002CC50C
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

	// Token: 0x06006E1E RID: 28190 RVA: 0x002CE3AC File Offset: 0x002CC5AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void acceptBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_QuestEntry selectedEntry = this.questList.SelectedEntry;
		SharedQuestEntry sharedQuest = (selectedEntry != null) ? selectedEntry.SharedQuestEntry : null;
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		PartyQuests.AcceptSharedQuest(sharedQuest, entityPlayer);
	}

	// Token: 0x06006E1F RID: 28191 RVA: 0x002CE3E7 File Offset: 0x002CC5E7
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnChangedHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.filterText = _text;
		this.IsDirty = true;
	}

	// Token: 0x06006E20 RID: 28192 RVA: 0x002CE3F7 File Offset: 0x002CC5F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void questRemoveBtn_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.questList.SelectedEntry != null)
		{
			base.xui.playerUI.entityPlayer.QuestJournal.RemoveSharedQuestEntry(this.questList.SelectedEntry.SharedQuestEntry);
		}
	}

	// Token: 0x06006E21 RID: 28193 RVA: 0x002CE430 File Offset: 0x002CC630
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.filterText = this.filterText.ToLower();
			this.currentItems = (from quest in this.player.QuestJournal.sharedQuestEntries
			where this.filterText == "" || quest.QuestClass.Name.ToLower().Contains(this.filterText)
			select quest).ToList<SharedQuestEntry>();
			this.questList.SetSharedQuestList(this.currentItems);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006E22 RID: 28194 RVA: 0x002CE4A4 File Offset: 0x002CC6A4
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.player.SharedQuestAdded += this.QuestJournal_SharedQuestAdded;
		this.player.SharedQuestRemoved += this.QuestJournal_SharedQuestRemoved;
		this.IsDirty = true;
	}

	// Token: 0x06006E23 RID: 28195 RVA: 0x002CE502 File Offset: 0x002CC702
	public override void OnClose()
	{
		base.OnClose();
		this.player.SharedQuestAdded -= this.QuestJournal_SharedQuestAdded;
		this.player.SharedQuestRemoved -= this.QuestJournal_SharedQuestRemoved;
	}

	// Token: 0x06006E24 RID: 28196 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_SharedQuestAdded(SharedQuestEntry entry)
	{
		this.IsDirty = true;
	}

	// Token: 0x06006E25 RID: 28197 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_SharedQuestRemoved(SharedQuestEntry entry)
	{
		this.IsDirty = true;
	}

	// Token: 0x06006E26 RID: 28198 RVA: 0x002CE538 File Offset: 0x002CC738
	public void ShowTrackButton(bool _show)
	{
		this.acceptBtn.IsVisible = _show;
		if (this.showingTrackButton != _show)
		{
			this.showingTrackButton = _show;
			this.acceptBtn.Enabled = _show;
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

	// Token: 0x040053A6 RID: 21414
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x040053A7 RID: 21415
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestSharedListWindow.SearchTypes searchType;

	// Token: 0x040053A8 RID: 21416
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestSharedList questList;

	// Token: 0x040053A9 RID: 21417
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button acceptBtn;

	// Token: 0x040053AA RID: 21418
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button questRemoveBtn;

	// Token: 0x040053AB RID: 21419
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button showOnMapBtn;

	// Token: 0x040053AC RID: 21420
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x040053AD RID: 21421
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SharedQuestEntry> currentItems;

	// Token: 0x040053AE RID: 21422
	[PublicizedFrom(EAccessModifier.Private)]
	public string filterText = "";

	// Token: 0x040053AF RID: 21423
	[PublicizedFrom(EAccessModifier.Private)]
	public int buttonSpacing;

	// Token: 0x040053B0 RID: 21424
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showingTrackButton = true;

	// Token: 0x02000DC0 RID: 3520
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SearchTypes
	{
		// Token: 0x040053B2 RID: 21426
		All,
		// Token: 0x040053B3 RID: 21427
		Active,
		// Token: 0x040053B4 RID: 21428
		Completed
	}
}
