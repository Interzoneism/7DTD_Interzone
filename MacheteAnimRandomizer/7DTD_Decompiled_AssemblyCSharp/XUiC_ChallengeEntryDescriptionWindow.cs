using System;
using Audio;
using Challenges;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C23 RID: 3107
[Preserve]
public class XUiC_ChallengeEntryDescriptionWindow : XUiController
{
	// Token: 0x170009DC RID: 2524
	// (get) Token: 0x06005F85 RID: 24453 RVA: 0x0026BBA8 File Offset: 0x00269DA8
	// (set) Token: 0x06005F86 RID: 24454 RVA: 0x0026BBB0 File Offset: 0x00269DB0
	public Challenge CurrentChallengeEntry
	{
		get
		{
			return this.currentChallenge;
		}
		set
		{
			this.currentChallenge = value;
			this.challengeClass = ((this.currentChallenge != null) ? this.currentChallenge.ChallengeClass : null);
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06005F87 RID: 24455 RVA: 0x0026BBDC File Offset: 0x00269DDC
	public override void Init()
	{
		base.Init();
		this.btnTrack = base.GetChildById("btnTrack").GetChildByType<XUiC_SimpleButton>();
		this.btnTrack.OnPressed += this.BtnTrack_OnPressed;
		this.btnComplete = base.GetChildById("btnComplete").GetChildByType<XUiC_SimpleButton>();
		this.btnComplete.OnPressed += this.BtnComplete_OnPressed;
		this.gotoButton = base.GetChildById("gotoButton");
		if (this.gotoButton != null)
		{
			this.gotoButton.OnPress += this.GotoButton_OnPress;
		}
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06005F88 RID: 24456 RVA: 0x0026BC80 File Offset: 0x00269E80
	[PublicizedFrom(EAccessModifier.Private)]
	public void GotoButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.currentChallenge != null)
		{
			BaseChallengeObjective navObjective = this.currentChallenge.GetNavObjective();
			ChallengeObjectiveCraft challengeObjectiveCraft = navObjective as ChallengeObjectiveCraft;
			if (challengeObjectiveCraft != null)
			{
				Recipe itemRecipe = challengeObjectiveCraft.itemRecipe;
				XUiC_WindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "crafting");
				XUiC_RecipeList childByType = base.xui.GetChildByType<XUiC_RecipeList>();
				if (childByType != null)
				{
					childByType.SetRecipeDataByItem(challengeObjectiveCraft.itemRecipe.itemValueType);
					return;
				}
			}
			else if (navObjective is ChallengeObjectiveTwitch)
			{
				XUiC_TwitchWindowSelector.OpenSelectorAndWindow(GameManager.Instance.World.GetPrimaryPlayer(), "Actions", true);
			}
		}
	}

	// Token: 0x06005F89 RID: 24457 RVA: 0x0026BD11 File Offset: 0x00269F11
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.IsDirty = true;
	}

	// Token: 0x06005F8A RID: 24458 RVA: 0x0026BD24 File Offset: 0x00269F24
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty || (this.currentChallenge != null && this.currentChallenge.NeedsUIUpdate))
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		if (base.ViewComponent.UiTransform.gameObject.activeInHierarchy && this.currentChallenge != null)
		{
			PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && !base.xui.playerUI.windowManager.IsInputActive())
			{
				if (guiactions.DPad_Up.WasPressed)
				{
					this.BtnComplete_OnPressed(this.btnComplete, -1);
				}
				if (guiactions.DPad_Left.WasPressed)
				{
					this.BtnTrack_OnPressed(this.btnTrack, -1);
				}
			}
		}
	}

	// Token: 0x06005F8B RID: 24459 RVA: 0x0026BDF8 File Offset: 0x00269FF8
	public void TrackCurrentChallenege()
	{
		if (this.currentChallenge.IsActive)
		{
			if (base.xui.QuestTracker.TrackedChallenge == this.currentChallenge)
			{
				base.xui.QuestTracker.TrackedChallenge = null;
			}
			else
			{
				base.xui.QuestTracker.TrackedChallenge = this.currentChallenge;
				Manager.PlayInsidePlayerHead("ui_challenge_track", -1, 0f, false, false);
			}
			this.entry.Owner.MarkDirty();
		}
	}

	// Token: 0x06005F8C RID: 24460 RVA: 0x0026BE78 File Offset: 0x0026A078
	public void CompleteCurrentChallenege()
	{
		if (this.currentChallenge != null && this.currentChallenge.ReadyToComplete)
		{
			this.currentChallenge.ChallengeState = Challenge.ChallengeStates.Redeemed;
			this.currentChallenge.Redeem();
			QuestEventManager.Current.ChallengeCompleted(this.challengeClass, true);
			this.currentChallenge = this.currentChallenge.Owner.GetNextRedeemableChallenge(this.currentChallenge);
			XUiC_ChallengeEntry selectedEntry = this.entry.Owner.SelectedEntry;
			if (selectedEntry != null && selectedEntry.Entry != this.currentChallenge)
			{
				this.entry.Owner.SetEntryByChallenge(this.currentChallenge);
			}
		}
	}

	// Token: 0x06005F8D RID: 24461 RVA: 0x0026BF19 File Offset: 0x0026A119
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnTrack_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.TrackCurrentChallenege();
	}

	// Token: 0x06005F8E RID: 24462 RVA: 0x0026BF21 File Offset: 0x0026A121
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnComplete_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CompleteCurrentChallenege();
	}

	// Token: 0x06005F8F RID: 24463 RVA: 0x0026BF29 File Offset: 0x0026A129
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
		this.RefreshButtonLabels(PlatformManager.NativePlatform.Input.CurrentInputStyle);
		QuestEventManager.Current.ChallengeComplete += this.Current_ChallengeComplete;
	}

	// Token: 0x06005F90 RID: 24464 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_ChallengeComplete(ChallengeClass challenge, bool isRedeemed)
	{
		base.RefreshBindings(false);
	}

	// Token: 0x06005F91 RID: 24465 RVA: 0x0026BF63 File Offset: 0x0026A163
	public override void OnClose()
	{
		base.OnClose();
		QuestEventManager.Current.ChallengeComplete -= this.Current_ChallengeComplete;
	}

	// Token: 0x06005F92 RID: 24466 RVA: 0x0026BF84 File Offset: 0x0026A184
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2281937273U)
		{
			if (num <= 908978632U)
			{
				if (num <= 227073911U)
				{
					if (num <= 193518673U)
					{
						if (num != 176741054U)
						{
							if (num == 193518673U)
							{
								if (bindingName == "hasobjective5")
								{
									if (this.currentChallenge != null)
									{
										value = (this.currentChallenge.ObjectiveList.Count > 4).ToString();
									}
									else
									{
										value = "false";
									}
									return true;
								}
							}
						}
						else if (bindingName == "hasobjective4")
						{
							if (this.currentChallenge != null)
							{
								value = (this.currentChallenge.ObjectiveList.Count > 3).ToString();
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
					else if (num != 210296292U)
					{
						if (num == 227073911U)
						{
							if (bindingName == "hasobjective3")
							{
								if (this.currentChallenge != null)
								{
									value = (this.currentChallenge.ObjectiveList.Count > 2).ToString();
								}
								else
								{
									value = "false";
								}
								return true;
							}
						}
					}
					else if (bindingName == "hasobjective2")
					{
						if (this.currentChallenge != null)
						{
							value = (this.currentChallenge.ObjectiveList.Count > 1).ToString();
						}
						else
						{
							value = "false";
						}
						return true;
					}
				}
				else if (num <= 338523197U)
				{
					if (num != 260629149U)
					{
						if (num == 338523197U)
						{
							if (bindingName == "enabletrack")
							{
								value = (this.currentChallenge != null && this.currentChallenge.ChallengeState == Challenge.ChallengeStates.Active).ToString();
								return true;
							}
						}
					}
					else if (bindingName == "hasobjective1")
					{
						if (this.currentChallenge != null)
						{
							value = (this.currentChallenge.ObjectiveList.Count > 0).ToString();
						}
						else
						{
							value = "false";
						}
						return true;
					}
				}
				else if (num != 718761959U)
				{
					if (num == 908978632U)
					{
						if (bindingName == "has4objective")
						{
							if (this.currentChallenge != null)
							{
								value = (this.currentChallenge.ObjectiveList.Count == 4).ToString();
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
				}
				else if (bindingName == "entrydescription")
				{
					value = ((this.currentChallenge != null) ? this.challengeClass.GetDescription() : "");
					return true;
				}
			}
			else if (num <= 1507958455U)
			{
				if (num <= 1152134642U)
				{
					if (num != 1003914668U)
					{
						if (num == 1152134642U)
						{
							if (bindingName == "entryicon")
							{
								value = ((this.currentChallenge != null) ? this.challengeClass.Icon : "");
								return true;
							}
						}
					}
					else if (bindingName == "rewardtitle")
					{
						value = Localization.Get("xuiRewards", false);
						return true;
					}
				}
				else if (num != 1498809836U)
				{
					if (num == 1507958455U)
					{
						if (bindingName == "entryshortdescription")
						{
							value = ((this.currentChallenge != null) ? this.challengeClass.ShortDescription : "");
							return true;
						}
					}
				}
				else if (bindingName == "haschallenge")
				{
					value = (this.currentChallenge != null).ToString();
					return true;
				}
			}
			else if (num <= 1833103823U)
			{
				if (num != 1518662865U)
				{
					if (num == 1833103823U)
					{
						if (bindingName == "showgoto")
						{
							value = (this.currentChallenge != null && this.currentChallenge.ChallengeClass.HasNavType).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "rewardtext")
				{
					value = ((this.currentChallenge != null) ? this.challengeClass.RewardText : "");
					return true;
				}
			}
			else if (num != 1960932512U)
			{
				if (num != 2048631988U)
				{
					if (num == 2281937273U)
					{
						if (bindingName == "has1objective")
						{
							if (this.currentChallenge != null)
							{
								value = (this.currentChallenge.ObjectiveList.Count == 1).ToString();
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
				}
				else if (bindingName == "hasreward")
				{
					value = ((this.currentChallenge != null) ? (this.challengeClass.RewardEvent != "").ToString() : "false");
					return true;
				}
			}
			else if (bindingName == "adjustedheight")
			{
				if (this.currentChallenge != null)
				{
					switch (this.currentChallenge.ObjectiveList.Count)
					{
					case 1:
						value = "276";
						return true;
					case 2:
						value = "236";
						return true;
					case 3:
						value = "196";
						return true;
					case 4:
						value = "156";
						return true;
					case 5:
						value = "116";
						return true;
					}
				}
				value = "196";
				return true;
			}
		}
		else if (num <= 3256735460U)
		{
			if (num <= 2958318179U)
			{
				if (num <= 2675497066U)
				{
					if (num != 2650623958U)
					{
						if (num == 2675497066U)
						{
							if (bindingName == "entrygroup")
							{
								value = ((this.currentChallenge != null) ? this.currentChallenge.ChallengeGroup.Title : "");
								return true;
							}
						}
					}
					else if (bindingName == "enableredeem")
					{
						value = (this.currentChallenge != null && this.currentChallenge.ReadyToComplete).ToString();
						return true;
					}
				}
				else if (num != 2725401013U)
				{
					if (num == 2958318179U)
					{
						if (bindingName == "entrytitle")
						{
							value = ((this.currentChallenge != null) ? this.challengeClass.Title : "");
							return true;
						}
					}
				}
				else if (bindingName == "has5objective")
				{
					if (this.currentChallenge != null)
					{
						value = (this.currentChallenge.ObjectiveList.Count == 5).ToString();
					}
					else
					{
						value = "false";
					}
					return true;
				}
			}
			else if (num <= 3223180222U)
			{
				if (num != 3206402603U)
				{
					if (num == 3223180222U)
					{
						if (bindingName == "objective2")
						{
							if (this.currentChallenge != null)
							{
								value = ((this.currentChallenge.ObjectiveList.Count > 1) ? this.currentChallenge.ObjectiveList[1].ObjectiveText : "");
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
				}
				else if (bindingName == "objective1")
				{
					if (this.currentChallenge != null)
					{
						value = ((this.currentChallenge.ObjectiveList.Count > 0) ? this.currentChallenge.ObjectiveList[0].ObjectiveText : "");
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (num != 3239957841U)
			{
				if (num == 3256735460U)
				{
					if (bindingName == "objective4")
					{
						if (this.currentChallenge != null)
						{
							value = ((this.currentChallenge.ObjectiveList.Count > 3) ? this.currentChallenge.ObjectiveList[3].ObjectiveText : "");
						}
						else
						{
							value = "";
						}
						return true;
					}
				}
			}
			else if (bindingName == "objective3")
			{
				if (this.currentChallenge != null)
				{
					value = ((this.currentChallenge.ObjectiveList.Count > 2) ? this.currentChallenge.ObjectiveList[2].ObjectiveText : "");
				}
				else
				{
					value = "";
				}
				return true;
			}
		}
		else if (num <= 3746182248U)
		{
			if (num <= 3279637982U)
			{
				if (num != 3273513079U)
				{
					if (num == 3279637982U)
					{
						if (bindingName == "has2objective")
						{
							if (this.currentChallenge != null)
							{
								value = (this.currentChallenge.ObjectiveList.Count == 2).ToString();
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
				}
				else if (bindingName == "objective5")
				{
					if (this.currentChallenge != null)
					{
						value = ((this.currentChallenge.ObjectiveList.Count > 4) ? this.currentChallenge.ObjectiveList[4].ObjectiveText : "");
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (num != 3479566019U)
			{
				if (num == 3746182248U)
				{
					if (bindingName == "objectivefill1")
					{
						if (this.currentChallenge != null)
						{
							value = ((this.currentChallenge.ObjectiveList.Count > 0) ? this.currentChallenge.ObjectiveList[0].FillAmount.ToString() : "0");
						}
						else
						{
							value = "0";
						}
						return true;
					}
				}
			}
			else if (bindingName == "has3objective")
			{
				if (this.currentChallenge != null)
				{
					value = (this.currentChallenge.ObjectiveList.Count == 3).ToString();
				}
				else
				{
					value = "false";
				}
				return true;
			}
		}
		else if (num <= 3796515105U)
		{
			if (num != 3779737486U)
			{
				if (num == 3796515105U)
				{
					if (bindingName == "objectivefill2")
					{
						if (this.currentChallenge != null)
						{
							value = ((this.currentChallenge.ObjectiveList.Count > 1) ? this.currentChallenge.ObjectiveList[1].FillAmount.ToString() : "0");
						}
						else
						{
							value = "0";
						}
						return true;
					}
				}
			}
			else if (bindingName == "objectivefill3")
			{
				if (this.currentChallenge != null)
				{
					value = ((this.currentChallenge.ObjectiveList.Count > 2) ? this.currentChallenge.ObjectiveList[2].FillAmount.ToString() : "0");
				}
				else
				{
					value = "0";
				}
				return true;
			}
		}
		else if (num != 3813292724U)
		{
			if (num != 3830070343U)
			{
				if (num == 4060322893U)
				{
					if (bindingName == "showempty")
					{
						value = (this.currentChallenge == null).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "objectivefill4")
			{
				if (this.currentChallenge != null)
				{
					value = ((this.currentChallenge.ObjectiveList.Count > 3) ? this.currentChallenge.ObjectiveList[3].FillAmount.ToString() : "0");
				}
				else
				{
					value = "0";
				}
				return true;
			}
		}
		else if (bindingName == "objectivefill5")
		{
			if (this.currentChallenge != null)
			{
				value = ((this.currentChallenge.ObjectiveList.Count > 4) ? this.currentChallenge.ObjectiveList[4].FillAmount.ToString() : "0");
			}
			else
			{
				value = "0";
			}
			return true;
		}
		return false;
	}

	// Token: 0x06005F93 RID: 24467 RVA: 0x0026CB61 File Offset: 0x0026AD61
	public void SetChallenge(XUiC_ChallengeEntry challengeEntry)
	{
		this.entry = challengeEntry;
		if (this.entry != null)
		{
			this.CurrentChallengeEntry = this.entry.Entry;
			return;
		}
		this.CurrentChallengeEntry = null;
	}

	// Token: 0x06005F94 RID: 24468 RVA: 0x0026CB8B File Offset: 0x0026AD8B
	[PublicizedFrom(EAccessModifier.Private)]
	public new void OnLastInputStyleChanged(PlayerInputManager.InputStyle _style)
	{
		this.RefreshButtonLabels(_style);
	}

	// Token: 0x06005F95 RID: 24469 RVA: 0x0026CB94 File Offset: 0x0026AD94
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshButtonLabels(PlayerInputManager.InputStyle _style)
	{
		if (_style == PlayerInputManager.InputStyle.Keyboard)
		{
			(this.btnTrack.GetChildById("btnLabel").ViewComponent as XUiV_Label).Text = string.Format(Localization.Get("journalTrack", false), LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Left.GetBindingString(false, PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.KeyboardWithAngleBrackets, false, null));
			(this.btnComplete.GetChildById("btnLabel").ViewComponent as XUiV_Label).Text = string.Format(Localization.Get("journalComplete", false), LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Up.GetBindingString(false, PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.KeyboardWithAngleBrackets, false, null));
			return;
		}
		(this.btnTrack.GetChildById("btnLabel").ViewComponent as XUiV_Label).Text = string.Format(Localization.Get("journalTrack", false), LocalPlayerUI.primaryUI.playerInput.GUIActions.HalfStack.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
		(this.btnComplete.GetChildById("btnLabel").ViewComponent as XUiV_Label).Text = string.Format(Localization.Get("journalComplete", false), LocalPlayerUI.primaryUI.playerInput.GUIActions.Inspect.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
	}

	// Token: 0x040047FE RID: 18430
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ChallengeEntry entry;

	// Token: 0x040047FF RID: 18431
	[PublicizedFrom(EAccessModifier.Private)]
	public Challenge currentChallenge;

	// Token: 0x04004800 RID: 18432
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeClass challengeClass;

	// Token: 0x04004801 RID: 18433
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnTrack;

	// Token: 0x04004802 RID: 18434
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnComplete;

	// Token: 0x04004803 RID: 18435
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController gotoButton;

	// Token: 0x04004804 RID: 18436
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string DayTimeFormatString = Localization.Get("xuiDay", false) + " {0}, {1:00}:{2:00}";

	// Token: 0x04004805 RID: 18437
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<ulong> daytimeFormatter = new CachedStringFormatter<ulong>((ulong _worldTime) => ValueDisplayFormatters.WorldTime(_worldTime, XUiC_ChallengeEntryDescriptionWindow.DayTimeFormatString));
}
