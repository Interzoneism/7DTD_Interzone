using System;
using Audio;
using Challenges;
using Platform;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E87 RID: 3719
[Preserve]
public class XUiC_TwitchEntryDescriptionWindow : XUiController
{
	// Token: 0x17000BEE RID: 3054
	// (get) Token: 0x0600751D RID: 29981 RVA: 0x002F9F6E File Offset: 0x002F816E
	// (set) Token: 0x0600751E RID: 29982 RVA: 0x002F9F76 File Offset: 0x002F8176
	public TwitchAction CurrentTwitchActionEntry
	{
		get
		{
			return this.currentTwitchActionEntry;
		}
		set
		{
			this.currentTwitchActionEntry = value;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x17000BEF RID: 3055
	// (get) Token: 0x0600751F RID: 29983 RVA: 0x002F9F86 File Offset: 0x002F8186
	// (set) Token: 0x06007520 RID: 29984 RVA: 0x002F9F8E File Offset: 0x002F818E
	public TwitchVote CurrentTwitchVoteEntry
	{
		get
		{
			return this.currentTwitchVoteEntry;
		}
		set
		{
			this.currentTwitchVoteEntry = value;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x17000BF0 RID: 3056
	// (get) Token: 0x06007521 RID: 29985 RVA: 0x002F9F9E File Offset: 0x002F819E
	// (set) Token: 0x06007522 RID: 29986 RVA: 0x002F9FA6 File Offset: 0x002F81A6
	public TwitchActionHistoryEntry CurrentTwitchActionHistoryEntry
	{
		get
		{
			return this.currentTwitchActionHistoryEntry;
		}
		set
		{
			this.currentTwitchActionHistoryEntry = value;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06007523 RID: 29987 RVA: 0x002F9FB8 File Offset: 0x002F81B8
	public override void Init()
	{
		base.Init();
		XUiController childById = ((XUiC_TwitchInfoWindowGroup)this.windowGroup.Controller).GetChildByType<XUiC_TwitchHowToWindow>().GetChildById("leftButton");
		XUiController childById2 = this.windowGroup.Controller.GetChildById("windowTwitchInfoDescription");
		XUiController childById3 = childById2.GetChildById("btnEnable");
		XUiController childById4 = childById2.GetChildById("statClick");
		this.btnEnable = (XUiV_Button)childById3.GetChildById("clickable").ViewComponent;
		this.btnEnable.Controller.OnPress += this.btnEnable_OnPress;
		this.btnEnable.NavDownTarget = childById.ViewComponent;
		childById3 = childById2.GetChildById("btnRefund");
		this.btnRefund = (XUiV_Button)childById3.GetChildById("clickable").ViewComponent;
		this.btnRefund.Controller.OnPress += this.btnRefund_OnPress;
		this.btnRetry = childById2.GetChildById("btnRetry").GetChildByType<XUiC_SimpleButton>();
		this.btnRetry.OnPressed += this.btnRetry_OnPress;
		this.btnRetry.ViewComponent.NavDownTarget = childById.ViewComponent;
		childById2.GetChildById("btnIncrease").GetChildByType<XUiC_SimpleButton>().OnPressed += this.BtnIncrease_OnPressed;
		childById2.GetChildById("btnDecrease").GetChildByType<XUiC_SimpleButton>().OnPressed += this.BtnDecrease_OnPressed;
		childById4.OnPress += this.RectStat_OnPress;
		this.lblStartGamestage = Localization.Get("TwitchInfo_ActionStartGamestage", false);
		this.lblEndGamestage = Localization.Get("TwitchInfo_ActionEndGamestage", false);
		this.lblPointCost = Localization.Get("TwitchInfo_ActionPointCost", false);
		this.lblDiscountCost = Localization.Get("TwitchInfo_ActionDiscountCost", false);
		this.lblCooldown = Localization.Get("TwitchInfo_ActionCooldown", false);
		this.lblRandomDaily = Localization.Get("TwitchInfo_ActionRandomDaily", false);
		this.lblIsPositive = Localization.Get("TwitchInfo_ActionIsPositive", false);
		this.lblPointType = Localization.Get("TwitchInfo_ActionPointType", false);
		this.lblEnableAction = Localization.Get("TwitchInfo_ActionEnableAction", false) + " ([action:gui:GUI D-Pad Up])";
		this.lblDisableAction = Localization.Get("TwitchInfo_ActionDisableAction", false) + " ([action:gui:GUI D-Pad Up])";
		this.lblIncreasePrice = Localization.Get("TwitchInfo_IncreasePriceButton", false) + " ([action:gui:GUI D-Pad Right])";
		this.lblDecreasePrice = Localization.Get("TwitchInfo_DecreasePriceButton", false) + " ([action:gui:GUI D-Pad Left])";
		this.lblEnableVote = Localization.Get("TwitchInfo_ActionEnableVote", false) + " ([action:gui:GUI D-Pad Up])";
		this.lblDisableVote = Localization.Get("TwitchInfo_ActionDisableVote", false) + " ([action:gui:GUI D-Pad Up])";
		this.lblEnableAction_Controller = Localization.Get("TwitchInfo_ActionEnableAction", false) + " [action:gui:GUI HalfStack]";
		this.lblDisableAction_Controller = Localization.Get("TwitchInfo_ActionDisableAction", false) + " [action:gui:GUI HalfStack]";
		this.lblIncreasePrice_Controller = Localization.Get("TwitchInfo_IncreasePriceButton", false) + " [action:gui:GUI Inspect] + [action:gui:GUI D-Pad Right]";
		this.lblDecreasePrice_Controller = Localization.Get("TwitchInfo_DecreasePriceButton", false) + " [action:gui:GUI Inspect] + [action:gui:GUI D-Pad Left]";
		this.lblEnableVote_Controller = Localization.Get("TwitchInfo_ActionEnableVote", false) + " [action:gui:GUI HalfStack]";
		this.lblDisableVote_Controller = Localization.Get("TwitchInfo_ActionDisableVote", false) + " [action:gui:GUI HalfStack]";
		this.lblActionEmpty = Localization.Get("TwitchInfo_ActionEmpty", false);
		this.lblVoteEmpty = Localization.Get("TwitchInfo_VoteEmpty", false);
		this.lblActionHistoryEmpty = Localization.Get("TwitchInfo_ActionHistoryEmpty", false);
		this.lblLeaderboardEmpty = Localization.Get("TwitchInfo_LeaderboardEmpty", false);
		this.lblHistoryTargetTitle = Localization.Get("TwitchInfo_ActionHistoryTarget", false);
		this.lblHistoryStateTitle = Localization.Get("xuiLightPropState", false);
		this.lblHistoryTimeStampTitle = Localization.Get("ObjectiveTime_keyword", false);
		this.lblRefund = Localization.Get("TwitchInfo_ActionHistoryRefund", false);
		this.lblNoRefund = Localization.Get("TwitchInfo_ActionHistoryRefundNotAvailable", false);
		this.lblRetry = Localization.Get("TwitchInfo_ActionHistoryRetry", false);
		this.lblNoRetry = Localization.Get("TwitchInfo_ActionHistoryRetryNotAvailable", false);
		this.lblRetryActionUnavailable = Localization.Get("TwitchInfo_ActionHistoryRetryActionUnavailable", false);
		this.lblLeaderboardStats = Localization.Get("TwitchInfo_LeaderboardStats", false);
		this.lblShowBitTotal = Localization.Get("TwitchInfo_LeaderboardShowBitTotal", false);
		this.lblTopKiller = Localization.Get("TwitchInfo_TopKiller", false);
		this.lblTopGood = Localization.Get("TwitchInfo_TopGood", false);
		this.lblTopEvil = Localization.Get("TwitchInfo_TopEvil", false);
		this.lblCurrentGood = Localization.Get("TwitchInfo_CurrentGood", false);
		this.lblMostBits = Localization.Get("TwitchInfo_MostBits", false);
		this.lblTotalBits = Localization.Get("TwitchInfo_TotalBits", false);
		this.lblTotalBad = Localization.Get("TwitchInfo_TotalBad", false);
		this.lblTotalGood = Localization.Get("TwitchInfo_TotalGood", false);
		this.lblTotalActions = Localization.Get("TwitchInfo_TotalActions", false);
		this.lblLargestPimpPot = Localization.Get("TwitchInfo_LargestPimpPot", false);
		this.lblTrue = Localization.Get("statTrue", false);
		this.lblFalse = Localization.Get("statFalse", false);
		this.lblPointsPP = Localization.Get("TwitchPoints_PP", false);
		this.lblPointsSP = Localization.Get("TwitchPoints_SP", false);
		this.lblPointsBits = Localization.Get("TwitchPoints_Bits", false);
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06007524 RID: 29988 RVA: 0x002FA4F8 File Offset: 0x002F86F8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.ViewComponent.UiTransform.gameObject.activeInHierarchy && (this.actionEntry != null || this.voteEntry != null))
		{
			PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && !base.xui.playerUI.windowManager.IsInputActive())
			{
				if (this.actionEntry != null)
				{
					if (guiactions.DPad_Up.WasPressed)
					{
						this.btnEnable_OnPress(this.btnEnable.Controller, -1);
					}
					if (guiactions.DPad_Left.WasPressed)
					{
						this.BtnDecrease_OnPressed(null, -1);
					}
					if (guiactions.DPad_Right.WasPressed)
					{
						this.BtnIncrease_OnPressed(null, -1);
						return;
					}
				}
				else if (this.voteEntry != null && guiactions.DPad_Up.WasPressed)
				{
					this.btnEnable_OnPress(this.btnEnable.Controller, -1);
					return;
				}
			}
			else if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
			{
				if (this.actionEntry != null)
				{
					if (guiactions.HalfStack.WasPressed)
					{
						this.btnEnable_OnPress(this.btnEnable.Controller, -1);
					}
					if (guiactions.Inspect.IsPressed)
					{
						if (guiactions.DPad_Left.WasPressed)
						{
							this.BtnDecrease_OnPressed(null, -1);
						}
						if (guiactions.DPad_Right.WasPressed)
						{
							this.BtnIncrease_OnPressed(null, -1);
							return;
						}
					}
				}
				else if (this.voteEntry != null && guiactions.HalfStack.WasPressed)
				{
					this.btnEnable_OnPress(this.btnEnable.Controller, -1);
				}
			}
		}
	}

	// Token: 0x06007525 RID: 29989 RVA: 0x002FA69C File Offset: 0x002F889C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDecrease_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.currentTwitchActionEntry != null)
		{
			this.currentTwitchActionEntry.DecreaseCost();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007526 RID: 29990 RVA: 0x002FA6B8 File Offset: 0x002F88B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnIncrease_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.currentTwitchActionEntry != null)
		{
			this.currentTwitchActionEntry.IncreaseCost();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007527 RID: 29991 RVA: 0x002FA6D4 File Offset: 0x002F88D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void RectStat_OnPress(XUiController _sender, int _mouseButton)
	{
		if (!this.showBitTotal)
		{
			this.showBitTotal = true;
			base.RefreshBindings(false);
			_sender.ViewComponent.EventOnPress = false;
		}
	}

	// Token: 0x06007528 RID: 29992 RVA: 0x002FA6F8 File Offset: 0x002F88F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnRefund_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.currentTwitchActionHistoryEntry != null)
		{
			this.currentTwitchActionHistoryEntry.Refund();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007529 RID: 29993 RVA: 0x002FA714 File Offset: 0x002F8914
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnRetry_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.currentTwitchActionHistoryEntry != null)
		{
			this.currentTwitchActionHistoryEntry.Retry();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x0600752A RID: 29994 RVA: 0x002FA730 File Offset: 0x002F8930
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnEnable_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.currentTwitchActionEntry != null)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			TwitchActionPreset currentActionPreset = twitchManager.CurrentActionPreset;
			if (this.currentTwitchActionEntry.IsInPresetDefault(currentActionPreset))
			{
				if (currentActionPreset.RemovedActions.Contains(this.currentTwitchActionEntry.Name))
				{
					currentActionPreset.RemovedActions.Remove(this.currentTwitchActionEntry.Name);
				}
				else
				{
					currentActionPreset.RemovedActions.Add(this.currentTwitchActionEntry.Name);
				}
			}
			else if (currentActionPreset.AddedActions.Contains(this.currentTwitchActionEntry.Name))
			{
				currentActionPreset.AddedActions.Remove(this.currentTwitchActionEntry.Name);
			}
			else
			{
				currentActionPreset.AddedActions.Add(this.currentTwitchActionEntry.Name);
				if (this.currentTwitchActionEntry.DisplayCategory.Name == "Extras")
				{
					QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.EnableExtras, this.currentTwitchActionEntry.DisplayCategory.Name);
				}
			}
			twitchManager.HandleChangedPropertyList();
			Manager.PlayInsidePlayerHead("craft_click_craft", -1, 0f, false, false);
			twitchManager.SetupAvailableCommands();
			twitchManager.HandleCooldownActionLocking();
			base.RefreshBindings(false);
			this.actionEntry.RefreshBindings(false);
			return;
		}
		if (this.currentTwitchVoteEntry != null)
		{
			this.currentTwitchVoteEntry.Enabled = !this.currentTwitchVoteEntry.Enabled;
			TwitchManager.Current.HandleChangedPropertyList();
			Manager.PlayInsidePlayerHead("craft_click_craft", -1, 0f, false, false);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x0600752B RID: 29995 RVA: 0x002FA8A7 File Offset: 0x002F8AA7
	public override void OnOpen()
	{
		base.OnOpen();
		this.stats = TwitchManager.LeaderboardStats;
		this.stats.StatsChanged += this.Stats_StatsChanged;
		base.RefreshBindings(false);
	}

	// Token: 0x0600752C RID: 29996 RVA: 0x002FA8D8 File Offset: 0x002F8AD8
	public override void OnClose()
	{
		base.OnClose();
		this.stats.StatsChanged -= this.Stats_StatsChanged;
		TwitchManager.Current.HandleChangedPropertyList();
		TwitchManager.Current.ResetPrices();
	}

	// Token: 0x0600752D RID: 29997 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void Stats_StatsChanged()
	{
		base.RefreshBindings(false);
	}

	// Token: 0x0600752E RID: 29998 RVA: 0x002FA90C File Offset: 0x002F8B0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2236179277U)
		{
			if (num <= 1143702022U)
			{
				if (num <= 750543317U)
				{
					if (num <= 337301254U)
					{
						if (num <= 118022627U)
						{
							if (num != 59652435U)
							{
								if (num == 118022627U)
								{
									if (bindingName == "showhistory_event")
									{
										value = (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.EventEntry != null).ToString();
										return true;
									}
								}
							}
							else if (bindingName == "actionrandomgroup")
							{
								value = ((this.currentTwitchActionEntry != null) ? ((this.currentTwitchActionEntry.RandomGroup != "") ? this.lblTrue : this.lblFalse) : "");
								return true;
							}
						}
						else if (num != 140690669U)
						{
							if (num == 337301254U)
							{
								if (bindingName == "voteendgamestage")
								{
									if (this.currentTwitchVoteEntry != null && this.currentTwitchVoteEntry.EndGameStage > 0)
									{
										value = this.currentTwitchVoteEntry.EndGameStage.ToString();
									}
									else
									{
										value = "";
									}
									return true;
								}
							}
						}
						else if (bindingName == "actiondefaultcost")
						{
							if (this.currentTwitchActionEntry != null)
							{
								value = this.currentTwitchActionEntry.ModifiedCost.ToString();
							}
							else if (this.currentTwitchActionHistoryEntry != null)
							{
								value = this.currentTwitchActionHistoryEntry.PointsSpent.ToString();
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
					else if (num <= 422696738U)
					{
						if (num != 362072451U)
						{
							if (num == 422696738U)
							{
								if (bindingName == "currentgood_title")
								{
									value = ((this.stats != null) ? string.Format(this.lblCurrentGood, this.stats.GoodRewardTime) : "");
									return true;
								}
							}
						}
						else if (bindingName == "sessiontotalgood_title")
						{
							value = this.lblTotalGood;
							return true;
						}
					}
					else if (num != 718761959U)
					{
						if (num == 750543317U)
						{
							if (bindingName == "decreasepricetext")
							{
								value = "";
								if (this.currentTwitchActionEntry != null)
								{
									value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblDecreasePrice : this.lblDecreasePrice_Controller);
								}
								return true;
							}
						}
					}
					else if (bindingName == "entrydescription")
					{
						if (this.currentTwitchActionEntry != null)
						{
							value = this.currentTwitchActionEntry.Description;
						}
						else if (this.currentTwitchVoteEntry != null)
						{
							value = this.currentTwitchVoteEntry.Description;
						}
						else if (this.currentTwitchActionHistoryEntry != null)
						{
							value = this.currentTwitchActionHistoryEntry.Description;
						}
						else
						{
							value = "";
						}
						return true;
					}
				}
				else if (num <= 853102448U)
				{
					if (num <= 825770997U)
					{
						if (num != 782028412U)
						{
							if (num == 825770997U)
							{
								if (bindingName == "sessionkiller_title")
								{
									value = this.lblTopKiller;
									return true;
								}
							}
						}
						else if (bindingName == "actioncommand")
						{
							if (this.currentTwitchActionEntry != null)
							{
								value = this.currentTwitchActionEntry.Command;
							}
							else if (this.currentTwitchActionHistoryEntry != null)
							{
								value = this.currentTwitchActionHistoryEntry.UserName;
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
					else if (num != 831944002U)
					{
						if (num == 853102448U)
						{
							if (bindingName == "historystate")
							{
								value = ((this.currentTwitchActionHistoryEntry != null) ? this.currentTwitchActionHistoryEntry.EntryState.ToString() : "");
								return true;
							}
						}
					}
					else if (bindingName == "historytarget")
					{
						if (this.currentTwitchActionHistoryEntry != null)
						{
							value = this.currentTwitchActionHistoryEntry.Target;
						}
						else
						{
							value = "";
						}
						return true;
					}
				}
				else if (num <= 900437321U)
				{
					if (num != 884373603U)
					{
						if (num == 900437321U)
						{
							if (bindingName == "leaderboard_goodperson")
							{
								value = ((this.stats != null && this.stats.TopGoodViewer != null) ? string.Format("[{0}]{1}[-] ({2})", this.stats.TopGoodViewer.UserColor, this.stats.TopGoodViewer.Name, this.stats.TopGoodViewer.GoodActions) : "--");
								return true;
							}
						}
					}
					else if (bindingName == "showleaderboard")
					{
						value = (this.OwnerList != null && this.OwnerList.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard).ToString();
						return true;
					}
				}
				else if (num != 930150597U)
				{
					if (num != 1079842190U)
					{
						if (num == 1143702022U)
						{
							if (bindingName == "historystatetitle")
							{
								value = this.lblHistoryStateTitle;
								return true;
							}
						}
					}
					else if (bindingName == "leaderboard_mostbits")
					{
						value = ((this.stats != null && this.stats.MostBitsSpentViewer != null) ? string.Format("[{0}]{1}[-] ({2})", this.stats.MostBitsSpentViewer.UserColor, this.stats.MostBitsSpentViewer.Name, this.stats.MostBitsSpentViewer.BitsUsed) : "--");
						return true;
					}
				}
				else if (bindingName == "leaderboard_totalbits")
				{
					if (this.showBitTotal)
					{
						value = ((this.stats != null) ? this.stats.TotalBits.ToString() : "0");
					}
					else
					{
						value = string.Format("<{0}>", this.lblShowBitTotal);
					}
					return true;
				}
			}
			else if (num <= 1548958396U)
			{
				if (num <= 1300396129U)
				{
					if (num <= 1182727743U)
					{
						if (num != 1152134642U)
						{
							if (num == 1182727743U)
							{
								if (bindingName == "leaderboard_currentgood")
								{
									value = ((this.stats != null && this.stats.CurrentGoodViewer != null) ? string.Format("[{0}]{1}[-] ({2})", this.stats.CurrentGoodViewer.UserColor, this.stats.CurrentGoodViewer.Name, this.stats.CurrentGoodViewer.CurrentGoodActions) : "--");
									return true;
								}
							}
						}
						else if (bindingName == "entryicon")
						{
							value = "";
							if (this.currentTwitchActionEntry != null && this.currentTwitchActionEntry.MainCategory != null)
							{
								value = this.currentTwitchActionEntry.MainCategory.Icon;
							}
							else if (this.currentTwitchVoteEntry != null && this.currentTwitchVoteEntry.MainVoteType != null)
							{
								value = this.currentTwitchVoteEntry.MainVoteType.Icon;
							}
							else if (this.currentTwitchActionHistoryEntry != null)
							{
								if (this.currentTwitchActionHistoryEntry.Action != null)
								{
									TwitchAction action = this.currentTwitchActionHistoryEntry.Action;
									if (action.MainCategory != null)
									{
										value = action.MainCategory.Icon;
									}
								}
								else if (this.currentTwitchActionHistoryEntry.Vote != null)
								{
									TwitchVoteType mainVoteType = this.currentTwitchActionHistoryEntry.Vote.MainVoteType;
									if (mainVoteType != null)
									{
										value = mainVoteType.Icon;
									}
								}
							}
							return true;
						}
					}
					else if (num != 1187804961U)
					{
						if (num == 1300396129U)
						{
							if (bindingName == "votestartgamestagetitle")
							{
								value = this.lblStartGamestage;
								return true;
							}
						}
					}
					else if (bindingName == "historytimestamptitle")
					{
						value = this.lblHistoryTimeStampTitle;
						return true;
					}
				}
				else if (num <= 1413159236U)
				{
					if (num != 1383999083U)
					{
						if (num == 1413159236U)
						{
							if (bindingName == "actionispositive")
							{
								value = ((this.currentTwitchActionEntry != null) ? (this.currentTwitchActionEntry.IsPositive ? this.lblTrue : this.lblFalse) : "");
								return true;
							}
						}
					}
					else if (bindingName == "showhistory_action")
					{
						value = (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.Action != null).ToString();
						return true;
					}
				}
				else if (num != 1453562025U)
				{
					if (num == 1548958396U)
					{
						if (bindingName == "actioncooldowntitle")
						{
							value = this.lblCooldown;
							return true;
						}
					}
				}
				else if (bindingName == "actioncostcolor")
				{
					value = "222,206,163,255";
					if (this.currentTwitchActionEntry != null)
					{
						int num2 = this.currentTwitchActionEntry.ModifiedCost - this.currentTwitchActionEntry.DefaultCost;
						if (num2 != 0)
						{
							if (num2 > 0)
							{
								value = "255,0,0,255";
							}
							else if (num2 < 0)
							{
								value = "0,255,0,255";
							}
						}
					}
					return true;
				}
			}
			else if (num <= 1835511105U)
			{
				if (num <= 1715807981U)
				{
					if (num != 1589536157U)
					{
						if (num == 1715807981U)
						{
							if (bindingName == "actiondiscountcost")
							{
								if (this.currentTwitchActionEntry != null && TwitchManager.Current.BitPriceMultiplier != 1f && this.currentTwitchActionEntry.PointType == TwitchAction.PointTypes.Bits && !this.currentTwitchActionEntry.IgnoreDiscount)
								{
									value = string.Format("{0} {1}", this.currentTwitchActionEntry.GetModifiedDiscountCost(), this.lblPointsBits);
								}
								else
								{
									value = "";
								}
								return true;
							}
						}
					}
					else if (bindingName == "actiongamestage")
					{
						value = ((this.currentTwitchActionEntry != null) ? this.currentTwitchActionEntry.StartGameStage.ToString() : "");
						return true;
					}
				}
				else if (num != 1780150248U)
				{
					if (num == 1835511105U)
					{
						if (bindingName == "actiondefaultcosttitle")
						{
							value = this.lblPointCost;
							return true;
						}
					}
				}
				else if (bindingName == "sessionevil_title")
				{
					value = this.lblTopEvil;
					return true;
				}
			}
			else if (num <= 1882578559U)
			{
				if (num != 1841054916U)
				{
					if (num == 1882578559U)
					{
						if (bindingName == "sessiontotalactions_title")
						{
							value = this.lblTotalActions;
							return true;
						}
					}
				}
				else if (bindingName == "historytargettitle")
				{
					value = this.lblHistoryTargetTitle;
					return true;
				}
			}
			else if (num != 2003818719U)
			{
				if (num != 2170302190U)
				{
					if (num == 2236179277U)
					{
						if (bindingName == "historytimestamp")
						{
							value = ((this.currentTwitchActionHistoryEntry != null) ? this.currentTwitchActionHistoryEntry.ActionTime : "");
							return true;
						}
					}
				}
				else if (bindingName == "showaction")
				{
					value = (this.currentTwitchActionEntry != null).ToString();
					return true;
				}
			}
			else if (bindingName == "sessiontotalbad_title")
			{
				value = this.lblTotalBad;
				return true;
			}
		}
		else if (num <= 3257770903U)
		{
			if (num <= 2818986817U)
			{
				if (num <= 2493832296U)
				{
					if (num <= 2415022578U)
					{
						if (num != 2373030536U)
						{
							if (num == 2415022578U)
							{
								if (bindingName == "actionispositivetitle")
								{
									value = this.lblIsPositive;
									return true;
								}
							}
						}
						else if (bindingName == "leaderboard_sessionkiller")
						{
							value = ((this.stats != null && this.stats.TopKillerViewer != null) ? string.Format("[{0}]{1}[-] ({2})", this.stats.TopKillerViewer.UserColor, this.stats.TopKillerViewer.Name, this.stats.TopKillerViewer.Kills) : "--");
							return true;
						}
					}
					else if (num != 2476815079U)
					{
						if (num == 2493832296U)
						{
							if (bindingName == "refundtext")
							{
								if (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.CanRefund())
								{
									value = string.Format(this.lblRefund, this.GetHistoryPointCost());
								}
								else
								{
									value = this.lblNoRefund;
								}
								return true;
							}
						}
					}
					else if (bindingName == "actionpointtypetitle")
					{
						value = this.lblPointType;
						return true;
					}
				}
				else if (num <= 2646313357U)
				{
					if (num != 2493865531U)
					{
						if (num == 2646313357U)
						{
							if (bindingName == "votestartgamestage")
							{
								if (this.currentTwitchVoteEntry != null && this.currentTwitchVoteEntry.StartGameStage > 0)
								{
									value = this.currentTwitchVoteEntry.StartGameStage.ToString();
								}
								else
								{
									value = "";
								}
								return true;
							}
						}
					}
					else if (bindingName == "sessiongood_title")
					{
						value = this.lblTopGood;
						return true;
					}
				}
				else if (num != 2658197062U)
				{
					if (num == 2818986817U)
					{
						if (bindingName == "actiondiscountcosttitle")
						{
							if (this.currentTwitchActionEntry != null && TwitchManager.Current.BitPriceMultiplier != 1f && this.currentTwitchActionEntry.PointType == TwitchAction.PointTypes.Bits && !this.currentTwitchActionEntry.IgnoreDiscount)
							{
								value = this.lblDiscountCost;
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
				}
				else if (bindingName == "leaderboard_largestpot")
				{
					string arg = (TwitchManager.Current.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? Localization.Get("TwitchPoints_SP", false) : Localization.Get("TwitchPoints_PP", false);
					value = string.Format("{0} {1}", (this.stats != null) ? this.stats.LargestPimpPot : 0, arg);
					return true;
				}
			}
			else if (num <= 3051633273U)
			{
				if (num <= 2958318179U)
				{
					if (num != 2846886419U)
					{
						if (num == 2958318179U)
						{
							if (bindingName == "entrytitle")
							{
								if (this.currentTwitchActionEntry != null)
								{
									value = this.currentTwitchActionEntry.Title;
								}
								else if (this.currentTwitchVoteEntry != null)
								{
									value = this.currentTwitchVoteEntry.VoteDescription;
								}
								else if (this.currentTwitchActionHistoryEntry != null)
								{
									value = this.currentTwitchActionHistoryEntry.Title;
								}
								else if (this.OwnerList != null && this.OwnerList.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard)
								{
									value = this.lblLeaderboardStats;
								}
								else
								{
									value = "";
								}
								return true;
							}
						}
					}
					else if (bindingName == "sessionlargestpimppot_title")
					{
						value = this.lblLargestPimpPot;
						return true;
					}
				}
				else if (num != 3051255413U)
				{
					if (num == 3051633273U)
					{
						if (bindingName == "increasepricetext")
						{
							value = "";
							if (this.currentTwitchActionEntry != null)
							{
								value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblIncreasePrice : this.lblIncreasePrice_Controller);
							}
							return true;
						}
					}
				}
				else if (bindingName == "sessionmostbits_title")
				{
					value = this.lblMostBits;
					return true;
				}
			}
			else if (num <= 3184965712U)
			{
				if (num != 3144329307U)
				{
					if (num == 3184965712U)
					{
						if (bindingName == "showvote")
						{
							value = (this.currentTwitchVoteEntry != null).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "showenable")
				{
					value = (this.currentTwitchActionEntry != null || this.currentTwitchVoteEntry != null).ToString();
					return true;
				}
			}
			else if (num != 3213443644U)
			{
				if (num != 3214603061U)
				{
					if (num == 3257770903U)
					{
						if (bindingName == "showstats")
						{
							value = (this.currentTwitchActionEntry != null || this.currentTwitchVoteEntry != null || this.currentTwitchActionHistoryEntry != null || (this.OwnerList != null && this.OwnerList.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard)).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "showhistory_retry")
				{
					value = (this.currentTwitchActionHistoryEntry != null && (this.currentTwitchActionHistoryEntry.Action != null || this.currentTwitchActionHistoryEntry.EventEntry != null)).ToString();
					return true;
				}
			}
			else if (bindingName == "enablerefund")
			{
				value = (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.CanRefund()).ToString();
				return true;
			}
		}
		else if (num <= 3639271085U)
		{
			if (num <= 3487000492U)
			{
				if (num <= 3314030970U)
				{
					if (num != 3275561864U)
					{
						if (num == 3314030970U)
						{
							if (bindingName == "retrytext")
							{
								if (this.currentTwitchActionHistoryEntry != null)
								{
									if (this.currentTwitchActionHistoryEntry.CanRetry())
									{
										value = this.lblRetry;
									}
									else
									{
										value = (this.currentTwitchActionHistoryEntry.HasRetried ? this.lblNoRetry : this.lblRetryActionUnavailable);
									}
								}
								else
								{
									value = "";
								}
								return true;
							}
						}
					}
					else if (bindingName == "leaderboard_totalactions")
					{
						value = ((this.stats != null) ? this.stats.TotalActions.ToString() : "0");
						return true;
					}
				}
				else if (num != 3387479438U)
				{
					if (num == 3487000492U)
					{
						if (bindingName == "showhistory")
						{
							value = (this.currentTwitchActionHistoryEntry != null).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "actionpointcost")
				{
					if (this.currentTwitchActionEntry != null)
					{
						switch (this.currentTwitchActionEntry.PointType)
						{
						case TwitchAction.PointTypes.PP:
							value = string.Format("{0} {1}", this.currentTwitchActionEntry.ModifiedCost, this.lblPointsPP);
							break;
						case TwitchAction.PointTypes.SP:
							value = string.Format("{0} {1}", this.currentTwitchActionEntry.ModifiedCost, this.lblPointsSP);
							break;
						case TwitchAction.PointTypes.Bits:
							value = string.Format("{0} {1}", this.currentTwitchActionEntry.ModifiedCost, this.lblPointsBits);
							break;
						}
					}
					else if (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.Action != null)
					{
						value = this.GetHistoryPointCost();
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (num <= 3526138964U)
			{
				if (num != 3500355537U)
				{
					if (num == 3526138964U)
					{
						if (bindingName == "enableretry")
						{
							value = (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.CanRetry()).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "actiongamestagetitle")
				{
					value = this.lblStartGamestage;
					return true;
				}
			}
			else if (num != 3616278858U)
			{
				if (num == 3639271085U)
				{
					if (bindingName == "enablebuttontext")
					{
						value = "";
						if (this.currentTwitchActionEntry != null)
						{
							if (this.currentTwitchActionEntry.IsInPreset(TwitchManager.Current.CurrentActionPreset))
							{
								value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblDisableAction : this.lblDisableAction_Controller);
							}
							else
							{
								value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblEnableAction : this.lblEnableAction_Controller);
							}
						}
						else if (this.currentTwitchVoteEntry != null)
						{
							if (this.currentTwitchVoteEntry.Enabled)
							{
								value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblDisableVote : this.lblDisableVote_Controller);
							}
							else
							{
								value = ((PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard) ? this.lblEnableVote : this.lblEnableVote_Controller);
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "actioncooldown")
			{
				value = ((this.currentTwitchActionEntry != null) ? XUiM_PlayerBuffs.ConvertToTimeString(this.currentTwitchActionEntry.Cooldown) : "");
				return true;
			}
		}
		else if (num <= 4060322893U)
		{
			if (num <= 3745003420U)
			{
				if (num != 3649566128U)
				{
					if (num == 3745003420U)
					{
						if (bindingName == "leaderboard_totalgood")
						{
							value = string.Format("[AFAFFF]{0}[-]", (this.stats != null) ? this.stats.TotalGood.ToString() : "0");
							return true;
						}
					}
				}
				else if (bindingName == "voteendgamestagetitle")
				{
					value = this.lblEndGamestage;
					return true;
				}
			}
			else if (num != 3755168009U)
			{
				if (num == 4060322893U)
				{
					if (bindingName == "showempty")
					{
						value = (this.currentTwitchActionEntry == null && this.currentTwitchVoteEntry == null && this.currentTwitchActionHistoryEntry == null && this.OwnerList != null && this.OwnerList.CurrentType != XUiC_TwitchEntryListWindow.ListTypes.Leaderboard).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "showhistory_vote")
			{
				value = (this.currentTwitchActionHistoryEntry != null && this.currentTwitchActionHistoryEntry.Vote != null).ToString();
				return true;
			}
		}
		else if (num <= 4142793257U)
		{
			if (num != 4101101692U)
			{
				if (num == 4142793257U)
				{
					if (bindingName == "leaderboard_badperson")
					{
						value = ((this.stats != null && this.stats.TopBadViewer != null) ? string.Format("[{0}]{1}[-] ({2})", this.stats.TopBadViewer.UserColor, this.stats.TopBadViewer.Name, this.stats.TopBadViewer.BadActions) : "--");
						return true;
					}
				}
			}
			else if (bindingName == "leaderboard_totalbad")
			{
				value = string.Format("[FFAFAF]{0}[-]", (this.stats != null) ? this.stats.TotalBad.ToString() : "0");
				return true;
			}
		}
		else if (num != 4227361571U)
		{
			if (num != 4259958381U)
			{
				if (num == 4292913866U)
				{
					if (bindingName == "sessiontotalbits_title")
					{
						value = this.lblTotalBits;
						return true;
					}
				}
			}
			else if (bindingName == "emptytext")
			{
				value = "";
				if (this.OwnerList != null)
				{
					switch (this.OwnerList.CurrentType)
					{
					case XUiC_TwitchEntryListWindow.ListTypes.Actions:
						value = this.lblActionEmpty;
						break;
					case XUiC_TwitchEntryListWindow.ListTypes.Votes:
						value = this.lblVoteEmpty;
						break;
					case XUiC_TwitchEntryListWindow.ListTypes.ActionHistory:
						value = this.lblActionHistoryEmpty;
						break;
					case XUiC_TwitchEntryListWindow.ListTypes.Leaderboard:
						value = this.lblLeaderboardEmpty;
						break;
					}
				}
				return true;
			}
		}
		else if (bindingName == "actionrandomgrouptitle")
		{
			value = this.lblRandomDaily;
			return true;
		}
		return false;
	}

	// Token: 0x0600752F RID: 29999 RVA: 0x002FC044 File Offset: 0x002FA244
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetHistoryPointCost()
	{
		if (this.currentTwitchActionHistoryEntry == null || this.currentTwitchActionHistoryEntry.Action == null)
		{
			return "";
		}
		switch (this.currentTwitchActionHistoryEntry.Action.PointType)
		{
		case TwitchAction.PointTypes.PP:
			return string.Format("{0} {1}", this.currentTwitchActionHistoryEntry.PointsSpent, this.lblPointsPP);
		case TwitchAction.PointTypes.SP:
			return string.Format("{0} {1}", this.currentTwitchActionHistoryEntry.PointsSpent, this.lblPointsSP);
		case TwitchAction.PointTypes.Bits:
			return string.Format("{0} {1}", this.currentTwitchActionHistoryEntry.PointsSpent, this.lblPointsBits);
		default:
			return "";
		}
	}

	// Token: 0x06007530 RID: 30000 RVA: 0x002FC0FC File Offset: 0x002FA2FC
	public void SetTwitchAction(XUiC_TwitchActionEntry twitchInfoEntry)
	{
		this.actionEntry = twitchInfoEntry;
		this.voteEntry = null;
		this.CurrentTwitchVoteEntry = null;
		this.historyEntry = null;
		this.CurrentTwitchActionHistoryEntry = null;
		if (this.actionEntry != null)
		{
			this.CurrentTwitchActionEntry = this.actionEntry.Action;
			return;
		}
		this.CurrentTwitchActionEntry = null;
	}

	// Token: 0x06007531 RID: 30001 RVA: 0x002FC150 File Offset: 0x002FA350
	public void SetTwitchVote(XUiC_TwitchVoteInfoEntry twitchInfoEntry)
	{
		this.voteEntry = twitchInfoEntry;
		this.actionEntry = null;
		this.CurrentTwitchActionEntry = null;
		this.historyEntry = null;
		this.CurrentTwitchActionHistoryEntry = null;
		if (this.voteEntry != null)
		{
			this.CurrentTwitchVoteEntry = this.voteEntry.Vote;
			return;
		}
		this.CurrentTwitchVoteEntry = null;
	}

	// Token: 0x06007532 RID: 30002 RVA: 0x002FC1A4 File Offset: 0x002FA3A4
	public void SetTwitchHistory(XUiC_TwitchActionHistoryEntry twitchInfoEntry)
	{
		this.historyEntry = twitchInfoEntry;
		this.actionEntry = null;
		this.CurrentTwitchActionEntry = null;
		this.voteEntry = null;
		this.CurrentTwitchVoteEntry = null;
		if (this.historyEntry != null)
		{
			this.CurrentTwitchActionHistoryEntry = this.historyEntry.HistoryItem;
			return;
		}
		this.CurrentTwitchActionHistoryEntry = null;
	}

	// Token: 0x06007533 RID: 30003 RVA: 0x002FC1F5 File Offset: 0x002FA3F5
	public void ClearEntries()
	{
		this.actionEntry = null;
		this.CurrentTwitchActionEntry = null;
		this.voteEntry = null;
		this.CurrentTwitchVoteEntry = null;
		this.historyEntry = null;
		this.CurrentTwitchActionHistoryEntry = null;
	}

	// Token: 0x04005921 RID: 22817
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchActionEntry actionEntry;

	// Token: 0x04005922 RID: 22818
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchVoteInfoEntry voteEntry;

	// Token: 0x04005923 RID: 22819
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchActionHistoryEntry historyEntry;

	// Token: 0x04005924 RID: 22820
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnEnable;

	// Token: 0x04005925 RID: 22821
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnRefund;

	// Token: 0x04005926 RID: 22822
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnRetry;

	// Token: 0x04005927 RID: 22823
	public XUiC_TwitchEntryListWindow OwnerList;

	// Token: 0x04005928 RID: 22824
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchAction currentTwitchActionEntry;

	// Token: 0x04005929 RID: 22825
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchVote currentTwitchVoteEntry;

	// Token: 0x0400592A RID: 22826
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchActionHistoryEntry currentTwitchActionHistoryEntry;

	// Token: 0x0400592B RID: 22827
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStartGamestage;

	// Token: 0x0400592C RID: 22828
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblEndGamestage;

	// Token: 0x0400592D RID: 22829
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPointCost;

	// Token: 0x0400592E RID: 22830
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblCooldown;

	// Token: 0x0400592F RID: 22831
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblRandomDaily;

	// Token: 0x04005930 RID: 22832
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblIsPositive;

	// Token: 0x04005931 RID: 22833
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPointType;

	// Token: 0x04005932 RID: 22834
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblEnableAction;

	// Token: 0x04005933 RID: 22835
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDisableAction;

	// Token: 0x04005934 RID: 22836
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblEnableVote;

	// Token: 0x04005935 RID: 22837
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDisableVote;

	// Token: 0x04005936 RID: 22838
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblActionEmpty;

	// Token: 0x04005937 RID: 22839
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblVoteEmpty;

	// Token: 0x04005938 RID: 22840
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblActionHistoryEmpty;

	// Token: 0x04005939 RID: 22841
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblLeaderboardEmpty;

	// Token: 0x0400593A RID: 22842
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblHistoryTargetTitle;

	// Token: 0x0400593B RID: 22843
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblHistoryStateTitle;

	// Token: 0x0400593C RID: 22844
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblHistoryTimeStampTitle;

	// Token: 0x0400593D RID: 22845
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblRefund;

	// Token: 0x0400593E RID: 22846
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblNoRefund;

	// Token: 0x0400593F RID: 22847
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblRetry;

	// Token: 0x04005940 RID: 22848
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblNoRetry;

	// Token: 0x04005941 RID: 22849
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblRetryActionUnavailable;

	// Token: 0x04005942 RID: 22850
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblLeaderboardStats;

	// Token: 0x04005943 RID: 22851
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblShowBitTotal;

	// Token: 0x04005944 RID: 22852
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDiscountCost;

	// Token: 0x04005945 RID: 22853
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblIncreasePrice;

	// Token: 0x04005946 RID: 22854
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDecreasePrice;

	// Token: 0x04005947 RID: 22855
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblEnableAction_Controller;

	// Token: 0x04005948 RID: 22856
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDisableAction_Controller;

	// Token: 0x04005949 RID: 22857
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblEnableVote_Controller;

	// Token: 0x0400594A RID: 22858
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDisableVote_Controller;

	// Token: 0x0400594B RID: 22859
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblIncreasePrice_Controller;

	// Token: 0x0400594C RID: 22860
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblDecreasePrice_Controller;

	// Token: 0x0400594D RID: 22861
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTopKiller;

	// Token: 0x0400594E RID: 22862
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTopGood;

	// Token: 0x0400594F RID: 22863
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTopEvil;

	// Token: 0x04005950 RID: 22864
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblCurrentGood;

	// Token: 0x04005951 RID: 22865
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblMostBits;

	// Token: 0x04005952 RID: 22866
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTotalBits;

	// Token: 0x04005953 RID: 22867
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTotalBad;

	// Token: 0x04005954 RID: 22868
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTotalGood;

	// Token: 0x04005955 RID: 22869
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTotalActions;

	// Token: 0x04005956 RID: 22870
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblLargestPimpPot;

	// Token: 0x04005957 RID: 22871
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTrue;

	// Token: 0x04005958 RID: 22872
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblFalse;

	// Token: 0x04005959 RID: 22873
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPointsPP;

	// Token: 0x0400595A RID: 22874
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPointsSP;

	// Token: 0x0400595B RID: 22875
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPointsBits;

	// Token: 0x0400595C RID: 22876
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchLeaderboardStats stats;

	// Token: 0x0400595D RID: 22877
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showBitTotal;

	// Token: 0x0400595E RID: 22878
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string DayTimeFormatString = Localization.Get("xuiDay", false) + "{0}, {1:00}:{2:00}";

	// Token: 0x0400595F RID: 22879
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<ulong> daytimeFormatter = new CachedStringFormatter<ulong>((ulong _worldTime) => ValueDisplayFormatters.WorldTime(_worldTime, XUiC_TwitchEntryDescriptionWindow.DayTimeFormatString));
}
