using System;
using System.Runtime.CompilerServices;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000DCC RID: 3532
[Preserve]
public class XUiC_QuestWindowGroup : XUiController
{
	// Token: 0x06006E89 RID: 28297 RVA: 0x002D1974 File Offset: 0x002CFB74
	public override void Init()
	{
		base.Init();
		this.objectivesWindow = base.GetChildByType<XUiC_QuestObjectivesWindow>();
		this.rewardsWindow = base.GetChildByType<XUiC_QuestRewardsWindow>();
		this.descriptionWindow = base.GetChildByType<XUiC_QuestDescriptionWindow>();
		XUiC_QuestListWindow childByType = base.GetChildByType<XUiC_QuestListWindow>();
		this.questList = ((childByType != null) ? childByType.GetChildByType<XUiC_QuestList>() : null);
		XUiC_QuestSharedListWindow childByType2 = base.GetChildByType<XUiC_QuestSharedListWindow>();
		this.sharedList = ((childByType2 != null) ? childByType2.GetChildByType<XUiC_QuestSharedList>() : null);
		if (this.questList != null)
		{
			this.questList.SharedList = this.sharedList;
		}
		if (this.sharedList != null)
		{
			this.sharedList.QuestList = this.questList;
		}
	}

	// Token: 0x06006E8A RID: 28298 RVA: 0x002D1A0D File Offset: 0x002CFC0D
	public void SetQuest(XUiC_QuestEntry q)
	{
		this.objectivesWindow.SetQuest(q);
		this.rewardsWindow.SetQuest(q);
		this.descriptionWindow.SetQuest(q);
	}

	// Token: 0x06006E8B RID: 28299 RVA: 0x002D1A34 File Offset: 0x002CFC34
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
		XUiController xuiController = base.xui.FindWindowGroupByName("windowpaging");
		if (xuiController != null)
		{
			XUiC_WindowSelector childByType = xuiController.GetChildByType<XUiC_WindowSelector>();
			if (childByType != null)
			{
				childByType.SetSelected("quests");
			}
		}
		base.RefreshBindings(true);
		this.AsyncUISelectionOnOpen();
	}

	// Token: 0x06006E8C RID: 28300 RVA: 0x002D1AF4 File Offset: 0x002CFCF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void AsyncUISelectionOnOpen()
	{
		XUiC_QuestWindowGroup.<AsyncUISelectionOnOpen>d__8 <AsyncUISelectionOnOpen>d__;
		<AsyncUISelectionOnOpen>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AsyncUISelectionOnOpen>d__.<>4__this = this;
		<AsyncUISelectionOnOpen>d__.<>1__state = -1;
		<AsyncUISelectionOnOpen>d__.<>t__builder.Start<XUiC_QuestWindowGroup.<AsyncUISelectionOnOpen>d__8>(ref <AsyncUISelectionOnOpen>d__);
	}

	// Token: 0x06006E8D RID: 28301 RVA: 0x0026DB0F File Offset: 0x0026BD0F
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
	}

	// Token: 0x06006E8E RID: 28302 RVA: 0x002D1B2C File Offset: 0x002CFD2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "questsautoshare")
		{
			_value = PartyQuests.AutoShare.ToString();
			return true;
		}
		if (_bindingName == "questsautoaccept")
		{
			_value = PartyQuests.AutoAccept.ToString();
			return true;
		}
		if (!(_bindingName == "queststier"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (entityPlayer != null)
		{
			int currentFactionTier = entityPlayer.QuestJournal.GetCurrentFactionTier(1, 0, false);
			_value = string.Format(Localization.Get("xuiQuestTierDescription", false), ValueDisplayFormatters.RomanNumber(entityPlayer.QuestJournal.GetCurrentFactionTier(1, 0, false)), entityPlayer.QuestJournal.GetQuestFactionPoints(1), entityPlayer.QuestJournal.GetQuestFactionMax(1, currentFactionTier));
		}
		return true;
	}

	// Token: 0x04005409 RID: 21513
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestList questList;

	// Token: 0x0400540A RID: 21514
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestSharedList sharedList;

	// Token: 0x0400540B RID: 21515
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestObjectivesWindow objectivesWindow;

	// Token: 0x0400540C RID: 21516
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestRewardsWindow rewardsWindow;

	// Token: 0x0400540D RID: 21517
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestDescriptionWindow descriptionWindow;
}
