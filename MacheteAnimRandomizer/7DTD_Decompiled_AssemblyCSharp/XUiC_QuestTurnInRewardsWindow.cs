using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DC8 RID: 3528
[Preserve]
public class XUiC_QuestTurnInRewardsWindow : XUiController
{
	// Token: 0x17000B1F RID: 2847
	// (get) Token: 0x06006E69 RID: 28265 RVA: 0x002D08A6 File Offset: 0x002CEAA6
	// (set) Token: 0x06006E6A RID: 28266 RVA: 0x002D08AE File Offset: 0x002CEAAE
	public Quest CurrentQuest
	{
		get
		{
			return this.currentQuest;
		}
		set
		{
			this.currentQuest = value;
			this.questClass = ((value != null) ? QuestClass.GetQuest(this.currentQuest.ID) : null);
			base.RefreshBindings(true);
			this.SetupOptions();
		}
	}

	// Token: 0x17000B20 RID: 2848
	// (get) Token: 0x06006E6B RID: 28267 RVA: 0x002D08E0 File Offset: 0x002CEAE0
	// (set) Token: 0x06006E6C RID: 28268 RVA: 0x002D08E8 File Offset: 0x002CEAE8
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x17000B21 RID: 2849
	// (get) Token: 0x06006E6D RID: 28269 RVA: 0x002D08F1 File Offset: 0x002CEAF1
	// (set) Token: 0x06006E6E RID: 28270 RVA: 0x002D08FC File Offset: 0x002CEAFC
	public XUiC_QuestTurnInEntry SelectedEntry
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
			if (this.selectedEntry == null)
			{
				this.InfoWindow.SetItemStack(null, true);
				return;
			}
			this.selectedEntry.Selected = true;
			if (this.selectedEntry.Reward is RewardItem || this.selectedEntry.Reward is RewardLootItem)
			{
				this.InfoWindow.SetItemStack(this.selectedEntry, true);
				return;
			}
			this.InfoWindow.SetItemStack(null, true);
		}
	}

	// Token: 0x06006E6F RID: 28271 RVA: 0x002D098C File Offset: 0x002CEB8C
	public override void Init()
	{
		base.Init();
		this.xuiQuestDescriptionLabel = Localization.Get("xuiDescriptionLabel", false);
		this.entryList = base.GetChildById("rectOptions").GetChildById("gridOptions").GetChildrenByType<XUiC_QuestTurnInEntry>(null);
		this.btnAccept = base.GetChildById("rectAccept").GetChildById("btnAccept");
		((XUiV_Button)this.btnAccept.GetChildById("clickable").ViewComponent).Controller.OnPress += this.BtnAccept_OnPress;
	}

	// Token: 0x06006E70 RID: 28272 RVA: 0x002D0A1C File Offset: 0x002CEC1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnAccept_OnPress(XUiController _sender, int _mouseButton)
	{
		float value = EffectManager.GetValue(PassiveEffects.QuestRewardChoiceCount, null, 1f, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (this.selectedEntry != null)
		{
			if (this.selectedEntryList.Contains(this.selectedEntry))
			{
				this.selectedEntry.Chosen = false;
				this.selectedEntryList.Remove(this.selectedEntry);
			}
			else
			{
				this.selectedEntry.Chosen = true;
				this.selectedEntryList.Add(this.selectedEntry);
			}
		}
		if (this.optionCount <= 1 || (float)this.selectedEntryList.Count == value)
		{
			List<BaseReward> list = new List<BaseReward>();
			for (int i = 0; i < this.selectedEntryList.Count; i++)
			{
				list.Add(this.selectedEntryList[i].Reward);
			}
			if (this.CurrentQuest.CanTurnInQuest(list))
			{
				this.CurrentQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.TurnIn, list, true, this.NPC);
				(base.WindowGroup.Controller as XUiC_QuestTurnInWindowGroup).TryNextComplete();
			}
			else
			{
				GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "You need to clear up some inventory space before turning in this quest.", string.Empty, "ui_denied", null, false, false, 0f);
				this.selectedEntry.Chosen = false;
				this.selectedEntryList.Remove(this.selectedEntry);
			}
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006E71 RID: 28273 RVA: 0x002D0B8C File Offset: 0x002CED8C
	public override void OnOpen()
	{
		base.OnOpen();
		this.SelectedEntry = null;
		this.CurrentQuest = base.xui.Dialog.QuestTurnIn;
	}

	// Token: 0x06006E72 RID: 28274 RVA: 0x002D0BB4 File Offset: 0x002CEDB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupOptions()
	{
		float value = EffectManager.GetValue(PassiveEffects.QuestRewardOptionCount, null, (float)this.currentQuest.QuestClass.RewardChoicesCount, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.optionCount = 0;
		int num = 0;
		if (this.selectedEntry != null)
		{
			this.SelectedEntry = null;
		}
		this.selectedEntryList.Clear();
		this.rewardList.Clear();
		for (int i = 0; i < this.entryList.Length; i++)
		{
			this.entryList[i].OnPress -= this.TurnInEntryPressed;
			this.entryList[i].SetBaseReward(null);
		}
		for (int j = 0; j < this.currentQuest.Rewards.Count; j++)
		{
			this.rewardList.Add(this.currentQuest.Rewards[j]);
		}
		this.rewardList = (from o in this.rewardList
		orderby o.RewardIndex
		select o).ToList<BaseReward>();
		for (int k = 0; k < this.rewardList.Count; k++)
		{
			BaseReward baseReward = this.rewardList[k];
			this.entryList[num].OnPress -= this.TurnInEntryPressed;
			if (baseReward.isChosenReward)
			{
				this.entryList[num].SetBaseReward(baseReward);
				this.entryList[num].Chosen = false;
				this.entryList[num++].OnPress += this.TurnInEntryPressed;
				this.optionCount++;
				if (value == (float)num || num >= this.entryList.Length)
				{
					break;
				}
			}
		}
		this.entryList[0].SelectCursorElement(true, false);
		if (this.optionCount == 1)
		{
			XUiC_QuestTurnInEntry xuiC_QuestTurnInEntry = this.entryList[0];
			this.SelectedEntry = xuiC_QuestTurnInEntry;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006E73 RID: 28275 RVA: 0x002D0DB4 File Offset: 0x002CEFB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TurnInEntryPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_QuestTurnInEntry xuiC_QuestTurnInEntry = _sender as XUiC_QuestTurnInEntry;
		if (xuiC_QuestTurnInEntry != null)
		{
			this.SelectedEntry = xuiC_QuestTurnInEntry;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006E74 RID: 28276 RVA: 0x002D0DDC File Offset: 0x002CEFDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleEntryPressed(XUiC_QuestTurnInEntry entry)
	{
		float value = EffectManager.GetValue(PassiveEffects.QuestRewardChoiceCount, null, 1f, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (this.selectedEntryList.Contains(entry))
		{
			entry.Selected = false;
			this.selectedEntryList.Remove(entry);
			if (entry == this.SelectedEntry)
			{
				this.SelectedEntry = null;
				return;
			}
		}
		else if ((float)this.selectedEntryList.Count < value)
		{
			this.selectedEntryList.Add(entry);
			this.SelectedEntry = entry;
			entry.Selected = true;
		}
	}

	// Token: 0x06006E75 RID: 28277 RVA: 0x002D0E78 File Offset: 0x002CF078
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2646005107U)
		{
			if (num <= 1061431296U)
			{
				if (num != 161079706U)
				{
					if (num != 213234470U)
					{
						if (num == 1061431296U)
						{
							if (bindingName == "questitemrewardstitle")
							{
								value = Localization.Get("xuiItems", false);
								return true;
							}
						}
					}
					else if (bindingName == "acceptbuttontext")
					{
						if (this.currentQuest == null)
						{
							value = "";
						}
						if (this.SelectedEntry != null)
						{
							if (this.SelectedEntry.Chosen)
							{
								value = Localization.Get("xuiUnSelect", false);
							}
							else
							{
								float value2 = EffectManager.GetValue(PassiveEffects.QuestRewardChoiceCount, null, 1f, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
								if ((float)(this.selectedEntryList.Count + 1) == value2)
								{
									value = Localization.Get("lblContextActionComplete", false);
								}
								else
								{
									value = Localization.Get("mmBtnSelect", false);
								}
							}
						}
						else
						{
							value = Localization.Get("mmBtnSelect", false);
						}
						return true;
					}
				}
				else if (bindingName == "sharedbyname")
				{
					if (this.currentQuest == null)
					{
						value = "";
					}
					else
					{
						PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.currentQuest.SharedOwnerID);
						if (playerDataFromEntityID != null)
						{
							value = GameUtils.SafeStringFormat(playerDataFromEntityID.PlayerName.DisplayName);
						}
						else
						{
							value = "";
						}
					}
					return true;
				}
			}
			else if (num <= 1895377404U)
			{
				if (num != 1468586257U)
				{
					if (num == 1895377404U)
					{
						if (bindingName == "questhasxpreward")
						{
							if (this.currentQuest == null)
							{
								value = "false";
							}
							else
							{
								int num2 = 0;
								for (int i = 0; i < this.currentQuest.Rewards.Count; i++)
								{
									if (this.currentQuest.Rewards[i] is RewardExp && !this.currentQuest.Rewards[i].isChosenReward)
									{
										num2++;
									}
								}
								value = (num2 > 0).ToString();
							}
							return true;
						}
					}
				}
				else if (bindingName == "questsubtitle")
				{
					value = ((this.currentQuest != null) ? this.questClass.SubTitle : "");
					return true;
				}
			}
			else if (num != 1985695849U)
			{
				if (num == 2646005107U)
				{
					if (bindingName == "chosentitle")
					{
						if (this.currentQuest == null)
						{
							value = "";
						}
						if (XUiM_Quest.HasQuestRewards(this.currentQuest, base.xui.playerUI.entityPlayer, true))
						{
							float value3 = EffectManager.GetValue(PassiveEffects.QuestRewardChoiceCount, null, 1f, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
							value = ((value3 == 1f) ? Localization.Get("xuiChooseOne", false) : Localization.Get("xuiChooseTwo", false));
						}
						else
						{
							value = "";
						}
						return true;
					}
				}
			}
			else if (bindingName == "questcategory")
			{
				value = ((this.currentQuest != null) ? this.questClass.Category : "");
				return true;
			}
		}
		else if (num <= 2940790865U)
		{
			if (num <= 2718428424U)
			{
				if (num != 2680020434U)
				{
					if (num == 2718428424U)
					{
						if (bindingName == "questhasitemrewards")
						{
							if (this.currentQuest == null)
							{
								value = "false";
							}
							else
							{
								int num3 = 0;
								for (int j = 0; j < this.currentQuest.Rewards.Count; j++)
								{
									if ((this.currentQuest.Rewards[j] is RewardItem || this.currentQuest.Rewards[j] is RewardLootItem) && !this.currentQuest.Rewards[j].isChosenReward)
									{
										num3++;
									}
								}
								value = (num3 > 0).ToString();
							}
							return true;
						}
					}
				}
				else if (bindingName == "questxpreward")
				{
					if (this.currentQuest == null)
					{
						value = "";
					}
					else
					{
						int num4 = 0;
						for (int k = 0; k < this.currentQuest.Rewards.Count; k++)
						{
							if (this.currentQuest.Rewards[k] is RewardExp)
							{
								int num5 = Convert.ToInt32(this.currentQuest.Rewards[k].Value) * GameStats.GetInt(EnumGameStats.XPMultiplier) / 100;
								num4 += Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.PlayerExpGain, null, (float)num5, base.xui.playerUI.entityPlayer, null, XUiM_Quest.QuestTag, true, true, true, true, true, 1, true, false));
							}
						}
						value = string.Format(this.rewardNumberFormat, num4);
					}
					return true;
				}
			}
			else if (num != 2745591350U)
			{
				if (num == 2940790865U)
				{
					if (bindingName == "questdescription")
					{
						value = ((this.currentQuest != null) ? this.currentQuest.GetParsedText(this.questClass.Description) : "");
						return true;
					}
				}
			}
			else if (bindingName == "questitemrewards")
			{
				value = XUiM_Quest.GetQuestItemRewards(this.currentQuest, base.xui.playerUI.entityPlayer, this.rewardItemFormat, this.rewardItemBonusFormat);
				return true;
			}
		}
		else if (num <= 3357817217U)
		{
			if (num != 3047389681U)
			{
				if (num == 3357817217U)
				{
					if (bindingName == "questcompletetext")
					{
						value = ((this.currentQuest != null) ? this.questClass.CompleteText : "Needs real complete text.");
						return true;
					}
				}
			}
			else if (bindingName == "questtitle")
			{
				value = ((this.currentQuest != null) ? this.questtitleFormatter.Format(this.questClass.Category, this.questClass.SubTitle) : this.xuiQuestDescriptionLabel);
				return true;
			}
		}
		else if (num != 3461297649U)
		{
			if (num == 4060322893U)
			{
				if (bindingName == "showempty")
				{
					value = (this.currentQuest == null).ToString();
					return true;
				}
			}
		}
		else if (bindingName == "questxptitle")
		{
			if (this.currentQuest == null)
			{
				value = "";
			}
			else
			{
				value = Localization.Get("RewardXP_keyword", false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006E76 RID: 28278 RVA: 0x002D1538 File Offset: 0x002CF738
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "xp_format"))
			{
				if (!(name == "xp_bonus_format"))
				{
					if (!(name == "item_format"))
					{
						if (!(name == "item_bonus_format"))
						{
							return false;
						}
						this.rewardItemBonusFormat = value;
					}
					else
					{
						this.rewardItemFormat = value;
					}
				}
				else
				{
					this.rewardNumberBonusFormat = value;
				}
			}
			else
			{
				this.rewardNumberFormat = value;
			}
			return true;
		}
		return flag;
	}

	// Token: 0x040053EC RID: 21484
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x040053ED RID: 21485
	[PublicizedFrom(EAccessModifier.Private)]
	public string xuiQuestDescriptionLabel;

	// Token: 0x040053EE RID: 21486
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardNumberFormat = "[DECEA3]{0}[-]";

	// Token: 0x040053EF RID: 21487
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardNumberBonusFormat = "[DECEA3]{0}[-] ([DECEA3]{1}[-] {2})";

	// Token: 0x040053F0 RID: 21488
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardItemFormat = "[DECEA3]{0}[-] {1}";

	// Token: 0x040053F1 RID: 21489
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardItemBonusFormat = "[DECEA3]{0}[-] {1} ([DECEA3]{2}[-] {3})";

	// Token: 0x040053F2 RID: 21490
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestTurnInEntry[] entryList;

	// Token: 0x040053F3 RID: 21491
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button acceptButton;

	// Token: 0x040053F4 RID: 21492
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BaseReward> rewardList = new List<BaseReward>();

	// Token: 0x040053F5 RID: 21493
	public EntityNPC NPC;

	// Token: 0x040053F6 RID: 21494
	[PublicizedFrom(EAccessModifier.Private)]
	public int optionCount;

	// Token: 0x040053F7 RID: 21495
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest currentQuest;

	// Token: 0x040053F9 RID: 21497
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_QuestTurnInEntry> selectedEntryList = new List<XUiC_QuestTurnInEntry>();

	// Token: 0x040053FA RID: 21498
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestTurnInEntry selectedEntry;

	// Token: 0x040053FB RID: 21499
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnAccept;

	// Token: 0x040053FC RID: 21500
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, string> questtitleFormatter = new CachedStringFormatter<string, string>((string _s, string _s1) => string.Format("{0} : {1}", _s, _s1));
}
