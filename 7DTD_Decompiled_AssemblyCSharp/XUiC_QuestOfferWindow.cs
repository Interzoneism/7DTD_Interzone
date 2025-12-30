using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DB5 RID: 3509
[Preserve]
public class XUiC_QuestOfferWindow : XUiController
{
	// Token: 0x17000B01 RID: 2817
	// (get) Token: 0x06006DC5 RID: 28101 RVA: 0x002CC3E3 File Offset: 0x002CA5E3
	// (set) Token: 0x06006DC6 RID: 28102 RVA: 0x002CC3EB File Offset: 0x002CA5EB
	public XUiC_QuestOfferWindow.OfferTypes OfferType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000B02 RID: 2818
	// (get) Token: 0x06006DC7 RID: 28103 RVA: 0x002CC3F4 File Offset: 0x002CA5F4
	// (set) Token: 0x06006DC8 RID: 28104 RVA: 0x002CC3FC File Offset: 0x002CA5FC
	public Quest Quest
	{
		get
		{
			return this.quest;
		}
		set
		{
			this.quest = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000B03 RID: 2819
	// (get) Token: 0x06006DC9 RID: 28105 RVA: 0x002CC40C File Offset: 0x002CA60C
	// (set) Token: 0x06006DCA RID: 28106 RVA: 0x002CC414 File Offset: 0x002CA614
	public int Variation
	{
		get
		{
			return this.variation;
		}
		set
		{
			this.variation = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000B04 RID: 2820
	// (get) Token: 0x06006DCB RID: 28107 RVA: 0x002CC424 File Offset: 0x002CA624
	// (set) Token: 0x06006DCC RID: 28108 RVA: 0x002CC42C File Offset: 0x002CA62C
	public XUiC_ItemStack ItemStackController { get; set; }

	// Token: 0x17000B05 RID: 2821
	// (get) Token: 0x06006DCD RID: 28109 RVA: 0x002CC435 File Offset: 0x002CA635
	// (set) Token: 0x06006DCE RID: 28110 RVA: 0x002CC43D File Offset: 0x002CA63D
	public int QuestGiverID { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06006DCF RID: 28111 RVA: 0x002CC448 File Offset: 0x002CA648
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "questname")
		{
			value = ((this.Quest != null) ? this.Quest.GetParsedText(this.Quest.QuestClass.Name) : "");
			return true;
		}
		if (bindingName == "questicon")
		{
			value = ((this.Quest != null) ? this.Quest.QuestClass.Icon : "");
			return true;
		}
		if (bindingName == "questoffer")
		{
			value = ((this.Quest != null) ? this.Quest.GetParsedText(this.Quest.QuestClass.Offer) : "");
			return true;
		}
		if (bindingName == "questdifficulty")
		{
			value = ((this.Quest != null) ? this.Quest.QuestClass.Difficulty : "");
			return true;
		}
		if (bindingName == "tieradd")
		{
			if (this.Quest != null && this.Quest.QuestClass.AddsToTierComplete)
			{
				if (!base.xui.playerUI.entityPlayer.QuestJournal.CanAddProgression)
				{
					value = "";
				}
				else
				{
					string arg = ((this.Quest.QuestClass.DifficultyTier > 0) ? "+" : "-") + this.Quest.QuestClass.DifficultyTier.ToString();
					value = string.Format(Localization.Get("xuiQuestTierAdd", false), arg);
				}
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (!(bindingName == "tieraddlimited"))
		{
			return false;
		}
		if (this.Quest != null && this.Quest.QuestClass.AddsToTierComplete && !base.xui.playerUI.entityPlayer.QuestJournal.CanAddProgression)
		{
			value = "true";
		}
		else
		{
			value = "false";
		}
		return true;
	}

	// Token: 0x06006DD0 RID: 28112 RVA: 0x002CC63C File Offset: 0x002CA83C
	public override void Init()
	{
		base.Init();
		this.btnAccept = base.GetChildById("btnAccept");
		this.btnAccept_Background = (XUiV_Button)this.btnAccept.GetChildById("clickable").ViewComponent;
		this.btnAccept_Background.Controller.OnPress += this.btnAccept_OnPress;
		this.btnAccept_Background.Controller.OnHover += this.btnAccept_OnHover;
		this.btnDecline = base.GetChildById("btnDecline");
		this.btnDecline_Background = (XUiV_Button)this.btnDecline.GetChildById("clickable").ViewComponent;
		this.btnDecline_Background.Controller.OnPress += this.btnDecline_OnPress;
		this.btnDecline_Background.Controller.OnHover += this.btnDecline_OnHover;
	}

	// Token: 0x06006DD1 RID: 28113 RVA: 0x002CC721 File Offset: 0x002CA921
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnAccept_OnHover(XUiController _sender, bool _isOver)
	{
		this.btnAcceptHovered = _isOver;
		base.RefreshBindings(false);
	}

	// Token: 0x06006DD2 RID: 28114 RVA: 0x002CC731 File Offset: 0x002CA931
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnDecline_OnHover(XUiController _sender, bool _isOver)
	{
		this.btnDeclineHovered = _isOver;
		base.RefreshBindings(false);
	}

	// Token: 0x06006DD3 RID: 28115 RVA: 0x002CC744 File Offset: 0x002CA944
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnAccept_OnPress(XUiController _sender, int _mouseButton)
	{
		Quest quest = this.Quest;
		quest.QuestGiverID = this.QuestGiverID;
		if (this.OfferType == XUiC_QuestOfferWindow.OfferTypes.Item)
		{
			ItemStack itemStack = this.ItemStackController.ItemStack;
			if (itemStack.count > 1)
			{
				itemStack.count--;
				this.ItemStackController.ForceSetItemStack(itemStack.Clone());
				this.ItemStackController.WindowGroup.Controller.SetAllChildrenDirty(false);
			}
			else
			{
				this.ItemStackController.ItemStack = ItemStack.Empty.Clone();
				this.ItemStackController.WindowGroup.Controller.SetAllChildrenDirty(false);
			}
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (this.QuestGiverID != -1)
		{
			base.xui.Dialog.Respondent.PlayVoiceSetEntry("quest_accepted", entityPlayer, true, true);
		}
		this.questAccepted = true;
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		entityPlayer.QuestJournal.AddQuest(quest, true);
	}

	// Token: 0x06006DD4 RID: 28116 RVA: 0x002CC850 File Offset: 0x002CAA50
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnDecline_OnPress(XUiController _sender, int _mouseButton)
	{
		EntityNPC respondent = base.xui.Dialog.Respondent;
		if (this.QuestGiverID != -1)
		{
			base.xui.Dialog.Respondent.PlayVoiceSetEntry("quest_declined", base.xui.playerUI.entityPlayer, true, true);
		}
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		if (this.OnCancel != null)
		{
			this.OnCancel(respondent);
		}
	}

	// Token: 0x06006DD5 RID: 28117 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeButton_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006DD6 RID: 28118 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006DD7 RID: 28119 RVA: 0x002CC8D8 File Offset: 0x002CAAD8
	public static XUiC_QuestOfferWindow OpenQuestOfferWindow(XUi xui, Quest q, int listIndex = -1, XUiC_QuestOfferWindow.OfferTypes offerType = XUiC_QuestOfferWindow.OfferTypes.Item, int questGiverID = -1, Action<EntityNPC> onCancel = null)
	{
		bool flag = offerType == XUiC_QuestOfferWindow.OfferTypes.Item;
		XUiC_QuestOfferWindow childByType = xui.FindWindowGroupByName("questOffer").GetChildByType<XUiC_QuestOfferWindow>();
		childByType.Quest = q;
		childByType.Variation = -1;
		childByType.listIndex = listIndex;
		childByType.QuestGiverID = questGiverID;
		childByType.OfferType = offerType;
		childByType.OnCancel = onCancel;
		xui.playerUI.windowManager.Open("questOffer", flag, false, flag);
		return childByType;
	}

	// Token: 0x06006DD8 RID: 28120 RVA: 0x002CC940 File Offset: 0x002CAB40
	public override void OnOpen()
	{
		base.OnOpen();
		this.questAccepted = false;
		Manager.PlayInsidePlayerHead("quest_note_offer", -1, 0f, false, false);
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		base.xui.playerUI.windowManager.CloseIfOpen("toolbelt");
		if (this.OfferType == XUiC_QuestOfferWindow.OfferTypes.Dialog)
		{
			Dialog currentDialog = base.xui.Dialog.DialogWindowGroup.CurrentDialog;
			base.xui.Dialog.DialogWindowGroup.RefreshDialog();
			base.xui.Dialog.DialogWindowGroup.ShowResponseWindow(false);
			if (this.QuestGiverID != -1)
			{
				base.xui.Dialog.Respondent.PlayVoiceSetEntry("quest_offer", base.xui.playerUI.entityPlayer, true, true);
			}
		}
	}

	// Token: 0x06006DD9 RID: 28121 RVA: 0x002CCA20 File Offset: 0x002CAC20
	public override void OnClose()
	{
		base.OnClose();
		if (!this.questAccepted)
		{
			Manager.PlayInsidePlayerHead("quest_note_decline", -1, 0f, false, false);
		}
		if (this.ItemStackController != null)
		{
			this.ItemStackController.QuestLock = false;
			this.ItemStackController = null;
		}
		if (this.OfferType == XUiC_QuestOfferWindow.OfferTypes.Dialog)
		{
			if (this.questAccepted)
			{
				EntityTrader entityTrader = base.xui.Dialog.Respondent as EntityTrader;
				if (entityTrader != null && entityTrader.activeQuests != null)
				{
					entityTrader.activeQuests.Remove(this.Quest);
				}
				if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageNPCQuestList>().Setup(this.QuestGiverID, base.xui.playerUI.entityPlayer.entityId, (int)this.Quest.QuestClass.DifficultyTier, (byte)this.listIndex), false);
				}
				if (this.Quest.QuestTags.Test_AnySet(QuestEventManager.treasureTag) && GameSparksCollector.CollectGamePlayData)
				{
					GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.QuestAcceptedDistance, ((int)Vector3.Distance(this.Quest.Position, base.xui.Dialog.Respondent.position) / 50 * 50).ToString(), 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
				}
			}
			Dialog currentDialog = base.xui.Dialog.DialogWindowGroup.CurrentDialog;
			if (currentDialog.CurrentStatement == null || currentDialog.CurrentStatement.NextStatementID == "")
			{
				base.xui.playerUI.windowManager.Close("dialog");
				return;
			}
			currentDialog.CurrentStatement = currentDialog.GetStatement(currentDialog.CurrentStatement.NextStatementID);
			base.xui.Dialog.DialogWindowGroup.RefreshDialog();
			base.xui.Dialog.DialogWindowGroup.ShowResponseWindow(true);
		}
	}

	// Token: 0x0400535E RID: 21342
	[PublicizedFrom(EAccessModifier.Private)]
	public bool btnAcceptHovered;

	// Token: 0x0400535F RID: 21343
	[PublicizedFrom(EAccessModifier.Private)]
	public bool btnDeclineHovered;

	// Token: 0x04005360 RID: 21344
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnAccept;

	// Token: 0x04005361 RID: 21345
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnDecline;

	// Token: 0x04005362 RID: 21346
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnAccept_Background;

	// Token: 0x04005363 RID: 21347
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnDecline_Background;

	// Token: 0x04005364 RID: 21348
	[PublicizedFrom(EAccessModifier.Private)]
	public bool questAccepted;

	// Token: 0x04005365 RID: 21349
	[PublicizedFrom(EAccessModifier.Private)]
	public Action<EntityNPC> OnCancel;

	// Token: 0x04005366 RID: 21350
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest quest;

	// Token: 0x04005367 RID: 21351
	[PublicizedFrom(EAccessModifier.Private)]
	public int variation = -1;

	// Token: 0x04005368 RID: 21352
	[PublicizedFrom(EAccessModifier.Private)]
	public int listIndex = -1;

	// Token: 0x0400536B RID: 21355
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastAnyKey = true;

	// Token: 0x02000DB6 RID: 3510
	public enum OfferTypes
	{
		// Token: 0x0400536D RID: 21357
		Item,
		// Token: 0x0400536E RID: 21358
		Dialog
	}
}
