using System;
using System.Collections.Generic;
using Twitch;
using UniLinq;
using UnityEngine.Scripting;

// Token: 0x02000E89 RID: 3721
[Preserve]
public class XUiC_TwitchEntryListWindow : XUiController
{
	// Token: 0x17000BF1 RID: 3057
	// (get) Token: 0x06007539 RID: 30009 RVA: 0x002FC288 File Offset: 0x002FA488
	// (set) Token: 0x0600753A RID: 30010 RVA: 0x002FC290 File Offset: 0x002FA490
	public string ActionCategory { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000BF2 RID: 3058
	// (get) Token: 0x0600753B RID: 30011 RVA: 0x002FC299 File Offset: 0x002FA499
	// (set) Token: 0x0600753C RID: 30012 RVA: 0x002FC2A1 File Offset: 0x002FA4A1
	public string VoteCategory { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000BF3 RID: 3059
	// (get) Token: 0x0600753D RID: 30013 RVA: 0x002FC2AA File Offset: 0x002FA4AA
	// (set) Token: 0x0600753E RID: 30014 RVA: 0x002FC2B2 File Offset: 0x002FA4B2
	public string ActionHistoryCategory { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000BF4 RID: 3060
	// (get) Token: 0x0600753F RID: 30015 RVA: 0x002FC2BB File Offset: 0x002FA4BB
	// (set) Token: 0x06007540 RID: 30016 RVA: 0x002FC2C3 File Offset: 0x002FA4C3
	public string LeaderboardCategory { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06007541 RID: 30017 RVA: 0x002FC2CC File Offset: 0x002FA4CC
	public override void Init()
	{
		base.Init();
		this.twitchActionEntryList = base.GetChildByType<XUiC_TwitchActionEntryList>();
		this.twitchActionEntryList.TwitchEntryListWindow = this;
		this.twitchVoteInfoEntryList = base.GetChildByType<XUiC_TwitchVoteInfoEntryList>();
		this.twitchVoteInfoEntryList.TwitchEntryListWindow = this;
		this.twitchActionHistoryEntryList = base.GetChildByType<XUiC_TwitchActionHistoryEntryList>();
		this.twitchActionHistoryEntryList.TwitchEntryListWindow = this;
		this.twitchLeaderboardEntryList = base.GetChildByType<XUiC_TwitchLeaderboardEntryList>();
		this.twitchLeaderboardEntryList.TwitchEntryListWindow = this;
		this.categoryList = (XUiC_CategoryList)this.windowGroup.Controller.GetChildById("actioncategories");
		this.categoryList.CategoryChanged += this.HandleCategoryChanged;
		this.voteCategoryList = (XUiC_CategoryList)this.windowGroup.Controller.GetChildById("votecategories");
		this.voteCategoryList.CategoryChanged += this.HandleVoteCategoryChanged;
		this.actionHistoryCategoryList = (XUiC_CategoryList)this.windowGroup.Controller.GetChildById("actionHistoryCategories");
		this.actionHistoryCategoryList.CategoryChanged += this.HandleActionHistoryCategoryChanged;
		this.leaderboardCategoryList = (XUiC_CategoryList)this.windowGroup.Controller.GetChildById("leaderboardCategories");
		this.leaderboardCategoryList.CategoryChanged += this.HandleLeaderboardCategoryChanged;
		this.txtInput = (XUiC_TextInput)this.windowGroup.Controller.GetChildById("searchInput");
		XUiController childById = base.GetChildById("allactions");
		if (childById != null)
		{
			this.allActionsButton = (childById.ViewComponent as XUiV_Button);
			if (this.allActionsButton != null)
			{
				childById.OnPress += this.AllActionsButtonCtrl_OnPress;
			}
		}
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.HandleOnChangedHandler;
			this.txtInput.Text = "";
		}
		this.ActionCategory = "";
		this.lblActions = Localization.Get("TwitchInfo_Actions", false);
		this.lblVotes = Localization.Get("TwitchInfo_Votes", false);
		this.lblActionHistory = Localization.Get("TwitchInfo_ActionHistory", false);
		this.lblLeaderboard = Localization.Get("TwitchInfo_Leaderboard", false);
	}

	// Token: 0x06007542 RID: 30018 RVA: 0x002FC4F3 File Offset: 0x002FA6F3
	[PublicizedFrom(EAccessModifier.Private)]
	public void AllActionsButtonCtrl_OnPress(XUiController _sender, int _mouseButton)
	{
		this.showAllActions = !this.showAllActions;
		this.allActionsButton.Selected = this.showAllActions;
		this.IsDirty = true;
	}

	// Token: 0x06007543 RID: 30019 RVA: 0x002FC51C File Offset: 0x002FA71C
	public void SetOpenToActions(bool openExtras = false)
	{
		this.CurrentType = XUiC_TwitchEntryListWindow.ListTypes.Actions;
		this.IsDirty = true;
		this.forceExtras = openExtras;
	}

	// Token: 0x06007544 RID: 30020 RVA: 0x002FC533 File Offset: 0x002FA733
	public void SetOpenToLeaderboard()
	{
		this.CurrentType = XUiC_TwitchEntryListWindow.ListTypes.Leaderboard;
		this.IsDirty = true;
	}

	// Token: 0x06007545 RID: 30021 RVA: 0x002FC543 File Offset: 0x002FA743
	public void SetOpenToVotes()
	{
		this.CurrentType = XUiC_TwitchEntryListWindow.ListTypes.Votes;
		this.IsDirty = true;
	}

	// Token: 0x06007546 RID: 30022 RVA: 0x002FC553 File Offset: 0x002FA753
	public void SetOpenToHistory()
	{
		this.CurrentType = XUiC_TwitchEntryListWindow.ListTypes.ActionHistory;
		this.IsDirty = true;
	}

	// Token: 0x06007547 RID: 30023 RVA: 0x002FC563 File Offset: 0x002FA763
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.ActionCategory = _categoryEntry.CategoryName;
		this.HandleFilterActions();
	}

	// Token: 0x06007548 RID: 30024 RVA: 0x002FC590 File Offset: 0x002FA790
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFilterActions()
	{
		if (this.currentActions != null && this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Actions)
		{
			TwitchActionPreset currentPreset = this.tm.CurrentActionPreset;
			List<TwitchAction> newActionEntryList = (from entry in this.currentActions
			where (this.filterText == "" || entry.Title.ContainsCaseInsensitive(this.filterText) || entry.Command.ContainsCaseInsensitive(this.filterText)) && ((this.ActionCategory == "" && entry.DisplayCategory == entry.MainCategory) || entry.DisplayCategory.Name == this.ActionCategory) && entry.ShowInActionList && (currentPreset.AllowPointGeneration || entry.PointType == TwitchAction.PointTypes.Bits)
			orderby entry.Title
			select entry).ToList<TwitchAction>();
			this.twitchActionEntryList.SetTwitchActionList(newActionEntryList, this.tm.CurrentActionPreset);
			base.RefreshBindings(false);
			return;
		}
		this.IsDirty = true;
	}

	// Token: 0x06007549 RID: 30025 RVA: 0x002FC632 File Offset: 0x002FA832
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleVoteCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.VoteCategory = _categoryEntry.CategoryName;
		this.IsDirty = true;
	}

	// Token: 0x0600754A RID: 30026 RVA: 0x002FC660 File Offset: 0x002FA860
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleActionHistoryCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		if (_categoryEntry.CategoryName != this.ActionHistoryCategory)
		{
			this.ActionHistoryCategory = _categoryEntry.CategoryName;
			this.twitchActionHistoryEntryList.SelectedEntry = null;
			this.twitchActionHistoryEntryList.setFirstEntry = true;
		}
		this.IsDirty = true;
	}

	// Token: 0x0600754B RID: 30027 RVA: 0x002FC6C3 File Offset: 0x002FA8C3
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleLeaderboardCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.LeaderboardCategory = _categoryEntry.CategoryName;
		this.IsDirty = true;
		this.UpdateDelay = 0f;
	}

	// Token: 0x0600754C RID: 30028 RVA: 0x002FC6FB File Offset: 0x002FA8FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnChangedHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.filterText = _text;
		if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Actions)
		{
			this.HandleFilterActions();
			return;
		}
		this.IsDirty = true;
	}

	// Token: 0x0600754D RID: 30029 RVA: 0x002FC71C File Offset: 0x002FA91C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.UpdateDelay > 0f)
		{
			this.UpdateDelay -= _dt;
			return;
		}
		if (this.IsDirty)
		{
			if (this.tm == null)
			{
				this.tm = TwitchManager.Current;
			}
			switch (this.CurrentType)
			{
			case XUiC_TwitchEntryListWindow.ListTypes.Actions:
			{
				this.currentActions = (from entry in TwitchActionManager.TwitchActions.Values.ToList<TwitchAction>()
				where entry != null && entry.ShowInActionList && (entry.IsInPresetForList(this.tm.CurrentActionPreset) || (this.showAllActions && !entry.OnlyShowInPreset) || (entry.DisplayCategory != null && entry.DisplayCategory.AlwaysShowInMenu))
				orderby entry.Title
				select entry).ToList<TwitchAction>();
				this.twitchActionEntryList.SetTwitchActionList(this.currentActions, this.tm.CurrentActionPreset);
				string text = (this.categoryList.CurrentCategory != null) ? this.categoryList.CurrentCategory.CategoryName : "";
				this.categoryList.SetupCategoriesBasedOnTwitchActions(this.currentActions);
				if (this.forceExtras)
				{
					text = "Extras";
					this.forceExtras = false;
				}
				if (text == "")
				{
					this.categoryList.SetCategoryToFirst();
				}
				else
				{
					this.categoryList.SetCategory(text);
					if (this.categoryList.CurrentCategory == null || this.categoryList.CurrentCategory.CategoryName == "")
					{
						this.categoryList.SetCategoryToFirst();
					}
				}
				break;
			}
			case XUiC_TwitchEntryListWindow.ListTypes.Votes:
				this.currentVotes = (from entry in TwitchActionManager.TwitchVotes.Values.ToList<TwitchVote>()
				where (this.filterText == "" || entry.Display.ContainsCaseInsensitive(this.filterText)) && (this.VoteCategory == "" || (entry.MainVoteType != null && entry.MainVoteType.Name == this.VoteCategory && entry.MainVoteType.IsInPreset(this.tm.CurrentVotePreset.Name))) && entry.IsInPreset(this.tm.CurrentVotePreset)
				orderby entry.VoteDescription
				select entry).ToList<TwitchVote>();
				this.twitchVoteInfoEntryList.SetTwitchVoteList(this.currentVotes);
				break;
			case XUiC_TwitchEntryListWindow.ListTypes.ActionHistory:
			{
				List<TwitchActionHistoryEntry> list = null;
				string actionHistoryCategory = this.ActionHistoryCategory;
				if (!(actionHistoryCategory == "action"))
				{
					if (!(actionHistoryCategory == "vote"))
					{
						if (actionHistoryCategory == "event")
						{
							list = this.tm.EventHistory;
						}
					}
					else
					{
						list = this.tm.VoteHistory;
					}
				}
				else
				{
					list = this.tm.ActionHistory;
				}
				if (list != null)
				{
					this.currentRedemptions = (from entry in list
					where entry != null && entry.IsValid() && (this.filterText == "" || entry.UserName.ContainsCaseInsensitive(this.filterText) || entry.Action.Command.ContainsCaseInsensitive(this.filterText)) && (this.ActionHistoryCategory == "" || this.ActionHistoryCategory == entry.HistoryType)
					select entry).ToList<TwitchActionHistoryEntry>();
					this.twitchActionHistoryEntryList.SetTwitchActionHistoryList(this.currentRedemptions);
				}
				break;
			}
			case XUiC_TwitchEntryListWindow.ListTypes.Leaderboard:
			{
				string a = (this.leaderboardCategoryList == null) ? "" : this.leaderboardCategoryList.CurrentCategory.CategoryName;
				if (!(a == "global_kills"))
				{
					if (!(a == "session_kills"))
					{
						if (!(a == "session_good"))
						{
							if (!(a == "session_bad"))
							{
								if (!(a == "session_bits"))
								{
									if (a == "current_good")
									{
										Dictionary<string, TwitchLeaderboardStats.StatEntry> statEntries = TwitchManager.LeaderboardStats.StatEntries;
										this.currentLeaderboard = (from viewer in statEntries.Values
										where (this.filterText == "" || viewer.Name.ContainsCaseInsensitive(this.filterText)) && viewer.CurrentActions > 0
										orderby viewer.CurrentGoodActions descending
										select new TwitchLeaderboardEntry(viewer.Name, viewer.UserColor, viewer.CurrentGoodActions)).ToList<TwitchLeaderboardEntry>();
										this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
									}
								}
								else
								{
									Dictionary<string, TwitchLeaderboardStats.StatEntry> statEntries2 = TwitchManager.LeaderboardStats.StatEntries;
									this.currentLeaderboard = (from viewer in statEntries2.Values
									where (this.filterText == "" || viewer.Name.ContainsCaseInsensitive(this.filterText)) && viewer.BitsUsed > 0
									orderby viewer.BitsUsed descending
									select new TwitchLeaderboardEntry(viewer.Name, viewer.UserColor, viewer.BitsUsed)).ToList<TwitchLeaderboardEntry>();
									this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
								}
							}
							else
							{
								Dictionary<string, TwitchLeaderboardStats.StatEntry> statEntries3 = TwitchManager.LeaderboardStats.StatEntries;
								this.currentLeaderboard = (from viewer in statEntries3.Values
								where (this.filterText == "" || viewer.Name.ContainsCaseInsensitive(this.filterText)) && viewer.BadActions > 0
								orderby viewer.BadActions descending
								select new TwitchLeaderboardEntry(viewer.Name, viewer.UserColor, viewer.BadActions)).ToList<TwitchLeaderboardEntry>();
								this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
							}
						}
						else
						{
							Dictionary<string, TwitchLeaderboardStats.StatEntry> statEntries4 = TwitchManager.LeaderboardStats.StatEntries;
							this.currentLeaderboard = (from viewer in statEntries4.Values
							where (this.filterText == "" || viewer.Name.ContainsCaseInsensitive(this.filterText)) && viewer.GoodActions > 0
							orderby viewer.GoodActions descending
							select new TwitchLeaderboardEntry(viewer.Name, viewer.UserColor, viewer.GoodActions)).ToList<TwitchLeaderboardEntry>();
							this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
						}
					}
					else
					{
						Dictionary<string, TwitchLeaderboardStats.StatEntry> statEntries5 = TwitchManager.LeaderboardStats.StatEntries;
						this.currentLeaderboard = (from viewer in statEntries5.Values
						where (this.filterText == "" || viewer.Name.ContainsCaseInsensitive(this.filterText)) && viewer.Kills > 0
						orderby viewer.Kills descending
						select new TwitchLeaderboardEntry(viewer.Name, viewer.UserColor, viewer.Kills)).ToList<TwitchLeaderboardEntry>();
						this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
					}
				}
				else
				{
					this.currentLeaderboard = (from entry in this.tm.Leaderboard
					where this.filterText == "" || entry.UserName.ContainsCaseInsensitive(this.filterText)
					orderby entry.Kills descending
					select entry).ToList<TwitchLeaderboardEntry>();
					this.twitchLeaderboardEntryList.SetTwitchLeaderboardList(this.currentLeaderboard);
				}
				this.UpdateDelay = 1f;
				break;
			}
			}
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600754E RID: 30030 RVA: 0x002FCD60 File Offset: 0x002FAF60
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.isInitialized)
		{
			this.player = base.xui.playerUI.entityPlayer;
			this.categoryList.SetupCategoriesBasedOnTwitchCategories(TwitchActionManager.Current.CategoryList);
			this.categoryList.SetCategoryToFirst();
			this.voteCategoryList.SetupCategoriesBasedOnTwitchVoteCategories((from v in TwitchManager.Current.VotingManager.VoteTypes.Values
			where v.IsInPreset(TwitchManager.Current.CurrentVotePreset.Name)
			select v).ToList<TwitchVoteType>());
			this.voteCategoryList.SetCategoryToFirst();
			this.actionHistoryCategoryList.SetCategoryEntry(0, "action", "ui_game_symbol_twitch_actions", Localization.Get("TwitchInfo_Actions", false));
			this.actionHistoryCategoryList.SetCategoryEntry(1, "vote", "ui_game_symbol_twitch_vote", Localization.Get("TwitchInfo_Votes", false));
			this.actionHistoryCategoryList.SetCategoryEntry(2, "event", "ui_game_symbol_twitch_custom_actions", Localization.Get("xuiOptionsTwitchEvents", false));
			this.actionHistoryCategoryList.SetCategoryToFirst();
			this.leaderboardCategoryList.SetCategoryEntry(0, "global_kills", "ui_game_symbol_twitch_top_killer", Localization.Get("TwitchInfo_LeaderboardGlobalKills", false));
			this.leaderboardCategoryList.SetCategoryEntry(1, "session_kills", "ui_game_symbol_skull", Localization.Get("TwitchInfo_LeaderboardSessionKills", false));
			this.leaderboardCategoryList.SetCategoryEntry(2, "session_good", "ui_game_symbol_twitch_top_good", Localization.Get("TwitchInfo_LeaderboardSessionGood", false));
			this.leaderboardCategoryList.SetCategoryEntry(3, "session_bad", "ui_game_symbol_twitch_top_bad", Localization.Get("TwitchInfo_LeaderboardSessionBad", false));
			this.leaderboardCategoryList.SetCategoryEntry(4, "session_bits", "ui_game_symbol_twitch_bits", Localization.Get("TwitchInfo_LeaderboardSessionBits", false));
			this.leaderboardCategoryList.SetCategoryEntry(5, "current_good", "ui_game_symbol_twitch_best_helper", Localization.Get("TwitchInfo_LeaderboardCurrentGood", false));
			this.leaderboardCategoryList.SetCategoryToFirst();
			this.IsDirty = true;
			this.isInitialized = true;
		}
		this.tm = TwitchManager.Current;
		this.tm.ActionHistoryAdded += this.Tm_ActionHistoryAdded;
		this.tm.VoteHistoryAdded += this.Tm_VoteHistoryAdded;
		this.tm.EventHistoryAdded += this.Tm_EventHistoryAdded;
		TwitchManager.LeaderboardStats.LeaderboardChanged += this.LeaderboardStats_LeaderboardChanged;
	}

	// Token: 0x0600754F RID: 30031 RVA: 0x002FCFBC File Offset: 0x002FB1BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LeaderboardStats_LeaderboardChanged()
	{
		if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard)
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x06007550 RID: 30032 RVA: 0x002FCFCE File Offset: 0x002FB1CE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Tm_ActionHistoryAdded()
	{
		if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.ActionHistory && this.ActionHistoryCategory == "action")
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x06007551 RID: 30033 RVA: 0x002FCFF2 File Offset: 0x002FB1F2
	[PublicizedFrom(EAccessModifier.Private)]
	public void Tm_VoteHistoryAdded()
	{
		if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.ActionHistory && this.ActionHistoryCategory == "vote")
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x06007552 RID: 30034 RVA: 0x002FD016 File Offset: 0x002FB216
	[PublicizedFrom(EAccessModifier.Private)]
	public void Tm_EventHistoryAdded()
	{
		if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.ActionHistory && this.ActionHistoryCategory == "event")
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x06007553 RID: 30035 RVA: 0x002FD03C File Offset: 0x002FB23C
	public override void OnClose()
	{
		base.OnClose();
		TwitchManager twitchManager = TwitchManager.Current;
		twitchManager.ActionHistoryAdded -= this.Tm_ActionHistoryAdded;
		twitchManager.VoteHistoryAdded -= this.Tm_VoteHistoryAdded;
		twitchManager.EventHistoryAdded -= this.Tm_EventHistoryAdded;
		TwitchManager.LeaderboardStats.LeaderboardChanged -= this.LeaderboardStats_LeaderboardChanged;
	}

	// Token: 0x06007554 RID: 30036 RVA: 0x002FD0A0 File Offset: 0x002FB2A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2855073766U)
		{
			if (num <= 956754208U)
			{
				if (num != 884373603U)
				{
					if (num == 956754208U)
					{
						if (bindingName == "showcategories")
						{
							value = (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Actions).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "showleaderboard")
				{
					value = (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard).ToString();
					return true;
				}
			}
			else if (num != 1092444455U)
			{
				if (num == 2855073766U)
				{
					if (bindingName == "leaderboard_header_value")
					{
						if (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Leaderboard)
						{
							string a = (this.leaderboardCategoryList == null) ? "" : this.leaderboardCategoryList.CurrentCategory.CategoryName;
							if (!(a == "global_kills") && !(a == "session_kills"))
							{
								if (!(a == "session_good") && !(a == "session_bad") && !(a == "current_good"))
								{
									if (a == "session_bits")
									{
										value = Localization.Get("TwitchPoints_Bits", false);
									}
								}
								else
								{
									value = Localization.Get("TwitchInfo_Actions", false);
								}
							}
							else
							{
								value = Localization.Get("TwitchInfo_KillsHeader", false);
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "showactions")
			{
				value = (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Actions).ToString();
				return true;
			}
		}
		else if (num <= 3272249238U)
		{
			if (num != 3131143201U)
			{
				if (num == 3272249238U)
				{
					if (bindingName == "headertitle")
					{
						switch (this.CurrentType)
						{
						case XUiC_TwitchEntryListWindow.ListTypes.Actions:
						{
							string text = (this.categoryList == null) ? "" : this.categoryList.CurrentCategory.CategoryDisplayName;
							value = ((text != "") ? text : Localization.Get("lblAll", false));
							break;
						}
						case XUiC_TwitchEntryListWindow.ListTypes.Votes:
						{
							string text2 = (this.voteCategoryList == null) ? "" : this.voteCategoryList.CurrentCategory.CategoryDisplayName;
							value = ((text2 != "") ? text2 : Localization.Get("lblAll", false));
							break;
						}
						case XUiC_TwitchEntryListWindow.ListTypes.ActionHistory:
						{
							string text3 = (this.actionHistoryCategoryList == null) ? "" : this.actionHistoryCategoryList.CurrentCategory.CategoryDisplayName;
							value = ((text3 != "") ? text3 : Localization.Get("lblAll", false));
							break;
						}
						case XUiC_TwitchEntryListWindow.ListTypes.Leaderboard:
						{
							string text4 = (this.leaderboardCategoryList == null) ? "" : this.leaderboardCategoryList.CurrentCategory.CategoryDisplayName;
							value = ((text4 != "") ? text4 : Localization.Get("lblAll", false));
							break;
						}
						}
						return true;
					}
				}
			}
			else if (bindingName == "headericon")
			{
				switch (this.CurrentType)
				{
				case XUiC_TwitchEntryListWindow.ListTypes.Actions:
					value = "ui_game_symbol_twitch_actions";
					break;
				case XUiC_TwitchEntryListWindow.ListTypes.Votes:
					value = "ui_game_symbol_twitch_vote";
					break;
				case XUiC_TwitchEntryListWindow.ListTypes.ActionHistory:
					value = "ui_game_symbol_twitch_history";
					break;
				case XUiC_TwitchEntryListWindow.ListTypes.Leaderboard:
					value = "ui_game_symbol_twitch_leaderboard_1";
					break;
				}
				return true;
			}
		}
		else if (num != 4008283398U)
		{
			if (num == 4228112153U)
			{
				if (bindingName == "showvotes")
				{
					value = (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.Votes).ToString();
					return true;
				}
			}
		}
		else if (bindingName == "showactionhistory")
		{
			value = (this.CurrentType == XUiC_TwitchEntryListWindow.ListTypes.ActionHistory).ToString();
			return true;
		}
		return false;
	}

	// Token: 0x04005962 RID: 22882
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04005963 RID: 22883
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchActionEntryList twitchActionEntryList;

	// Token: 0x04005964 RID: 22884
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchVoteInfoEntryList twitchVoteInfoEntryList;

	// Token: 0x04005965 RID: 22885
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchActionHistoryEntryList twitchActionHistoryEntryList;

	// Token: 0x04005966 RID: 22886
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchLeaderboardEntryList twitchLeaderboardEntryList;

	// Token: 0x04005967 RID: 22887
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x04005968 RID: 22888
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchAction> currentActions;

	// Token: 0x04005969 RID: 22889
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchVote> currentVotes;

	// Token: 0x0400596A RID: 22890
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchActionHistoryEntry> currentRedemptions;

	// Token: 0x0400596B RID: 22891
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchLeaderboardEntry> currentLeaderboard;

	// Token: 0x0400596C RID: 22892
	[PublicizedFrom(EAccessModifier.Private)]
	public string filterText = "";

	// Token: 0x0400596D RID: 22893
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;

	// Token: 0x0400596E RID: 22894
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList voteCategoryList;

	// Token: 0x0400596F RID: 22895
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList actionHistoryCategoryList;

	// Token: 0x04005970 RID: 22896
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList leaderboardCategoryList;

	// Token: 0x04005975 RID: 22901
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInitialized;

	// Token: 0x04005976 RID: 22902
	public XUiC_TwitchEntryListWindow.ListTypes CurrentType;

	// Token: 0x04005977 RID: 22903
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblActions;

	// Token: 0x04005978 RID: 22904
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblVotes;

	// Token: 0x04005979 RID: 22905
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblActionHistory;

	// Token: 0x0400597A RID: 22906
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblLeaderboard;

	// Token: 0x0400597B RID: 22907
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showAllActions;

	// Token: 0x0400597C RID: 22908
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button allActionsButton;

	// Token: 0x0400597D RID: 22909
	[PublicizedFrom(EAccessModifier.Private)]
	public float UpdateDelay;

	// Token: 0x0400597E RID: 22910
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchManager tm;

	// Token: 0x0400597F RID: 22911
	[PublicizedFrom(EAccessModifier.Private)]
	public bool forceExtras;

	// Token: 0x02000E8A RID: 3722
	public enum ListTypes
	{
		// Token: 0x04005981 RID: 22913
		Actions,
		// Token: 0x04005982 RID: 22914
		Votes,
		// Token: 0x04005983 RID: 22915
		ActionHistory,
		// Token: 0x04005984 RID: 22916
		Leaderboard
	}
}
