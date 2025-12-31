using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DAB RID: 3499
[Preserve]
public class XUiC_QuestEntry : XUiController
{
	// Token: 0x17000AF5 RID: 2805
	// (get) Token: 0x06006D6B RID: 28011 RVA: 0x002C9EEE File Offset: 0x002C80EE
	// (set) Token: 0x06006D6C RID: 28012 RVA: 0x002C9EF8 File Offset: 0x002C80F8
	public Quest Quest
	{
		get
		{
			return this.quest;
		}
		set
		{
			this.quest = value;
			this.questClass = ((value != null) ? QuestClass.GetQuest(this.quest.ID) : null);
			this.IsDirty = true;
			base.ViewComponent.Enabled = (value != null);
			this.viewComponent.IsNavigatable = (base.ViewComponent.IsSnappable = (value != null));
		}
	}

	// Token: 0x17000AF6 RID: 2806
	// (get) Token: 0x06006D6D RID: 28013 RVA: 0x002C9F5B File Offset: 0x002C815B
	// (set) Token: 0x06006D6E RID: 28014 RVA: 0x002C9F63 File Offset: 0x002C8163
	public XUiC_QuestWindowGroup QuestUIHandler { get; set; }

	// Token: 0x17000AF7 RID: 2807
	// (get) Token: 0x06006D6F RID: 28015 RVA: 0x002C9F6C File Offset: 0x002C816C
	// (set) Token: 0x06006D70 RID: 28016 RVA: 0x002C9F74 File Offset: 0x002C8174
	public bool Tracked { get; set; }

	// Token: 0x17000AF8 RID: 2808
	// (get) Token: 0x06006D71 RID: 28017 RVA: 0x002C9F7D File Offset: 0x002C817D
	// (set) Token: 0x06006D72 RID: 28018 RVA: 0x002C9F85 File Offset: 0x002C8185
	public SharedQuestEntry SharedQuestEntry { get; set; }

	// Token: 0x06006D73 RID: 28019 RVA: 0x002C9F90 File Offset: 0x002C8190
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.quest != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 783488098U)
		{
			if (num <= 112224632U)
			{
				if (num != 33281100U)
				{
					if (num == 112224632U)
					{
						if (bindingName == "questicon")
						{
							value = "";
							if (flag)
							{
								if (this.quest.CurrentState == Quest.QuestState.Failed)
								{
									value = this.failedIcon;
								}
								else if (this.quest.CurrentState == Quest.QuestState.Completed)
								{
									value = this.completeIcon;
								}
								else if (this.quest.CurrentState == Quest.QuestState.InProgress || this.quest.CurrentState == Quest.QuestState.ReadyForTurnIn)
								{
									if (this.quest.CurrentPhase == this.questClass.HighestPhase && this.questClass.CompletionType == QuestClass.CompletionTypes.TurnIn)
									{
										value = this.finishedIcon;
									}
									else if (this.quest.QuestClass.AddsToTierComplete && !base.xui.playerUI.entityPlayer.QuestJournal.CanAddProgression)
									{
										value = this.questlimitedIcon;
									}
									else if (this.quest.SharedOwnerID == -1)
									{
										value = this.questClass.Icon;
									}
									else
									{
										value = this.sharedIcon;
									}
								}
								else if (!this.quest.QuestClass.AddsToTierComplete || base.xui.playerUI.entityPlayer.QuestJournal.CanAddProgression)
								{
									value = this.questClass.Icon;
								}
								else
								{
									value = this.questlimitedIcon;
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "istracking")
				{
					value = (flag ? this.quest.Tracked.ToString() : "false");
					return true;
				}
			}
			else if (num != 765459171U)
			{
				if (num == 783488098U)
				{
					if (bindingName == "distance")
					{
						if (flag && (this.quest.Active || this.SharedQuestEntry != null) && this.quest.HasPosition)
						{
							Vector3 position = this.quest.Position;
							Vector3 position2 = base.xui.playerUI.entityPlayer.GetPosition();
							position.y = 0f;
							position2.y = 0f;
							float num2 = (position - position2).magnitude;
							float num3 = num2;
							string text = "m";
							if (num2 >= 1000f)
							{
								num2 /= 1000f;
								text = "km";
							}
							if (this.quest.MapObject is MapObjectTreasureChest)
							{
								float num4 = (float)(this.quest.MapObject as MapObjectTreasureChest).DefaultRadius;
								if (num3 < num4)
								{
									num4 = EffectManager.GetValue(PassiveEffects.TreasureRadius, null, num4, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
									num4 = Mathf.Clamp(num4, 0f, 13f);
									if (num3 < num4)
									{
										value = this.zerodistanceFormatter.Format(text);
									}
								}
								else
								{
									value = this.distanceFormatter.Format(num2, text);
								}
							}
							else
							{
								value = this.distanceFormatter.Format(num2, text);
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
			else if (bindingName == "rowstatecolor")
			{
				value = (this.Selected ? "255,255,255,255" : (this.IsHovered ? this.hoverColor : this.rowColor));
				return true;
			}
		}
		else if (num <= 2730462270U)
		{
			if (num != 1656712805U)
			{
				if (num == 2730462270U)
				{
					if (bindingName == "questname")
					{
						value = (flag ? this.questClass.Name : "");
						return true;
					}
				}
			}
			else if (bindingName == "rowstatesprite")
			{
				value = (this.Selected ? "ui_game_select_row" : "menu_empty");
				return true;
			}
		}
		else if (num != 3106195591U)
		{
			if (num == 3644377122U)
			{
				if (bindingName == "textstatecolor")
				{
					value = "255,255,255,255";
					if (flag)
					{
						Quest.QuestState currentState = this.quest.CurrentState;
						if (currentState <= Quest.QuestState.ReadyForTurnIn)
						{
							value = this.enabledColor;
						}
						else
						{
							value = this.disabledColor;
						}
					}
					return true;
				}
			}
		}
		else if (bindingName == "iconcolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				switch (this.quest.CurrentState)
				{
				case Quest.QuestState.NotStarted:
				case Quest.QuestState.InProgress:
				case Quest.QuestState.ReadyForTurnIn:
					if (this.quest.CurrentPhase == this.questClass.HighestPhase && this.questClass.CompletionType == QuestClass.CompletionTypes.TurnIn)
					{
						value = this.finishedColor;
					}
					else if (this.quest.QuestClass.AddsToTierComplete && !base.xui.playerUI.entityPlayer.QuestJournal.CanAddProgression)
					{
						value = this.failedColor;
					}
					else if (this.quest.SharedOwnerID == -1)
					{
						value = this.enabledColor;
					}
					else
					{
						value = this.sharedColor;
					}
					break;
				case Quest.QuestState.Completed:
					value = this.completeColor;
					break;
				case Quest.QuestState.Failed:
					value = this.failedColor;
					break;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006D74 RID: 28020 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x06006D75 RID: 28021 RVA: 0x002CA4EA File Offset: 0x002C86EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.Quest == null)
		{
			this.IsHovered = false;
			return;
		}
		if (this.IsHovered != _isOver)
		{
			this.IsHovered = _isOver;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006D76 RID: 28022 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x06006D77 RID: 28023 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x06006D78 RID: 28024 RVA: 0x002CA51C File Offset: 0x002C871C
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 2218008692U)
		{
			if (num <= 1285243605U)
			{
				if (num != 185729116U)
				{
					if (num != 1048851580U)
					{
						if (num == 1285243605U)
						{
							if (name == "finished_icon")
							{
								this.finishedIcon = value;
								return true;
							}
						}
					}
					else if (name == "failed_icon")
					{
						this.failedIcon = value;
						return true;
					}
				}
				else if (name == "shared_icon")
				{
					this.sharedIcon = value;
					return true;
				}
			}
			else if (num != 1553164900U)
			{
				if (num != 1627114004U)
				{
					if (num == 2218008692U)
					{
						if (name == "shared_color")
						{
							this.sharedColor = value;
							return true;
						}
					}
				}
				else if (name == "failed_color")
				{
					this.failedColor = value;
					return true;
				}
			}
			else if (name == "quest_limited_icon")
			{
				this.questlimitedIcon = value;
				return true;
			}
		}
		else if (num <= 3152862043U)
		{
			if (num != 2531019123U)
			{
				if (num != 2911778486U)
				{
					if (num == 3152862043U)
					{
						if (name == "finished_color")
						{
							this.finishedColor = value;
							return true;
						}
					}
				}
				else if (name == "complete_color")
				{
					this.completeColor = value;
					return true;
				}
			}
			else if (name == "row_color")
			{
				this.rowColor = value;
				return true;
			}
		}
		else if (num <= 3868148786U)
		{
			if (num != 3387915097U)
			{
				if (num == 3868148786U)
				{
					if (name == "enabled_color")
					{
						this.enabledColor = value;
						return true;
					}
				}
			}
			else if (name == "hover_color")
			{
				this.hoverColor = value;
				return true;
			}
		}
		else if (num != 4076031121U)
		{
			if (num == 4270887654U)
			{
				if (name == "complete_icon")
				{
					this.completeIcon = value;
					return true;
				}
			}
		}
		else if (name == "disabled_color")
		{
			this.disabledColor = value;
			return true;
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x04005304 RID: 21252
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x04005305 RID: 21253
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x04005306 RID: 21254
	[PublicizedFrom(EAccessModifier.Private)]
	public string failedColor;

	// Token: 0x04005307 RID: 21255
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeColor;

	// Token: 0x04005308 RID: 21256
	[PublicizedFrom(EAccessModifier.Private)]
	public string finishedColor;

	// Token: 0x04005309 RID: 21257
	[PublicizedFrom(EAccessModifier.Private)]
	public string sharedColor;

	// Token: 0x0400530A RID: 21258
	[PublicizedFrom(EAccessModifier.Private)]
	public string failedIcon;

	// Token: 0x0400530B RID: 21259
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeIcon;

	// Token: 0x0400530C RID: 21260
	[PublicizedFrom(EAccessModifier.Private)]
	public string finishedIcon;

	// Token: 0x0400530D RID: 21261
	[PublicizedFrom(EAccessModifier.Private)]
	public string sharedIcon;

	// Token: 0x0400530E RID: 21262
	[PublicizedFrom(EAccessModifier.Private)]
	public string questlimitedIcon = "ui_game_symbol_quest_limited";

	// Token: 0x0400530F RID: 21263
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x04005310 RID: 21264
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x04005311 RID: 21265
	public new bool Selected;

	// Token: 0x04005312 RID: 21266
	public bool IsHovered;

	// Token: 0x04005313 RID: 21267
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x04005314 RID: 21268
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest quest;

	// Token: 0x04005318 RID: 21272
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<float, string> distanceFormatter = new CachedStringFormatter<float, string>((float _f, string _s) => _f.ToCultureInvariantString("0.0") + " " + _s);

	// Token: 0x04005319 RID: 21273
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string> zerodistanceFormatter = new CachedStringFormatter<string>((string _s) => "0 " + _s);
}
