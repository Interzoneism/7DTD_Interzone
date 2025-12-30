using System;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E96 RID: 3734
[Preserve]
public class XUiC_TwitchWindow : XUiController
{
	// Token: 0x17000C00 RID: 3072
	// (get) Token: 0x060075CD RID: 30157 RVA: 0x002FF34A File Offset: 0x002FD54A
	// (set) Token: 0x060075CE RID: 30158 RVA: 0x002FF352 File Offset: 0x002FD552
	public EntityPlayerLocal localPlayer { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x060075CF RID: 30159 RVA: 0x002FF35C File Offset: 0x002FD55C
	public override void Init()
	{
		base.Init();
		XUiC_TwitchWindow.ID = base.WindowGroup.ID;
		this.twitchManager = TwitchManager.Current;
		this.twitchManager.ConnectionStateChanged -= this.TwitchManager_ConnectionStateChanged;
		this.twitchManager.ConnectionStateChanged += this.TwitchManager_ConnectionStateChanged;
		TwitchVotingManager votingManager = this.twitchManager.VotingManager;
		votingManager.VoteStarted = (OnGameEventVoteAction)Delegate.Remove(votingManager.VoteStarted, new OnGameEventVoteAction(this.TwitchManager_VoteStarted));
		TwitchVotingManager votingManager2 = this.twitchManager.VotingManager;
		votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Combine(votingManager2.VoteStarted, new OnGameEventVoteAction(this.TwitchManager_VoteStarted));
		this.CommandListUI = base.GetChildByType<XUiC_TwitchCommandList>();
		this.CommandListUI.Owner = this;
		XUiC_TwitchVoteList[] childrenByType = base.GetChildrenByType<XUiC_TwitchVoteList>(null);
		for (int i = 0; i < childrenByType.Length; i++)
		{
			childrenByType[i].Owner = this;
		}
		XUiController childById = base.GetChildById("leftButton");
		childById.OnPress += this.Left_OnPress;
		childById = base.GetChildById("rightButton");
		childById.OnPress += this.Right_OnPress;
		childById = base.GetChildById("pauseButton");
		childById.OnPress += this.cooldown_OnPress;
		this.pauseButton = (childById.ViewComponent as XUiV_Button);
		childById = base.GetChildById("optionsButton");
		childById.OnPress += this.options_OnPress;
		this.optionsButton = (childById.ViewComponent as XUiV_Button);
		childById = base.GetChildById("historyButton");
		childById.OnPress += this.history_OnPress;
		this.historyButton = (childById.ViewComponent as XUiV_Button);
		this.defaultGearColor = this.optionsButton.CurrentColor;
		this.gearSpriteSize = new Vector2((float)this.optionsButton.Sprite.width, (float)this.optionsButton.Sprite.height);
		this.window = (base.ViewComponent as XUiV_Window);
		this.lblStatusReady = Localization.Get("TwitchCooldownStatus_Ready", false);
		this.lblStatusWaiting = Localization.Get("TwitchCooldownStatus_Waiting", false);
		this.lblStatusBloodMoon = Localization.Get("TwitchCooldownStatus_BloodMoon", false);
		this.lblStatusCooldown = Localization.Get("TwitchCooldownStatus_Cooldown", false);
		this.lblStatusPaused = Localization.Get("TwitchCooldownStatus_Paused", false);
		this.lblStatusSafe = Localization.Get("TwitchCooldownStatus_Safe", false);
		this.lblStatusVote = Localization.Get("TwitchCooldownStatus_Vote", false);
		this.lblStatusQuest = Localization.Get("TwitchCooldownStatus_Quest", false);
		this.lblVoteLocked = Localization.Get("TwitchCooldownStatus_VoteLocked", false);
		this.lblPointsPP = Localization.Get("TwitchPoints_PP", false);
		this.lblPointsSP = Localization.Get("TwitchPoints_SP", false);
	}

	// Token: 0x060075D0 RID: 30160 RVA: 0x002FF61C File Offset: 0x002FD81C
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		if (this.progressionButton != null)
		{
			this.progressionButton.Selected = this.twitchManager.UseProgression;
		}
		if (this.pauseButton != null)
		{
			this.pauseButton.Selected = !this.twitchManager.TwitchActive;
		}
	}

	// Token: 0x060075D1 RID: 30161 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void TwitchManager_VoteStarted()
	{
		this.IsDirty = true;
	}

	// Token: 0x060075D2 RID: 30162 RVA: 0x002FF66F File Offset: 0x002FD86F
	[PublicizedFrom(EAccessModifier.Private)]
	public void options_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_TwitchWindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "Actions", false);
	}

	// Token: 0x060075D3 RID: 30163 RVA: 0x002FF68C File Offset: 0x002FD88C
	[PublicizedFrom(EAccessModifier.Private)]
	public void history_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_TwitchWindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "ActionHistory", false);
	}

	// Token: 0x060075D4 RID: 30164 RVA: 0x002FF6A9 File Offset: 0x002FD8A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void cooldown_OnPress(XUiController _sender, int _mouseButton)
	{
		this.twitchManager.ToggleTwitchActive();
		if (this.pauseButton != null)
		{
			this.pauseButton.Selected = !this.twitchManager.TwitchActive;
		}
	}

	// Token: 0x060075D5 RID: 30165 RVA: 0x002FF6D7 File Offset: 0x002FD8D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void Left_OnPress(XUiController _sender, int _mouseButton)
	{
		this.CommandListUI.MoveBackward();
	}

	// Token: 0x060075D6 RID: 30166 RVA: 0x002FF6E4 File Offset: 0x002FD8E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Right_OnPress(XUiController _sender, int _mouseButton)
	{
		this.CommandListUI.MoveForward();
	}

	// Token: 0x060075D7 RID: 30167 RVA: 0x002FF6F1 File Offset: 0x002FD8F1
	[PublicizedFrom(EAccessModifier.Private)]
	public void TwitchManager_ConnectionStateChanged(TwitchManager.InitStates oldState, TwitchManager.InitStates newState)
	{
		this.viewComponent.IsVisible = (newState == TwitchManager.InitStates.Ready);
		this.IsDirty = true;
	}

	// Token: 0x060075D8 RID: 30168 RVA: 0x002FF70C File Offset: 0x002FD90C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (Time.time - this.lastUpdate >= this.secondRotation)
		{
			if (this.showingBitPot || (!this.showingBitPot && this.twitchManager.BitPot != 0))
			{
				this.IsDirty = true;
				this.showingBitPot = !this.showingBitPot;
			}
			this.lastUpdate = Time.time;
		}
		if (this.showingBitPot && this.twitchManager.BitPot == 0)
		{
			this.showingBitPot = false;
			this.IsDirty = true;
			this.lastUpdate = Time.time;
		}
		if (this.localPlayer == null)
		{
			this.localPlayer = base.xui.playerUI.entityPlayer;
		}
		this.window.IsVisible = (this.twitchManager.InitState == TwitchManager.InitStates.Ready && !this.localPlayer.IsDead() && !XUi.InGameMenuOpen);
		if (this.twitchManager != null && this.twitchManager.LocalPlayerXUi == null)
		{
			this.twitchManager.LocalPlayerXUi = base.xui;
		}
		if (!this.window.IsVisible)
		{
			return;
		}
		if (this.window.TargetAlpha == 0f)
		{
			this.window.TargetAlpha = 1f;
		}
		if (!this.twitchManager.HasViewedSettings)
		{
			float num = Mathf.PingPong(Time.time, 0.5f);
			this.optionsButton.DefaultSpriteColor = Color.Lerp(this.defaultGearColor, this.gearBlinkColor, num * 4f);
			float num2 = 1f;
			if (num > 0.25f)
			{
				num2 = 1f + num - 0.25f;
			}
			this.optionsButton.Sprite.SetDimensions((int)(this.gearSpriteSize.x * num2), (int)(this.gearSpriteSize.y * num2));
		}
		else
		{
			this.optionsButton.DefaultSpriteColor = this.defaultGearColor;
		}
		if (this.CommandListUI.IsDirty)
		{
			this.IsDirty = true;
		}
		if (this.twitchManager.CooldownTime > 0f)
		{
			this.IsDirty = true;
		}
		if (this.twitchManager.UIDirty)
		{
			this.IsDirty = true;
		}
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x060075D9 RID: 30169 RVA: 0x002FF943 File Offset: 0x002FDB43
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.progressionButton != null)
		{
			this.progressionButton.Selected = this.twitchManager.UseProgression;
		}
	}

	// Token: 0x060075DA RID: 30170 RVA: 0x002FF96C File Offset: 0x002FDB6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2061600441U)
		{
			if (num <= 1257793062U)
			{
				if (num <= 1195155980U)
				{
					if (num != 1023565667U)
					{
						if (num == 1195155980U)
						{
							if (bindingName == "grouptitlevisible")
							{
								if (this.twitchManager == null)
								{
									value = "false";
									return true;
								}
								if (!this.twitchManager.TwitchActive)
								{
									value = "false";
									return true;
								}
								if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Time || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Startup || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
								{
									value = "false";
									return true;
								}
								if (!this.twitchManager.AllowActions && !this.twitchManager.VotingManager.VotingIsActive && this.twitchManager.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked)
								{
									value = "false";
									return true;
								}
								value = "true";
								return true;
							}
						}
					}
					else if (bindingName == "show_vote_tip")
					{
						value = "false";
						if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready)
						{
							value = "false";
							return true;
						}
						if (this.twitchManager.VotingManager.WinnerShowing && this.twitchManager.VotingManager.VoteTip != "")
						{
							value = "true";
						}
						return true;
					}
				}
				else if (num != 1215668937U)
				{
					if (num == 1257793062U)
					{
						if (bindingName == "status_title")
						{
							if (this.twitchManager == null)
							{
								value = "";
								return true;
							}
							if (this.twitchManager.InitState == TwitchManager.InitStates.Ready)
							{
								if (!this.twitchManager.TwitchActive)
								{
									value = this.lblStatusPaused;
								}
								else if (this.twitchManager.CooldownTime > 0f && this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Time)
								{
									value = this.lblStatusCooldown;
								}
								else if (this.twitchManager.VotingManager.VotingIsActive)
								{
									value = this.lblStatusVote;
								}
								else if (this.twitchManager.CooldownTime > 0f || this.twitchManager.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Always)
								{
									value = this.lblStatusCooldown;
								}
								else
								{
									value = this.lblStatusReady;
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
				else if (bindingName == "showcontent")
				{
					if (this.twitchManager == null)
					{
						value = "false";
						return true;
					}
					if (!this.twitchManager.TwitchActive)
					{
						value = "false";
						return true;
					}
					if (this.twitchManager.InitState != TwitchManager.InitStates.Ready)
					{
						value = "false";
						return true;
					}
					if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Time || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Startup)
					{
						value = "false";
						return true;
					}
					if (this.twitchManager.VotingManager.VotingIsActive)
					{
						value = "true";
						return true;
					}
					if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
					{
						value = "false";
						return true;
					}
					if (this.twitchManager.VoteLockedLevel == TwitchVoteLockTypes.ActionsLocked)
					{
						value = "false";
						return true;
					}
					if (!this.twitchManager.AllowActions)
					{
						value = "false";
						return true;
					}
					if (this.twitchManager.IntegrationSetting == TwitchManager.IntegrationSettings.ExtensionOnly)
					{
						value = "false";
						return true;
					}
					value = "true";
					return true;
				}
			}
			else if (num <= 1875832543U)
			{
				if (num != 1461653202U)
				{
					if (num == 1875832543U)
					{
						if (bindingName == "vote_tip")
						{
							if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready)
							{
								value = "";
								return true;
							}
							value = this.twitchManager.VotingManager.VoteTip;
							return true;
						}
					}
				}
				else if (bindingName == "showcommands")
				{
					value = (this.twitchManager != null && this.twitchManager.InitState == TwitchManager.InitStates.Ready && !this.twitchManager.VotingManager.VotingIsActive).ToString();
					return true;
				}
			}
			else if (num != 1968087740U)
			{
				if (num == 2061600441U)
				{
					if (bindingName == "cooldownfill")
					{
						if (this.twitchManager == null)
						{
							value = "0";
							return true;
						}
						if (this.twitchManager.CurrentCooldownPreset != null && this.twitchManager.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.None)
						{
							value = "0";
							return true;
						}
						if (this.twitchManager.CooldownType != TwitchManager.CooldownTypes.SafeCooldown && this.twitchManager.CooldownType != TwitchManager.CooldownTypes.SafeCooldownExit && (this.twitchManager.CooldownTime > 0f || (this.twitchManager.CurrentCooldownPreset != null && this.twitchManager.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Always)))
						{
							value = "1";
							return true;
						}
						value = (this.twitchManager.CurrentCooldownFill / this.twitchManager.CurrentCooldownPreset.CooldownFillMax).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "commandsheight")
			{
				if (this.CommandListUI != null)
				{
					value = this.CommandListUI.GetHeight().ToString();
				}
				return true;
			}
		}
		else if (num <= 3125508079U)
		{
			if (num <= 2728359946U)
			{
				if (num != 2584460877U)
				{
					if (num == 2728359946U)
					{
						if (bindingName == "grouptitle")
						{
							if (this.twitchManager == null)
							{
								value = "";
								return true;
							}
							if (!this.twitchManager.TwitchActive)
							{
								value = "";
								return true;
							}
							if (this.twitchManager.VotingManager.VotingIsActive)
							{
								value = this.twitchManager.VotingManager.VoteTypeText;
								return true;
							}
							if (this.twitchManager.VoteLockedLevel == TwitchVoteLockTypes.ActionsLocked)
							{
								value = this.lblVoteLocked;
								return true;
							}
							if (this.twitchManager.CooldownTime > 0f && (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Time || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Startup))
							{
								value = "";
								return true;
							}
							if (!this.twitchManager.AllowActions)
							{
								value = "";
								return true;
							}
							if (this.twitchManager.IntegrationSetting == TwitchManager.IntegrationSettings.ExtensionOnly)
							{
								value = Localization.Get("xuiTwitchIntegrationOptionsExtensionOnly", false);
								return true;
							}
							if (this.CommandListUI != null)
							{
								value = this.CommandListUI.CurrentTitle;
								return true;
							}
							value = "";
							return true;
						}
					}
				}
				else if (bindingName == "showpotbalance")
				{
					if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready || this.twitchManager.PimpPotType == TwitchManager.PimpPotSettings.Disabled)
					{
						value = "false";
						return true;
					}
					value = "true";
					return true;
				}
			}
			else if (num != 3053359380U)
			{
				if (num == 3125508079U)
				{
					if (bindingName == "status")
					{
						if (this.twitchManager == null)
						{
							value = "";
							return true;
						}
						value = "";
						if (this.twitchManager.InitState == TwitchManager.InitStates.Ready)
						{
							if ((this.twitchManager.CooldownTime > 0f && this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Time) || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.Startup)
							{
								value = XUiM_PlayerBuffs.ConvertToTimeString(this.twitchManager.CooldownTime);
							}
							else if (this.twitchManager.VotingManager.VotingIsActive && this.twitchManager.VotingManager.CurrentVoteState != TwitchVotingManager.VoteStateTypes.WaitingForActive && this.twitchManager.VotingManager.CurrentVoteState != TwitchVotingManager.VoteStateTypes.EventActive)
							{
								if (this.twitchManager.VotingManager.VoteTimeRemaining > 0f)
								{
									value = XUiM_PlayerBuffs.GetCVarValueAsTimeString(this.twitchManager.VotingManager.VoteTimeRemaining);
								}
							}
							else if (this.twitchManager.CooldownTime > 0f)
							{
								if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting)
								{
									value = this.lblStatusWaiting;
								}
								else if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown)
								{
									value = this.lblStatusBloodMoon;
								}
								else if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.QuestDisabled || this.twitchManager.CooldownType == TwitchManager.CooldownTypes.QuestCooldown)
								{
									value = this.lblStatusQuest;
								}
								else if (this.twitchManager.CooldownType == TwitchManager.CooldownTypes.SafeCooldown)
								{
									value = this.lblStatusSafe;
								}
								else
								{
									value = XUiM_PlayerBuffs.ConvertToTimeString(this.twitchManager.CooldownTime);
								}
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "tip_offset")
			{
				if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready)
				{
					value = "0";
					return true;
				}
				value = this.twitchManager.VotingManager.VoteOffset;
				return true;
			}
		}
		else if (num <= 3380638462U)
		{
			if (num != 3220589183U)
			{
				if (num == 3380638462U)
				{
					if (bindingName == "statuscolor")
					{
						value = ((this.twitchManager != null) ? "[ffffff]" : "[ffffff]");
						return true;
					}
				}
			}
			else if (bindingName == "voteitems")
			{
				value = "3";
				return true;
			}
		}
		else if (num != 3759131736U)
		{
			if (num != 3811588448U)
			{
				if (num == 4228112153U)
				{
					if (bindingName == "showvotes")
					{
						value = (this.twitchManager != null && this.twitchManager.InitState == TwitchManager.InitStates.Ready && this.twitchManager.VotingManager.VotingIsActive).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "potbalance")
			{
				if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready)
				{
					value = "";
					return true;
				}
				if (this.showingBitPot)
				{
					value = string.Format("[FFB400]{0} BC[-]", this.twitchManager.BitPot);
				}
				else
				{
					value = string.Format("{0} {1}", this.twitchManager.RewardPot, (this.twitchManager.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? this.lblPointsSP : this.lblPointsPP);
				}
				return true;
			}
		}
		else if (bindingName == "arrowvisible")
		{
			if (this.twitchManager == null || this.twitchManager.InitState != TwitchManager.InitStates.Ready)
			{
				value = "true";
				return true;
			}
			if (this.twitchManager.VotingManager.VotingIsActive || this.twitchManager.VoteLockedLevel == TwitchVoteLockTypes.ActionsLocked)
			{
				value = "false";
				return true;
			}
			if (!this.twitchManager.AllowActions)
			{
				value = "false";
				return true;
			}
			if (this.twitchManager.IntegrationSetting == TwitchManager.IntegrationSettings.ExtensionOnly)
			{
				value = "false";
				return true;
			}
			value = "true";
			return true;
		}
		return false;
	}

	// Token: 0x060075DB RID: 30171 RVA: 0x0030041F File Offset: 0x002FE61F
	public override void Cleanup()
	{
		base.Cleanup();
		if (this.twitchManager != null)
		{
			this.twitchManager.ConnectionStateChanged -= this.TwitchManager_ConnectionStateChanged;
		}
	}

	// Token: 0x040059D9 RID: 23001
	public static string ID = "";

	// Token: 0x040059DA RID: 23002
	public TwitchManager twitchManager;

	// Token: 0x040059DB RID: 23003
	[PublicizedFrom(EAccessModifier.Private)]
	public Color defaultGearColor;

	// Token: 0x040059DC RID: 23004
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 gearSpriteSize;

	// Token: 0x040059DD RID: 23005
	[PublicizedFrom(EAccessModifier.Private)]
	public Color gearBlinkColor = new Color32(byte.MaxValue, 180, 0, byte.MaxValue);

	// Token: 0x040059DE RID: 23006
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdate;

	// Token: 0x040059DF RID: 23007
	[PublicizedFrom(EAccessModifier.Private)]
	public float secondRotation = 5f;

	// Token: 0x040059E0 RID: 23008
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TwitchCommandList CommandListUI;

	// Token: 0x040059E1 RID: 23009
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button progressionButton;

	// Token: 0x040059E2 RID: 23010
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button optionsButton;

	// Token: 0x040059E3 RID: 23011
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button historyButton;

	// Token: 0x040059E4 RID: 23012
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button pauseButton;

	// Token: 0x040059E5 RID: 23013
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Window window;

	// Token: 0x040059E6 RID: 23014
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showingBitPot;

	// Token: 0x040059E8 RID: 23016
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusReady;

	// Token: 0x040059E9 RID: 23017
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusWaiting;

	// Token: 0x040059EA RID: 23018
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusBloodMoon;

	// Token: 0x040059EB RID: 23019
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusCooldown;

	// Token: 0x040059EC RID: 23020
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusPaused;

	// Token: 0x040059ED RID: 23021
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusSafe;

	// Token: 0x040059EE RID: 23022
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusVote;

	// Token: 0x040059EF RID: 23023
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStatusQuest;

	// Token: 0x040059F0 RID: 23024
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblVoteLocked;

	// Token: 0x040059F1 RID: 23025
	public string lblPointsPP;

	// Token: 0x040059F2 RID: 23026
	public string lblPointsSP;

	// Token: 0x040059F3 RID: 23027
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt trackerheightFormatter = new CachedStringFormatterInt();
}
